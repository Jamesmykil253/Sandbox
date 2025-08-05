// EnemyAIController.cs (v1.1 - Integrated stealth checks)
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Platformer
{
    [RequireComponent(typeof(CharacterStats), typeof(NavMeshAgent), typeof(StateMachine))]
    public class EnemyAIController : MonoBehaviour
        {
        // ... (All variables are the same) ...

        private bool ShouldChasePlayer()
        {
            if (PlayerTarget == null || !canFollowPlayer) return false;

            CharacterStats playerStats = PlayerTarget.GetComponent<CharacterStats>();
            if (playerStats == null) return false;

            // --- VISIBILITY CHECK ---
            // If the player is in the grass...
            if (playerStats.isInGrass)
            {
                // ...and I am NOT a neutral enemy, then I cannot see them. Do not chase.
                if (this.MyStats.team != Team.Neutral)
                {
                    // If we were chasing, lose aggro now that they're hidden.
                    if (IsAggroed) LoseAggro();
                    return false;
                }
                // If I AM a neutral enemy, I can see them. The check continues...
            }

            // If we are already aggroed and the player is within our leash range.
            if (IsAggroed && IsPlayerWithinLeash())
            {
                return true;
            }

            return false;
        }

        // ... (The rest of the script is exactly the same) ...
    
    
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
        public Color combatColor = Color.magenta; // New color for the combined state
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
            // --- NEW, SIMPLIFIED STATE MACHINE ---
            var idleState = new EnemyIdleState(this);
            var combatState = new EnemyCombatState(this); // The new, combined state
            var returnState = new EnemyReturnState(this);

            // When attacked, go into combat.
            StateMachine.AddTransition(idleState, combatState, new FunkPredicate(() => IsAggroed));
            
            // If in combat and the player gets outside the leash, return home.
            StateMachine.AddTransition(combatState, returnState, new FunkPredicate(() => !IsPlayerWithinLeash()));
            
            // If returning and we get close to our start position, go back to idle.
            StateMachine.AddTransition(returnState, idleState, new FunkPredicate(() => Vector3.Distance(transform.position, StartPosition) < 1f));

            StateMachine.SetState(idleState);
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
        private void UpdateAgentSpeed() { Agent.speed = MyStats.baseStats.Speed; }
        private void Update() { if (StateMachine.enabled) StateMachine.Tick(); }
        public bool IsPlayerInRadius(float radius) { if (PlayerTarget == null) return false; return Vector3.Distance(transform.position, PlayerTarget.position) <= radius; }
        public bool IsPlayerWithinLeash() { if (PlayerTarget == null) return false; return Vector3.Distance(StartPosition, PlayerTarget.position) <= leashRadius; }
        public void SetStateColor(Color color) { if (debugMaterial != null) debugMaterial.color = color; }
        private void OnDrawGizmosSelected() { Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(StartPosition, leashRadius); Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRadius); }
    }
}