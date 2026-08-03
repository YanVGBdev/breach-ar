using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Random helper utility functions
    /// </summary>
    public static class RandomHelper
    {
        /// <summary>
        /// Get random point in circle
        /// </summary>
        public static Vector2 PointInCircle(float radius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        }

        /// <summary>
        /// Get random point in ring
        /// </summary>
        public static Vector2 PointInRing(float innerRadius, float outerRadius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Random.Range(innerRadius, outerRadius);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
        }

        /// <summary>
        /// Get random point in sphere
        /// </summary>
        public static Vector3 PointInSphere(float radius)
        {
            return Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// Get random point on sphere surface
        /// </summary>
        public static Vector3 PointOnSphere(float radius)
        {
            return Random.onUnitSphere * radius;
        }

        /// <summary>
        /// Get random point in bounds
        /// </summary>
        public static Vector3 PointInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        /// <summary>
        /// Get random color
        /// </summary>
        public static Color RandomColor()
        {
            return new Color(Random.value, Random.value, Random.value);
        }

        /// <summary>
        /// Get random color from palette
        /// </summary>
        public static Color RandomColor(Color[] palette)
        {
            if (palette == null || palette.Length == 0) return Color.white;
            return palette[Random.Range(0, palette.Length)];
        }

        /// <summary>
        /// Get random element from array
        /// </summary>
        public static T RandomElement<T>(T[] array)
        {
            if (array == null || array.Length == 0) return default;
            return array[Random.Range(0, array.Length)];
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
        /// Get random bool
        /// </summary>
        public static bool RandomBool(float probability = 0.5f)
        {
            return Random.value < probability;
        }

        /// <summary>
        /// Get random sign (-1 or 1)
        /// </summary>
        public static int RandomSign()
        {
            return Random.value > 0.5f ? 1 : -1;
        }
    }
}
