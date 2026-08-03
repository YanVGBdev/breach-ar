using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// HashSet extension methods
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Add range of elements
        /// </summary>
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                set.Add(item);
            }
        }

        /// <summary>
        /// Remove where condition
        /// </summary>
        public static int RemoveWhere<T>(this HashSet<T> set, System.Func<T, bool> predicate)
        {
            int count = 0;
            List<T> toRemove = new List<T>();
            
            foreach (T item in set)
            {
                if (predicate(item))
                {
                    toRemove.Add(item);
                }
            }
            
            foreach (T item in toRemove)
            {
                set.Remove(item);
                count++;
            }
            
            return count;
        }
    }
}
