using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Queue helper utility functions
    /// </summary>
    public static class QueueHelper
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(Queue<T> queue)
        {
            if (queue == null || queue.Count == 0) return default;
            
            T[] array = queue.ToArray();
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Dequeue all elements
        /// </summary>
        public static List<T> DequeueAll<T>(Queue<T> queue)
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
        public static List<T> PeekMultiple<T>(Queue<T> queue, int count)
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
        public static bool Contains<T>(Queue<T> queue, T element)
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
        /// Remove element
        /// </summary>
        public static bool Remove<T>(Queue<T> queue, T element)
        {
            List<T> list = new List<T>(queue);
            bool removed = list.Remove(element);
            
            if (removed)
            {
                queue.Clear();
                foreach (T item in list)
                {
                    queue.Enqueue(item);
                }
            }
            
            return removed;
        }

        /// <summary>
        /// Clear and add element
        /// </summary>
        public static void ClearAndEnqueue<T>(Queue<T> queue, T element)
        {
            queue.Clear();
            queue.Enqueue(element);
        }

        /// <summary>
        /// Enqueue range
        /// </summary>
        public static void EnqueueRange<T>(Queue<T> queue, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Get as array
        /// </summary>
        public static T[] ToArray<T>(Queue<T> queue)
        {
            return queue.ToArray();
        }

        /// <summary>
        /// Get as list
        /// </summary>
        public static List<T> ToList<T>(Queue<T> queue)
        {
            return new List<T>(queue);
        }
    }
}
