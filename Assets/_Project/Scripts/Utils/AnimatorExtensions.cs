using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Animator extension methods
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Set trigger safely
        /// </summary>
        public static void SetTriggerSafe(this Animator animator, string triggerName)
        {
            if (animator != null && animator.gameObject.activeInHierarchy)
            {
                animator.SetTrigger(triggerName);
            }
        }

        /// <summary>
        /// Set bool safely
        /// </summary>
        public static void SetBoolSafe(this Animator animator, string boolName, bool value)
        {
            if (animator != null && animator.gameObject.activeInHierarchy)
            {
                animator.SetBool(boolName, value);
            }
        }

        /// <summary>
        /// Set integer safely
        /// </summary>
        public static void SetIntegerSafe(this Animator animator, string intName, int value)
        {
            if (animator != null && animator.gameObject.activeInHierarchy)
            {
                animator.SetInteger(intName, value);
            }
        }

        /// <summary>
        /// Set float safely
        /// </summary>
        public static void SetFloatSafe(this Animator animator, string floatName, float value)
        {
            if (animator != null && animator.gameObject.activeInHierarchy)
            {
                animator.SetFloat(floatName, value);
            }
        }

        /// <summary>
        /// Get current state name
        /// </summary>
        public static string GetCurrentStateName(this Animator animator, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("") ? 
                "Any State" : 
                animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash.ToString();
        }

        /// <summary>
        /// Check if in transition
        /// </summary>
        public static bool IsInTransition(this Animator animator, int layerIndex = 0)
        {
            return animator.IsInTransition(layerIndex);
        }

        /// <summary>
        /// Get normalized time of current state
        /// </summary>
        public static float GetNormalizedTime(this Animator animator, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
        }

        /// <summary>
        /// Check if current state is looping
        /// </summary>
        public static bool IsCurrentStateLooping(this Animator animator, int layerIndex = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layerIndex).loop;
        }
    }
}
