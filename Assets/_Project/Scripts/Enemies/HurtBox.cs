// Hurtbox.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    // This component's only job is to receive damage and pass it to the main stats controller.
    public class Hurtbox : MonoBehaviour
    {
        // We will link this to the main EnemyAIController in the Inspector.
        public CharacterStats statsController;
    }
}