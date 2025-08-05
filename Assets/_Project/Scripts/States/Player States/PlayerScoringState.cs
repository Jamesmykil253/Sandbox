// PlayerScoringState.cs (v1.1)
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
            // **FIX**: The scoring time is now calculated dynamically.
            // **WHY**: This is a core balancing mechanic. Scoring more points should take more time,
            // creating a risk/reward scenario. This calculation uses the 'timePerCoin' value
            // from the GoalZone to ensure the timing is configurable per-goal if needed.
            _scoringTimer = player.coinCount * _currentGoalZone.timePerCoin;
            
            Debug.Log($"Starting to score {player.coinCount} coins. This will take {_scoringTimer:F2} seconds.");
            _currentGoalZone.StartScoringVisual();
            
            // Stop player movement while scoring.
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

            // If the player moves or releases the score button, cancel the action.
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
