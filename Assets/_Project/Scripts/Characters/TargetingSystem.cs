// TargetingSystem.cs (v1.0)
using UnityEngine;
using System.Collections.Generic;

namespace Platformer
{
    // Defines the different ways the system can prioritize targets.
    public enum TargetingPriority
    {
        LowestAbsoluteHP,
        LowestPercentageHP
    }

    public class TargetingSystem : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The current targeting priority used by the system.")]
        public TargetingPriority currentPriority = TargetingPriority.LowestPercentageHP;

        [Tooltip("The maximum range to scan for targets.")]
        public float targetingRange = 10f;

        [Tooltip("The layer mask that contains enemy characters.")]
        public LayerMask enemyLayerMask;

        /// <summary>
        /// Finds the best enemy target based on the current targeting priority.
        /// </summary>
        /// <returns>The Transform of the best target, or null if no valid target is found.</returns>
        public Transform FindBestTarget()
        {
            // Find all potential targets within the targeting range.
            Collider[] hits = Physics.OverlapSphere(transform.position, targetingRange, enemyLayerMask);

            Transform bestTarget = null;
            float bestTargetValue = float.MaxValue;

            if (hits.Length == 0)
            {
                return null; // No targets in range.
            }

            // Iterate through all found colliders to find the best one.
            foreach (var hit in hits)
            {
                // We need to get the CharacterStats to evaluate the target.
                if (hit.TryGetComponent<CharacterStats>(out var stats))
                {
                    // Skip dead enemies.
                    if (stats.currentHealth <= 0) continue;

                    float currentTargetValue = 0;

                    // Calculate the value to compare based on the selected priority.
                    switch (currentPriority)
                    {
                        case TargetingPriority.LowestAbsoluteHP:
                            currentTargetValue = stats.currentHealth;
                            break;
                        
                        case TargetingPriority.LowestPercentageHP:
                            // Ensure we don't divide by zero if max HP is somehow 0.
                            if (stats.baseStats.HP > 0)
                            {
                                currentTargetValue = (float)stats.currentHealth / stats.baseStats.HP;
                            }
                            break;
                    }

                    // If this target is "better" (has a lower value) than our previous best, it's the new best.
                    if (currentTargetValue < bestTargetValue)
                    {
                        bestTargetValue = currentTargetValue;
                        bestTarget = hit.transform;
                    }
                }
            }

            return bestTarget;
        }

        // This allows us to see the targeting range in the editor for easy debugging.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, targetingRange);
        }
    }
}
