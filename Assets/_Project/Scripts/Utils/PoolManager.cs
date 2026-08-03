using UnityEngine;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Centralized object pool manager
    /// </summary>
    public class PoolManager : MonoBehaviour
    {

        [System.Serializable]
        public class PoolConfig
        {
            public string Tag;
            public GameObject Prefab;
            public int InitialSize;
            public int MaxSize;
        }

        [Header("Pool Configurations")]
        [SerializeField] private List<PoolConfig> poolConfigs;

        private Dictionary<string, Queue<GameObject>> pools;
        private Dictionary<string, PoolConfig> configLookup;
        private Dictionary<string, Transform> poolParents;

        [Inject]
        private void Initialize()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            pools = new Dictionary<string, Queue<GameObject>>();
            configLookup = new Dictionary<string, PoolConfig>();
            poolParents = new Dictionary<string, Transform>();

            foreach (var config in poolConfigs)
            {
                CreatePool(config);
            }
        }

        private void CreatePool(PoolConfig config)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            Transform parent = new GameObject($"Pool_{config.Tag}").transform;
            parent.SetParent(transform);

            poolParents[config.Tag] = parent;
            configLookup[config.Tag] = config;

            for (int i = 0; i < config.InitialSize; i++)
            {
                GameObject obj = Instantiate(config.Prefab, parent);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            pools[config.Tag] = pool;
        }

        /// <summary>
        /// Get object from pool
        /// </summary>
        public GameObject Get(string tag)
        {
            if (!pools.ContainsKey(tag))
            {
                Debug.LogWarning($"[PoolManager] Pool with tag {tag} not found");
                return null;
            }

            Queue<GameObject> pool = pools[tag];
            GameObject obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                // Pool empty, create new object
                PoolConfig config = configLookup[tag];
                obj = Instantiate(config.Prefab, poolParents[tag]);
            }

            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Get object from pool at position
        /// </summary>
        public GameObject Get(string tag, Vector3 position, Quaternion rotation)
        {
            GameObject obj = Get(tag);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// Return object to pool
        /// </summary>
        public void Return(string tag, GameObject obj)
        {
            if (obj == null) return;

            if (!pools.ContainsKey(tag))
            {
                Debug.LogWarning($"[PoolManager] Pool with tag {tag} not found, destroying object");
                Destroy(obj);
                return;
            }

            PoolConfig config = configLookup[tag];
            obj.SetActive(false);
            obj.transform.SetParent(poolParents[tag]);

            if (pools[tag].Count < config.MaxSize)
            {
                pools[tag].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        /// <summary>
        /// Check if pool exists
        /// </summary>
        public bool HasPool(string tag)
        {
            return pools.ContainsKey(tag);
        }

        /// <summary>
        /// Get available count in pool
        /// </summary>
        public int GetAvailableCount(string tag)
        {
            if (!pools.ContainsKey(tag)) return 0;
            return pools[tag].Count;
        }

        /// <summary>
        /// Pre-warm pool
        /// </summary>
        public void Prewarm(string tag, int count)
        {
            if (!pools.ContainsKey(tag))
            {
                Debug.LogWarning($"[PoolManager] Pool with tag {tag} not found");
                return;
            }

            PoolConfig config = configLookup[tag];
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(config.Prefab, poolParents[tag]);
                obj.SetActive(false);
                pools[tag].Enqueue(obj);
            }
        }
    }
}
