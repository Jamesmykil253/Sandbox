// Projectile.cs (v1.1 - No changes from v1.0)
// Note: For PUN2, use Photon.Instantiate in PlayerController when spawning.
using UnityEngine;

namespace Platformer
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 3f;

        private int _damage;
        private CharacterStats _owner;
        private bool _isEmpowered;
        private LayerMask _targetLayerMask;

        public void Initialize(CharacterStats owner, int damage, bool isEmpowered, LayerMask targetLayerMask)
        {
            _owner = owner;
            _damage = damage;
            _isEmpowered = isEmpowered;
            _targetLayerMask = targetLayerMask;

            // Destroy the projectile after its lifetime expires to clean up the scene.
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // Move the projectile forward every frame.
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        // This is called when the projectile's trigger collider hits another collider.
        private void OnTriggerEnter(Collider other)
        {
            // Check if the object we hit is on a layer we can damage.
            if ((_targetLayerMask.value & (1 << other.gameObject.layer)) > 0)
            {
                // Check if the object has a hurtbox.
                if (other.TryGetComponent<Hurtbox>(out var hurtbox))
                {
                    // Make sure we are not hitting ourselves or our own team.
                    if (hurtbox.statsController != _owner && hurtbox.statsController.team != _owner.team)
                    {
                        Debug.Log($"{_owner.name}'s projectile hit {hurtbox.statsController.name}!");
                        hurtbox.statsController.TakeDamage(_damage, _owner);
                        Destroy(gameObject); // Destroy the projectile on impact.
                    }
                }
            }
        }
    }
}