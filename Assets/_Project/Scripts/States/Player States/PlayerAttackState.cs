// PlayerAttackState.cs (v1.1 - Updated with Lerp smoothing)
using UnityEngine;

namespace Platformer
{
    public class PlayerAttackState : State
    {
        private float _attackDuration = 0.5f;
        private float _timer;
        private float _normalizedTimer;  // FIX: New var for 0-1 progress—used in Lerp for gradual slowdown, like easing a car brake instead of slamming.
        private IState _stateToReturnTo;

        public PlayerAttackState(PlayerController player, StateMachine stateMachine, IState stateToReturnTo) : base(player, stateMachine)
        {
            _stateToReturnTo = stateToReturnTo;
        }

        public override void OnEnter()
        {
            bool isEmpowered = player.MyStats.IsNextAttackEmpowered();
            player.SetStateColor(isEmpowered ? player.empoweredAttackColor : player.attackColor);
            _timer = _attackDuration;
            _normalizedTimer = 0f;  // Start at 0 for Lerp.
            FindAndDamageEnemy(isEmpowered);
            player.MyStats.PerformBasicAttack();
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            _normalizedTimer = 1f - (_timer / _attackDuration);  // FIX: 0 at start, 1 at end—progress for easing.
            if (_timer <= 0f)
            {
                stateMachine.ChangeState(_stateToReturnTo);
            }
        }

        public override void FixedUpdate()
        {
            if (_stateToReturnTo is PlayerAirborneState)
            {
                ApplyAirborneMovement();
            }
            else
            {
                ApplyGroundedMovement();
            }
        }

        private void ApplyGroundedMovement()
        {
            Vector3 moveDirection = new Vector3(player.MoveInput.x, 0, player.MoveInput.y);
            float currentMultiplier = Mathf.Lerp(1f, player.attackMoveSpeedMultiplier, _normalizedTimer);  // FIX: Lerp from full speed (1f) to half—gradual slowdown over duration, like Unite's smooth attack motion (feels natural, not jerky).
            Vector3 movement = moveDirection * player.MyStats.baseStats.Speed * currentMultiplier;
            movement.y = player.PlayerVelocity.y;
            player.Controller.Move(movement * Time.fixedDeltaTime);
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.rotationSpeed * Time.fixedDeltaTime);
            }
        }

        private void ApplyAirborneMovement()
        {
            var v = player.PlayerVelocity;
            v.y += player.gravity * Time.fixedDeltaTime;
            player.PlayerVelocity = v;
            float currentMultiplier = Mathf.Lerp(1f, player.attackMoveSpeedMultiplier, _normalizedTimer);  // FIX: Same Lerp for air—eases speed during jump attacks, preventing "frozen" feel while mid-air battling AI for XP.
            Vector3 move = new Vector3(player.MoveInput.x, 0, player.MoveInput.y) * player.MyStats.baseStats.Speed * currentMultiplier;
            move.y = v.y;
            player.Controller.Move(move * Time.fixedDeltaTime);
        }

        private void FindAndDamageEnemy(bool isEmpowered)
        {
            Collider[] hits = Physics.OverlapSphere(player.transform.position, player.meleeAttackRadius);
            EnemyAIController closestEnemy = null;
            float closestDist = float.MaxValue;
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<EnemyAIController>(out var enemy))
                {
                    float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestEnemy = enemy;
                    }
                }
            }
            if (closestEnemy != null)
            {
                int damage = CombatCalculator.CalculateDamage(player.MyStats, closestEnemy.MyStats, isEmpowered);
                closestEnemy.MyStats.TakeDamage(damage, player.MyStats);
            }
        }
    }
}