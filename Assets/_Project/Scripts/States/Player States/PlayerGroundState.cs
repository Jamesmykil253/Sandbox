// PlayerGroundedState.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    public class PlayerGroundedState : State
    {
        public PlayerGroundedState(PlayerController player, StateMachine stateMachine) 
            : base(player, stateMachine) { }

        public override void OnEnter()
        {
            player.JumpsRemaining = 1; 
            var v = player.PlayerVelocity; 
            v.y = -2f; 
            player.PlayerVelocity = v;
            player.UpdateGroundedColor();
        }

        public override void Update()
        {
            // **THE FIX**: This now calls the instant attack method and does NOT change state.
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
            if (player.MoveInput == Vector2.zero) 
            { 
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine)); 
            }
        }

        public override void FixedUpdate()
        {
            Vector3 moveDirection = new Vector3(player.MoveInput.x, 0, player.MoveInput.y);
            Vector3 movement = moveDirection * player.MyStats.baseStats.Speed;
            movement.y = player.PlayerVelocity.y;
            player.Controller.Move(movement * Time.fixedDeltaTime);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}