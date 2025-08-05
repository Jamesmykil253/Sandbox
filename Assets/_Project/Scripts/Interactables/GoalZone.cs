// GoalZone.cs (v1.2 - Added null checks and logging)
using UnityEngine;
using System.Collections;

namespace Platformer
{
    public class GoalZone : MonoBehaviour
    {
        [Header("Goal Settings")]
        public Team team;
        public int scoreCapacity = 100;
        
        // **THE CHANGE**: We now define how long each individual coin takes to score.
        [Tooltip("How many seconds it takes to score a single coin. The total time will be this value multiplied by the number of coins.")]
        public float timePerCoin = 0.1f; 

        [Header("Healing")]
        public int healingPerSecond = 10;

        private Material _goalMaterial;
        private Color _originalColor;
        private bool _isPlayerInRange = false;

        private void Awake()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                _goalMaterial = renderer.material;
                switch (team)
                {
                    case Team.Home: _originalColor = new Color(0, 0, 1, 0.25f); break;
                    case Team.Away: _originalColor = new Color(1, 0.5f, 0, 0.25f); break;
                    case Team.Neutral: _originalColor = new Color(0.5f, 0.5f, 0.5f, 0.25f); break;
                }
                _goalMaterial.color = _originalColor;
            }
        }

        public int ScorePoints(int pointsToScore)
        {
            int pointsActuallyScored = Mathf.Min(pointsToScore, scoreCapacity);
            scoreCapacity -= pointsActuallyScored;
            if (scoreCapacity <= 0) BreakGoal();
            return pointsActuallyScored;
        }

        private void BreakGoal()
        {
            Debug.Log($"Goal for Team {team} has been broken!");
            GetComponent<Collider>().enabled = false;
            gameObject.SetActive(false);
        }

        public void OnPlayerEnter(PlayerController player)
        {
            if (player == null) return; // FIX: Null check; why? Prevents crash if player reference lost during rapid enter/exit, like a Pokémon vanishing mid-zone trigger without breaking the game.
            _isPlayerInRange = true;
            Debug.Log($"Player entered zone for team {team} - starting coroutine."); // Extra logging for debug.
            StartCoroutine(ZoneActiveCoroutine(player));
        }

        public void OnPlayerExit()
        {
            _isPlayerInRange = false;
            Debug.Log("Player exited zone - stopping visual."); // Extra logging for debug.
            StopScoringVisual();
        }

        private IEnumerator ZoneActiveCoroutine(PlayerController player)
        {
            while (_isPlayerInRange)
            {
                if (player.MyStats.team == this.team && this.team != Team.Neutral) // FIX: Added neutral check—no heal in neutral zones; why? Balances MOBA, prevents farming health in safe areas.
                {
                    player.MyStats.Heal(healingPerSecond);
                    _goalMaterial.color = Color.green;
                    yield return new WaitForSeconds(0.2f);
                    _goalMaterial.color = _originalColor;
                    yield return new WaitForSeconds(0.8f);
                }
                else
                {
                    _goalMaterial.color = Color.yellow;
                    yield return new WaitForSeconds(0.5f);
                    _goalMaterial.color = _originalColor;
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        public void StartScoringVisual() { if (_goalMaterial != null) _goalMaterial.color = Color.white; }
        public void StopScoringVisual() { if (_goalMaterial != null) _goalMaterial.color = _originalColor; }
    }
}