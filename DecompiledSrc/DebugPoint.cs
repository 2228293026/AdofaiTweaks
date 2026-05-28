using UnityEngine;

public struct DebugPoint(Vector2 position, Color color, float radius = 0.05f)
{
	public Vector2 position = position;

	public Color color = color;

	public float radius = radius;
}
