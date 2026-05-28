using UnityEngine;

public class Mawaru_Rock : MonoBehaviour
{
	public scrFloor floor;

	public Mawaru_Sprite rock;

	public Mawaru_Sprite explo;

	public ParticleSystem fire;

	public float spawnTime;

	public float targetTime;

	public float tweenTime;

	public Vector3 spawnPos;

	public Vector3 targetPos;

	public float angle;

	public bool spawned;

	public bool fadingIn;

	public bool fadingOut;

	public bool hit;

	public int quant;

	public float beat;

	public float fract;

	private void Awake()
	{
		fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void UpdateFire()
	{
		if (quant == 2)
		{
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = fire.colorOverLifetime;
			colorOverLifetime.enabled = true;
			Gradient gradient = new Gradient();
			gradient.SetKeys(new GradientColorKey[2]
			{
				new GradientColorKey(Color.blue, 0f),
				new GradientColorKey(Color.cyan, 1f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			});
			colorOverLifetime.color = gradient;
		}
		else if (quant == 3)
		{
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime2 = fire.colorOverLifetime;
			colorOverLifetime2.enabled = true;
			Gradient gradient2 = new Gradient();
			gradient2.SetKeys(new GradientColorKey[2]
			{
				new GradientColorKey(new Color(0.5f, 0f, 1f, 1f), 0f),
				new GradientColorKey(new Color(1f, 0f, 0.5f, 1f), 1f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			});
			colorOverLifetime2.color = gradient2;
		}
		else if (quant == 4)
		{
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime3 = fire.colorOverLifetime;
			colorOverLifetime3.enabled = true;
			Gradient gradient3 = new Gradient();
			gradient3.SetKeys(new GradientColorKey[2]
			{
				new GradientColorKey(Color.yellow, 0f),
				new GradientColorKey(Color.yellow, 1f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			});
			colorOverLifetime3.color = gradient3;
		}
		else if (quant == 5)
		{
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime4 = fire.colorOverLifetime;
			colorOverLifetime4.enabled = true;
			Gradient gradient4 = new Gradient();
			gradient4.SetKeys(new GradientColorKey[2]
			{
				new GradientColorKey(new Color(1f, 0.5f, 0f, 1f), 0f),
				new GradientColorKey(new Color(1f, 0.5f, 0f, 1f), 1f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			});
			colorOverLifetime4.color = gradient4;
		}
	}
}
