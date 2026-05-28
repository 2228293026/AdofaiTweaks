using UnityEngine;
using UnityEngine.UI;

public class UIGridRenderer : Graphic
{
	public Vector2Int gridSize = new Vector2Int(1, 1);

	public float thickness = 10f;

	private float width;

	private float height;

	private float cellWidth;

	private float cellHeight;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		width = base.rectTransform.rect.width;
		height = base.rectTransform.rect.height;
		cellWidth = width / (float)gridSize.x;
		cellHeight = height / (float)gridSize.y;
		int num = 0;
		for (int i = 0; i < gridSize.y; i++)
		{
			for (int j = 0; j < gridSize.x; j++)
			{
				DrawCell(j, i, num, vh);
				num++;
			}
		}
	}

	private void DrawCell(int x, int y, int index, VertexHelper vh)
	{
		float num = cellWidth * (float)x;
		float num2 = cellHeight * (float)y;
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = color;
		simpleVert.position = new Vector3(num, num2);
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num, num2 + cellHeight);
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num + cellWidth, num2 + cellHeight);
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num + cellWidth, num2);
		vh.AddVert(simpleVert);
		float num3 = Mathf.Sqrt(thickness * thickness / 2f);
		simpleVert.position = new Vector3(num + num3, num2 + num3);
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num + num3, num2 + (cellHeight - num3));
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num + (cellWidth - num3), num2 + (cellHeight - num3));
		vh.AddVert(simpleVert);
		simpleVert.position = new Vector3(num + (cellWidth - num3), num2 + num3);
		vh.AddVert(simpleVert);
		int num4 = index * 8;
		vh.AddTriangle(num4, num4 + 1, num4 + 5);
		vh.AddTriangle(num4 + 5, num4 + 4, num4);
		vh.AddTriangle(num4 + 1, num4 + 2, num4 + 6);
		vh.AddTriangle(num4 + 6, num4 + 5, num4 + 1);
		vh.AddTriangle(num4 + 2, num4 + 3, num4 + 7);
		vh.AddTriangle(num4 + 7, num4 + 6, num4 + 2);
		vh.AddTriangle(num4 + 3, num4, num4 + 4);
		vh.AddTriangle(num4 + 4, num4 + 7, num4 + 3);
	}
}
