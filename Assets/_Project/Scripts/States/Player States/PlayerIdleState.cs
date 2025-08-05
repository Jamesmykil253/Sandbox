// PlayerIdleState.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    public class PlayerIdleState : State
    {
        public PlayerIdleState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void OnEnter()
        {
            player.JumpsRemaining = 1;
            var v = player.PlayerVelocity;
            v.y = -2f;
            v.x = 0;
            v.z = 0;
            player.PlayerVelocity = v;
            player.UpdateGroundedColor();
        }

        public override void Update()
        {
            // **THE FIX**: Use the instant attack method and does NOT change state.
            if (player.ConsumeAttackPress())
            {
                player.ExecuteAttack(this);  // FIX: Added 'this' as stateToReturnTo; why? Tells attack to return here (idle) after—prevents state loss, keeping ambushes stealthy without forcing movement.
            }
            if (player.ConsumeJumpBuffer())
            {
                var v = player.PlayerVelocity;
                v.y = Mathf.Sqrt(player.initialJumpHeight * -2f * player.gravity);
                player.PlayerVelocity = v;
                return;
            }
            if (player.MoveInput != Vector2.zero)
            {
                stateMachine.ChangeState(new PlayerGroundedState(player, stateMachine));
            }
        }

        public override void FixedUpdate()
        {
            player.Controller.Move(new Vector3(0, player.PlayerVelocity.y, 0) * Time.fixedDeltaTime);
        }
    }
}