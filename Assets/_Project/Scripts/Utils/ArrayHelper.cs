using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Array helper utility functions
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// Get random element from array
        /// </summary>
        public static T RandomElement<T>(T[] array)
        {
            if (array == null || array.Length == 0) return default;
            return array[Random.Range(0, array.Length)];
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
        /// Shuffle array
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
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
        /// Get weighted random index
        /// </summary>
        public static int WeightedRandom(float[] weights)
        {
            float total = 0f;
            foreach (float weight in weights)
            {
                total += weight;
            }

            float random = Random.Range(0f, total);
            float current = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                current += weights[i];
                if (random <= current)
                {
                    return i;
                }
            }

            return weights.Length - 1;
        }

        /// <summary>
        /// Get closest element to point
        /// </summary>
        public static T GetClosest<T>(T[] array, Vector3 point) where T : Component
        {
            T closest = null;
            float closestDistance = float.MaxValue;

            foreach (T element in array)
            {
                float distance = Vector3.Distance(element.transform.position, point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = element;
                }
            }

            return closest;
        }

        /// <summary>
        /// Get elements within radius
        /// </summary>
        public static List<T> GetWithinRadius<T>(T[] array, Vector3 center, float radius) where T : Component
        {
            List<T> result = new List<T>();
            float radiusSqr = radius * radius;

            foreach (T element in array)
            {
                if ((element.transform.position - center).sqrMagnitude <= radiusSqr)
                {
                    result.Add(element);
                }
            }

            return result;
        }

        /// <summary>
        /// Resize array
        /// </summary>
        public static T[] Resize<T>(T[] array, int newSize)
        {
            T[] newArray = new T[newSize];
            int copySize = Mathf.Min(array.Length, newSize);
            System.Array.Copy(array, newArray, copySize);
            return newArray;
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        public static T[] Add<T>(T[] array, T element)
        {
            T[] newArray = new T[array.Length + 1];
            array.CopyTo(newArray, 0);
            newArray[array.Length] = element;
            return newArray;
        }

        /// <summary>
        /// Remove element from array
        /// </summary>
        public static T[] Remove<T>(T[] array, T element)
        {
            List<T> list = new List<T>(array);
            list.Remove(element);
            return list.ToArray();
        }

        /// <summary>
        /// Remove element at index
        /// </summary>
        public static T[] RemoveAt<T>(T[] array, int index)
        {
            List<T> list = new List<T>(array);
            list.RemoveAt(index);
            return list.ToArray();
        }
    }
}
