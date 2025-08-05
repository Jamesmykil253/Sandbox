// PlayerIdleState.cs (v1.2)
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
            // **FIX**: Added the check to initiate scoring.
            // **WHY**: This was the missing piece of logic. The state now constantly checks if the
            // player is holding the score button AND if they are in a valid position to score.
            // If both are true, it transitions to the PlayerScoringState.
            if (player.IsScoreButtonPressed && player.CanStartScoring())
            {
                stateMachine.ChangeState(new PlayerScoringState(player, stateMachine, player.GetCurrentGoalZone()));
                return; // Exit early to prevent other actions this frame.
            }

            if (player.ConsumeAttackPress())
            {
                player.ExecuteAttack(this);
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
