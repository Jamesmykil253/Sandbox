// Coin.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    public class Coin : MonoBehaviour
    {
        [Tooltip("How fast the coin spins.")]
        public float spinSpeed = 90f;
        [Tooltip("The number of points this coin is worth.")]
        public int coinValue = 1;

        private void Update()
        {
            // Rotate the coin around its up axis (Y-axis) over time.
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }

        public void Collect()
        {
            // When collected, we simply destroy the coin GameObject.
            // We can add sound effects or particles here later.
            Destroy(gameObject);
        }
    }
}