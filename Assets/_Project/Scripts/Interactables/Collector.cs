// Collector.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    // This component requires a Collider to function.
    [RequireComponent(typeof(Collider))]
    public class Collector : MonoBehaviour
    {
        // We will link this to the main PlayerController in the Inspector.
        private PlayerController _playerController;

        private void Awake()
        {
            // Find the PlayerController on the parent object.
            _playerController = GetComponentInParent<PlayerController>();
        }

        // This is a built-in Unity method that is called whenever this trigger
        // overlaps with another collider.
        private void OnTriggerEnter(Collider other)
        {
            // We check if the object we overlapped with has a Coin component.
            if (other.TryGetComponent<Coin>(out Coin coin))
            {
                // If it's a coin, tell the PlayerController to collect it.
                if (_playerController != null)
                {
                    _playerController.CollectCoin(coin);
                }
            }
        }
    }
}