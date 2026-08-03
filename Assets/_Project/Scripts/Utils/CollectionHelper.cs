using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Collection helper utility functions
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Get random element from dictionary values
        /// </summary>
        public static V RandomValue<K, V>(Dictionary<K, V> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0) return default;
            
            int index = Random.Range(0, dictionary.Count);
            int currentIndex = 0;
            
            foreach (var kvp in dictionary)
            {
                if (currentIndex == index)
                {
                    return kvp.Value;
                }
                currentIndex++;
            }
            
            return default;
        }

        /// <summary>
        /// Get random key from dictionary
        /// </summary>
        public static K RandomKey<K, V>(Dictionary<K, V> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0) return default;
            
            int index = Random.Range(0, dictionary.Count);
            int currentIndex = 0;
            
            foreach (var kvp in dictionary)
            {
                if (currentIndex == index)
                {
                    return kvp.Key;
                }
                currentIndex++;
            }
            
            return default;
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
        /// Get random element from list
        /// </summary>
        public static T RandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Remove and return last element
        /// </summary>
        public static T Pop<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            
            T last = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return last;
        }

        /// <summary>
        /// Remove and return first element
        /// </summary>
        public static T Dequeue<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            
            T first = list[0];
            list.RemoveAt(0);
            return first;
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
        /// Get intersection of two lists
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
        /// Get difference of two lists
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
