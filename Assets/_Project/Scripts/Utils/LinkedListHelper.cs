using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// LinkedList helper utility functions
    /// </summary>
    public static class LinkedListHelper
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(LinkedList<T> list)
        {
            if (list == null || list.Count == 0) return default;
            
            int index = Random.Range(0, list.Count);
            int currentIndex = 0;
            
            foreach (T item in list)
            {
                if (currentIndex == index)
                {
                    return item;
                }
                currentIndex++;
            }
            
            return default;
        }

        /// <summary>
        /// Get element at index
        /// </summary>
        public static T GetAt<T>(LinkedList<T> list, int index)
        {
            if (index < 0 || index >= list.Count) return default;
            
            int currentIndex = 0;
            foreach (T item in list)
            {
                if (currentIndex == index)
                {
                    return item;
                }
                currentIndex++;
            }
            
            return default;
        }

        /// <summary>
        /// Check if contains element
        /// </summary>
        public static bool Contains<T>(LinkedList<T> list, T element)
        {
            return list.Contains(element);
        }

        /// <summary>
        /// Get index of element
        /// </summary>
        public static int IndexOf<T>(LinkedList<T> list, T element)
        {
            int index = 0;
            foreach (T item in list)
            {
                if (EqualityComparer<T>.Default.Equals(item, element))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Get as list
        /// </summary>
        public static List<T> ToList<T>(LinkedList<T> list)
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Get as array
        /// </summary>
        public static T[] ToArray<T>(LinkedList<T> list)
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Add range to end
        /// </summary>
        public static void AddRange<T>(LinkedList<T> list, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                list.AddLast(item);
            }
        }

        /// <summary>
        /// Add range to beginning
        /// </summary>
        public static void AddRangeFirst<T>(LinkedList<T> list, IEnumerable<T> collection)
        {
            LinkedListNode<T> lastNode = null;
            foreach (T item in collection)
            {
                if (lastNode == null)
                {
                    lastNode = list.AddFirst(item);
                }
                else
                {
                    lastNode = list.AddAfter(lastNode, item);
                }
            }
        }
    }
}
