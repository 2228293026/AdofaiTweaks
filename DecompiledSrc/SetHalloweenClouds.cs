using UnityEngine;

public class SetHalloweenClouds : ADOBase
{
	public Color color;

	private void Awake()
	{
		if (ADOBase.IsHalloweenWeek())
		{
			ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
			main.startColor = color;
		}
	}
}
