using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public enum DisplayOption { SHOW, XRAY, HIDE }

	[SerializeField] private TextMeshPro text;
	[SerializeField] private Color showColor;
	[SerializeField] private Color xrayColor;
	[SerializeField] private Color hideColor;
	[SerializeField] private DisplayOption currentState;
	[SerializeField] private List<EntityVisibilityManager> entities = new List<EntityVisibilityManager>();

	private TapEvents onSelect = new TapEvents();

	private void Start()
	{
		InitializeMenu();
	}

	private void InitializeMenu()
	{
		onSelect.AddListener(SwitchState);
		UpdateTextColor(DisplayOption.SHOW);

		foreach (var entity in entities)
		{
			entity.InitializeEntity();
			entity.UpdateVisibility(DisplayOption.SHOW);
		}
	}

	public void OnInteract()
	{
		onSelect.Invoke();
	}

	private void SwitchState()
	{
		currentState = (currentState == DisplayOption.HIDE) ? DisplayOption.SHOW : (DisplayOption)((int)currentState + 1);

		UpdateTextColor(currentState);

		foreach (var entity in entities)
		{
			entity.UpdateVisibility(currentState);
		}
	}

	private void UpdateTextColor(DisplayOption state)
	{
		text.color = state switch
		{
			DisplayOption.SHOW => showColor,
			DisplayOption.XRAY => xrayColor,
			DisplayOption.HIDE => hideColor,
			_ => text.color
		};
	}
}
