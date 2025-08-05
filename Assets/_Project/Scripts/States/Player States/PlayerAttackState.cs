// PlayerAttackState.cs (v1.1)
using UnityEngine;

namespace Platformer
{
    public class PlayerAttackState : State
    {
        private float _attackDuration = 0.5f;
        private float _timer;
        // **FIX**: New variable to track progress from 0 to 1 for smooth interpolation.
        private float _normalizedTimer;
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
            _normalizedTimer = 0f;
            
            // The empowered attack is a projectile, so we only do a melee sphere check for non-empowered attacks.
            if (!isEmpowered)
            {
                FindAndDamageEnemy();
            }
            
            player.MyStats.PerformBasicAttack();
        }

        public override void Update()
        {
            _timer -= Time.deltaTime;
            // **FIX**: Calculate the normalized progress of the attack (0 at the start, 1 at the end).
            _normalizedTimer = 1f - (_timer / _attackDuration);
            if (_timer <= 0f)
            {
                stateMachine.ChangeState(_stateToReturnTo);
            }
        }

        public override void FixedUpdate()
        {
            // Use different movement logic depending on if the attack started in the air or on the ground.
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
            // **FIX**: Use Lerp to smoothly transition from full speed (1f) to the reduced attack speed.
            // **WHY**: This prevents a jarring, instant change in speed. The movement now eases into
            // the slowdown, making the attack animation feel more fluid and polished.
            float currentMultiplier = Mathf.Lerp(1f, player.attackMoveSpeedMultiplier, _normalizedTimer);
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
            // **FIX**: Apply the same smoothing logic to airborne attacks for consistency.
            float currentMultiplier = Mathf.Lerp(1f, player.attackMoveSpeedMultiplier, _normalizedTimer);
            Vector3 move = new Vector3(player.MoveInput.x, 0, player.MoveInput.y) * player.MyStats.baseStats.Speed * currentMultiplier;
            move.y = v.y;
            player.Controller.Move(move * Time.fixedDeltaTime);
        }

        private void FindAndDamageEnemy()
        {
            Collider[] hits = Physics.OverlapSphere(player.transform.position, player.meleeAttackRadius, player.attackLayerMask);
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
                int damage = CombatCalculator.CalculateDamage(player.MyStats, closestEnemy.MyStats, false);
                closestEnemy.MyStats.TakeDamage(damage, player.MyStats);
            }
        }
    }
}
