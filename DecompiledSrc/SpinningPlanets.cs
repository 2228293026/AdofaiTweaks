using System;
using UnityEngine;

public class SpinningPlanets : ADOBase
{
	public PlanetRenderer[] planets;

	public float radius = 0.6f;

	public float speed = 1f;

	public bool clockwise;

	private float time;

	public void SetAppearance(scrPlayer player)
	{
		for (int i = 0; i < planets.Length; i++)
		{
			PlanetRenderer obj = planets[i];
			PlanetRenderer planetRenderer = player.planetarySystem.allPlanets[i].planetRenderer;
			obj.SetColor(planetRenderer.planetColor);
			obj.ClearParticles();
		}
	}

	private void Update()
	{
		time += Time.deltaTime * speed;
		float num = (float)Math.PI * 2f / (float)planets.Length;
		int num2 = (clockwise ? 1 : (-1));
		for (int i = 0; i < planets.Length; i++)
		{
			float f = time * (float)num2 + (float)i * num;
			Vector2 vector = new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * radius;
			planets[i].transform.parent.localPosition = vector;
		}
	}
}
