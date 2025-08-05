// PlayerAirborneState.cs (v1.4)
using UnityEngine;

namespace Platformer
{
    public class PlayerAirborneState : State
    {
        private float _jumpHoldTimer;

        public PlayerAirborneState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void OnEnter()
        {
            player.SetStateColor(player.airborneColor);
            _jumpHoldTimer = player.jumpHoldDuration;
        }

        public override void Update()
        {
            if (player.ConsumeAttackPress())
            {
                player.ExecuteAttack(this);
            }
        }

        public override void FixedUpdate()
        {
            var v = player.PlayerVelocity;
            if (player.ConsumeJumpBuffer() && player.JumpsRemaining > 0)
            {
                player.JumpsRemaining--;
                v.y = player.doubleJumpBoostVelocity;
                player.SetStateColor(player.doubleJumpColor);
                player.PlayerVelocity = v;
                player.Controller.Move(new Vector3(0, v.y, 0) * Time.fixedDeltaTime);
                return;
            }
            if (v.y > 0)
            {
                if (player.IsJumpButtonPressed && _jumpHoldTimer > 0)
                {
                    v.y = Mathf.Sqrt(player.maxFirstJumpHeight * -2f * player.gravity);
                    _jumpHoldTimer -= Time.deltaTime;
                    player.SetStateColor(player.highJumpColor);
                }
                else
                {
                    player.SetStateColor(player.airborneColor);
                }
            }
            else
            {
                player.SetStateColor(player.airborneColor);
            }
            v.y += player.gravity * Time.fixedDeltaTime;
            if (!player.IsJumpButtonPressed && player.PlayerVelocity.y > 0)
            {
                v.y += player.gravity * player.jumpCutOffMultiplier * Time.fixedDeltaTime;
            }
            player.PlayerVelocity = v;
            Vector3 move = new Vector3(player.MoveInput.x, 0, player.MoveInput.y) * player.MyStats.baseStats.Speed;
            move.y = v.y;
            player.Controller.Move(move * Time.fixedDeltaTime);
        }

        public override void OnExit()
        {
            // **FIX**: Reset y-velocity on exit to a stable grounded value.
            // **WHY**: This is a critical stability fix. If the player falls off a ledge (without jumping)
            // and lands, their y-velocity is still a large negative number. Without resetting it, the
            // CharacterController might not correctly detect the 'IsGrounded' condition on the next frame,
            // leaving the player stuck in an unresponsive airborne state. This ensures a firm, reliable landing.
            var v = player.PlayerVelocity;
            v.y = -2f; 
            player.PlayerVelocity = v;
            player.UpdateGroundedColor();
        }
    }
}
