using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Transform _worldTransform;
        private readonly HashSet<T> _active = new();
        private readonly Queue<T> _pool = new();

        public int ActiveCount => _active.Count;

        public ObjectPool(T prefab, Transform container, Transform worldTransform, int prewarmCount = 0)
        {
            _prefab = prefab;
            _container = container;
            _worldTransform = worldTransform;

            for (int i = 0; i < prewarmCount; i++)
            {
                T instance = Object.Instantiate(_prefab, _container);
                _pool.Enqueue(instance);
            }
        }

        public T Get()
        {
            T instance;
            if (_pool.TryDequeue(out instance))
            {
                instance.transform.SetParent(_worldTransform);
            }
            else
            {
                instance = Object.Instantiate(_prefab, _worldTransform);
            }

            if (instance is IPoolable poolable)
                poolable.OnGet();

            _active.Add(instance);
            return instance;
        }

        public void Return(T instance)
        {
            if (!instance || !_active.Remove(instance))
                return;

            if (instance is IPoolable poolable)
                poolable.OnReturn();

            instance.transform.SetParent(_container);
            _pool.Enqueue(instance);
        }

        public IReadOnlyList<T> GetActiveSnapshot()
        {
            return new List<T>(_active);
        }
    }
}
