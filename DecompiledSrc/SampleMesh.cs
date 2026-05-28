using System;
using System.Collections.Generic;
using UnityEngine;

public class SampleMesh : MonoBehaviour
{
	private const float pointSize = 0.05f;

	public int circleSides = 20;

	public float outerRadius = 1f;

	public float innerRadius = 0.4f;

	private MeshFilter meshFilter;

	private List<Vector3> points = new List<Vector3>();

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
	}

	private void Update()
	{
		points.Clear();
		DrawCircle(innerRadius);
		DrawCircle(outerRadius);
		int[] array = new int[circleSides * 2 * 3];
		for (int i = 0; i < circleSides; i++)
		{
			int num = circleSides;
			array[i * 6] = i;
			array[i * 6 + 2] = num + i;
			array[i * 6 + 1] = (num + i + 1) % circleSides + circleSides;
			array[i * 6 + 3] = i;
			array[i * 6 + 5] = (num + i + 1) % circleSides + circleSides;
			array[i * 6 + 4] = (i + 1) % circleSides;
		}
		string text = "";
		int[] array2 = array;
		foreach (int num2 in array2)
		{
			text = text + num2 + ",";
		}
		if (Time.frameCount == 5)
		{
			Debug.Log(text);
		}
		Mesh mesh = new Mesh();
		mesh.SetVertices(points);
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		List<Vector2> list = new List<Vector2>();
		for (int k = 0; k < points.Count; k++)
		{
			if (k < points.Count / 2)
			{
				Vector2 item = new Vector2(0.5f, 0f);
				list.Add(item);
			}
			else
			{
				Vector2 item2 = new Vector2(0.5f, 1f);
				list.Add(item2);
			}
		}
		mesh.uv = list.ToArray();
		meshFilter.mesh = mesh;
		void DrawCircle(float radius)
		{
			for (int l = 0; l < circleSides; l++)
			{
				float f = (float)l * 1f / (float)circleSides * ((float)Math.PI * 2f);
				Vector2 vector = new Vector2(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius);
				points.Add(vector);
			}
		}
	}
}
