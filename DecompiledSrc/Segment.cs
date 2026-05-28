using UnityEngine;

public struct Segment
{
	public Vector2 start;

	public Vector2 end;

	public Color color;

	public float x0 => start.x;

	public float x1 => end.x;

	public float y0 => start.y;

	public float y1 => end.y;

	public float slope => (y1 - y0) / (x1 - x0);

	public Segment(Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;
		color = Color.black;
	}

	public Segment(Vector2 start, Vector2 end, Color color)
	{
		this.start = start;
		this.end = end;
		this.color = color;
	}

	public void Draw()
	{
	}

	public override string ToString()
	{
		return $"x0 {x0} x1 {x1} y0 {y0} y1 {y1}";
	}
}
