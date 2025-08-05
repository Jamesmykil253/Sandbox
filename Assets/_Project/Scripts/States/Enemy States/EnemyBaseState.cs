// EnemyBaseState.cs (v1.1 - Updated EnemyCombatState with attackTimer reset)
using UnityEngine;

namespace Platformer
{
    public abstract class EnemyBaseState : IState { public virtual void OnEnter() { } public virtual void Update() { } public virtual void FixedUpdate() { } public virtual void OnExit() { } protected readonly EnemyAIController enemy; protected EnemyBaseState(EnemyAIController enemy) { this.enemy = enemy; } }
    
    public class EnemyIdleState : EnemyBaseState 
    { 
        public EnemyIdleState(EnemyAIController enemy) : base(enemy) { } 
        public override void OnEnter() 
        { 
            enemy.SetStateColor(enemy.idleColor); 
            if (enemy.Agent.isOnNavMesh) enemy.Agent.isStopped = true; 
            enemy.LoseAggro(); 
        } 
    }

    // --- NEW COMBAT STATE ---
    public class EnemyCombatState : EnemyBaseState
    {
        private float _attackCooldown = 1.5f;
        private float _attackTimer;

        public EnemyCombatState(EnemyAIController enemy) : base(enemy) { }
        
        public override void OnEnter()
        {
            enemy.SetStateColor(enemy.combatColor);
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.isStopped = false;
                // We don't use stopping distance; we control attacks manually.
                enemy.Agent.stoppingDistance = 0; 
            }
            _attackTimer = 0; // Can attack immediately upon entering combat.
        }

        public override void Update()
        {
            // --- MOVEMENT LOGIC ---
            // Always try to move towards the player.
            if (enemy.PlayerTarget != null && enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.SetDestination(enemy.PlayerTarget.position);
            }

            // --- ATTACK LOGIC ---
            _attackTimer -= Time.deltaTime;
            // Check if we can attack (cooldown ready AND in range).
            if (_attackTimer <= 0 && enemy.IsPlayerInRadius(enemy.attackRadius))
            {
                Attack();
                _attackTimer = _attackCooldown;
            }
        }

        private void Attack()
        {
            CharacterStats playerStats = enemy.PlayerTarget.GetComponent<CharacterStats>();
            if (playerStats == null) return;

            Debug.Log("--- ENEMY ATTACKING ---");

            bool isEmpowered = enemy.MyStats.IsNextAttackEmpowered();
            // Flash the empowered color for the attack itself.
            enemy.SetStateColor(isEmpowered ? enemy.empoweredAttackColor : enemy.combatColor);

            Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.attackRadius, enemy.targetLayerMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Hurtbox>(out var hurtbox) && hurtbox.statsController == playerStats)
                {
                    int damage = CombatCalculator.CalculateDamage(enemy.MyStats, playerStats, isEmpowered);
                    playerStats.TakeDamage(damage, enemy.MyStats);
                }
            }
            
            enemy.MyStats.PerformBasicAttack();
        }

        public override void OnExit()
        {
            // When leaving combat, reset the agent's path.
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.ResetPath();
            }
        }
    }

    public class EnemyReturnState : EnemyBaseState 
    { 
        public EnemyReturnState(EnemyAIController enemy) : base(enemy) { } 
        public override void OnEnter() 
        { 
            enemy.SetStateColor(enemy.returnColor); 
            if (enemy.Agent.isOnNavMesh) 
            { 
                enemy.Agent.isStopped = false; 
                enemy.Agent.SetDestination(enemy.StartPosition); 
            } 
        } 
    }
}