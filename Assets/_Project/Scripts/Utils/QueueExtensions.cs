using System.Collections.Generic;
using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Queue extension methods
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(this Queue<T> queue)
        {
            if (queue == null || queue.Count == 0) return default;
            
            T[] array = queue.ToArray();
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Dequeue all elements
        /// </summary>
        public static List<T> DequeueAll<T>(this Queue<T> queue)
        {
            List<T> list = new List<T>();
            while (queue.Count > 0)
            {
                list.Add(queue.Dequeue());
            }
            return list;
        }

        /// <summary>
        /// Peek multiple elements
        /// </summary>
        public static List<T> PeekMultiple<T>(this Queue<T> queue, int count)
        {
            List<T> list = new List<T>();
            T[] array = queue.ToArray();
            int limit = Mathf.Min(count, array.Length);
            
            for (int i = 0; i < limit; i++)
            {
                list.Add(array[i]);
            }
            
            return list;
        }

        /// <summary>
        /// Check if contains element
        /// </summary>
        public static bool Contains<T>(this Queue<T> queue, T element)
        {
            foreach (T item in queue)
            {
                if (EqualityComparer<T>.Default.Equals(item, element))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Enqueue range
        /// </summary>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Enqueue(item);
            }
        }
    }
}
