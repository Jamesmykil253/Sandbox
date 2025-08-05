// EnemyAIController.cs (v1.1)
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Platformer
{
    [RequireComponent(typeof(CharacterStats), typeof(NavMeshAgent), typeof(StateMachine))]
    public class EnemyAIController : MonoBehaviour
    {
        [Header("AI Behavior")]
        public bool canFollowPlayer = true;
        public float leashRadius = 15f;
        public float attackRadius = 2f;
        public LayerMask targetLayerMask;

        [Header("Loot")]
        public GameObject coinPrefab;
        public int coinDropAmount = 3;

        [Header("Debug Colors")]
        public Material debugMaterial;
        public Color idleColor = Color.gray;
        public Color combatColor = Color.magenta;
        public Color returnColor = new Color(1f, 0.5f, 0f);
        public Color empoweredAttackColor = Color.white;
        
        public CharacterStats MyStats { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public Transform PlayerTarget { get; private set; }
        public Vector3 StartPosition { get; private set; }
        public bool IsAggroed { get; private set; }

        private void Awake()
        {
            MyStats = GetComponent<CharacterStats>();
            Agent = GetComponent<NavMeshAgent>();
            StateMachine = GetComponent<StateMachine>();
            StartPosition = transform.position;
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer != null) debugMaterial = renderer.material;
            
            MyStats.OnLevelUp += UpdateAgentSpeed;
            MyStats.OnDied += HandleDeath;
        }

        private void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) PlayerTarget = playerObject.transform;
            UpdateAgentSpeed();
            SetupStateMachine();
        }

        private void OnDestroy()
        {
            if (MyStats != null) { MyStats.OnLevelUp -= UpdateAgentSpeed; MyStats.OnDied -= HandleDeath; }
        }

        private void SetupStateMachine()
        {
            // **FIX**: A new, simplified state machine using the combined Combat state.
            // **WHY**: The AI's logic is simple: if it's angry, it moves towards the player and attacks.
            // If the player runs away or hides, it returns home. This is more efficient and less
            // prone to bugs than managing separate Chase and Attack states.
            var idleState = new EnemyIdleState(this);
            var combatState = new EnemyCombatState(this);
            var returnState = new EnemyReturnState(this);

            StateMachine.AddTransition(idleState, combatState, new FunkPredicate(() => IsAggroed && ShouldChasePlayer()));
            StateMachine.AddTransition(combatState, returnState, new FunkPredicate(() => !ShouldChasePlayer()));
            StateMachine.AddTransition(returnState, idleState, new FunkPredicate(() => Vector3.Distance(transform.position, StartPosition) < 1f));

            StateMachine.SetState(idleState);
        }
        
        /// <summary>
        /// **NEW**: The core decision-making logic for the AI.
        /// </summary>
        private bool ShouldChasePlayer()
        {
            if (PlayerTarget == null || !canFollowPlayer) return false;

            CharacterStats playerStats = PlayerTarget.GetComponent<CharacterStats>();
            if (playerStats == null) return false;

            // **FIX**: Added stealth detection logic.
            // **WHY**: This implements the core stealth mechanic. If the player is in grass and not
            // revealed, most AI will lose sight. The exception for Neutral AI adds a layer of
            // challenge to farming the "jungle" camps.
            if (playerStats.isInGrass && !playerStats.isRevealed)
            {
                if (this.MyStats.team != Team.Neutral)
                {
                    if (IsAggroed) LoseAggro();
                    return false;
                }
            }

            if (IsAggroed && IsPlayerWithinLeash())
            {
                return true;
            }

            return false;
        }

        public void AggroOnDamage(Transform attacker)
        {
            if (canFollowPlayer)
            {
                IsAggroed = true;
                PlayerTarget = attacker;
            }
        }

        public void LoseAggro() { IsAggroed = false; }
        private void HandleDeath() { StateMachine.enabled = false; Agent.enabled = false; foreach (Collider col in GetComponentsInChildren<Collider>()) { col.enabled = false; } StartCoroutine(DeathSequence()); }
        private IEnumerator DeathSequence() { if (coinPrefab != null) { for (int i = 0; i < coinDropAmount; i++) { Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, Random.Range(-0.5f, 0.5f)); Instantiate(coinPrefab, transform.position + randomOffset, Quaternion.identity); } } float fadeDuration = 1.5f; float timer = 0f; Color startColor = debugMaterial.color; while (timer < fadeDuration) { timer += Time.deltaTime; float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration); debugMaterial.color = new Color(startColor.r, startColor.g, startColor.b, alpha); yield return null; } Destroy(gameObject); }
        private void UpdateAgentSpeed() { if(Agent.isOnNavMesh) Agent.speed = MyStats.baseStats.Speed; }
        private void Update() { if (StateMachine.enabled) StateMachine.Tick(); }
        public bool IsPlayerInRadius(float radius) { if (PlayerTarget == null) return false; return Vector3.Distance(transform.position, PlayerTarget.position) <= radius; }
        public bool IsPlayerWithinLeash() { if (PlayerTarget == null) return false; return Vector3.Distance(StartPosition, PlayerTarget.position) <= leashRadius; }
        public void SetStateColor(Color color) { if (debugMaterial != null) debugMaterial.color = color; }
        private void OnDrawGizmosSelected() { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(StartPosition, leashRadius); Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRadius); }
    }
}
