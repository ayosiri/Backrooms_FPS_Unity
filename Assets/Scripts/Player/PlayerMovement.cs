using UnityEngine;

// Handles player movement, sprinting, gravity, and footstep audio
public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.1f;
    public float sprintSpeed = 4.2f;
    public float gravity = -9.18f;

    private CharacterController cc;
    private Vector3 velocity;
    private bool wasMovingLastFrame = false;

    [Header("Footsteps")]
    public AudioClip footstep1;
    public AudioClip footstep2;
    public float stepDistance = 1.6f;

    private float distanceMoved;
    private Vector3 lastPosition;
    private bool playFirstStep = true;

    private AudioSource audioSource;

    void Start()
    {
        // Cache required components
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // Read movement input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Determine movement speed (walking vs sprinting)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // Calculate movement direction relative to player orientation
        Vector3 move = transform.right * h + transform.forward * v;
        cc.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Debug sprint input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Left Shift Pressed Player Sprinting");
        }

        // Apply gravity and keep player grounded
        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        // Determine if player is currently moving
        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // Track distance traveled for footstep timing
        float distance = Vector3.Distance(transform.position, lastPosition);
        distanceMoved += distance;
        lastPosition = transform.position;

        // Play immediate step when movement starts
        if (cc.isGrounded && isMoving && !wasMovingLastFrame)
        {
            PlayFootstep();
            distanceMoved = 0f;
        }
        // Play steps at consistent intervals while moving
        else if (cc.isGrounded && isMoving && distanceMoved >= stepDistance)
        {
            PlayFootstep();
            distanceMoved = 0f;
        }

        // Reset tracking when player stops moving
        if (!isMoving)
        {
            distanceMoved = 0f;
            lastPosition = transform.position;
        }

        wasMovingLastFrame = isMoving;
    }

    // Plays alternating footstep sounds for variation
    void PlayFootstep()
    {
        if (audioSource == null) return;

        if (playFirstStep)
        {
            audioSource.PlayOneShot(footstep1);
        }
        else
        {
            audioSource.PlayOneShot(footstep2);
        }

        playFirstStep = !playFirstStep;
    }
}
