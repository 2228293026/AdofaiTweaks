using UnityEngine;
using UnityEngine.Audio;

public class HitSoundPreviewPlayer : ADOBase
{
	private TweakableDropdownItem item;

	private HitSound hitSound;

	private AudioSource audioSource;

	private AudioMixerGroup group;

	private void Awake()
	{
		item = GetComponentInParent<TweakableDropdownItem>();
		hitSound = (HitSound)item.index;
		group = RDUtils.GetMixerGroup("ConductorHitsounds");
		if (hitSound == HitSound.None)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void CreateAudioSource()
	{
		audioSource = AudioManager.Instance.MakeSource($"snd{hitSound}");
		audioSource.volume = 1f;
		audioSource.ignoreListenerPause = true;
		audioSource.outputAudioMixerGroup = group;
	}

	public void OnClick()
	{
		if (audioSource == null)
		{
			CreateAudioSource();
		}
		audioSource.Play();
	}
}
