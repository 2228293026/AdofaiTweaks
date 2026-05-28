using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathGeneratorOld : MonoBehaviour
{
	[Header("LevelMaker")]
	public string levelString;

	public bool useString;

	public bool lockUpdates;

	[Header("GameObjects")]
	public GameObject floorPrefab;

	[Header("Variable")]
	public float length;

	public float width;

	public float height;

	public float border;

	public Material material;

	public List<float> angles = new List<float>
	{
		0f, 45f, 90f, 180f, 90f, 180f, 90f, 0f, 10f, 0f,
		120f, 240f, 0f, 0f, 0f
	};

	public List<FloorMeshOld> floors;

	private string lastLevelString;

	private void Awake()
	{
		if (floors == null)
		{
			floors = new List<FloorMeshOld>();
		}
	}

	private void Update()
	{
		if (!Application.isEditor || Application.isPlaying || lockUpdates)
		{
			return;
		}
		if (useString)
		{
			angles = new List<float>();
			char[] array = levelString.ToCharArray();
			for (int i = 0; i < levelString.Length; i++)
			{
				bool exists = false;
				float angleFromFloorCharDirectionWithCheck = scrLevelMaker.GetAngleFromFloorCharDirectionWithCheck(array[i], out exists);
				if (exists)
				{
					angles.Add(angleFromFloorCharDirectionWithCheck);
				}
			}
		}
		float num = ((angles.Count > 0) ? angles[0] : 0f);
		Vector2 vector = default(Vector2);
		if (!useString)
		{
			foreach (FloorMeshOld floor in floors)
			{
				floor.gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j <= angles.Count; j++)
		{
			bool flag = j == floors.Count - 1;
			if (j >= floors.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(floorPrefab, base.transform);
				gameObject.name = "floor";
				floors.Add(gameObject.GetComponent<FloorMeshOld>());
			}
			FloorMeshOld floorMeshOld = floors[j];
			if (floorMeshOld == null)
			{
				floorMeshOld = UnityEngine.Object.Instantiate(floorPrefab, base.transform).GetComponent<FloorMeshOld>();
				floorMeshOld.name = "floor";
				floors[j] = floorMeshOld;
			}
			floorMeshOld.gameObject.SetActive(value: true);
			float f = (float)Math.PI / 180f * num;
			floorMeshOld.transform.localPosition = new Vector2(vector.x + Mathf.Cos(f) * length, vector.y + Mathf.Sin(f) * length);
			floorMeshOld.startAngle = num - 180f;
			floorMeshOld.endAngle = (flag ? num : angles[j]);
			floorMeshOld.width = width;
			floorMeshOld.length = length;
			floorMeshOld.height = height;
			floorMeshOld.border = border;
			floorMeshOld.material = material;
			floorMeshOld.index = j;
			floorMeshOld.UpdateMesh();
			vector = floorMeshOld.transform.localPosition;
			if (j != angles.Count)
			{
				num = angles[j];
			}
		}
		lastLevelString = levelString;
	}
}
