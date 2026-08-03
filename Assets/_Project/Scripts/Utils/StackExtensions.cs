using System.Collections.Generic;
using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Stack extension methods
    /// </summary>
    public static class StackExtensions
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(this Stack<T> stack)
        {
            if (stack == null || stack.Count == 0) return default;
            
            T[] array = stack.ToArray();
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Pop all elements
        /// </summary>
        public static List<T> PopAll<T>(this Stack<T> stack)
        {
            List<T> list = new List<T>();
            while (stack.Count > 0)
            {
                list.Add(stack.Pop());
            }
            return list;
        }

        /// <summary>
        /// Peek multiple elements
        /// </summary>
        public static List<T> PeekMultiple<T>(this Stack<T> stack, int count)
        {
            List<T> list = new List<T>();
            T[] array = stack.ToArray();
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
        public static bool Contains<T>(this Stack<T> stack, T element)
        {
            foreach (T item in stack)
            {
                if (EqualityComparer<T>.Default.Equals(item, element))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Push range
        /// </summary>
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                stack.Push(item);
            }
        }
    }
}
