using System;
using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public abstract class ffxPlusBase : ADOBase
{
	public bool hifiEffect;

	public bool disableIfMinFx;

	public bool disableIfMaxFx;

	public double startTime;

	public double startEffectOffset;

	public float duration;

	public Ease ease = Ease.Linear;

	public bool triggered;

	public float degreeOffset;

	public bool[] conditionalInfo;

	public bool runManually;

	public LevelEvent sourceLevelEvent;

	[NonSerialized]
	public scrCamera cam;

	[NonSerialized]
	public scrController ctrl;

	[NonSerialized]
	public scrConductor cond;

	[NonSerialized]
	public scrFloor floor;

	[NonSerialized]
	public scrVfxPlus vfx;

	[NonSerialized]
	public List<scrFloor> floors;

	[NonSerialized]
	public int floorID;

	[NonSerialized]
	public float crotchet;

	public int instanceID => GetInstanceID();

	protected virtual IEnumerable<Tween> eventTweens => null;

	public virtual bool runOnHit => false;

	public virtual void Awake()
	{
		cam = scrCamera.instance;
		ctrl = scrController.instance;
		cond = scrConductor.instance;
		vfx = scrVfxPlus.instance;
		floor = GetComponent<scrFloor>();
		if (conditionalInfo != null && conditionalInfo.Length == 5)
		{
			conditionalInfo = new bool[8]
			{
				conditionalInfo[0],
				conditionalInfo[1],
				conditionalInfo[1],
				conditionalInfo[2],
				conditionalInfo[2],
				conditionalInfo[3],
				conditionalInfo[3],
				conditionalInfo[4]
			};
		}
		else
		{
			conditionalInfo = new bool[8];
		}
	}

	public virtual void Decode(LevelEvent evnt)
	{
	}

	public virtual void PrepVfx()
	{
	}

	public void StartEffect()
	{
		StartEffect(null);
	}

	public abstract void StartEffect(scrPlanet planet);

	public void StartEffectWithOffset(scrPlanet planet = null)
	{
		DOVirtual.DelayedCall(60f / ADOBase.conductor.bpm / ADOBase.conductor.song.pitch * floor.speed * (degreeOffset / 180f), delegate
		{
			if (!ADOBase.controller.paused)
			{
				StartEffect(planet);
			}
		}, ignoreTimeScale: false);
	}

	public virtual void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		scrFloor component = base.gameObject.GetComponent<scrFloor>();
		startTime = component.entryTime + (double)(degreeOffset * ((float)Math.PI / 180f) / (float)Math.PI * (60f / (bpm * component.speed)));
		this.degreeOffset = degreeOffset;
	}

	public virtual void ScrubToTime(float t)
	{
		if ((double)t < startTime)
		{
			return;
		}
		StartEffect();
		triggered = true;
		double num = startTime + (double)duration;
		bool flag = Mathf.Approximately(t, (float)num);
		if (ADOBase.currentLevel == "XO-X" && this is ffxCameraPlus)
		{
			flag = true;
		}
		if (eventTweens == null)
		{
			return;
		}
		if ((double)t >= num || flag)
		{
			foreach (Tween eventTween in eventTweens)
			{
				eventTween?.Kill(complete: true);
			}
			return;
		}
		float num2 = (t - (float)startTime) / cond.song.pitch - scrConductor.calibration_i;
		foreach (Tween eventTween2 in eventTweens)
		{
			if (eventTween2 != null)
			{
				bool num3 = Mathf.Approximately(num2, 0f);
				if (eventTween2.active)
				{
					eventTween2.Goto(num2, andPlay: true);
				}
				if (!num3)
				{
					vfx.pausedTweens.Add(eventTween2);
				}
			}
		}
	}

	public virtual void Kill()
	{
		if (eventTweens == null)
		{
			return;
		}
		foreach (Tween eventTween in eventTweens)
		{
			eventTween.Kill();
		}
	}

	public void AdjustDurationForHardbake()
	{
		if (!ADOBase.customLevel)
		{
			duration /= scrConductor.instance.song.pitch;
		}
	}

	public bool IsAllowedByVisualSettings(VisualQuality visualQuality, VisualEffects visualEffects)
	{
		if (hifiEffect && visualQuality != VisualQuality.High)
		{
			return false;
		}
		if (disableIfMinFx && visualEffects == VisualEffects.Minimum)
		{
			return false;
		}
		if (disableIfMaxFx && visualEffects == VisualEffects.Full)
		{
			return false;
		}
		return true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "star.png", allowScaling: true);
	}
}
