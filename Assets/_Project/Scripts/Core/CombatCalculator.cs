// CombatCalculator.cs (v1.1 - Minor logging for crits)
using UnityEngine;

namespace Platformer
{
    public static class CombatCalculator
    {
        private const float EMPOWERED_MULTIPLIER = 1.5f;
        private const float CRITICAL_MULTIPLIER = 1.5f;

        /// <summary>
        /// Calculates damage based on a formula inspired by Pokémon Unite.
        /// </summary>
        public static int CalculateDamage(CharacterStats attacker, CharacterStats defender, bool isEmpowered)
        {
            // --- Step 1: Get Base Stats ---
            int attackerLevel = attacker.level;
            int attackerAttack = attacker.baseStats.Attack;
            int defenderDefense = defender.baseStats.Defense;

            // --- Step 2: Calculate Raw Damage (The Unite Formula Core) ---
            // This formula is a simplified version of the one found on Unite-DB.
            // It ensures that the attacker's level and Attack stat are the primary factors,
            // while the defender's Defense provides damage reduction.
            // The number 600 is a standard "denominator" used in the real game's math.
            float rawDamage = (attackerAttack * 1.2f) + (attackerLevel * 20);

            // Apply defense reduction.
            float defenseMitigation = 1 - (defenderDefense / (defenderDefense + 600f));
            float damageAfterDefense = rawDamage * defenseMitigation;

            // --- Step 3: Apply Multipliers ---
            // Apply the empowered attack bonus if applicable.
            if (isEmpowered)
            {
                damageAfterDefense *= EMPOWERED_MULTIPLIER;
            }

            // Check for a critical hit.
            bool isCritical = Random.value < attacker.baseStats.CritRate;
            if (isCritical)
            {
                damageAfterDefense *= CRITICAL_MULTIPLIER;
                Debug.Log("Critical Hit!");
            }

            // --- Step 4: Finalize Damage ---
            // Add a small amount of randomness (+/- 5%) for variety.
            float randomModifier = Random.Range(0.95f, 1.05f);
            int finalDamage = Mathf.RoundToInt(damageAfterDefense * randomModifier);

            // Ensure every attack does at least 1 damage.
            return Mathf.Max(1, finalDamage);
        }
    }
}