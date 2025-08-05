// GoalZone.cs (v1.2)
using UnityEngine;
using System.Collections;

namespace Platformer
{
    public class GoalZone : MonoBehaviour
    {
        [Header("Goal Settings")]
        public Team team;
        public int scoreCapacity = 100;
        
        // **FIX**: Added a configurable time-per-coin value.
        // **WHY**: This allows for fine-tuning game balance. Instead of a fixed scoring time,
        // the duration is now proportional to the number of points being scored, making
        // large scores riskier and more impactful.
        [Tooltip("How many seconds it takes to score a single coin.")]
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
            // **FIX**: Added a null check for robustness.
            // **WHY**: This prevents a potential "NullReferenceException" if the player object
            // is destroyed at the exact moment it enters the trigger.
            if (player == null) return;
            _isPlayerInRange = true;
            StartCoroutine(ZoneActiveCoroutine(player));
        }

        public void OnPlayerExit()
        {
            _isPlayerInRange = false;
            StopScoringVisual();
        }

        private IEnumerator ZoneActiveCoroutine(PlayerController player)
        {
            while (_isPlayerInRange)
            {
                // **FIX**: Added a check to prevent healing in neutral zones.
                // **WHY**: This is a game balance decision. Neutral zones are for scoring only
                // and should not provide a safe healing spot for either team.
                if (player.MyStats.team == this.team && this.team != Team.Neutral)
                {
                    player.MyStats.Heal(healingPerSecond);
                    _goalMaterial.color = Color.green;
                    yield return new WaitForSeconds(0.2f);
                    _goalMaterial.color = _originalColor;
                    yield return new WaitForSeconds(0.8f);
                }
                else
                {
                    yield return new WaitForSeconds(1.0f); // Wait a second before checking again
                }
            }
        }

        public void StartScoringVisual() { if (_goalMaterial != null) _goalMaterial.color = Color.white; }
        public void StopScoringVisual() { if (_goalMaterial != null) _goalMaterial.color = _originalColor; }
    }
}
