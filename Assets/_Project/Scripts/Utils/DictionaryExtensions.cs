using System.Collections.Generic;
using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Dictionary extension methods
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get or add value
        /// </summary>
        public static V GetOrAdd<K, V>(this Dictionary<K, V> dictionary, K key) where V : new()
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
        public static V TryGetValue<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue = default)
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
        public static K RandomKey<K, V>(this Dictionary<K, V> dictionary)
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
        public static V RandomValue<K, V>(this Dictionary<K, V> dictionary)
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
        /// Get keys as list
        /// </summary>
        public static List<K> GetKeys<K, V>(this Dictionary<K, V> dictionary)
        {
            return new List<K>(dictionary.Keys);
        }

        /// <summary>
        /// Get values as list
        /// </summary>
        public static List<V> GetValues<K, V>(this Dictionary<K, V> dictionary)
        {
            return new List<V>(dictionary.Values);
        }
    }
}
