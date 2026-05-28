using System.Collections.Generic;
using UnityEngine;

public class WeldingTest : MonoBehaviour
{
	public MeshFilter meshFilter;

	private List<Vector2> outerPoints = new List<Vector2>();

	private List<int> outerConn = new List<int>();

	private List<Vector2> innerPoints = new List<Vector2>();

	private int trianglesCount;

	private void Start()
	{
		RDUtils.targetFrameRate = 60;
		outerPoints.AddMany(new Vector2(1f, 1f), new Vector2(0f, 1.5f), new Vector2(-1f, 1f), new Vector2(-1f, 0f), new Vector2(-1f, -2.5f), new Vector2(-0.75f, -2f), new Vector2(0.75f, -2f), new Vector2(1f, -2.5f));
		innerPoints.AddMany(new Vector2(0.5f, 0.5f), new Vector2(-0.5f, 0.5f), new Vector2(-0.5f, -0.5f), new Vector2(0.5f, -0.5f));
		outerConn.AddMany(0, 0, 1, 1, 1, 2, 2, 3);
	}

	private void Update()
	{
		if (Time.frameCount == 1)
		{
			CreateTriangles(sp: true);
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			trianglesCount = Mathf.Max(trianglesCount - 1, 0);
			CreateTriangles();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			trianglesCount = Mathf.Max(trianglesCount + 1, 0);
			CreateTriangles();
		}
	}

	private void CreateTriangles(bool sp = false)
	{
		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		List<Vector2> list = new List<Vector2>();
		list.AddRange(outerPoints);
		list.AddRange(innerPoints);
		Vector3[] array = new Vector3[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			Vector2 vector = list[i];
			array[i] = new Vector3(vector.x, vector.y, 0f);
		}
		mesh.vertices = array;
		int count = outerPoints.Count;
		int num = 0;
		List<int> list2 = new List<int>();
		for (int j = 0; j < outerPoints.Count; j++)
		{
			list2.Add(j);
			list2.Add((j + 1) % outerPoints.Count);
			list2.Add(count + outerConn[j]);
			num = list2.Count;
			if (sp)
			{
				MonoBehaviour.print($"({j}) {list2[num - 3]}, {list2[num - 2]}, {list2[num - 1]}");
			}
			if (outerConn[(j + 1) % outerConn.Count] != outerConn[j])
			{
				list2.Add((j + 1) % outerPoints.Count);
				list2.Add(count + outerConn[j]);
				list2.Add(count + (outerConn[j] + 1) % innerPoints.Count);
				num = list2.Count;
				if (sp)
				{
					MonoBehaviour.print($">> ({j}) {list2[num - 3]}, {list2[num - 2]}, {list2[num - 1]}");
				}
			}
		}
		list2 = list2.GetRange(0, trianglesCount * 3);
		mesh.triangles = list2.ToArray();
		num = list2.Count;
		if (list2.Count >= 3)
		{
			MonoBehaviour.print($"{list2[num - 3]}, {list2[num - 2]}, {list2[num - 1]}");
		}
	}
}
