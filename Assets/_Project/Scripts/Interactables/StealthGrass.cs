// StealthGrass.cs (v1.1 - Added trigger logic)
using UnityEngine;

namespace Platformer
{
    public class StealthGrass : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // FIX: Added trigger logic; why? Empty script did nothing—now sets hidden status for stealth plays, like sneaking to score without detection (except by neutrals).
            if (other.CompareTag("Player") && other.TryGetComponent<CharacterStats>(out var stats))  // Limit to player; AI don't hide.
            {
                stats.SetInGrassStatus(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent<CharacterStats>(out var stats))
            {
                stats.SetInGrassStatus(false);
            }
        }
    }
}