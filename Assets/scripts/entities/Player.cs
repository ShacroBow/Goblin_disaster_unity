using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Your existing variables
    public float speed = 10f;
    public float jumpForce = 21f;
    public float ascentDampening = 6.5f;
    public float extraFallAcceleration = 64f;
    public float rotationSpeed = 400f;
    public float rotationSmoothing = 8f;

    private Rigidbody2D rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool isGrounded;

    // --- NEW: Variables for screen wrapping ---
    private Camera mainCamera;
    private float screenLeft;
    private float screenRight;
    private float playerWidth;
    // ------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/d")
            .With("Positive", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
    }

    // --- NEW: Start method to set up screen boundaries ---
    private void Start()
    {
        // Get the main camera in the scene (must be tagged "MainCamera")
        mainCamera = Camera.main;

        // Calculate the screen boundaries in world coordinates
        screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        screenRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        // Get the player's width to make the wrap look smooth
        // We use the Renderer's bounds for accuracy
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            playerWidth = renderer.bounds.size.x / 2;
        }
    }
    // -----------------------------------------------------

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        if (jumpAction.triggered && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        float h = moveAction.ReadValue<float>();
        float horizontal = h * speed;
        float verticalVelocity = rb.linearVelocity.y;

        if (!isGrounded)
        {
            if (verticalVelocity > 0f)
            {
                verticalVelocity = verticalVelocity * (1f - ascentDampening * Time.fixedDeltaTime);
            }
            else if (verticalVelocity < 0f)
            {
                verticalVelocity -= extraFallAcceleration * Time.fixedDeltaTime;
            }
            float targetAngVel = -h * rotationSpeed;
            rb.angularVelocity = Mathf.Lerp(
                rb.angularVelocity,
                targetAngVel,
                rotationSmoothing * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector2(horizontal, verticalVelocity);

        // --- MODIFIED: Replaced position clamping with screen wrap logic ---
        // Vector2 pos = rb.position;
        // pos.x = Mathf.Clamp(pos.x, -12f, 12f); // This line has been REMOVED
        // rb.position = pos;

        // We now call our new function here instead
        HandleScreenWrap();
        // -------------------------------------------------------------------
    }

    // --- NEW: Method to handle the screen wrapping ---
    private void HandleScreenWrap()
    {
        Vector2 currentPosition = rb.position;

        // Check if the player is beyond the right edge of the screen
        if (currentPosition.x > screenRight + playerWidth)
        {
            // If so, teleport them to just off the left edge
            currentPosition.x = screenLeft - playerWidth;
        }
        // Check if the player is beyond the left edge of the screen
        else if (currentPosition.x < screenLeft - playerWidth)
        {
            // If so, teleport them to just off the right edge
            currentPosition.x = screenRight + playerWidth;
        }

        // Apply the new teleported position to the rigidbody
        rb.position = currentPosition;
    }
    // -------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.angularVelocity = 0f;
            rb.rotation = 0f;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }
}