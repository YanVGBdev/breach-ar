using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// List helper utility functions
    /// </summary>
    public static class ListHelper
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Shuffle list
        /// </summary>
        public static void Shuffle<T>(List<T> list)
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
        public static List<T> GetShuffled<T>(List<T> list)
        {
            List<T> copy = new List<T>(list);
            Shuffle(copy);
            return copy;
        }

        /// <summary>
        /// Add unique element
        /// </summary>
        public static bool AddUnique<T>(List<T> list, T element)
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
        public static List<T> RemoveDuplicates<T>(List<T> list)
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
        /// Get elements at indices
        /// </summary>
        public static List<T> GetAtIndices<T>(List<T> list, int[] indices)
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

        /// <summary>
        /// Remove null elements
        /// </summary>
        public static void RemoveNulls<T>(List<T> list) where T : class
        {
            list.RemoveAll(item => item == null);
        }

        /// <summary>
        /// Remove null elements (UnityEngine.Object)
        /// </summary>
        public static void RemoveNulls<T>(List<T> list) where T : UnityEngine.Object
        {
            list.RemoveAll(item => item == null);
        }

        /// <summary>
        /// Get intersection
        /// </summary>
        public static List<T> Intersection<T>(List<T> list1, List<T> list2)
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
        public static List<T> Difference<T>(List<T> list1, List<T> list2)
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
    }
}
