using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Modules.Inventories
{
    public partial class Inventory : IEnumerable<Item>
    {
        public event Action<Item, Vector2Int> OnAdded;
        public event Action<Item, Vector2Int> OnRemoved;
        public event Action<Item, Vector2Int> OnMoved;
        public event Action OnCleared;

        public int Width => _width;
        public int Height => _height;
        public int Count => _count;

        private readonly int[] _slots;
        private readonly int[] _keys;
        private readonly int[] _values;
        private readonly Item[] _items;

        private readonly int _width;
        private readonly int _height;

        private int _indexSize;
        private int _count;

        public Inventory(int width, int height)
        {
            ThrowIfSizeZero(width, height);

            _width = width;
            _height = height;

            int cellCount = width * height;

            _slots = new int[cellCount];
            Array.Fill(_slots, -1);

            _keys = new int[cellCount];
            _values = new int[cellCount];
            _items = new Item[cellCount];
        }

        public Inventory(
            int width,
            int height,
            params KeyValuePair<Item, Vector2Int>[] items
        ) : this(width, height)
        {
            ThrowIfNull(items);

            foreach (var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(
            int width,
            int height,
            params Item[] items
        ) : this(width, height)
        {
            ThrowIfNull(items);

            foreach (var item in items)
                AddItem(item);
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<KeyValuePair<Item, Vector2Int>> items
        ) : this(width, height)
        {
            ThrowIfNull(items);

            foreach (var item in items)
                AddItem(item.Key, item.Value);
        }

        public Inventory(
            int width,
            int height,
            IEnumerable<Item> items
        ) : this(width, height)
        {
            ThrowIfNull(items);

            foreach (var item in items)
                AddItem(item);
        }

        /// <summary>
        /// Creates new inventory 
        /// </summary>
        public Inventory(Inventory other) : this(other._width, other._height)
        {
            ThrowIfNull(other);


            Array.Copy(other._slots, _slots, other._slots.Length);
            Array.Copy(other._keys, _keys, other._keys.Length);
            Array.Copy(other._values, _values, other._values.Length);
            Array.Copy(other._items, _items, other._items.Length);


            _indexSize = other._indexSize;
            _count = other._count;
        }

        /// <summary>
        /// Adds an item on a free position
        /// </summary>
        public bool AddItem(Item item)
        {
            if (item == null) return false;

            ThrowIfSizeZero(item.Size.x, item.Size.y);
            return TryAdd(item);
        }

        /// <summary>
        /// Adds an item on a specified position
        /// </summary>
        public bool AddItem(Item item, Vector2Int pos) => AddItem(item, pos.x, pos.y);


        public bool AddItem(Item item, int x, int y)
        {
            if (item == null) return false;

            ThrowIfSizeZero(item.Size.x, item.Size.y);

            if (TryGetPlacement(item, x, y, out var finalPos))
            {
                PlaceItem(item, finalPos.x, finalPos.y);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks for adding an item on a free position
        /// </summary>
        public bool CanAddItem(Item item)
        {
            if (item == null) return false;

            ThrowIfSizeZero(item.Size.x, item.Size.y);
            return TryGetPlacement(item, out _);
        }

        /// <summary>
        /// Checks for adding an item on a specified position
        /// </summary>
        public bool CanAddItem(Item item, Vector2Int pos) => CanAddItem(item, pos.x, pos.y);


        public bool CanAddItem(Item item, int x, int y)
        {
            if (item == null) return false;

            ThrowIfSizeZero(item.Size.x, item.Size.y);
            return TryGetPlacement(item, x, y, out _);
        }


        /// <summary>
        /// Returns a free position for a specified item
        /// </summary>
        public bool FindFreePosition(Item item, out Vector2Int pos)
        {
            if (item == null) { pos = default; return false; }
            return FindFreePosition(item.Size.x, item.Size.y, out pos);
        }


        public bool FindFreePosition(Vector2Int size, out Vector2Int pos) => FindFreePosition(size.x, size.y, out pos);


        public bool FindFreePosition(int sizeX, int sizeY, out Vector2Int pos)
        {
            ThrowIfSizeZero(sizeX, sizeY);
            return ScanForFreeRect(sizeX, sizeY, out pos);
        }

        /// <summary>
        /// Checks if the specified element exists
        /// </summary>
        public bool Contains(Item item)
        {
            if (item == null) return false;
            return TryGetItemById(item.Id, out _);
        }

        /// <summary>
        /// Checks if the specified position is occupied
        /// </summary>
        public bool IsOccupied(Vector2Int pos) => IsOccupied(pos.x, pos.y);


        public bool IsOccupied(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            return _slots[ToIndex(x, y)] != -1;
        }


        /// <summary>
        /// Checks if the specified position is free
        /// </summary>
        public bool IsFree(Vector2Int pos) => IsFree(pos.x, pos.y);


        public bool IsFree(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            return _slots[ToIndex(x, y)] == -1;
        }

        /// <summary>
        /// Returns an item at specified position 
        /// </summary>
        public Item GetItem(Vector2Int pos) => GetItem(pos.x, pos.y);


        public Item GetItem(int x, int y)
        {
            ThrowIfPosNotFitsInventory(x, y);

            int id = _slots[ToIndex(x, y)];
            return TryGetItemById(id, out var item) ? item : null;
        }


        public bool TryGetItem(Vector2Int pos, out Item item) => TryGetItem(pos.x, pos.y, out item);


        public bool TryGetItem(int x, int y, out Item item)
        {
            item = null;

            if (!InBounds(x, y))
                return false;

            int id = _slots[ToIndex(x, y)];
            return TryGetItemById(id, out item);
        }

        /// <summary>
        /// Returns positions of a specified item 
        /// </summary>
        public Vector2Int[] GetPositions(Item item)
        {
            ThrowIfNullReference(item);

            if (!TryGetPositions(item, out var pos))
                throw new KeyNotFoundException("Item not found in inventory.");

            return pos;
        }

        public bool TryGetPositions(Item item, out Vector2Int[] pos)
        {
            pos = null;
            if (item == null) return false;

            if (!TryFindItemIndex(item.Id, out int i))
                return false;

            Vector2Int start = ToPos(_values[i]);
            int w = item.Size.x;
            int h = item.Size.y;
            int count = w * h;

            var result = new Vector2Int[count];
            int k = 0;
            for (int dx = 0; dx < w; dx++)
                for (int dy = 0; dy < h; dy++)
                    result[k++] = new Vector2Int(start.x + dx, start.y + dy);

            pos = result;
            return true;
        }

        /// <summary>
        /// Returns count of items with a specified name
        /// </summary>
        public int GetItemCount(string name)
        {
            int count = 0;

            for (int i = 0; i < _indexSize; i++)
            {
                if (string.Equals(_items[i].Name, name))
                    count++;
            }

            return count;
        }

        public bool MoveItem(Item item, Vector2Int pos)
        {
            ThrowIfNull(item);

            if (!TryGetItemById(item.Id, out _))
                return false;

            int w = item.Size.x;
            int h = item.Size.y;

            if (!IsValidMoveTarget(pos.x, pos.y, w, h, item.Id))
                return false;

            ClearItemSlots(item.Id);

            UpdateItemRoot(item.Id, ToIndex(pos.x, pos.y));
            SetSlots(item.Id, pos.x, pos.y, w, h);

            OnMoved?.Invoke(item, pos);
            return true;
        }

        /// <summary>
        /// Copies items to a specified matrix
        /// </summary>
        public void CopyTo(Item[,] matrix)
        {
            ThrowIfMatrixNotFits(matrix);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int index = ToIndex(x, y);
                    int itemId = _slots[index];

                    matrix[x, y] = TryGetItemById(itemId, out var item)
                        ? item
                        : null;
                }
            }
        }

        /// <summary>
        /// Removes specified item
        /// </summary>
        public bool RemoveItem(Item item) => RemoveItem(item, out _);

        public bool RemoveItem(Item item, out Vector2Int pos)
        {
            pos = default;
            if (item == null) return false;

            int i = FindItemIndex(item.Id);
            if (i >= _indexSize || _keys[i] != item.Id)
                return false;

            pos = ToPos(_values[i]);
            ClearItemSlots(item.Id);
            RemoveIndexAt(i);

            _count--;
            OnRemoved?.Invoke(item, pos);
            return true;
        }

        /// <summary>
        /// Clears all items 
        /// </summary>
        public void Clear()
        {
            if (_count == 0) return;

            Array.Fill(_slots, -1);

            _indexSize = 0;
            _count = 0;

            OnCleared?.Invoke();
        }

        /// <summary>
        /// Rearranges an inventory space with max free slots 
        /// </summary>
        public void OptimizeSpace()
        {
            if (_indexSize <= 1)
                return;

            int n = _indexSize;
            Item[] buffer = ArrayPool<Item>.Shared.Rent(n);
            try
            {
                Array.Copy(_items, 0, buffer, 0, n);
                Array.Sort(buffer, 0, n, Comparer<Item>.Create(Compare));

                Array.Fill(_slots, -1);
                _indexSize = 0;
                _count = 0;

                for (int i = 0; i < n; i++)
                {
                    var item = buffer[i];
                    if (item == null)
                        continue;

                    if (!FindFreePosition(item.Size.x, item.Size.y, out var pos))
                        throw new InvalidOperationException($"OptimizeSpace failed: item '{item.Name}' (id={item.Id}) does not fit.");

                    PlaceItem(item, pos.x, pos.y);
                }
            }
            finally
            {
                ArrayPool<Item>.Shared.Return(buffer, clearArray: true);
            }
        }

        /// <summary>
        /// Iterates by all items 
        /// </summary>
        public struct Enumerator
        {
            private readonly Item[] _items;
            private readonly int _indexSize;
            private int _index;

            internal Enumerator(Item[] items, int indexSize)
            {
                _items = items;
                _indexSize = indexSize;
                _index = -1;
            }

            public Item Current => _items[_index];

            public bool MoveNext() => ++_index < _indexSize;

            public void Reset() => _index = -1;
        }

        public Enumerator GetEnumerator() => new(_items, _indexSize);

        IEnumerator<Item> IEnumerable<Item>.GetEnumerator()
        {
            for (int i = 0; i < _indexSize; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Item>)this).GetEnumerator();

        /// <summary>
        /// Returns an inventory matrix in string format
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int id = _slots[ToIndex(x, y)];

                    if (TryGetItemById(id, out var item))
                    {
                        char c = string.IsNullOrEmpty(item.Name)
                            ? '?'
                            : item.Name[0];

                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }



                if (y < _height - 1)
                    sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}