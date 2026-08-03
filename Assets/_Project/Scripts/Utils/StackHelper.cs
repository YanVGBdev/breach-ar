using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Stack helper utility functions
    /// </summary>
    public static class StackHelper
    {
        /// <summary>
        /// Get random element
        /// </summary>
        public static T RandomElement<T>(Stack<T> stack)
        {
            if (stack == null || stack.Count == 0) return default;
            
            T[] array = stack.ToArray();
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Pop all elements
        /// </summary>
        public static List<T> PopAll<T>(Stack<T> stack)
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
        public static List<T> PeekMultiple<T>(Stack<T> stack, int count)
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
        public static bool Contains<T>(Stack<T> stack, T element)
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
        /// Clear and push element
        /// </summary>
        public static void ClearAndPush<T>(Stack<T> stack, T element)
        {
            stack.Clear();
            stack.Push(element);
        }

        /// <summary>
        /// Push range
        /// </summary>
        public static void PushRange<T>(Stack<T> stack, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                stack.Push(item);
            }
        }

        /// <summary>
        /// Get as array
        /// </summary>
        public static T[] ToArray<T>(Stack<T> stack)
        {
            return stack.ToArray();
        }

        /// <summary>
        /// Get as list
        /// </summary>
        public static List<T> ToList<T>(Stack<T> stack)
        {
            return new List<T>(stack);
        }
    }
}
