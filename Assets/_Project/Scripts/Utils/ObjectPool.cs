using UnityEngine;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Generic object pool for reusable game objects
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string Tag;
            public GameObject Prefab;
            public int Size;
        }

        [Header("Pools")]
        [SerializeField] private List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, Pool> poolConfigs;

        [Inject]
        private void Initialize()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            poolConfigs = new Dictionary<string, Pool>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.Size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.Tag, objectPool);
                poolConfigs.Add(pool.Tag, pool);
            }
        }

        /// <summary>
        /// Get an object from the pool
        /// </summary>
        public GameObject Spawn(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            GameObject obj = poolDictionary[tag].Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            // Re-enqueue when done
            poolDictionary[tag].Enqueue(obj);

            return obj;
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Despawn(string tag, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return;
            }

            obj.SetActive(false);
        }

        /// <summary>
        /// Check if pool has available objects
        /// </summary>
        public bool HasAvailable(string tag)
        {
            if (!poolDictionary.ContainsKey(tag))
                return false;

            return poolDictionary[tag].Count > 0;
        }

        /// <summary>
        /// Get current count of available objects in pool
        /// </summary>
        public int GetAvailableCount(string tag)
        {
            if (!poolDictionary.ContainsKey(tag))
                return 0;

            return poolDictionary[tag].Count;
        }

        /// <summary>
        /// Expand a pool dynamically
        /// </summary>
        public void ExpandPool(string tag, int additionalCount)
        {
            if (!poolConfigs.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return;
            }

            Pool pool = poolConfigs[tag];
            for (int i = 0; i < additionalCount; i++)
            {
                GameObject obj = Instantiate(pool.Prefab, transform);
                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
            }
        }
    }
}
