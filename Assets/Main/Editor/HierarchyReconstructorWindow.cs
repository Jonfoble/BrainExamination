using UnityEngine;
using UnityEditor;
using System.IO;

public class HierarchyReconstructorWindow : EditorWindow
{
	[MenuItem("Tools/Hierarchy Reconstructor")]
	static void OpenWindow()
	{
		GetWindow<HierarchyReconstructorWindow>("Hierarchy Reconstructor");
	}

	void OnGUI()
	{
		GUILayout.Label("Select an FBX or Model prefab to reconstruct hierarchy.", EditorStyles.boldLabel);
		if (GUILayout.Button("Reconstruct Selected Model"))
		{
			ReconstructSelected();
		}
	}

	void ReconstructSelected()
	{
		if (Selection.activeObject == null)
		{
			Debug.LogError("No FBX or Model prefab selected.");
			return;
		}

		string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		var originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
		if (originalPrefab == null)
		{
			Debug.LogError("Selected asset is not a valid GameObject/FBX.");
			return;
		}

		string directory = Path.GetDirectoryName(assetPath);
		string fileNameNoExt = Path.GetFileNameWithoutExtension(assetPath);

		GameObject cleanRoot = new GameObject(fileNameNoExt + "_Clean");
		GameObject unwantedRoot = new GameObject(fileNameNoExt + "_Unwanted");

		CloneWantedHierarchy(originalPrefab.transform, cleanRoot.transform);
		CloneUnwantedHierarchy(originalPrefab.transform, unwantedRoot.transform);

		string cleanPath = Path.Combine(directory, fileNameNoExt + "_Clean.prefab");
		string unwantedPath = Path.Combine(directory, fileNameNoExt + "_Unwanted.prefab");

		var cleanPrefab = PrefabUtility.SaveAsPrefabAsset(cleanRoot, cleanPath);
		var unwantedPrefab = PrefabUtility.SaveAsPrefabAsset(unwantedRoot, unwantedPath);

		DestroyImmediate(cleanRoot);
		DestroyImmediate(unwantedRoot);

		if (cleanPrefab != null) Debug.Log("Created Clean prefab at " + cleanPath);
		if (unwantedPrefab != null) Debug.Log("Created Unwanted prefab at " + unwantedPath);

		void CloneWantedHierarchy(Transform original, Transform parent)
		{
			if (!ShouldCloneWanted(original) && !HasWantedChildren(original)) return;
			GameObject newObj = new GameObject(original.name);
			newObj.transform.SetPositionAndRotation(original.position, original.rotation);
			newObj.transform.localScale = original.localScale;
			newObj.transform.parent = parent;
			CopyMeshIfWanted(original, newObj);
			foreach (Transform child in original)
			{
				CloneWantedHierarchy(child, newObj.transform);
			}
		}

		void CloneUnwantedHierarchy(Transform original, Transform parent)
		{
			if (!ShouldCloneUnwanted(original) && !HasUnwantedChildren(original)) return;
			GameObject newObj = new GameObject(original.name);
			newObj.transform.SetPositionAndRotation(original.position, original.rotation);
			newObj.transform.localScale = original.localScale;
			newObj.transform.parent = parent;
			CopyMeshIfUnwanted(original, newObj);
			foreach (Transform child in original)
			{
				CloneUnwantedHierarchy(child, newObj.transform);
			}
		}

		bool ShouldCloneWanted(Transform t)
		{
			var mf = t.GetComponent<MeshFilter>();
			var mr = t.GetComponent<SkinnedMeshRenderer>();
			if (mf && mf.sharedMesh) return !IsUnwantedMesh(mf.sharedMesh);
			if (mr && mr.sharedMesh) return !IsUnwantedMesh(mr.sharedMesh);
			return false;
		}

		bool ShouldCloneUnwanted(Transform t)
		{
			var mf = t.GetComponent<MeshFilter>();
			var mr = t.GetComponent<SkinnedMeshRenderer>();
			if (mf && mf.sharedMesh) return IsUnwantedMesh(mf.sharedMesh);
			if (mr && mr.sharedMesh) return IsUnwantedMesh(mr.sharedMesh);
			return false;
		}

		bool HasWantedChildren(Transform t)
		{
			foreach (Transform child in t)
			{
				if (ShouldCloneWanted(child) || HasWantedChildren(child)) return true;
			}
			return false;
		}

		bool HasUnwantedChildren(Transform t)
		{
			foreach (Transform child in t)
			{
				if (ShouldCloneUnwanted(child) || HasUnwantedChildren(child)) return true;
			}
			return false;
		}

		bool IsUnwantedMesh(Mesh mesh)
		{
			return mesh.bounds.center == Vector3.zero && mesh.bounds.size == new Vector3(0.5f, 0f, 0.5f);
		}

		void CopyMeshIfWanted(Transform source, GameObject destination)
		{
			var mf = source.GetComponent<MeshFilter>();
			var mr = source.GetComponent<SkinnedMeshRenderer>();
			if (mf && mf.sharedMesh && !IsUnwantedMesh(mf.sharedMesh))
			{
				var newMf = destination.AddComponent<MeshFilter>();
				var newMr = destination.AddComponent<MeshRenderer>();
				newMf.sharedMesh = mf.sharedMesh;
				newMr.sharedMaterials = source.GetComponent<MeshRenderer>().sharedMaterials;
			}
			else if (mr && mr.sharedMesh && !IsUnwantedMesh(mr.sharedMesh))
			{
				var newSkinned = destination.AddComponent<SkinnedMeshRenderer>();
				newSkinned.sharedMesh = mr.sharedMesh;
				newSkinned.sharedMaterials = mr.sharedMaterials;
			}
		}

		void CopyMeshIfUnwanted(Transform source, GameObject destination)
		{
			var mf = source.GetComponent<MeshFilter>();
			var mr = source.GetComponent<SkinnedMeshRenderer>();
			if (mf && mf.sharedMesh && IsUnwantedMesh(mf.sharedMesh))
			{
				var newMf = destination.AddComponent<MeshFilter>();
				var newMr = destination.AddComponent<MeshRenderer>();
				newMf.sharedMesh = mf.sharedMesh;
				newMr.sharedMaterials = source.GetComponent<MeshRenderer>().sharedMaterials;
			}
			else if (mr && mr.sharedMesh && IsUnwantedMesh(mr.sharedMesh))
			{
				var newSkinned = destination.AddComponent<SkinnedMeshRenderer>();
				newSkinned.sharedMesh = mr.sharedMesh;
				newSkinned.sharedMaterials = mr.sharedMaterials;
			}
		}
	}
}
