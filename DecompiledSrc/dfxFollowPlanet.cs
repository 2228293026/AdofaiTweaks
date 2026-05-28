using System;
using DG.Tweening;
using UnityEngine;

public class dfxFollowPlanet : ADOBase
{
	public bool isBlue;

	private Vector3 planetStartPos;

	private scrPlanet planet;

	public float rotation;

	public float scale;

	public float lifeTime;

	private Vector3 startPos;

	private float time;

	private bool fadeTriggered;

	private void Start()
	{
		if (isBlue)
		{
			planet = scrController.instance.planetBlue;
		}
		else
		{
			planet = scrController.instance.planetRed;
		}
		planetStartPos = scrController.instance.planetBlue.transform.position;
		startPos = base.transform.position;
		base.transform.GetComponent<SpriteRenderer>().SetAlpha(0f);
		base.transform.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
	}

	private void Update()
	{
		Vector2 vector = planet.transform.position - planetStartPos;
		float f = (float)Math.PI / 180f * rotation;
		Vector2 vector2 = new Vector2(vector.x * Mathf.Cos(f) - vector.y * Mathf.Sin(f), vector.x * Mathf.Sin(f) + vector.y * Mathf.Cos(f)) * scale;
		base.transform.position = (Vector3)vector2 + startPos;
		time += (float)(scrConductor.instance.deltaSongPos * (double)ADOBase.controller.d_speed);
		if (time > lifeTime && !fadeTriggered && lifeTime != 0f)
		{
			fadeTriggered = true;
			base.transform.GetComponent<SpriteRenderer>().DOFade(0f, 1f);
		}
	}
}
