using System;
using ADOFAI;
using UnityEngine;
using UnityEngine.Audio;

public class ffxPlaySound : ffxPlusBase
{
	public HitSound hitSound;

	public string soundKey;

	public double offset;

	public double playDuration;

	public float pitch;

	public float volume = 0.5f;

	private bool useHitSound = true;

	private AudioMixerGroup group;

	private bool ready
	{
		get
		{
			if (!useHitSound)
			{
				return ADOBase.audioManager.audioLib.ContainsKey(soundKey);
			}
			return false;
		}
	}

	public override void Awake()
	{
		base.Awake();
		startEffectOffset = 1.0;
		group = RDUtils.GetMixerGroup("ConductorPlaySound");
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (useHitSound || ready)
		{
			double num = ADOBase.conductor.dspTimeSongPosZero + startTime / (double)ADOBase.conductor.song.pitch - offset;
			AudioSource audioSource;
			if (useHitSound)
			{
				double value = 0.0;
				ADOBase.gc.hitSoundOffsets.TryGetValue(hitSound, out value);
				audioSource = AudioManager.Play("snd" + hitSound, num - value, group, volume);
				audioSource.time = Mathf.Max((float)(value - startEffectOffset), 0f);
			}
			else
			{
				audioSource = AudioManager.Play(soundKey, num, group, volume);
			}
			if (playDuration > 0.0)
			{
				audioSource.SetScheduledEndTime(num + playDuration / (double)ADOBase.conductor.song.pitch);
			}
			audioSource.pitch = pitch;
		}
	}

	public override void ScrubToTime(float t)
	{
		double num = startTime - (double)t;
		startEffectOffset = Math.Clamp(num, 0.1, startEffectOffset);
		if (num <= 0.10000000149011612)
		{
			triggered = true;
		}
		else
		{
			base.ScrubToTime(t);
		}
	}

	public void OnDestroy()
	{
	}

	public override void Decode(LevelEvent evnt)
	{
		string text = evnt.GetString("hitsound");
		if (Enum.IsDefined(typeof(HitSound), text))
		{
			Enum.TryParse<HitSound>(text, out var result);
			hitSound = result;
			useHitSound = true;
		}
		else
		{
			soundKey = text + "*external";
			useHitSound = false;
		}
		offset = (float)evnt.GetInt("offset") / 1000f;
		playDuration = (float)evnt.GetInt("playDuration") / 1000f;
		pitch = (float)evnt.GetInt("pitch") / 100f;
		volume = (float)evnt.GetInt("hitsoundVolume") / 100f;
	}
}
