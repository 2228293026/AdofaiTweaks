using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ffxLightBridge : ffxPlusBase
{
	public int tilesAhead;

	public int tileRange;

	public float brightness = 0.5f;

	public Color color = Color.white;

	public bool turnOff;

	private List<scrFloor> brigeFloors = new List<scrFloor>();

	public override bool runOnHit => true;

	public void Start()
	{
		List<scrFloor> listFloors = ADOBase.lm.listFloors;
		int seqID = GetComponent<scrFloor>().seqID;
		int num = Math.Max(0, seqID + tilesAhead);
		int num2 = Math.Min(seqID + tilesAhead + tileRange + 1, listFloors.Count);
		for (int i = num; i < num2; i++)
		{
			scrFloor scrFloor2 = ADOBase.lm.listFloors[i];
			brigeFloors.Add(scrFloor2);
			scrFloor2.floorRenderer.material.SetFloat("_Flash", brightness);
			if (!turnOff)
			{
				scrFloor2.opacity = 0f;
			}
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		float num = 60f / (cond.bpm * GetComponent<scrFloor>().speed);
		foreach (scrFloor floor in brigeFloors)
		{
			floor.floorRenderer.color = color;
			float endValue = (turnOff ? 0f : 1f);
			DOTween.To(() => floor.opacity, delegate(float o)
			{
				floor.opacity = o;
			}, endValue, num).SetEase(Ease.Flash, 5f, 0f);
			if (turnOff)
			{
				floor.topGlow.DOColor(floor.topGlow.color * new Color(1f, 1f, 1f, 0f), num).SetEase(Ease.Flash, 5f, 0f);
				floor.dontChangeMySprite = true;
			}
		}
	}
}
