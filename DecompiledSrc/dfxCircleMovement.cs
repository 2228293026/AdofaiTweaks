using System;
using UnityEngine;

public class dfxCircleMovement : ADOBase
{
	private float starty;

	private float startx;

	private float startingOffsetPercent;

	private float time;

	public float amplitude = 0.6f;

	public float period = 2f;

	public bool useLocalPos;

	public bool reverse;

	private void Start()
	{
		startingOffsetPercent = UnityEngine.Random.value;
		Vector3 vector = (useLocalPos ? base.transform.localPosition : base.transform.position);
		starty = vector.y;
		startx = vector.x;
	}

	private void Update()
	{
		time += (float)(scrConductor.instance.deltaSongPos * (double)ADOBase.controller.d_speed);
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		float num = time;
		if (reverse)
		{
			num = 0f - time;
		}
		float num2 = period / ADOBase.conductor.song.pitch;
		float x = startx + amplitude * Mathf.Cos(num * ((float)Math.PI * 2f) / num2 + startingOffsetPercent * (float)Math.PI);
		float y = starty + amplitude * Mathf.Sin(num * ((float)Math.PI * 2f) / num2 + startingOffsetPercent * (float)Math.PI);
		if (useLocalPos)
		{
			base.transform.LocalMoveXY(x, y);
		}
		else
		{
			base.transform.MoveXY(x, y);
		}
	}
}
