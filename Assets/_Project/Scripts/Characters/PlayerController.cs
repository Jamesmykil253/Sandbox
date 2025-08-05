// PlayerController.cs (v1.9)
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

namespace Platformer
{
    // **FIX**: The [RequireComponent] attribute must be stacked for each component.
    // **WHY**: C# attribute syntax does not allow multiple arguments in this context. To tell
    // Unity that this script requires several other components, we must provide a separate
    // attribute declaration for each one. This resolves the CS1729 compiler error.
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StateMachine))]
    [RequireComponent(typeof(CharacterStats))]
    [RequireComponent(typeof(TargetingSystem))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Combat Settings")]
        [Tooltip("The radius for melee attacks (first two basic).")]
        public float meleeAttackRadius = 2f;
        [Tooltip("The projectile prefab for the empowered third attack.")]
        public GameObject boostedProjectilePrefab;
        [Tooltip("Movement speed multiplier during attack cast.")]
        public float attackMoveSpeedMultiplier = 0.5f;
        [Tooltip("Layer mask for attack targets.")]
        public LayerMask attackLayerMask;
        [Tooltip("Reveal duration after attacking from grass.")]
        public float revealDurationOnAttack = 2f;

        [Header("Team")]
        public Team team;

        [Header("Scoring")]
        public int coinCount = 0;
        private GoalZone _currentGoalZone = null;
        public bool IsScoreButtonPressed { get; private set; }

        [Header("Asset References")]
        [SerializeField] private InputReader inputReader;

        [Header("Visuals & Debugging")]
        public Material debugMaterial;
        public Color idleColor = Color.blue;
        public Color groundedColor = Color.green;
        public Color airborneColor = Color.red;
        public Color highJumpColor = Color.yellow;
        public Color doubleJumpColor = Color.magenta;
        public Color empoweredAttackColor = Color.white;
        public Color attackColor = new Color(1f, 0.5f, 0f);

        [Header("Jumping Settings")]
        public float rotationSpeed = 15f;
        public float gravity = -25f;
        public LayerMask groundLayer;
        public float initialJumpHeight = 1f;
        public float maxFirstJumpHeight = 1.5f;
        public float doubleJumpBoostVelocity = 6f;
        public float jumpHoldDuration = 0.25f;
        public float jumpCutOffMultiplier = 2f;
        public float jumpBufferTime = 0.15f;

        private float _jumpBufferTimer;
        private bool _attackInputPressed;
        private float _attackCooldownTimer;
        
        private TargetingSystem _targetingSystem;

        public Vector3 PlayerVelocity { get; set; }
        public int JumpsRemaining { get; set; }
        public bool IsJumpButtonPressed { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public CharacterController Controller { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public CharacterStats MyStats { get; private set; }
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            StateMachine = GetComponent<StateMachine>();
            MyStats = GetComponent<CharacterStats>();
            _targetingSystem = GetComponent<TargetingSystem>();

            var renderer = GetComponentInChildren<Renderer>();
            if (renderer != null) debugMaterial = renderer.material;
            MyStats.OnDied += HandleDeath;
            MyStats.OnGrassStatusChanged += HandleVisibilityChange;
            MyStats.OnRevealStatusChanged += HandleVisibilityChange;
        }

        private void OnDestroy()
        {
            if (MyStats != null)
            {
                MyStats.OnDied -= HandleDeath;
                MyStats.OnGrassStatusChanged -= HandleVisibilityChange;
                MyStats.OnRevealStatusChanged -= HandleVisibilityChange;
            }
        }

        private void Start()
        {
            SetupStateMachine();
            FindFirstObjectByType<CameraController>()?.SetTarget(this.transform);
        }

        private void OnEnable()
        {
            if (inputReader == null) return;
            inputReader.MoveEvent += OnMove;
            inputReader.JumpEvent += OnJump;
            inputReader.JumpCancelledEvent += OnJumpCancelled;
            inputReader.AttackEvent += OnAttack;
            inputReader.ScoreEvent += OnScore;
            inputReader.ScoreCancelledEvent += OnScoreCancelled;
        }

        private void OnDisable()
        {
            if (inputReader == null) return;
            inputReader.MoveEvent -= OnMove;
            inputReader.JumpEvent -= OnJump;
            inputReader.JumpCancelledEvent -= OnJumpCancelled;
            inputReader.AttackEvent -= OnAttack;
            inputReader.ScoreEvent -= OnScore;
            inputReader.ScoreCancelledEvent -= OnScoreCancelled;
        }

        private void SetupStateMachine()
        {
            var idleState = new PlayerIdleState(this, StateMachine);
            var groundedState = new PlayerGroundedState(this, StateMachine);
            var airborneState = new PlayerAirborneState(this, StateMachine);
            StateMachine.AddAnyTransition(airborneState, new FunkPredicate(() => !IsGrounded));
            StateMachine.AddTransition(airborneState, groundedState, new FunkPredicate(() => IsGrounded && MoveInput != Vector2.zero));
            StateMachine.AddTransition(airborneState, idleState, new FunkPredicate(() => IsGrounded && MoveInput == Vector2.zero));
            StateMachine.SetState(idleState);
        }

        private void Update()
        {
            _jumpBufferTimer -= Time.deltaTime;
            _attackCooldownTimer -= Time.deltaTime;
            StateMachine.Tick();
        }

        private void OnAttack()
        {
            if (_attackCooldownTimer <= 0f)
            {
                _attackInputPressed = true;
            }
        }

        public void ExecuteAttack(IState stateToReturnTo)
        {
            if (MyStats.isInGrass)
            {
                MyStats.RevealCharacter(revealDurationOnAttack);
            }
            _jumpBufferTimer = 0f; 
            _attackCooldownTimer = 1f / MyStats.baseStats.AttackSpeed;
            
            if (MyStats.IsNextAttackEmpowered() && boostedProjectilePrefab != null)
            {
                Transform target = _targetingSystem.FindBestTarget();
                Quaternion projectileRotation;

                if (target != null)
                {
                    Debug.Log($"Found best target: {target.name}");
                    Vector3 directionToTarget = (target.position - transform.position).normalized;
                    projectileRotation = Quaternion.LookRotation(directionToTarget);
                }
                else
                {
                    Debug.Log("No target found, firing forward.");
                    projectileRotation = transform.rotation;
                }
                
                var projectile = Instantiate(boostedProjectilePrefab, transform.position + transform.forward, projectileRotation);
                projectile.GetComponent<Projectile>().Initialize(MyStats, MyStats.baseStats.Attack, true, attackLayerMask);
            }

            StateMachine.ChangeState(new PlayerAttackState(this, StateMachine, stateToReturnTo));
        }

        private void HandleVisibilityChange(bool status)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (debugMaterial == null) return;
            bool isHidden = MyStats.isInGrass && !MyStats.isRevealed;
            if (isHidden)
            {
                Color c = debugMaterial.color;
                debugMaterial.color = new Color(c.r, c.g, c.b, 0.3f);
            }
            else
            {
                Color c = debugMaterial.color;
                debugMaterial.color = new Color(c.r, c.g, c.b, 1f);
                UpdateGroundedColor();
            }
        }

        public void UpdateGroundedColor()
        {
            if (!IsGrounded || (MyStats.isInGrass && !MyStats.isRevealed)) return;
            if (MyStats.IsNextAttackEmpowered())
            {
                SetStateColor(empoweredAttackColor);
            }
            else
            {
                if (MoveInput == Vector2.zero) SetStateColor(idleColor);
                else SetStateColor(groundedColor);
            }
        }

        private void FixedUpdate()
        {
            CheckGroundedStatus();
            StateMachine.FixedTick();
            CheckCeilingCollision();
        }

        private void CheckCeilingCollision()
        {
            if ((Controller.collisionFlags & CollisionFlags.Above) != 0 && PlayerVelocity.y > 0)
            {
                var v = PlayerVelocity;
                v.y = -2f;
                PlayerVelocity = v;
            }
        }

        private void CheckGroundedStatus()
        {
            Vector3 pos = transform.position + Controller.center;
            pos.y -= (Controller.height / 2f) - Controller.radius + 0.01f;
            if (Physics.SphereCast(pos, Controller.radius, Vector3.down, out RaycastHit hit, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(Vector3.up, hit.normal) < Controller.slopeLimit)
                {
                    IsGrounded = true;
                    return;
                }
            }
            IsGrounded = false;
        }

        private void OnMove(Vector2 move)
        {
            MoveInput = move;
        }

        private void OnJump()
        {
            _jumpBufferTimer = jumpBufferTime;
            IsJumpButtonPressed = true;
        }

        private void OnJumpCancelled()
        {
            IsJumpButtonPressed = false;
        }

        public bool ConsumeAttackPress()
        {
            if (_attackInputPressed)
            {
                _attackInputPressed = false;
                return true;
            }
            return false;
        }

        public bool ConsumeJumpBuffer()
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpBufferTimer = 0f;
                return true;
            }
            return false;
        }

        public void SetStateColor(Color color)
        {
            if (debugMaterial != null)
            {
                Color c = debugMaterial.color;
                debugMaterial.color = new Color(color.r, color.g, color.b, c.a);
            }
        }

        private void HandleDeath()
        {
            this.enabled = false;
            StateMachine.enabled = false;
            Invoke(nameof(ReloadScene), 2f);
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnScore()
        {
            IsScoreButtonPressed = true;
        }

        private void OnScoreCancelled()
        {
            IsScoreButtonPressed = false;
        }

        public bool CanStartScoring()
        {
            if (_currentGoalZone != null && coinCount > 0)
            {
                return _currentGoalZone.team == Team.Neutral || _currentGoalZone.team != this.MyStats.team;
            }
            return false;
        }

        public GoalZone GetCurrentGoalZone()
        {
            return _currentGoalZone;
        }

        public void ScorePoints(int pointsToScore)
        {
            if (_currentGoalZone == null) return;
            int pointsActuallyScored = _currentGoalZone.ScorePoints(pointsToScore);
            if (pointsActuallyScored > 0)
            {
                Debug.Log($"Player scored {pointsActuallyScored} points!");
                GameManager.Instance.AddScore(this.team, pointsActuallyScored);
                GrantScoringXp(pointsActuallyScored);
                coinCount -= pointsActuallyScored;
            }
        }

        private void GrantScoringXp(int pointsScored)
        {
            int xpGained = pointsScored;
            int bonusIncrements = pointsScored / 5;
            int bonusXp = bonusIncrements * 10;
            xpGained += bonusXp;
            Debug.Log($"Gained {xpGained} XP from scoring! (Base: {pointsScored}, Bonus: {bonusXp})");
            MyStats.AddXp(xpGained);
        }

        public void CollectCoin(Coin coin)
        {
            coinCount += coin.coinValue;
            Debug.Log($"Collected a coin! Total coins: {coinCount}");
            coin.Collect();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<GoalZone>(out GoalZone goalZone))
            {
                if (_currentGoalZone != null) Debug.LogWarning($"Overlapping goal zones? Current: {_currentGoalZone.team}, New: {goalZone.team}");
                Debug.Log($"Entered goal zone for team {goalZone.team}");
                _currentGoalZone = goalZone;
                _currentGoalZone.OnPlayerEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<GoalZone>(out GoalZone goalZone))
            {
                if (_currentGoalZone == goalZone)
                {
                    Debug.Log("Exited current goal zone.");
                    _currentGoalZone.OnPlayerExit();
                    _currentGoalZone = null;
                }
                else
                {
                    Debug.LogWarning($"Exited non-current goal zone: {goalZone.team}");
                }
            }
        }
    }
}
