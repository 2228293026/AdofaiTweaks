using System;
using System.Collections.Generic;
using UnityEngine;

public class scrSpike : MonoBehaviour
{
	public bool hit;

	public Transform shadow;

	public Transform ball;

	private float uptime;

	public bool hittable = true;

	public bool destroyed;

	public bool displayShadow = true;

	public Mawaru_Sprite ballSprite;

	public List<float> hitTime = new List<float>();

	private float tmult = 0.5f;

	private float scale = 0.7f;

	public Vector3 pos => base.transform.position;

	private void Awake()
	{
		ballSprite = ball.gameObject.GetComponent<Mawaru_Sprite>();
		for (int i = 0; i < 8; i++)
		{
			hitTime.Add(0f);
		}
	}

	private void Start()
	{
		uptime = (pos.x + pos.y) * 0.5f;
	}

	private void Update()
	{
		uptime += Time.deltaTime;
		scale = 0.6f - 0.04f * Mathf.Sin(uptime * (float)Math.PI * tmult);
		if (!destroyed)
		{
			ball.localPosition = Vector3.up * ((displayShadow ? 0.5f : 0f) + 0.06f * Mathf.Sin(uptime * (float)Math.PI * tmult)) + Vector3.right * 0.08f * Mathf.Sin(uptime * 0.8f * (float)Math.PI * tmult);
			ball.eulerAngles = Vector3.forward * 2f * Mathf.Sin(uptime * 0.6f * (float)Math.PI * tmult);
			if (displayShadow)
			{
				shadow.localScale = Vector3.up * scale * 1.1f + Vector3.right * scale + Vector3.forward;
				shadow.localPosition = Vector3.right * 0.06f * Mathf.Sin(uptime * 0.8f * (float)Math.PI * tmult);
			}
			else
			{
				shadow.localScale = Vector3.zero;
			}
		}
	}

	public void Die()
	{
		hittable = false;
		destroyed = true;
		shadow.localScale = Vector3.zero;
	}
}
