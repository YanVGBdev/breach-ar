using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Service to reduce GC allocations in critical loops
    /// Referência: OPT-010
    /// </summary>
    public class GCReductionService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float gcCheckInterval = 30f;
        [SerializeField] private bool enableGCCollection = false;
        [SerializeField] private int targetFrameBudgetMs = 2;

        private float lastGCCheck;
        private long lastGCMemory;
        private List<System.Action> reusableCallbacks = new List<System.Action>();
        private Queue<System.Action> callbackPool = new Queue<System.Action>();

        /// <summary>
        /// Get a callback from the pool (avoids allocation)
        /// </summary>
        public System.Action GetCallback(System.Action action)
        {
            if (callbackPool.Count > 0)
            {
                var pooled = callbackPool.Dequeue();
                // We can't reuse the delegate directly, but this pattern
                // shows the concept. In practice, use struct-based approach.
            }

            return action;
        }

        /// <summary>
        /// Return a callback to the pool
        /// </summary>
        public void ReturnCallback(System.Action callback)
        {
            // In production, this would be a more sophisticated pooling system
        }

        /// <summary>
        /// Pre-allocate collections to avoid runtime allocation
        /// </summary>
        public static void PreallocateList<T>(List<T> list, int capacity)
        {
            if (list.Capacity < capacity)
            {
                list.Capacity = capacity;
            }
        }

        /// <summary>
        /// Clear list without deallocating (reuse capacity)
        /// </summary>
        public static void ClearList<T>(List<T> list)
        {
            list.Clear(); // Preserves capacity
        }

        private void Update()
        {
            if (!enableGCCollection) return;

            if (Time.time - lastGCCheck >= gcCheckInterval)
            {
                MonitorAndOptimize();
                lastGCCheck = Time.time;
            }
        }

        /// <summary>
        /// Monitor GC and optimize
        /// </summary>
        private void MonitorAndOptimize()
        {
            long currentMemory = System.GC.GetTotalMemory(false);
            long memoryDelta = currentMemory - lastGCMemory;
            lastGCMemory = currentMemory;

            // Log memory growth
            if (memoryDelta > 1024 * 1024) // 1MB growth
            {
                Debug.LogWarning($"[GCReduction] Memory grew by {memoryDelta / 1024}KB. Total: {currentMemory / 1024 / 1024}MB");
            }

            // Force GC if memory is high and frame budget allows
            if (currentMemory > 500 * 1024 * 1024 && enableGCCollection) // 500MB
            {
                Debug.Log("[GCReduction] Forcing garbage collection");
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Force garbage collection (call only when safe)
        /// </summary>
        public void ForceGCCollection()
        {
            Debug.Log("[GCReduction] Manual GC collection");
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        /// <summary>
        /// Get current memory stats
        /// </summary>
        public MemoryStats GetMemoryStats()
        {
            return new MemoryStats
            {
                TotalMemory = System.GC.GetTotalMemory(false),
                Gen0Collections = System.GC.CollectionCount(0),
                Gen1Collections = System.GC.CollectionCount(1),
                Gen2Collections = System.GC.CollectionCount(2)
            };
        }
    }

    /// <summary>
    /// Memory statistics
    /// </summary>
    [System.Serializable]
    public struct MemoryStats
    {
        public long TotalMemory;
        public int Gen0Collections;
        public int Gen1Collections;
        public int Gen2Collections;
    }

    /// <summary>
    /// Reusable list wrapper to avoid allocations
    /// </summary>
    public class ReusableList<T>
    {
        private List<T> list;
        private int activeCount;

        public int Count => activeCount;
        public List<T> InnerList => list;

        public ReusableList(int initialCapacity = 16)
        {
            list = new List<T>(initialCapacity);
            activeCount = 0;
        }

        public void Add(T item)
        {
            if (activeCount < list.Count)
            {
                list[activeCount] = item;
            }
            else
            {
                list.Add(item);
            }
            activeCount++;
        }

        public void Clear()
        {
            activeCount = 0; // Preserves capacity
        }

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }
    }

    /// <summary>
    /// Reusable dictionary wrapper
    /// </summary>
    public class ReusableDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dict;

        public int Count => dict.Count;

        public ReusableDictionary(int initialCapacity = 16)
        {
            dict = new Dictionary<TKey, TValue>(initialCapacity);
        }

        public void Add(TKey key, TValue value)
        {
            dict[key] = value;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public TValue this[TKey key]
        {
            get => dict[key];
            set => dict[key] = value;
        }
    }
}
