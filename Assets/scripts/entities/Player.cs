using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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
			rb.constraints = RigidbodyConstraints2D.None; 
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
			isGrounded = false;
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

		Vector2 pos = rb.position;
		pos.x = Mathf.Clamp(pos.x, -12f, 12f);
		rb.position = pos;
	}

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
