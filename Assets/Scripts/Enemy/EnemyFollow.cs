using UnityEngine;
using UnityEngine.AI;

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

    private enum AudioState
    {
        None,
        Search,
        Chase,
        Attack
    }

    private AudioState currentAudioState = AudioState.None;

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

        // If enemy loses sight while chasing or attacking
        if (!canSeePlayer && (currentState == State.Chase || currentState == State.Attack))
        {
            agent.ResetPath();
            SetState(State.GoToLastKnown);
        }

        // Prevent attacking while reacting to damage
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

        // adjust search audio volume based on player distance
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

        // Muted debug
        // Debug.Log(currentState + " | hasSearchPoint=" + hasSearchPoint + " | stateTimer=" + stateTimer + " | vel=" + agent.velocity.magnitude + " | remaining=" + agent.remainingDistance);
    }

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
            return;
        }
    }

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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        Debug.Log("Enemy health now: " + health);  // shows every hit

        if (health > 0)
        {
            StartCoroutine(ReactToHit());
        }
        else
        {
            Debug.Log("ENEMY HEALTH REACHED ZERO - DIE() CALLED");  // <-- THIS IS THE IMPORTANT ONE
            Die();
        }
    }

    System.Collections.IEnumerator ReactToHit()
    {
        isReacting = true;

        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("Hit");

        yield return new WaitForSeconds(0.6f);

        isReacting = false;

        // force AI to immediately pick correct state again
        SetState(State.Chase);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // stop any looping audio (search/chase/attack)
        if (audioSource != null)
        {
            audioSource.Stop();
            currentAudioState = AudioState.None;
        }

        // play death sound
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

    public void DamagePlayer()
    {
        if (isDead) return;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10f);
        }
    }

    System.Collections.IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(4.5f); // length of death animation

        Destroy(gameObject);
    }

    System.Collections.IEnumerator DelayedChaseAudio()
    {
        yield return new WaitForSeconds(attackToChaseAudioDelay);

        if (currentState == State.Chase)
        {
            SetAudio(AudioState.Chase, chaseLoop);
        }
    }

    void SetAudio(AudioState newAudioState, AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        if (currentAudioState == newAudioState)
            return;

        audioSource.Stop();

        audioSource.clip = clip;
        audioSource.Play();

        currentAudioState = newAudioState;
    }

    bool CanSeePlayer()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerTarget = player.position + Vector3.up * 1f;

        Vector3 direction = playerTarget - eyePos;
        float distance = direction.magnitude;

        if (distance > visionDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, direction.normalized);

        if (angle > visionAngle)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(eyePos, direction.normalized, out hit, visionDistance))
        {
            if (hit.transform != player)
                return false;
        }

        return true;
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}