using UnityEngine;

public class RotationSystem : MonoBehaviour
{
	[SerializeField] private Transform rotationRootX;
	[SerializeField] private Transform rotationRootY;
	[SerializeField] private float rotationSpeed = 1f;

	private void Update()
	{
		HandleRotation();
	}

	private void HandleRotation()
	{
		float xRotation = 0f;
		float yRotation = 0f;

		if (Input.GetKey(KeyCode.I)) xRotation += rotationSpeed;
		if (Input.GetKey(KeyCode.K)) xRotation -= rotationSpeed;
		if (Input.GetKey(KeyCode.L)) yRotation += rotationSpeed;
		if (Input.GetKey(KeyCode.J)) yRotation -= rotationSpeed;

		if (xRotation != 0f) RotateX(xRotation);
		if (yRotation != 0f) RotateY(yRotation);
	}

	private void RotateX(float value)
	{
		rotationRootX.Rotate(Vector3.right, value);
	}

	private void RotateY(float value)
	{
		rotationRootY.Rotate(Vector3.up, value);
	}
}
