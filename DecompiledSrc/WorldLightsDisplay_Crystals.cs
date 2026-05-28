using System;
using System.Linq;
using UnityEngine;

public class WorldLightsDisplay_Crystals : WorldLightsDisplay
{
	public Sprite[] sprites;

	public Sprite bigCrystalSprite;

	public override void UpdateStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true)
	{
		lights[0].gameObject.SetActive(isWorldComplete);
		lights[1].gameObject.SetActive(isWorldPerfect);
		lights[2].gameObject.SetActive(isWorldSpeedTrial);
		int num = new bool[3] { isWorldComplete, isWorldPerfect, isWorldSpeedTrial }.Count((bool x) => x);
		System.Random RNG = new System.Random(base.gameObject.GetInstanceID());
		int num2 = 0;
		Transform[] array = new Transform[3];
		for (int num3 = 0; num3 < lights.Length; num3++)
		{
			Transform transform = lights[num3];
			if (transform.gameObject.activeSelf)
			{
				array[num2] = transform;
				num2++;
			}
		}
		if (num >= 3)
		{
			array[0].localEulerAngles = Vector3.forward * rnd(20f, 25f);
			array[1].localEulerAngles = Vector3.forward * rnd(-5f, 5f);
			array[2].localEulerAngles = Vector3.forward * (0f - rnd(20f, 25f));
			array[0].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.75f, 0.9f);
			array[1].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.9f, 1.1f);
			array[2].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.75f, 0.9f);
			array[0].GetComponentInChildren<SpriteRenderer>().sprite = sprites[RNG.Next(0, sprites.Length)];
			array[1].GetComponentInChildren<SpriteRenderer>().sprite = bigCrystalSprite;
			array[2].GetComponentInChildren<SpriteRenderer>().sprite = sprites[RNG.Next(0, sprites.Length)];
			return;
		}
		switch (num)
		{
		case 2:
			array[0].localEulerAngles = Vector3.forward * rnd(18f, 25f);
			array[1].localEulerAngles = Vector3.forward * (0f - rnd(18f, 25f));
			array[0].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.75f, 0.9f);
			array[1].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.75f, 0.9f);
			lights[0].GetComponentInChildren<SpriteRenderer>().sprite = sprites[RNG.Next(0, sprites.Length)];
			lights[1].GetComponentInChildren<SpriteRenderer>().sprite = sprites[RNG.Next(0, sprites.Length)];
			break;
		case 1:
			array[0].localEulerAngles = Vector3.forward * rnd(-5f, 5f);
			array[0].localScale = new Vector3(1f, rndSign(), 1f) * rnd(0.75f, 0.9f);
			lights[0].GetComponentInChildren<SpriteRenderer>().sprite = sprites[RNG.Next(0, sprites.Length)];
			break;
		}
		float rnd(float min, float max)
		{
			return min + (float)RNG.NextDouble() * (max - min);
		}
		int rndSign()
		{
			if (RNG.Next() % 2 != 0)
			{
				return 1;
			}
			return -1;
		}
	}
}
