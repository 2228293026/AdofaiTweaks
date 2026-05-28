using DG.Tweening;
using UnityEngine;

public class scrSfx : ADOBase
{
	[SerializeField]
	private AudioSource conductorMusicSrc;

	[SerializeField]
	private AudioSource hitsoundSrc;

	[SerializeField]
	private AudioSource sfxSrc;

	[SerializeField]
	private AudioSource conductorSfxSrc;

	[SerializeField]
	private AudioSource interfaceSrc;

	[SerializeField]
	private AudioSource fallbackSrc;

	[SerializeField]
	private float baseVolume = 1f;

	public static scrSfx instance { get; private set; }

	private void Awake()
	{
		instance = this;
		conductorMusicSrc.ignoreListenerPause = true;
		hitsoundSrc.ignoreListenerPause = true;
		sfxSrc.ignoreListenerPause = true;
		conductorSfxSrc.ignoreListenerPause = true;
		interfaceSrc.ignoreListenerPause = true;
		fallbackSrc.ignoreListenerPause = true;
	}

	public AudioClip PlaySfx(HitSound hitSound, MixerGroup group = MixerGroup.Fallback, float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		AudioClip clip = AudioManager.Instance.FindOrLoadAudioClip($"snd{hitSound}");
		return PlaySfx(clip, group, volume, pitch, pan);
	}

	public AudioClip PlaySfx(SfxSound sfxSound, MixerGroup group = MixerGroup.Fallback, float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		AudioClip clip = ADOBase.gc.soundEffects[(int)sfxSound];
		return PlaySfx(clip, group, volume, pitch, pan);
	}

	public AudioClip PlaySfx(string clipName, MixerGroup group = MixerGroup.Fallback, float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		AudioClip clip = AudioManager.Instance.FindOrLoadAudioClip(clipName);
		return PlaySfx(clip, group, volume, pitch, pan);
	}

	public AudioClip PlaySfx(AudioClip clip, MixerGroup group = MixerGroup.Fallback, float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		AudioSource audioSource = fallbackSrc;
		switch (group)
		{
		case MixerGroup.ConductorHitsounds:
			audioSource = hitsoundSrc;
			break;
		case MixerGroup.SfxParent:
			audioSource = sfxSrc;
			break;
		case MixerGroup.ConductorSfx:
			audioSource = conductorSfxSrc;
			break;
		case MixerGroup.InterfaceParent:
			audioSource = interfaceSrc;
			break;
		}
		audioSource.volume = baseVolume;
		audioSource.pitch = pitch;
		audioSource.panStereo = pan;
		audioSource.PlayOneShot(clip, volume);
		if (group == MixerGroup.Fallback || audioSource.outputAudioMixerGroup.ToString() == "Fallback" || audioSource.outputAudioMixerGroup == null)
		{
			Debug.LogWarning("Invalid mixer assignment for [" + clip.name + "], notify Satellite@7thbe.at");
		}
		return clip;
	}

	public void PlayMusicVolumePreview()
	{
		if (!conductorMusicSrc.isPlaying)
		{
			conductorMusicSrc.DOKill();
			if (ADOBase.conductor.song.clip != null)
			{
				conductorMusicSrc.clip = ADOBase.conductor.song.clip;
				conductorMusicSrc.volume = ADOBase.conductor.song.volume;
				conductorMusicSrc.pitch = ADOBase.conductor.song.pitch;
			}
			else
			{
				conductorMusicSrc.clip = ADOBase.gc.soundEffects[58];
				conductorMusicSrc.volume = 1f;
				conductorMusicSrc.pitch = 1f;
			}
			conductorMusicSrc.Play();
		}
	}

	public void StopMusicVolumePreview()
	{
		conductorMusicSrc.DOKill();
		conductorMusicSrc.DOFadeStop(0.3f).SetUpdate(isIndependentUpdate: true);
	}
}
