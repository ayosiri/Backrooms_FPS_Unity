using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2.1f;
    public float sprintSpeed = 4.2f; // NEW sprint speed
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
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // New Opitmazation left shift hold to sprint
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;


        Vector3 move = transform.right * h + transform.forward * v;
        cc.Move(move.normalized * currentSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Left Shift Pressed Player Sprinting");
        }

        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        float distance = Vector3.Distance(transform.position, lastPosition);
        distanceMoved += distance;
        lastPosition = transform.position;

        // instant first step the moment movement starts
        if (cc.isGrounded && isMoving && !wasMovingLastFrame)
        {
            PlayFootstep();
            distanceMoved = 0f;
        }
        // steady follow-up steps while holding movement
        else if (cc.isGrounded && isMoving && distanceMoved >= stepDistance)
        {
            PlayFootstep();
            distanceMoved = 0f;
        }

        if (!isMoving)
        {
            distanceMoved = 0f;
            lastPosition = transform.position;
        }

        wasMovingLastFrame = isMoving;
    }

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