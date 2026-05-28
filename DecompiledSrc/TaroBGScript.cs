using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaroBGScript : ADOBase
{
	public bool slumpo;

	private static TaroBGScript _instance;

	public List<Mawaru_Sprite> tile_Star = new List<Mawaru_Sprite>();

	private int tile_starIndex;

	public List<Mawaru_Sprite> tile_Glow = new List<Mawaru_Sprite>();

	private int tile_glowIndex;

	public List<Mawaru_Sprite> tile_Ember = new List<Mawaru_Sprite>();

	private int tile_emberIndex;

	public double songBeat;

	public double lastBeat;

	public double songTime;

	public double lastTime;

	public int lastFloor;

	public float rate = 1f;

	public float speed = 1f;

	public float currentBPM = 100f;

	public int lastSeenBPMIndex;

	public float aux = 1f;

	[NonSerialized]
	public List<Tuple<double, double>> bpms = new List<Tuple<double, double>>();

	public List<ActionEntry> beatActions = new List<ActionEntry>();

	public List<ActionEntryArg> beatActionArgs = new List<ActionEntryArg>();

	public List<ActionEntry> beatUpdates = new List<ActionEntry>();

	public int curActionB;

	public int curActionBArgs;

	public Camera BGMovingCam;

	public TextMeshPro sectionJudgment;

	public Transform thisTransform;

	public List<Shadow> textShadows;

	public int spawnFloor;

	private int updates;

	private bool firstUpdateCompleted;

	private bool practiceWon;

	[NonSerialized]
	public readonly Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private Vector3 modPosition;

	private float modRotation;

	public static TaroBGScript instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindAnyObjectByType<TaroBGScript>();
			}
			return _instance;
		}
	}

	public scrCamera camy => scrController.instance.camy;

	public scrPlanet redPlanet => scrController.instance.planetRed;

	public scrPlanet bluePlanet => scrController.instance.planetBlue;

	public float RandF(float mi = 0f, float ma = 1f)
	{
		return UnityEngine.Random.Range(mi, ma);
	}

	public float RandSpread(float spread)
	{
		return RandF() * spread * 2f - spread;
	}

	public Mawaru_Sprite GetTileStar()
	{
		Mawaru_Sprite result = tile_Star[tile_starIndex];
		tile_starIndex++;
		if (tile_starIndex >= tile_Star.Count)
		{
			tile_starIndex = 0;
		}
		return result;
	}

	public Mawaru_Sprite GetTileGlow()
	{
		Mawaru_Sprite result = tile_Glow[tile_glowIndex];
		tile_glowIndex++;
		if (tile_glowIndex >= tile_Glow.Count)
		{
			tile_glowIndex = 0;
		}
		return result;
	}

	public Mawaru_Sprite GetTileEmber()
	{
		Mawaru_Sprite result = tile_Ember[tile_emberIndex];
		tile_emberIndex++;
		if (tile_emberIndex >= tile_Ember.Count)
		{
			tile_emberIndex = 0;
		}
		return result;
	}

	public void FadeUITextColor(Color fore, Color back, float time)
	{
		DOTween.To(() => scrVfx.instance.currentColourScheme.colourText, delegate(Color x)
		{
			scrVfx.instance.currentColourScheme.colourText = x;
		}, fore, time).SetEase(Ease.Linear);
		foreach (Shadow s in textShadows)
		{
			DOTween.To(() => s.effectColor, delegate(Color x)
			{
				s.effectColor = x;
			}, back, time).SetEase(Ease.Linear);
		}
	}

	public float beats(float numBeats, float bpm = -1f)
	{
		return numBeats * (60f / ((bpm == -1f) ? currentBPM : bpm)) / rate;
	}

	public void SortTables()
	{
		beatActions.Sort(SortByTime);
		beatActionArgs.Sort(SortByTimeA);
	}

	public static int SortByTime(ActionEntry m1, ActionEntry m2)
	{
		return m1.fTime.CompareTo(m2.fTime);
	}

	public static int SortByTimeA(ActionEntryArg m1, ActionEntryArg m2)
	{
		return m1.fTime.CompareTo(m2.fTime);
	}

	public void mb(float b, Action a, float persist = -1f)
	{
		beatActions.Add(new ActionEntry(b, a, persist));
	}

	public void mba(float b, Action<List<float>> a, List<float> args, float persist = -1f)
	{
		beatActionArgs.Add(new ActionEntryArg(b, a, args, persist));
	}

	public void mu(float t, Action a, float persist = -1f)
	{
		beatUpdates.Add(new ActionEntry(t, a, persist));
	}

	public void mpf(float t, Action a, float persist = -1f)
	{
		mu(t, a, persist);
	}

	public void Awake()
	{
		GetComponentCalls();
		bpms.Add(new Tuple<double, double>(0.0, 100.0));
	}

	private void GetComponentCalls()
	{
		BGMovingCam = GameObject.Find("BGMovingCam").GetComponent<Camera>();
		thisTransform = base.transform;
	}

	public void SetResultTextPos()
	{
		if (!GCS.practiceMode)
		{
			scrUIController.instance.txtResults.transform.GetComponent<RectTransform>().DOAnchorPosY(-270f, 0f);
			ADOBase.controller.canExitLevel = false;
		}
	}

	public void SetResultTextPos2()
	{
		if (!GCS.practiceMode)
		{
			scrUIController.instance.txtResults.transform.GetComponent<RectTransform>().DOAnchorPosY(-190f, 0f);
			ADOBase.controller.canExitLevel = false;
		}
	}

	private void FirstUpdate()
	{
		if (ADOBase.controller.currentSeqID > 0)
		{
			songTime = ADOBase.controller.currFloor.entryTime - ADOBase.conductor.crotchetAtStart / (double)ADOBase.controller.currFloor.speed * (double)ADOBase.conductor.adjustedCountdownTicks;
			songBeat = scrMisc.GetBeatFromSongTimeAndBPMs(songTime, bpms);
		}
		else
		{
			songTime = -100.0;
			songBeat = -100.0;
		}
		currentBPM = (float)bpms[0].Item2;
		for (int i = lastSeenBPMIndex; i < bpms.Count && songBeat >= bpms[i].Item1; i++)
		{
			currentBPM = (float)bpms[i].Item2;
			lastSeenBPMIndex = i;
		}
		lastFloor = ADOBase.controller.currFloor.seqID;
		lastTime = songTime;
		lastBeat = songBeat;
		rate = ADOBase.conductor.song.pitch;
		speed = 1f / rate;
		CacheFloorPosition();
		firstUpdateCompleted = true;
		if (ADOBase.controller.currentSeqID > 0 && songBeat < 0.0)
		{
			firstUpdateCompleted = false;
		}
		spawnFloor = ADOBase.controller.currentSeqID;
	}

	public void LateUpdate()
	{
		if (updates > 1 && !firstUpdateCompleted)
		{
			FirstUpdate();
		}
		updates++;
	}

	public void Update()
	{
		if (firstUpdateCompleted)
		{
			practiceWon = ADOBase.controller.currentState == States.Won && GCS.practiceMode;
			if (ADOBase.controller.currentState != States.Start && ADOBase.controller.currentState != States.Fail2 && !practiceWon)
			{
				songTime = ADOBase.conductor.songposition_minusi - ADOBase.conductor.crotchetAtStart * (double)ADOBase.conductor.adjustedCountdownTicks + (double)Time.deltaTime;
				songBeat = scrMisc.GetBeatFromSongTimeAndBPMs(songTime, bpms);
			}
			currentBPM = (float)bpms[0].Item2;
			for (int i = lastSeenBPMIndex; i < bpms.Count && songBeat >= bpms[i].Item1; i++)
			{
				currentBPM = (float)bpms[i].Item2;
				lastSeenBPMIndex = i;
			}
			ReadTables(songBeat, songTime);
			lastTime = songTime - (double)Time.deltaTime;
			lastBeat = songBeat - (double)Time.deltaTime * ((double)ADOBase.conductor.bpm * ADOBase.controller.playerOne.planetarySystem.speed) / 60.0;
			lastFloor = ADOBase.controller.currFloor.seqID;
		}
	}

	public void ReadTables(double beat, double time)
	{
		while (curActionB < beatActions.Count && beat >= beatActions[curActionB].fTime)
		{
			if (beat < beatActions[curActionB].fPersist || (beatActions[curActionB].fPersist < 0.0 && lastBeat < beatActions[curActionB].fTime))
			{
				beatActions[curActionB].aFunc();
			}
			curActionB++;
		}
		while (curActionBArgs < beatActionArgs.Count && beat >= beatActionArgs[curActionBArgs].fTime)
		{
			if (beat < beatActionArgs[curActionBArgs].fPersist || (beatActionArgs[curActionBArgs].fPersist < 0.0 && lastBeat < beatActionArgs[curActionBArgs].fTime))
			{
				ActionEntryArg actionEntryArg = beatActionArgs[curActionBArgs];
				actionEntryArg.aFunc(actionEntryArg.args);
			}
			curActionBArgs++;
		}
		for (int i = 0; i < beatUpdates.Count; i++)
		{
			if (beat >= beatUpdates[i].fTime && beat < beatUpdates[i].fPersist)
			{
				beatUpdates[i].aFunc();
			}
		}
	}

	public void EnableStuff(List<Mawaru_Sprite> stuff)
	{
		foreach (Mawaru_Sprite item in stuff)
		{
			Color color = item.render.material.GetColor("_Color");
			Color value = new Color(color.r, color.g, color.b, 0f);
			item.enabled = true;
			item.render.enabled = true;
			item.render.material.SetColor("_Color", value);
		}
	}

	public void EnableStuff(List<Mawaru_Sprite> stuff, Color col)
	{
		foreach (Mawaru_Sprite item in stuff)
		{
			item.enabled = true;
			item.render.enabled = true;
			item.render.material.SetColor("_Color", col);
		}
	}

	public void FadeStuff(List<Mawaru_Sprite> stuff, float opacity, float time)
	{
		foreach (Mawaru_Sprite s in stuff)
		{
			Color color = s.render.material.GetColor("_Color");
			Color endValue = new Color(color.r, color.g, color.b, opacity);
			if (opacity <= 0f)
			{
				s.render.material.DOColor(endValue, time).OnComplete(delegate
				{
					s.render.enabled = false;
					s.enabled = false;
				});
			}
			else
			{
				s.render.material.DOColor(endValue, time);
			}
		}
	}

	public void FadeStuff(List<Mawaru_Sprite> stuff, Color col, float time)
	{
		foreach (Mawaru_Sprite s in stuff)
		{
			if (col.a == 0f)
			{
				s.render.material.DOColor(col, time).OnComplete(delegate
				{
					s.render.enabled = false;
					s.enabled = false;
				});
			}
			else
			{
				s.render.material.DOColor(col, time);
			}
		}
	}

	public void EnableStuff(Mawaru_Sprite s)
	{
		Color color = s.render.material.GetColor("_Color");
		Color value = new Color(color.r, color.g, color.b, 0f);
		s.enabled = true;
		s.render.enabled = true;
		s.render.material.SetColor("_Color", value);
	}

	public void EnableStuff(Mawaru_Sprite s, Color col)
	{
		s.enabled = true;
		s.render.enabled = true;
		s.render.material.SetColor("_Color", col);
	}

	public void FadeStuff(Mawaru_Sprite s, float opacity, float time)
	{
		Color color = s.render.material.GetColor("_Color");
		Color endValue = new Color(color.r, color.g, color.b, opacity);
		if (opacity <= 0f)
		{
			s.render.material.DOColor(endValue, time).OnComplete(delegate
			{
				s.render.enabled = false;
				s.enabled = false;
			});
		}
		else
		{
			s.render.material.DOColor(endValue, time);
		}
	}

	public void FadeStuff(Mawaru_Sprite s, Color col, float time)
	{
		if (col.a == 0f)
		{
			s.render.material.DOColor(col, time).OnComplete(delegate
			{
				s.render.enabled = false;
				s.enabled = false;
			});
		}
		else
		{
			s.render.material.DOColor(col, time);
		}
	}

	public void BGOrtho()
	{
		BGMovingCam.orthographic = true;
	}

	public void BGPersp()
	{
		BGMovingCam.orthographic = false;
	}

	private float SCALE(float x, float l1, float h1, float l2, float h2)
	{
		return (x - l1) * (h2 - l2) / (h1 - l1) + l2;
	}

	public void CacheFloorPosition()
	{
		foreach (scrFloor listFloor in scrLevelMaker.instance.listFloors)
		{
			listFloor.storedPos = listFloor.transform.position;
			listFloor.storedRot = listFloor.transform.eulerAngles;
			double angleMoved = scrMisc.GetAngleMoved(listFloor.entryangle, listFloor.exitangle, !listFloor.isCCW);
			float f = (float)(listFloor.entryangle - angleMoved / 2.0);
			listFloor.perpendicularAngleVector = Vector3.right * Mathf.Sin(f) + Vector3.up * Mathf.Cos(f);
		}
	}

	public void CacheFloorPosition(int min, int max)
	{
		for (int i = Math.Max(min, 0); i < Math.Min(max + 1, scrLevelMaker.instance.listFloors.Count); i++)
		{
			scrFloor scrFloor2 = scrLevelMaker.instance.listFloors[i];
			scrFloor2.storedPos = scrFloor2.transform.position;
			scrFloor2.storedRot = scrFloor2.transform.eulerAngles;
			double angleMoved = scrMisc.GetAngleMoved(scrFloor2.entryangle, scrFloor2.exitangle, !scrFloor2.isCCW);
			_ = scrFloor2.isCCW;
			float f = (float)(scrFloor2.entryangle - angleMoved / 2.0);
			scrFloor2.perpendicularAngleVector = Vector3.right * Mathf.Cos(f) + Vector3.up * Mathf.Sin(f);
		}
	}

	public void FloorResetPosition(scrFloor floor)
	{
		floor.transform.position = floor.storedPos;
		floor.transform.eulerAngles = floor.storedRot;
	}

	public void FloorBeatVibe(scrFloor floor, float amt, float ang = -1f, float fBeatOffset = 0f, float fBeatMult = 1f, float rotAmt = 0f)
	{
		float num = 0.2f;
		float num2 = 0.5f;
		float num3 = ((float)songBeat + num + fBeatOffset) * fBeatMult;
		bool flag = Mathf.Floor(num3) % 2f != 0f;
		modPosition = Vector3.zero;
		modRotation = 0f;
		bool flag2 = false;
		if (num3 < 0f)
		{
			flag2 = true;
		}
		if (!flag2)
		{
			num3 -= Mathf.Floor(num3);
			num3 += 1f;
			num3 -= Mathf.Floor(num3);
		}
		if (num3 >= num2)
		{
			flag2 = true;
		}
		if (!flag2)
		{
			float num4;
			if (num3 < num)
			{
				num4 = SCALE(num3, 0f, num, 0f, 1f);
				num4 *= num4;
			}
			else
			{
				num4 = SCALE(num3, num, num2, 1f, 0f);
				num4 = 1f - (1f - num4) * (1f - num4);
			}
			if (flag)
			{
				num4 *= -1f;
			}
			if (floor.seqID % 2 == 1)
			{
				num4 *= -1f;
			}
			float num5 = 0.5f * num4;
			modRotation = num5 * rotAmt;
			if (ang == -1f)
			{
				modPosition = floor.perpendicularAngleVector * amt * num5;
			}
			else
			{
				modPosition = (Vector3.right * Mathf.Sin(ang) + Vector3.up * Mathf.Cos(ang)) * amt * num5;
			}
		}
		floor.thisTransform.position += modPosition;
		floor.thisTransform.eulerAngles += Vector3.forward * modRotation;
	}

	public void FloorDrunkVibe(scrFloor floor, float amt, float ang = -1f, float period = 4f, float offset = 0f, float drunkspeed = 1f)
	{
		float num = floor.seqID;
		if (floor.prevfloor != null && floor.prevfloor.midSpin)
		{
			num -= 2f;
		}
		float num2 = 0.5f * Mathf.Sin((num + Time.time * drunkspeed) / period * 2f * (float)Math.PI + offset);
		if (ang == -1f)
		{
			modPosition = floor.perpendicularAngleVector * amt * num2;
		}
		else
		{
			modPosition = (Vector3.right * Mathf.Sin(ang) + Vector3.up * Mathf.Cos(ang)) * amt * num2;
		}
		floor.thisTransform.position += modPosition;
	}

	public void FloorDrunkVibeXPos(scrFloor floor, float amt, float ang = -1f, float period = 4f, float offset = 0f, float drunkspeed = 1f)
	{
		float x = floor.transform.position.x;
		float num = 0.5f * Mathf.Sin((x + Time.time * drunkspeed) / period * 2f * (float)Math.PI + offset);
		if (ang == -1f)
		{
			modPosition = floor.perpendicularAngleVector * amt * num;
		}
		else
		{
			modPosition = (Vector3.right * Mathf.Sin(ang) + Vector3.up * Mathf.Cos(ang)) * amt * num;
		}
		floor.thisTransform.position += modPosition;
	}

	public int[] SaveMedals(string worldString, List<int> newMedals)
	{
		int[] medalsForDLCLevel = Persistence.GetMedalsForDLCLevel(worldString);
		for (int i = 0; i < newMedals.Count; i++)
		{
			if (newMedals[i] > medalsForDLCLevel[i])
			{
				medalsForDLCLevel[i] = newMedals[i];
			}
		}
		Persistence.SetMedalsForDLCLevel(worldString, medalsForDLCLevel);
		return medalsForDLCLevel;
	}

	public void SaveT5Time(float newTime)
	{
		float num = Persistence.t5BestTime;
		if (newTime < num)
		{
			num = newTime;
		}
		Persistence.t5BestTime = num;
	}
}
