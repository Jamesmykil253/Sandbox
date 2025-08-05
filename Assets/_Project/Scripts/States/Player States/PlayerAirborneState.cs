// PlayerAirborneState.cs (v1.4 - Added OnExit velocity reset)
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
            // **THE FIX**: This now calls the instant attack method and does NOT change state.
            if (player.ConsumeAttackPress())
            {
                player.ExecuteAttack(this);  // Ensure 'this' for return—though not the error, consistency helps.
            }
        }

        public override void FixedUpdate()
        {
            var v = player.PlayerVelocity;
            if (player.ConsumeJumpBuffer() && player.JumpsRemaining > 0)
            {
                player.JumpsRemaining--;
                v.y += player.doubleJumpBoostVelocity;
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
            var v = player.PlayerVelocity;
            v.y = -2f; // FIX: Reset y-velocity on exit to grounded/idle; why? Ensures snap to stable ground state after drop from sky, like a Pokémon landing firmly without lingering fall momentum—fixes stuck airborne without input in MOBA drops.
            player.PlayerVelocity = v;
            player.UpdateGroundedColor();
        }
    }
}