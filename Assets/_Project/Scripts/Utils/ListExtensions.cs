using System.Collections.Generic;
using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// List extension methods
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Shuffle list
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// Get shuffled copy
        /// </summary>
        public static List<T> GetShuffled<T>(this List<T> list)
        {
            List<T> copy = new List<T>(list);
            copy.Shuffle();
            return copy;
        }

        /// <summary>
        /// Add unique element
        /// </summary>
        public static bool AddUnique<T>(this List<T> list, T element)
        {
            if (!list.Contains(element))
            {
                list.Add(element);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove duplicates
        /// </summary>
        public static List<T> RemoveDuplicates<T>(this List<T> list)
        {
            HashSet<T> seen = new HashSet<T>();
            List<T> result = new List<T>();
            
            foreach (T item in list)
            {
                if (seen.Add(item))
                {
                    result.Add(item);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get intersection
        /// </summary>
        public static List<T> Intersection<T>(this List<T> list1, List<T> list2)
        {
            HashSet<T> set = new HashSet<T>(list1);
            List<T> result = new List<T>();
            
            foreach (T item in list2)
            {
                if (set.Contains(item))
                {
                    result.Add(item);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get difference
        /// </summary>
        public static List<T> Difference<T>(this List<T> list1, List<T> list2)
        {
            HashSet<T> set = new HashSet<T>(list2);
            List<T> result = new List<T>();
            
            foreach (T item in list1)
            {
                if (!set.Contains(item))
                {
                    result.Add(item);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Remove null elements
        /// </summary>
        public static void RemoveNulls<T>(this List<T> list) where T : class
        {
            list.RemoveAll(item => item == null);
        }

        /// <summary>
        /// Get elements at indices
        /// </summary>
        public static List<T> GetAtIndices<T>(this List<T> list, int[] indices)
        {
            List<T> result = new List<T>();
            foreach (int index in indices)
            {
                if (index >= 0 && index < list.Count)
                {
                    result.Add(list[index]);
                }
            }
            return result;
        }
    }
}
