// StealthGrass.cs (v1.1)
using UnityEngine;

namespace Platformer
{
    public class StealthGrass : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // **FIX**: Added trigger logic to make the script functional.
            // **WHY**: The script was previously empty. This implementation checks for a "Player" tag,
            // gets their CharacterStats component, and informs it that the player is now in grass.
            // This enables all the downstream stealth logic in the AI and PlayerController.
            if (other.CompareTag("Player") && other.TryGetComponent<CharacterStats>(out var stats))
            {
                stats.SetInGrassStatus(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // When the player leaves the trigger, we reverse the status.
            if (other.CompareTag("Player") && other.TryGetComponent<CharacterStats>(out var stats))
            {
                stats.SetInGrassStatus(false);
            }
        }
    }
}
