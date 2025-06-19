using UnityEngine;

public enum States
{
    Idle, Walking, Running, Jumping, Falling
}

public class FN_Player_Movement : MonoBehaviour
{
    float fl_playerHeight = 2f;

    [Header("Movement")]
    [SerializeField] public float fl_baseMoveSpeed = 6f;
    [SerializeField] float fl_airMultiplier = 0.2f;
    float fl_movementMultiplier = 10f;
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    [SerializeField] private Rigidbody rb;

    [Header("Sprinting")]
    [SerializeField] float fl_walkSpeed = 4f;
    [SerializeField] public float fl_sprintSpeed = 6f;
    [SerializeField] float fl_acceleration = 10f;

    [Header("Jumping")]
    public float fl_jumpForce = 5f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float fl_groundDrag = 7f;
    [SerializeField] float fl_airDrag = 1f;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    RaycastHit slopeHit;


    private Vector3 currentMoveDirection = Vector3.zero;
    [SerializeField] private float moveLerpSpeed = 10f;

    private Vector3 v3_lastPosition;
    private float fl_movementThreshold = 0.1f;

    private FN_IdleTimer idleTimer;

    public bool isGrounded { get; private set; }

    private FN_CameraMovement cameraMovement;

    void Start()
    {
        cameraMovement = FindObjectOfType<FN_CameraMovement>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = GetComponentInChildren<Rigidbody>();

        if (rb != null)
            rb.velocity = Vector3.zero;
        else
            Debug.LogError("Rigidbody not found!");

        if (cameraMovement == null)
            Debug.LogError("FN_CameraMovement not found!");

        if (cameraMovement != null && cameraMovement.orientation == null)
            Debug.LogError("FN_CameraMovement found but orientation is NOT assigned!");

        v3_lastPosition = transform.position;
        idleTimer = FindObjectOfType<FN_IdleTimer>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!Input.anyKey)
        {
            horizontalMovement = 0;
            verticalMovement = 0;
            moveDirection = Vector3.zero;
            return;
        }
        PlayerInput();
        ControlDrag();
        ControlSpeed();

        float moved = Vector3.Distance(transform.position, v3_lastPosition);
        if (moved > fl_movementThreshold && idleTimer != null)
        {
            idleTimer.ReportMovement();
        }

        v3_lastPosition = transform.position;
        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();
    }

    private void FixedUpdate()
    {
        if (horizontalMovement == 0 && verticalMovement == 0)
        {
            // Smooth stop when idle
            Vector3 targetVelocity = new Vector3(0, rb.velocity.y, 0);
            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, moveLerpSpeed * Time.fixedDeltaTime);
            rb.angularVelocity = Vector3.zero;
            currentMoveDirection = Vector3.zero;
            return;
        }

        MovePlayer(moveDirection);
    }

    void PlayerInput()
    {
        if (cameraMovement == null || cameraMovement.orientation == null)
        {
            Debug.LogError("Missing camera or orientation!");
            moveDirection = Vector3.zero;
            return;
        }

        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontalMovement) < 0.01f) horizontalMovement = 0;
        if (Mathf.Abs(verticalMovement) < 0.01f) verticalMovement = 0;

        if (horizontalMovement == 0 && verticalMovement == 0)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 forward = cameraMovement.orientation.forward;
        Vector3 right = cameraMovement.orientation.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = forward * verticalMovement + right * horizontalMovement;
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            if (horizontalMovement == 0 && verticalMovement == 0)
                rb.drag = 10f; // extra friction when idle
            else
                rb.drag = fl_groundDrag;
        }
        else
        {
            rb.drag = fl_airDrag;
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
            fl_baseMoveSpeed = Mathf.Lerp(fl_baseMoveSpeed, fl_sprintSpeed, fl_acceleration * Time.deltaTime);
        else
            fl_baseMoveSpeed = Mathf.Lerp(fl_baseMoveSpeed, fl_walkSpeed, fl_acceleration * Time.deltaTime);
    }

    public void MovePlayer(Vector3 direction)
    {
        if (horizontalMovement == 0 && verticalMovement == 0 || direction == Vector3.zero)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero;
            currentMoveDirection = Vector3.zero;
            return;
        }

        // Tighten responsiveness: snappier lerp
        float responsiveLerpSpeed = isGrounded ? moveLerpSpeed * 1.5f : moveLerpSpeed;
        currentMoveDirection = Vector3.Lerp(currentMoveDirection, direction, responsiveLerpSpeed * Time.fixedDeltaTime);

        float adjustedMultiplier = fl_movementMultiplier;

        if (!isGrounded)
            adjustedMultiplier *= fl_airMultiplier;

        Vector3 desiredForce = currentMoveDirection * fl_baseMoveSpeed * adjustedMultiplier;

        // Slightly amplify initial push for quicker starts
        if (rb.velocity.magnitude < 1f)
            desiredForce *= 1.2f;

        rb.AddForce(desiredForce, ForceMode.Acceleration);
    }


    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, fl_playerHeight / 2 + 0.5f))
        {
            return slopeHit.normal != Vector3.up;
        }
        return false;
    }

    void Jump()
    {
        if (isGrounded)
        {
            FN_Sound_Manager.PlaySound(SoundType.Jump, 1.5f);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * fl_jumpForce, ForceMode.Impulse);
        }
    }
}
