using System;
using UnityEngine;

[Serializable]
public class CharlieAction
{
	public float fTime;

	public Vector3 location;

	public float duration;

	public string animName;

	public float facingDir = 1f;

	public float jumpHeight;

	public string label;

	public CharlieAction(Vector3 loc, float dur, string anim, float dir = 1f, float jum = 1.5f, string lab = "")
	{
		animName = anim;
		location = loc;
		duration = dur;
		facingDir = dir;
		jumpHeight = jum;
		label = lab;
	}
}
