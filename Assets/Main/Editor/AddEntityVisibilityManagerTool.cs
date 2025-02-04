using UnityEngine;
using UnityEditor;
using System.Reflection;

public class AddEntityVisibilityManagerTool : EditorWindow
{
	private GameObject parentGameObject;
	private Material xRayMaterial;

	[MenuItem("Tools/Add Entity Visibility Manager")]
	private static void ShowWindow()
	{
		var window = GetWindow<AddEntityVisibilityManagerTool>();
		window.titleContent = new GUIContent("Add EVM Tool");
		window.Show();
	}

	private void OnGUI()
	{
		EditorGUILayout.LabelField("Select the parent GameObject containing all children with MeshFilters:", EditorStyles.wordWrappedLabel);
		parentGameObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentGameObject, typeof(GameObject), true);
		xRayMaterial = (Material)EditorGUILayout.ObjectField("Material Object", xRayMaterial, typeof(Material), true);

		EditorGUILayout.Space();

		if (GUILayout.Button("Add Colliders & EntityVisibilityManager"))
		{
			if (parentGameObject == null)
			{
				Debug.LogWarning("No Parent Object set. Please assign a parent GameObject.");
				return;
			}

			AddCollidersAndManagers(parentGameObject);
		}
	}

	private void AddCollidersAndManagers(GameObject parent)
	{
		MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>(true);
		int updatedCount = 0;

		foreach (var meshFilter in meshFilters)
		{
			if (meshFilter.sharedMesh == null) continue;

			MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
			if (meshCollider == null)
			{
				Undo.AddComponent<MeshCollider>(meshFilter.gameObject);
				meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
			}
			meshCollider.sharedMesh = meshFilter.sharedMesh;

			EntityVisibilityManager manager = meshFilter.gameObject.GetComponent<EntityVisibilityManager>();
			if (manager == null)
			{
				Undo.AddComponent<EntityVisibilityManager>(meshFilter.gameObject);
				manager = meshFilter.gameObject.GetComponent<EntityVisibilityManager>();
			}

			MeshRenderer mr = meshFilter.gameObject.GetComponent<MeshRenderer>();
			AssignPrivateField(manager, "_renderer", mr);
			AssignPrivateField(manager, "_collider", meshCollider);
			AssignPrivateField(manager, "_xrayMaterial", xRayMaterial);

			updatedCount++;
		}
	}

	private void AssignPrivateField<T>(object target, string fieldName, T value)
	{
		BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		FieldInfo field = target.GetType().GetField(fieldName, flags);
		if (field != null)
		{
			Undo.RecordObject(target as Object, $"Assign {fieldName} on {target}");
			field.SetValue(target, value);
			EditorUtility.SetDirty(target as Object);
		}
		else
		{
			Debug.LogWarning($"Field '{fieldName}' not found on {target}");
		}
	}
}
