using UnityEngine;

public class EntityVisibilityManager : MonoBehaviour
{
	[SerializeField] private Renderer _renderer;
	[SerializeField] private Collider _collider;
	[SerializeField] private Material _xrayMaterial;
	private Material[] originalMaterials;
	private bool setupComplete;
	private MenuManager.DisplayOption state;

	void Start()
	{
		InitializeEntity();
	}

	public void InitializeEntity()
	{
		if (!setupComplete)
		{
			setupComplete = true;
			originalMaterials = _renderer.materials;
		}
	}

	public void UpdateVisibility(MenuManager.DisplayOption mode)
	{
		state = mode;
		if (mode == MenuManager.DisplayOption.SHOW) SetVisibility(true, false);
		if (mode == MenuManager.DisplayOption.XRAY) ApplyXRay();
		if (mode == MenuManager.DisplayOption.HIDE) SetVisibility(false, false);
	}

	void SetVisibility(bool show, bool forceXRay)
	{
		_renderer.enabled = show;
		_renderer.enabled = show;
		if (show && !forceXRay) _renderer.materials = originalMaterials;
	}

	void ApplyXRay()
	{
		var mats = new Material[originalMaterials.Length];
		for (int i = 0; i < mats.Length; i++) mats[i] = _xrayMaterial;
		_renderer.materials = mats;
	}

	public void OnEntityClick()
	{
		if (state == MenuManager.DisplayOption.SHOW)
		{
			ApplyXRay();
			state = MenuManager.DisplayOption.XRAY;
		}
		else if (state == MenuManager.DisplayOption.XRAY)
		{
			SetVisibility(true, false);
			state = MenuManager.DisplayOption.SHOW;
		}
	}
}
