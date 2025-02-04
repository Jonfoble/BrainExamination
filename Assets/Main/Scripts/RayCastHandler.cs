using UnityEngine;

public class RayCastHandler : MonoBehaviour
{
	[SerializeField] private LayerMask collisionLayers;
	[SerializeField] private float rayLength = 1000f;

	private void Update()
	{
		HandleRaycast();
	}

	private void HandleRaycast()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, rayLength, collisionLayers))
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				HandleInteraction(hit.collider);
			}
		}
	}

	private void HandleInteraction(Collider collider)
	{
		if (collider.TryGetComponent(out MenuManager menu))
		{
			menu.OnInteract();
		}

		if (collider.TryGetComponent(out EntityVisibilityManager entity))
		{
			entity.OnEntityClick();
		}
	}
}
