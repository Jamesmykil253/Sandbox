// CharacterStats.cs (v1.1)
// Note: For PUN2, you would add the [PunRPC] attribute to network-sensitive methods like TakeDamage.
using UnityEngine;
using System;
using System.Collections;
using Photon.Pun; // Assuming PUN2 is part of the project for future networking.

namespace Platformer
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Team Affiliation")]
        public Team team;

        [Header("Level & XP")]
        public int level = 1;
        public int currentXp = 0;
        public int xpToNextLevel = 100;

        [Header("Core Stats")]
        public StatBlock baseStats = new StatBlock();

        [Header("Current State")]
        public int currentHealth;
        public bool isInGrass { get; private set; } = false;
        // **NEW**: Tracks if the character is temporarily visible even while in grass.
        public bool isRevealed { get; private set; } = false; 
        private bool _isDead = false;
        private Coroutine _revealCoroutine;

        [Header("Combat")]
        private int _basicAttackCounter = 0;
        private const int ATTACKS_UNTIL_EMPOWERED = 3;

        // Events for other scripts to subscribe to.
        public event Action<bool> OnGrassStatusChanged;
        public event Action<bool> OnRevealStatusChanged; // **NEW**
        public event Action OnHealthChanged;
        public event Action OnDied;
        public event Action OnLevelUp;

        private void Awake()
        {
            currentHealth = baseStats.HP;
        }

        /// <summary>
        /// Sets the character's status of being inside stealth grass.
        /// This is called by the StealthGrass trigger.
        /// </summary>
        public void SetInGrassStatus(bool status)
        {
            isInGrass = status;
            OnGrassStatusChanged?.Invoke(status);
        }

        /// <summary>
        /// **NEW**: This method temporarily reveals the character, such as when they attack from grass.
        /// If the character is already revealed, it resets the timer.
        /// </summary>
        /// <param name="duration">How long the character should remain revealed.</param>
        public void RevealCharacter(float duration)
        {
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

        public void TakeDamage(int damage, CharacterStats damageDealer) 
        { 
            if (_isDead) return; 
            currentHealth -= damage; 
            currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.HP); 
            Debug.Log($"{gameObject.name} took {damage} damage from {damageDealer.name}, and has {currentHealth} HP remaining."); 
            OnHealthChanged?.Invoke(); 
            if (TryGetComponent<EnemyAIController>(out var enemyAI)) 
            { 
                enemyAI.AggroOnDamage(damageDealer.transform); 
            } 
            if (currentHealth <= 0) 
            { 
                if (damageDealer != null) { damageDealer.AddXp(100); } 
                Die(); 
            } 
        }
        
        public void Heal(int amount) 
        { 
            if (_isDead) return;
            currentHealth += amount; 
            currentHealth = Mathf.Clamp(currentHealth, 0, baseStats.HP); 
            OnHealthChanged?.Invoke(); 
        }
        
        public void AddXp(int amount) 
        { 
            if (_isDead) return;
            currentXp += amount; 
            while (currentXp >= xpToNextLevel) 
            { 
                LevelUp(); 
            } 
        }
        
        public bool IsNextAttackEmpowered() 
        { 
            return _basicAttackCounter >= ATTACKS_UNTIL_EMPOWERED - 1; 
        }
        
        public void PerformBasicAttack() 
        { 
            _basicAttackCounter++; 
            if (_basicAttackCounter >= ATTACKS_UNTIL_EMPOWERED) 
            { 
                _basicAttackCounter = 0; 
            } 
        }
        
        private void LevelUp() 
        { 
            level++; 
            currentXp -= xpToNextLevel; 
            xpToNextLevel = (int)(xpToNextLevel * 1.5f); 
            baseStats.HP += 20; 
            baseStats.Attack += 5; 
            baseStats.Defense += 3; 
            baseStats.SpecialAttack += 5; 
            baseStats.SpecialDefense += 3; 
            baseStats.Speed += 0.2f; 
            baseStats.CritRate = Mathf.Min(baseStats.CritRate + 0.02f, 1f); 
            currentHealth = baseStats.HP; 
            Debug.Log($"{gameObject.name} leveled up to Level {level}!"); 
            OnHealthChanged?.Invoke(); 
            OnLevelUp?.Invoke(); 
        }
        
        protected virtual void Die() 
        { 
            if (_isDead) return; 
            _isDead = true; 
            Debug.Log($"{gameObject.name} has died."); 
            OnDied?.Invoke(); 
        }
    }
}
