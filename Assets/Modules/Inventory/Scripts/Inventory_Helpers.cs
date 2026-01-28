using System;
using UnityEngine;

namespace Modules.Inventories
{
    public partial class Inventory
    {
        private int FindItemIndex(int id)
        {
            int lo = 0;
            int hi = _indexSize;

            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (_keys[mid] < id)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            return lo;
        }

        private bool CanPlaceAt(int startX, int startY, int w, int h)
        {
            return IsFits(startX, startY, w, h) && IsFreeSpace(startX, startY, w, h);
        }

        private void PlaceItem(Item item, Vector2Int pos) => PlaceItem(item, pos.x, pos.y);

        private void PlaceItem(Item item, int startX, int startY)
        {
            InsertItem(item, ToIndex(startX, startY));
            SetSlots(item.Id, startX, startY, item.Size.x, item.Size.y);

            _count++;
            OnAdded?.Invoke(item, new Vector2Int(startX, startY));
        }

        private void InsertItem(Item item, int rootIndex)
        {
            int insertIndex = FindItemIndex(item.Id);

            ShiftRight(insertIndex);

            _keys[insertIndex] = item.Id;
            _values[insertIndex] = rootIndex;
            _items[insertIndex] = item;

            _indexSize++;
        }

        private void SetSlots(int itemId, int startX, int startY, int width, int height)
        {
            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    _slots[ToIndex(x, y)] = itemId;
                }
            }
        }
        
        private int ToIndex(int x, int y)
        {
            return y * _width + x;
        }

        private Vector2Int ToPos(int index)
        {
            return new Vector2Int(index % _width, index / _width);
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        private bool IsFreeSpace(int startX, int startY, int w, int h)
        {
            for (int y = startY; y < startY + h; y++)
            for (int x = startX; x < startX + w; x++)
                if (_slots[ToIndex(x, y)] != -1)
                    return false;

            return true;
        }

        private bool IsIndexAtKey(int index, int id)
        {
            return index < _indexSize && _keys[index] == id;
        }

        private bool IsFits(int startX, int startY, int w, int h)
        {
            return startX >= 0 && startY >= 0 &&
                   startX + w <= _width &&
                   startY + h <= _height;
        }

        private bool TryGetPlacement(Item item, int? startX, int? startY, out Vector2Int position)
        {
            position = Vector2Int.zero;

            if (item == null || Contains(item))
                return false;

            if (!TryGetSize(item, out int w, out int h))
                return false;

            if (startX.HasValue && startY.HasValue)
            {
                if (!CanPlaceAt(startX.Value, startY.Value, w, h))
                    return false;

                position = new Vector2Int(startX.Value, startY.Value);
                return true;
            }

            return FindFreePosition(w, h, out position);
        }

        private void ShiftRight(int fromIndex)
        {
            if (fromIndex >= _indexSize)
                return;

            int count = _indexSize - fromIndex;

            Array.Copy(_keys, fromIndex, _keys, fromIndex + 1, count);
            Array.Copy(_values, fromIndex, _values, fromIndex + 1, count);
            Array.Copy(_items, fromIndex, _items, fromIndex + 1, count);
        }

        private bool TryGetSize(Item item, out int w, out int h)
        {
            w = 0;
            h = 0;

            if (item == null)
                return false;

            w = item.Size.x;
            h = item.Size.y;
            return true;
        }

        private bool TryGetItemById(int id, out Item item)
        {
            item = null;
            if (id == -1) return false;

            int i = FindItemIndex(id);
            if (IsIndexAtKey(i, id))
            {
                item = _items[i];
                return true;
            }

            return false;
        }

        private void RemoveIndexAt(int index)
        {
            int moveCount = _indexSize - index - 1;
            if (moveCount > 0)
            {
                Array.Copy(_keys, index + 1, _keys, index, moveCount);
                Array.Copy(_values, index + 1, _values, index, moveCount);
                Array.Copy(_items, index + 1, _items, index, moveCount);
            }

            int last = _indexSize - 1;
            _keys[last] = 0;
            _values[last] = 0;
            _items[last] = null;

            _indexSize--;
        }

        private static int Compare(Item a, Item b)
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a is null) return 1;
            if (b is null) return -1;

            int areaA = a.Size.x * a.Size.y;
            int areaB = b.Size.x * b.Size.y;

            int cmp = areaB.CompareTo(areaA);
            if (cmp != 0) return cmp;

            cmp = string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            if (cmp != 0) return cmp;

            return a.Id.CompareTo(b.Id);
        }

        private void ClearItemSlots(int itemId)
        {
            for (int i = 0; i < _slots.Length; i++)
                if (_slots[i] == itemId)
                    _slots[i] = -1;
        }

        private void UpdateItemRoot(int itemId, int newRootIndex)
        {
            int i = FindItemIndex(itemId);
            _values[i] = newRootIndex;
        }
    }
}