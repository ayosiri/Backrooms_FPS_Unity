using UnityEngine;
using UnityEngine.AI;

// Controls enemy AI behavior including detection, chasing, attacking, and searching
public class EnemyFollow : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public Transform player;

    [Header("Vision")]
    public float visionDistance = 15f;
    public float visionAngle = 70f;
    public float eyeHeight = 1.5f;

    [Header("Movement")]
    public float attackDistance = 2f;
    public float reachDistance = 0.8f;
    public float searchRadius = 10f;

    [Header("Search Timing")]
    public float idleBeforeSearch = 1.5f;
    public float idleBetweenSearchPoints = 5f;

    [Header("Health")]
    public float health = 100f;

    private bool isDead = false;
    private bool isReacting = false;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip chaseLoop;
    public AudioClip attackLoop;
    public AudioClip searchLoop;
    public float attackToChaseAudioDelay = 0.6f;

    private AudioSource audioSource;

    [Header("Search Audio Distance")]
    public float searchMaxDistance = 15f;
    public float searchMinDistance = 2f;
    public float searchMaxVolume = .75f;

    // Tracks current audio behavior to prevent unnecessary restarts
    private enum AudioState
    {
        None,
        Search,
        Chase,
        Attack
    }

    private AudioState currentAudioState = AudioState.None;

    // Core AI state machine
    private enum State
    {
        Chase,
        Attack,
        GoToLastKnown,
        SearchIdle,
        SearchWalk
    }

    private State currentState;

    private Vector3 lastKnownPosition;
    private Vector3 currentSearchPoint;

    private float stateTimer;
    private bool hasSearchPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerHealth = player.GetComponent<PlayerHealth>();

        // Configure navigation stopping behavior
        agent.stoppingDistance = attackDistance;
        agent.autoBraking = true;

        lastKnownPosition = transform.position;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        SetState(State.SearchIdle);
    }

    void Update()
    {
        if (isDead) return;

        bool canSeePlayer = CanSeePlayer();
        float distanceToPlayer = FlatDistance(transform.position, player.position);

        // Transition to last known position if player is lost during chase/attack
        if (!canSeePlayer && (currentState == State.Chase || currentState == State.Attack))
        {
            agent.ResetPath();
            SetState(State.GoToLastKnown);
        }

        // Prevent state switching while reacting to damage
        if (!isReacting)
        {
            if (canSeePlayer)
            {
                lastKnownPosition = player.position;

                if (distanceToPlayer <= attackDistance)
                {
                    if (currentState != State.Attack)
                        SetState(State.Attack);
                }
                else
                {
                    if (currentState != State.Chase)
                        SetState(State.Chase);
                }
            }
        }

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Chase:
                UpdateChase();
                break;

            case State.Attack:
                UpdateAttack();
                break;

            case State.GoToLastKnown:
                UpdateGoToLastKnown();
                break;

            case State.SearchIdle:
                UpdateSearchIdle();
                break;

            case State.SearchWalk:
                UpdateSearchWalk();
                break;
        }

        // Adjust search audio volume based on player proximity
        if (currentAudioState == AudioState.Search && audioSource != null && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance >= searchMaxDistance)
            {
                audioSource.volume = 0f;
            }
            else
            {
                float t = Mathf.InverseLerp(searchMaxDistance, searchMinDistance, distance);
                audioSource.volume = t * searchMaxVolume;
            }
        }
    }

    // Handles transitions between AI states
    void SetState(State newState)
    {
        if (isDead) return;

        currentState = newState;
        stateTimer = 0f;

        switch (currentState)
        {
            case State.Chase:
                agent.isStopped = false;
                agent.ResetPath();
                agent.SetDestination(player.position);

                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);

                // Delay audio transition if coming from attack state
                if (currentAudioState == AudioState.Attack)
                {
                    StartCoroutine(DelayedChaseAudio());
                }
                else
                {
                    SetAudio(AudioState.Chase, chaseLoop);
                }
                break;

            case State.Attack:
                if (isReacting) return;

                agent.isStopped = true;
                agent.ResetPath();

                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                animator.SetTrigger("Attack");

                SetAudio(AudioState.Attack, attackLoop);
                break;

            case State.GoToLastKnown:
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                agent.isStopped = false;
                agent.SetDestination(lastKnownPosition);

                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);

                // Stop looping audio when target is lost
                if (audioSource != null)
                {
                    audioSource.Stop();
                    currentAudioState = AudioState.None;
                }
                break;

            case State.SearchIdle:
                agent.isStopped = true;
                agent.ResetPath();

                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);

                SetAudio(AudioState.Search, searchLoop);
                break;

            case State.SearchWalk:
                hasSearchPoint = false;

                agent.isStopped = true;
                agent.ResetPath();

                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);

                SetAudio(AudioState.Search, searchLoop);
                break;
        }
    }

    // Continuously moves toward player while maintaining rotation
    void UpdateChase()
    {
        if (isReacting) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    // Handles attack positioning and facing logic
    void UpdateAttack()
    {
        if (isReacting) return;

        agent.isStopped = true;
        agent.ResetPath();

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);

        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }
    }

    // Moves enemy to last known player position before entering search behavior
    void UpdateGoToLastKnown()
    {
        agent.isStopped = false;
        agent.SetDestination(lastKnownPosition);

        bool reached = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

        if (reached)
        {
            agent.isStopped = true;
            agent.ResetPath();
            SetState(State.SearchIdle);
        }
    }

    // Idle phase before selecting a new search point
    void UpdateSearchIdle()
    {
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);

        if (stateTimer >= idleBeforeSearch)
        {
            hasSearchPoint = false;
            SetState(State.SearchWalk);
        }
    }

    // Randomized movement while searching for player
    void UpdateSearchWalk()
    {
        if (!hasSearchPoint)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0f;
            randomDir.Normalize();

            float distance = Random.Range(searchRadius * 0.9f, searchRadius);
            Vector3 candidate = transform.position + randomDir * distance;

            NavMeshHit hit;
            bool found = NavMesh.SamplePosition(candidate, out hit, searchRadius, NavMesh.AllAreas);

            if (!found)
            {
                SetState(State.SearchIdle);
                return;
            }

            currentSearchPoint = hit.position;
            hasSearchPoint = true;

            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(currentSearchPoint);
        }

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            hasSearchPoint = false;
            SetState(State.SearchIdle);
            return;
        }

        bool actuallyMoving = agent.velocity.magnitude > 0.1f;

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", actuallyMoving);

        bool reachedPoint = !agent.pathPending && agent.remainingDistance <= reachDistance;
        bool stalled = !agent.pathPending && agent.velocity.magnitude <= 0.05f && stateTimer > 0.75f;

        if (reachedPoint || stalled)
        {
            hasSearchPoint = false;
            SetState(State.SearchIdle);
        }
    }

    // Applies damage to enemy and triggers reaction or death
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (health > 0)
        {
            StartCoroutine(ReactToHit());
        }
        else
        {
            Die();
        }
    }

    // Temporarily interrupts AI behavior when hit
    System.Collections.IEnumerator ReactToHit()
    {
        isReacting = true;

        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("Hit");

        yield return new WaitForSeconds(0.6f);

        isReacting = false;

        SetState(State.Chase);
    }

    // Handles enemy death sequence including animation, audio, and cleanup
    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (audioSource != null)
        {
            audioSource.Stop();
            currentAudioState = AudioState.None;
        }

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        agent.isStopped = true;
        agent.ResetPath();
        agent.enabled = false;

        animator.CrossFade("death", 0.1f);

        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }
