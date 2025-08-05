// PlayerScoringState.cs (v1.1 - No changes from v1.0)
using UnityEngine;

namespace Platformer
{
    public class PlayerScoringState : State
    {
        private GoalZone _currentGoalZone;
        private float _scoringTimer;

        public PlayerScoringState(PlayerController player, StateMachine stateMachine, GoalZone goalZone) 
            : base(player, stateMachine) 
        {
            _currentGoalZone = goalZone;
        }

        public override void OnEnter()
        {
            // **THE FIX**: This now correctly uses 'timePerCoin' from the GoalZone
            // to calculate the total time based on the number of coins.
            _scoringTimer = player.coinCount * _currentGoalZone.timePerCoin;
            
            Debug.Log($"Starting to score {player.coinCount} coins. This will take {_scoringTimer:F2} seconds.");
            _currentGoalZone.StartScoringVisual();
            
            var v = player.PlayerVelocity;
            v.x = 0;
            v.z = 0;
            player.PlayerVelocity = v;
        }

        public override void Update()
        {
            _scoringTimer -= Time.deltaTime;

            if (_scoringTimer <= 0f)
            {
                player.ScorePoints(player.coinCount);
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
                return;
            }

            if (!player.IsScoreButtonPressed || player.MoveInput != Vector2.zero)
            {
                Debug.Log("Scoring cancelled!");
                stateMachine.ChangeState(new PlayerIdleState(player, stateMachine));
                return;
            }
        }

        public override void OnExit()
        {
            _currentGoalZone.StopScoringVisual();
        }
    }
}