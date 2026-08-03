using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Dictionary helper utility functions
    /// </summary>
    public static class DictionaryHelper
    {
        /// <summary>
        /// Get or add value
        /// </summary>
        public static V GetOrAdd<K, V>(Dictionary<K, V> dictionary, K key) where V : new()
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = new V();
            }
            return dictionary[key];
        }

        /// <summary>
        /// Try get value or return default
        /// </summary>
        public static V TryGetValue<K, V>(Dictionary<K, V> dictionary, K key, V defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out V value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Get random key
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
        /// Get random value
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
        /// Merge two dictionaries
        /// </summary>
        public static Dictionary<K, V> Merge<K, V>(Dictionary<K, V> dict1, Dictionary<K, V> dict2)
        {
            Dictionary<K, V> result = new Dictionary<K, V>(dict1);
            
            foreach (var kvp in dict2)
            {
                result[kvp.Key] = kvp.Value;
            }
            
            return result;
        }

        /// <summary>
        /// Get keys as list
        /// </summary>
        public static List<K> GetKeys<K, V>(Dictionary<K, V> dictionary)
        {
            return new List<K>(dictionary.Keys);
        }

        /// <summary>
        /// Get values as list
        /// </summary>
        public static List<V> GetValues<K, V>(Dictionary<K, V> dictionary)
        {
            return new List<V>(dictionary.Values);
        }

        /// <summary>
        /// Check if dictionary contains value
        /// </summary>
        public static bool ContainsValue<K, V>(Dictionary<K, V> dictionary, V value)
        {
            return dictionary.ContainsValue(value);
        }

        /// <summary>
        /// Get key by value
        /// </summary>
        public static K GetKeyByValue<K, V>(Dictionary<K, V> dictionary, V value)
        {
            foreach (var kvp in dictionary)
            {
                if (EqualityComparer<V>.Default.Equals(kvp.Value, value))
                {
                    return kvp.Key;
                }
            }
            return default;
        }
    }
}
