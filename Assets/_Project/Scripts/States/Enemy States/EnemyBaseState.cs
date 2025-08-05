// EnemyBaseState.cs (v1.1)
using UnityEngine;

namespace Platformer
{
    public abstract class EnemyBaseState : IState 
    { 
        protected readonly EnemyAIController enemy; 
        protected EnemyBaseState(EnemyAIController enemy) { this.enemy = enemy; } 
        public virtual void OnEnter() { } 
        public virtual void Update() { } 
        public virtual void FixedUpdate() { } 
        public virtual void OnExit() { } 
    }
    
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

    // **FIX**: New combined combat state.
    // **WHY**: This simplifies the AI's logic dramatically. Instead of needing to transition
    // between a "Chase" state and an "Attack" state, this single "Combat" state handles both
    // responsibilities. It always tries to move toward the player while simultaneously checking
    // if it's able to attack. This is more efficient and less prone to getting stuck.
    public class EnemyCombatState : EnemyBaseState
    {
        private float _attackCooldown;
        private float _attackTimer;

        public EnemyCombatState(EnemyAIController enemy) : base(enemy) { }
        
        public override void OnEnter()
        {
            enemy.SetStateColor(enemy.combatColor);
            _attackCooldown = 1f / enemy.MyStats.baseStats.AttackSpeed;
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.isStopped = false;
                enemy.Agent.stoppingDistance = 0; 
            }
            _attackTimer = 0; // Can attack immediately.
        }

        public override void Update()
        {
            if (enemy.PlayerTarget != null && enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.SetDestination(enemy.PlayerTarget.position);
            }

            _attackTimer -= Time.deltaTime;
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

            bool isEmpowered = enemy.MyStats.IsNextAttackEmpowered();
            enemy.SetStateColor(isEmpowered ? enemy.empoweredAttackColor : enemy.combatColor);

            Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.attackRadius, enemy.targetLayerMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Hurtbox>(out var hurtbox) && hurtbox.statsController == playerStats)
                {
                    int damage = CombatCalculator.CalculateDamage(enemy.MyStats, playerStats, isEmpowered);
                    playerStats.TakeDamage(damage, enemy.MyStats);
                    break; 
                }
            }
            
            enemy.MyStats.PerformBasicAttack();
        }

        public override void OnExit()
        {
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
            enemy.LoseAggro();
            enemy.SetStateColor(enemy.returnColor); 
            if (enemy.Agent.isOnNavMesh) 
            { 
                enemy.Agent.isStopped = false; 
                enemy.Agent.SetDestination(enemy.StartPosition); 
            } 
        } 
    }
}
