using System;
using UnityEngine;

public class WagBob : MonoBehaviour
{
	public float wagAmount;

	public float wagPeriod = 1f;

	public float wagOffset;

	public Vector3 pulseAmount;

	public float pulsePeriod = 1f;

	public float pulseOffset;

	private Transform thisTransform;

	private float rate = 1f;

	private float uptime;

	private void Awake()
	{
		if ((bool)TaroBGScript.instance)
		{
			rate = 1f / TaroBGScript.instance.speed;
		}
	}

	private void Start()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		if (wagAmount != 0f)
		{
			thisTransform.localEulerAngles = Vector3.forward * wagAmount * Mathf.Sin((float)Math.PI * 2f * (uptime + wagOffset) / wagPeriod);
		}
		if (pulseAmount.magnitude != 0f)
		{
			thisTransform.localScale = Vector3.one + pulseAmount * Mathf.Sin((float)Math.PI * 2f * (uptime + pulseOffset) / pulsePeriod);
		}
		uptime += Time.deltaTime * rate;
	}
}
