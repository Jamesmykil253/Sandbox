// StatBlock.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    [System.Serializable]
    public class StatBlock
    {
        [Tooltip("Health Points: The character's total health pool.")]
        public int HP = 100;

        [Tooltip("Physical Attack: Damage dealt by basic attacks.")]
        public int Attack = 10;

        [Tooltip("Physical Defense: Damage reduction from basic attacks.")]
        public int Defense = 5;

        [Tooltip("Special Attack: Damage dealt by special abilities.")]
        public int SpecialAttack = 10;

        [Tooltip("Special Defense: Damage reduction from special abilities.")]
        public int SpecialDefense = 5;

        [Tooltip("Movement Speed: How fast the character moves.")]
        public float Speed = 5f;

        [Tooltip("Critical Hit Rate: Chance to deal extra damage (0.0 to 1.0).")]
        [Range(0f, 1f)]
        public float CritRate = 0.05f;

        // **NEW STAT**
        [Tooltip("Attacks Per Second: How many basic attacks the character can perform in one second.")]
        public float AttackSpeed = 1.0f;
    }
}