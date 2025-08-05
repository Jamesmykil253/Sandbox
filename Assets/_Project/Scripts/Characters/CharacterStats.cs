// CharacterStats.cs (v1.1 - Added RevealCharacter with timer)
// Note: For PUN2, add [PunRPC] to TakeDamage for sync.
using UnityEngine;
using System;
using System.Collections;
using Photon.Pun; // Assuming PUN2 imported; foundational for multiplayer.

namespace Platformer
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Team Affiliation")]
        public Team team;

        [Header("Current State")]
        public int currentHealth;
        public bool isInGrass { get; private set; } = false;
        public bool isRevealed { get; private set; } = false; // **NEW**: Tracks temporary visibility
        private bool _isDead = false;
        private Coroutine _revealCoroutine;

        // Events for other scripts to listen to
        public event Action<bool> OnGrassStatusChanged;
        public event Action<bool> OnRevealStatusChanged; // **NEW**
        public event Action OnHealthChanged;
        public event Action OnDied;
        public event Action OnLevelUp;

        private void Awake()
        {
            currentHealth = baseStats.HP;
        }

        public void SetInGrassStatus(bool status)
        {
            isInGrass = status;
            OnGrassStatusChanged?.Invoke(status);
        }

        // **NEW**: This method is called by PlayerController when an attack happens.
        public void RevealCharacter(float duration)
        {
            // If we're already revealed, stop the old timer and start a new one.
            if (_revealCoroutine != null)
            {
                StopCoroutine(_revealCoroutine);
            }
            _revealCoroutine = StartCoroutine(RevealTimer(duration));
        }

        private IEnumerator RevealTimer(float duration)
        {
            isRevealed = true;
            OnRevealStatusChanged?.Invoke(true);
            
            yield return new WaitForSeconds(duration);

            isRevealed = false;
            OnRevealStatusChanged?.Invoke(false);
            _revealCoroutine = null;
        }

        [Header("Level & XP")]
        public int level = 1;
        public int currentXp = 0;
        public int xpToNextLevel = 100;
        [Header("Core Stats")]
        public StatBlock baseStats = new StatBlock();
        [Header("Combat")]
        private int _basicAttackCounter = 0;
        private const int ATTACKS_UNTIL_EMPOWERED = 3;
        public void TakeDamage(int damage, CharacterStats damageDealer) { if (_isDead) return; currentHealth -= damage; currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.HP); Debug.Log($"{gameObject.name} took {damage} damage from {damageDealer.name}, and has {currentHealth} HP remaining."); OnHealthChanged?.Invoke(); if (TryGetComponent<EnemyAIController>(out var enemyAI)) { enemyAI.AggroOnDamage(damageDealer.transform); } if (currentHealth <= 0) { if (damageDealer != null) { damageDealer.AddXp(100); } Die(); } } // [PunRPC] for PUN2 sync: public [PunRPC] void TakeDamageRPC(int damage, CharacterStats damageDealer) { TakeDamage(damage, damageDealer); }
        public void Heal(int amount) { currentHealth += amount; currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.HP); OnHealthChanged?.Invoke(); }
        public void AddXp(int amount) { currentXp += amount; if (currentXp >= xpToNextLevel) LevelUp(); }
        public bool IsNextAttackEmpowered() { return _basicAttackCounter >= ATTACKS_UNTIL_EMPOWERED - 1; }
        public void PerformBasicAttack() { _basicAttackCounter++; if (_basicAttackCounter >= ATTACKS_UNTIL_EMPOWERED) { _basicAttackCounter = 0; } }
        private void LevelUp() { level++; currentXp -= xpToNextLevel; xpToNextLevel = (int)(xpToNextLevel * 1.5f); baseStats.HP += 20; baseStats.Attack += 5; baseStats.Defense += 3; baseStats.SpecialAttack += 5; baseStats.SpecialDefense += 3; baseStats.Speed += 0.2f; baseStats.CritRate = Mathf.Min(baseStats.CritRate + 0.02f, 1f); currentHealth = baseStats.HP; Debug.Log($"{gameObject.name} leveled up to Level {level}!"); OnHealthChanged?.Invoke(); OnLevelUp?.Invoke(); }
        protected virtual void Die() { if (_isDead) return; _isDead = true; Debug.Log($"{gameObject.name} has died."); OnDied?.Invoke(); }
    }
}