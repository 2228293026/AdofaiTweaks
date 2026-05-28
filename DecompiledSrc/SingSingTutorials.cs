using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SingSingTutorials : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	private int singsing_next_notefield;

	private int singsing_next_midspin;

	public float singsing_notefield_spawntime = 3f;

	public List<SingSing_Notefield> notefields;

	public List<int> cubicleHitFloors;

	public List<float> cubicleHitBeats;

	public Mawaru_Sprite lightsOut;

	public int tutLevel;

	private int spawned;

	private float beat;

	private List<float> midspinsThisSpawn = new List<float>();

	private List<int> floorsThisSpawn = new List<int>();

	private new void Awake()
	{
		base.Awake();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 114.0));
		mpf(0f, CubicleUpdate, 99f);
		if (tutLevel == 3)
		{
			mb(3f, LightsOffVerySlow);
			mb(15f, LightsOn);
		}
		else if (tutLevel == 4)
		{
			mb(6f, LightsOffSlow);
			mb(15f, LightsOn);
		}
		SortTables();
	}

	private new void Update()
	{
		base.Update();
	}

	private float ios(float t, float b, float c, float d)
	{
		return (0f - c) / 2f * (Mathf.Cos((float)Math.PI * t / d) - 1f) + b;
	}

	private void CubicleUpdate()
	{
		beat = (float)songBeat;
		if (ADOBase.controller.currentState == States.Fail || ADOBase.controller.currentState == States.Fail2 || !(beat > singsing_notefield_spawntime - 6f))
		{
			return;
		}
		midspinsThisSpawn.Clear();
		floorsThisSpawn.Clear();
		if (beat < singsing_notefield_spawntime + 1f && singsing_notefield_spawntime < 10f)
		{
			for (int i = singsing_next_midspin; i < cubicleHitBeats.Count; i++)
			{
				if (cubicleHitBeats[i] >= singsing_notefield_spawntime && cubicleHitBeats[i] < singsing_notefield_spawntime + 6f)
				{
					midspinsThisSpawn.Add(cubicleHitBeats[i]);
					floorsThisSpawn.Add(cubicleHitFloors[i]);
					singsing_next_midspin = i;
				}
				if (cubicleHitBeats[i] >= singsing_notefield_spawntime + 6f)
				{
					break;
				}
			}
			spawned++;
			notefields[singsing_next_notefield].Spawn(singsing_notefield_spawntime, midspinsThisSpawn, floorsThisSpawn, spawned == 2);
			singsing_next_notefield++;
			if (singsing_next_notefield > 2)
			{
				singsing_next_notefield = 0;
			}
		}
		singsing_notefield_spawntime += 6f;
	}

	private void LightsOffVerySlow()
	{
		lightsOut.render.DOColor(Color.black, beats(6f));
	}

	private void LightsOffSlow()
	{
		lightsOut.render.DOColor(Color.black, beats(3f));
	}

	private void LightsOn()
	{
		lightsOut.render.DOColor(Color.clear, 0f);
	}
}
