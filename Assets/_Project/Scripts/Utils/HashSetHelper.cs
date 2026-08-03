using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// HashSet helper utility functions
    /// </summary>
    public static class HashSetHelper
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(HashSet<T> set)
        {
            if (set == null || set.Count == 0) return default;
            
            int index = Random.Range(0, set.Count);
            int currentIndex = 0;
            
            foreach (T item in set)
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
        /// Get intersection
        /// </summary>
        public static HashSet<T> Intersection<T>(HashSet<T> set1, HashSet<T> set2)
        {
            HashSet<T> result = new HashSet<T>(set1);
            result.IntersectWith(set2);
            return result;
        }

        /// <summary>
        /// Get union
        /// </summary>
        public static HashSet<T> Union<T>(HashSet<T> set1, HashSet<T> set2)
        {
            HashSet<T> result = new HashSet<T>(set1);
            result.UnionWith(set2);
            return result;
        }

        /// <summary>
        /// Get difference
        /// </summary>
        public static HashSet<T> Difference<T>(HashSet<T> set1, HashSet<T> set2)
        {
            HashSet<T> result = new HashSet<T>(set1);
            result.ExceptWith(set2);
            return result;
        }

        /// <summary>
        /// Convert to list
        /// </summary>
        public static List<T> ToList<T>(HashSet<T> set)
        {
            return new List<T>(set);
        }

        /// <summary>
        /// Convert to array
        /// </summary>
        public static T[] ToArray<T>(HashSet<T> set)
        {
            T[] array = new T[set.Count];
            set.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Add multiple elements
        /// </summary>
        public static void AddRange<T>(HashSet<T> set, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                set.Add(item);
            }
        }
    }
}
