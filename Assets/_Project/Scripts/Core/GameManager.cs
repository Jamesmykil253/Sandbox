using UnityEngine;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        // This is a "Singleton" pattern. It ensures there is only ever one GameManager
        // and that any script can easily access it.
        public static GameManager Instance { get; private set; }

        public int homeTeamScore = 0;
        public int awayTeamScore = 0;

        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void AddScore(Team team, int points)
        {
            if (team == Team.Home)
            {
                homeTeamScore += points;
                Debug.Log($"Home Team scored {points} points! Total: {homeTeamScore}");
            }
            else if (team == Team.Away)
            {
                awayTeamScore += points;
                Debug.Log($"Away Team scored {points} points! Total: {awayTeamScore}");
            }
        }
    }
}
