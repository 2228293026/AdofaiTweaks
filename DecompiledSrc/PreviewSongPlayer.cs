using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PreviewSongPlayer : ADOBase
{
	[Header("Tweakables")]
	public float fadeTime;

	public float minDuration;

	public float maxDuration;

	public AudioSource audioSource;

	private AudioClip audioClip;

	private float duration;

	private float startTime;

	private float endTime;

	private bool looping;

	public bool playing
	{
		get
		{
			if (audioSource != null && audioSource.isPlaying)
			{
				return audioSource.volume != 0f;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (playing)
		{
			if (audioSource.time >= endTime - fadeTime)
			{
				FadePreview(fadeOut: true, previewOnly: true);
			}
			if (looping && audioSource.time >= endTime - 1f / 60f)
			{
				Play(audioClip, startTime, duration);
			}
		}
	}

	private void FadePreview(bool fadeOut, bool previewOnly = false)
	{
		DOTween.To(() => audioSource.volume, delegate(float x)
		{
			audioSource.volume = x;
		}, (!fadeOut) ? 1 : 0, fadeTime);
		if (!previewOnly)
		{
			ADOBase.conductor.song.DOFade(fadeOut ? 1 : 0, fadeTime);
		}
	}

	public void Stop()
	{
		looping = false;
		FadePreview(fadeOut: true);
	}

	public void Play(AudioClip newAudioClip, float newStartTime, float newDuration, float volume = -1f)
	{
		audioSource.Stop();
		audioClip = newAudioClip;
		startTime = newStartTime;
		duration = newDuration;
		float length = audioClip.length;
		if (length < minDuration)
		{
			return;
		}
		if (startTime < 0f || startTime > audioClip.length)
		{
			startTime = 0f;
		}
		duration = Mathf.Clamp(duration, minDuration, maxDuration);
		if (startTime + duration > length)
		{
			duration = length - startTime;
			if (duration < minDuration)
			{
				return;
			}
		}
		audioSource.clip = audioClip;
		audioSource.time = startTime;
		if (volume != -1f)
		{
			audioSource.volume = volume;
		}
		endTime = startTime + duration;
		looping = true;
		FadePreview(fadeOut: false);
		audioSource.Play();
	}
}
