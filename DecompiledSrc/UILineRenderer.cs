using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
	public UIGridRenderer grid;

	public Vector2Int gridSize;

	public float thickness;

	public List<Vector2> points;

	private float width;

	private float height;

	private float unitWidth;

	private float unitHeight;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		width = base.rectTransform.rect.width;
		height = base.rectTransform.rect.height;
		unitWidth = width / (float)gridSize.x;
		unitHeight = height / (float)gridSize.y;
		if (points.Count < 2)
		{
			return;
		}
		float angle = 0f;
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2 point = points[i];
			Vector2 point2 = points[i + 1];
			if (i < points.Count - 1)
			{
				angle = GetAngle(points[i], points[i + 1]) + 90f;
			}
			DrawVerticesForPoint(point, point2, angle, vh);
		}
		for (int j = 0; j < points.Count - 1; j++)
		{
			int num = j * 4;
			vh.AddTriangle(num, num + 1, num + 2);
			vh.AddTriangle(num + 1, num + 2, num + 3);
		}
	}

	private void Update()
	{
		if (!(grid == null) && gridSize != grid.gridSize)
		{
			gridSize = grid.gridSize;
			SetVerticesDirty();
		}
	}

	public float GetAngle(Vector2 me, Vector2 target)
	{
		return Mathf.Atan2(9f * (target.y - me.y), 16f * (target.x - me.x)) * (180f / (float)Math.PI);
	}

	private void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
	{
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = color;
		simpleVert.position = Quaternion.Euler(0f, 0f, angle) * new Vector3((0f - thickness) / 2f, 0f);
		simpleVert.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(simpleVert);
		simpleVert.position = Quaternion.Euler(0f, 0f, angle) * new Vector3(thickness / 2f, 0f);
		simpleVert.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(simpleVert);
		simpleVert.position = Quaternion.Euler(0f, 0f, angle) * new Vector3((0f - thickness) / 2f, 0f);
		simpleVert.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
		vh.AddVert(simpleVert);
		simpleVert.position = Quaternion.Euler(0f, 0f, angle) * new Vector3(thickness / 2f, 0f);
		simpleVert.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
		vh.AddVert(simpleVert);
	}
}
