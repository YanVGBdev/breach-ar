using System.Collections.Generic;
using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// LinkedList extension methods
    /// </summary>
    public static class LinkedListExtensions
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(this LinkedList<T> list)
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
        public static T GetAt<T>(this LinkedList<T> list, int index)
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
        /// Get index of element
        /// </summary>
        public static int IndexOf<T>(this LinkedList<T> list, T element)
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
        public static List<T> ToList<T>(this LinkedList<T> list)
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Get as array
        /// </summary>
        public static T[] ToArray<T>(this LinkedList<T> list)
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Add range to end
        /// </summary>
        public static void AddRange<T>(this LinkedList<T> list, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                list.AddLast(item);
            }
        }
    }
}
