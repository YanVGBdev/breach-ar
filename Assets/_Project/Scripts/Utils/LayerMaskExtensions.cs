using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// LayerMask extension methods
    /// </summary>
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// Check if layer is in mask
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// Check if layer is in mask
        /// </summary>
        public static bool Contains(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return mask.Contains(layer);
        }

        /// <summary>
        /// Get layer from mask
        /// </summary>
        public static int ToLayer(this LayerMask mask)
        {
            int value = mask.value;
            int layer = 0;
            while (value > 1)
            {
                value >>= 1;
                layer++;
            }
            return layer;
        }

        /// <summary>
        /// Create mask from layers
        /// </summary>
        public static LayerMask CreateMask(params int[] layers)
        {
            int mask = 0;
            foreach (int layer in layers)
            {
                mask |= (1 << layer);
            }
            return mask;
        }

        /// <summary>
        /// Create mask from layer names
        /// </summary>
        public static LayerMask CreateMask(params string[] layerNames)
        {
            int mask = 0;
            foreach (string layerName in layerNames)
            {
                int layer = LayerMask.NameToLayer(layerName);
                if (layer >= 0)
                {
                    mask |= (1 << layer);
                }
            }
            return mask;
        }

        /// <summary>
        /// Add layer to mask
        /// </summary>
        public static LayerMask AddLayer(this LayerMask mask, int layer)
        {
            return mask | (1 << layer);
        }

        /// <summary>
        /// Remove layer from mask
        /// </summary>
        public static LayerMask RemoveLayer(this LayerMask mask, int layer)
        {
            return mask & ~(1 << layer);
        }

        /// <summary>
        /// Toggle layer in mask
        /// </summary>
        public static LayerMask ToggleLayer(this LayerMask mask, int layer)
        {
            return mask ^ (1 << layer);
        }
    }
}
