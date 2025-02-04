using UnityEngine;

public class UserController : MonoBehaviour
{
	private CharacterController characterController;

	[SerializeField]
	private float moveSpeed = 5f;

	private Vector2 rotation;

	[SerializeField]
	private float turnSensitivity = 0.5f;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		HandleRotation();
		HandleMovement();
	}

	private void HandleRotation()
	{
		float mouseX = Input.GetAxis("Mouse X") * turnSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * turnSensitivity;

		rotation.x += mouseX;
		rotation.y += mouseY;

		transform.localRotation = Quaternion.Euler(-rotation.y, rotation.x, 0f);
	}

	private void HandleMovement()
	{
		float verticalInput = Input.GetAxis("Vertical");
		float horizontalInput = Input.GetAxis("Horizontal");

		float verticalMovement = Input.GetKey(KeyCode.Q) ? -1f : (Input.GetKey(KeyCode.E) ? 1f : 0f);

		Vector3 forwardMovement = transform.forward * verticalInput;
		Vector3 rightMovement = transform.right * horizontalInput;
		Vector3 movementDirection = forwardMovement + rightMovement;

		movementDirection.y = verticalMovement;

		characterController.Move(movementDirection * moveSpeed * Time.deltaTime);
	}
}
