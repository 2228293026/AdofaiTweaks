using System;
using System.Collections.Generic;
using UnityEngine;

public class scrLoadingPlanet : MonoBehaviour
{
	public float radius = 1f;

	public float anglerad;

	public GameObject otherplanet;

	public List<float> listOffsets = new List<float>();

	private void Awake()
	{
		Update();
	}

	private void Update()
	{
		anglerad = (float)Math.PI * Time.unscaledTime;
		Vector3 position = otherplanet.transform.position;
		base.transform.position = new Vector3(position.x + Mathf.Sin(anglerad) * radius, position.y + Mathf.Cos(anglerad) * radius, position.z);
	}
}
