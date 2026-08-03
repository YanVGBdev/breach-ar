using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Generic object pool for reusable game objects
    /// </summary>
    public class ObjectPoolGeneric<T> where T : Component
    {
        private readonly Queue<T> pool;
        private readonly T prefab;
        private readonly Transform parent;
        private readonly int maxSize;

        /// <summary>
        /// Create a new object pool
        /// </summary>
        public ObjectPoolGeneric(T prefab, int initialSize, Transform parent = null, int maxSize = 100)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.maxSize = maxSize;
            pool = new Queue<T>(initialSize);

            // Pre-instantiate objects
            for (int i = 0; i < initialSize; i++)
            {
                T obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public T Get()
        {
            T obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = Object.Instantiate(prefab, parent);
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Get an object from the pool at a position
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T obj = Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Return(T obj)
        {
            if (obj == null) return;

            obj.gameObject.SetActive(false);

            if (pool.Count < maxSize)
            {
                pool.Enqueue(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }

        /// <summary>
        /// Return all active objects to the pool
        /// </summary>
        public void ReturnAll()
        {
            T[] activeObjects = Object.FindObjectsOfType<T>();
            foreach (T obj in activeObjects)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    Return(obj);
                }
            }
        }

        /// <summary>
        /// Get the number of available objects
        /// </summary>
        public int CountAvailable => pool.Count;

        /// <summary>
        /// Clear the pool
        /// </summary>
        public void Clear()
        {
            while (pool.Count > 0)
            {
                T obj = pool.Dequeue();
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
        }
    }
}
