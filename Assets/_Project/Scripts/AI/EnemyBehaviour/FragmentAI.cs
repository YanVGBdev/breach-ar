using UnityEngine;
using BreachAR.Gameplay;

namespace BreachAR.AI
{
    /// <summary>
    /// Utility AI for fragment decision making
    /// </summary>
    public class FragmentAI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FragmentController fragmentController;
        [SerializeField] private Transform coreTransform;

        [Header("Utility Weights")]
        [SerializeField] private float seekWeight = 1f;
        [SerializeField] private float flankWeight = 0.5f;
        [SerializeField] private float retreatWeight = 0.3f;

        [Header("Detection")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float orbAvoidanceRange = 3f;

        private Transform nearestOrb;
        private Vector3 lastKnownCorePosition;

        private void Start()
        {
            if (coreTransform == null)
            {
                // Find core in scene
                CoreController core = FindObjectOfType<CoreController>();
                if (core != null)
                {
                    coreTransform = core.transform;
                }
            }
        }

        private void Update()
        {
            if (fragmentController == null || !fragmentController.IsAlive) return;

            // Update detection
            DetectOrbs();

            // Calculate utility scores
            float seekScore = CalculateSeekScore();
            float flankScore = CalculateFlankScore();
            float retreatScore = CalculateRetreatScore();

            // Select highest utility action
            FragmentAction selectedAction = FragmentAction.Seek;
            float highestScore = seekScore;

            if (flankScore > highestScore)
            {
                selectedAction = FragmentAction.Flank;
                highestScore = flankScore;
            }

            if (retreatScore > highestScore)
            {
                selectedAction = FragmentAction.Retreat;
                highestScore = retreatScore;
            }

            // Execute action
            ExecuteAction(selectedAction);
        }

        /// <summary>
        /// Detect nearby orbs for avoidance
        /// </summary>
        private void DetectOrbs()
        {
            nearestOrb = null;
            float nearestDistance = orbAvoidanceRange;

            OrbController[] orbs = FindObjectsOfType<OrbController>();
            foreach (OrbController orb in orbs)
            {
                if (!orb.IsActive) continue;

                float distance = Vector3.Distance(transform.position, orb.transform.position);
                if (distance < nearestDistance)
                {
                    nearestOrb = orb.transform;
                    nearestDistance = distance;
                }
            }
        }

        /// <summary>
        /// Calculate seek utility (move toward core)
        /// </summary>
        private float CalculateSeekScore()
        {
            if (coreTransform == null) return 0f;

            float distance = Vector3.Distance(transform.position, coreTransform.position);
            float normalizedDistance = Mathf.Clamp01(distance / detectionRange);

            return seekWeight * normalizedDistance;
        }

        /// <summary>
        /// Calculate flank utility (move to side of core)
        /// </summary>
        private float CalculateFlankScore()
        {
            if (coreTransform == null) return 0f;

            // Flank if there are other fragments between us and core
            Vector3 directionToCore = (coreTransform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, directionToCore);

            // Higher score if facing away from core (need to flank)
            return flankWeight * (1f - dotProduct);
        }

        /// <summary>
        /// Calculate retreat utility (move away from orbs)
        /// </summary>
        private float CalculateRetreatScore()
        {
            if (nearestOrb == null) return 0f;

            float distance = Vector3.Distance(transform.position, nearestOrb.position);
            float normalizedDistance = 1f - Mathf.Clamp01(distance / orbAvoidanceRange);

            // Higher score when orb is close
            return retreatWeight * normalizedDistance;
        }

        /// <summary>
        /// Execute the selected action
        /// </summary>
        private void ExecuteAction(FragmentAction action)
        {
            switch (action)
            {
                case FragmentAction.Seek:
                    SeekCore();
                    break;
                case FragmentAction.Flank:
                    FlankCore();
                    break;
                case FragmentAction.Retreat:
                    RetreatFromOrb();
                    break;
            }
        }

        private void SeekCore()
        {
            if (coreTransform == null) return;

            // Move directly toward core
            Vector3 direction = (coreTransform.position - transform.position).normalized;
            fragmentController.transform.position += direction * fragmentController.FragmentDefinition.MoveSpeed * Time.deltaTime;
            
            // Look at core
            if (direction != Vector3.zero)
            {
                fragmentController.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void FlankCore()
        {
            if (coreTransform == null) return;

            // Move to side of core
            Vector3 directionToCore = (coreTransform.position - transform.position).normalized;
            Vector3 flankDirection = Vector3.Cross(directionToCore, Vector3.up).normalized;
            
            // Alternate left/right based on fragment position
            if (transform.position.x > coreTransform.position.x)
            {
                flankDirection = -flankDirection;
            }

            Vector3 targetPosition = coreTransform.position + flankDirection * 3f;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            fragmentController.transform.position += moveDirection * fragmentController.FragmentDefinition.MoveSpeed * Time.deltaTime;
        }

        private void RetreatFromOrb()
        {
            if (nearestOrb == null) return;

            // Move away from nearest orb
            Vector3 direction = (transform.position - nearestOrb.position).normalized;
            fragmentController.transform.position += direction * fragmentController.FragmentDefinition.MoveSpeed * 0.5f * Time.deltaTime;
        }
    }

    /// <summary>
    /// Fragment AI actions
    /// </summary>
    public enum FragmentAction
    {
        Seek,
        Flank,
        Retreat
    }
}
