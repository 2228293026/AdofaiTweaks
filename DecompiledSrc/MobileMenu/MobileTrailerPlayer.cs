using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MobileMenu;

public class MobileTrailerPlayer : ADOBase
{
	public VideoPlayer videoPlayer;

	public AudioSource audioSource;

	public Image thumbnail;

	public Image playButton;

	public Image loadingIcon;

	public TMP_Text noInternet;

	public Button playerButton;

	public Action onPlay;

	public Action onPause;

	public bool isSelected;

	private bool internetConnection = true;

	public void Awake()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		playerButton.onClick.AddListener(Toggle);
		videoPlayer.loopPointReached += new EventHandler(OnStop);
		videoPlayer.started += new EventHandler(OnPlay);
		audioSource.outputAudioMixerGroup = RDUtils.GetMixerGroup(MixerGroup.ConductorMusic);
	}

	public IEnumerator Prepare(string url, bool show)
	{
		if (show)
		{
			UnityWebRequest request = new UnityWebRequest("https://google.com");
			yield return request.SendWebRequest();
			bool flag = request.error != null;
			internetConnection = !flag;
			ShowNoInternet(flag);
			ShowPauseButton(!flag);
			if (flag)
			{
				noInternet.text = RDString.Get("levelSelect.checkInternetConnection");
				noInternet.SetLocalizedFont();
			}
			else if (videoPlayer.url != url)
			{
				videoPlayer.url = url;
				videoPlayer.audioOutputMode = (VideoAudioOutputMode)1;
				videoPlayer.EnableAudioTrack((ushort)0, true);
				videoPlayer.SetTargetAudioSource((ushort)0, audioSource);
				videoPlayer.Prepare();
			}
		}
	}

	public void Toggle()
	{
		if (!MobileMenuController.instance.dragging && internetConnection)
		{
			if (videoPlayer.isPlaying)
			{
				Pause();
			}
			else
			{
				Play();
			}
		}
	}

	public void Play()
	{
		videoPlayer.Play();
		ShowLoadingIcon(show: true);
		ShowPauseButton(show: false);
	}

	private void OnPlay(VideoPlayer _ = null)
	{
		if (onPlay != null)
		{
			onPlay();
		}
		ShowLoadingIcon(show: false);
		thumbnail.gameObject.SetActive(value: false);
	}

	public void Stop()
	{
		videoPlayer.Stop();
		OnStop();
	}

	private void OnStop(VideoPlayer _ = null)
	{
		if (onPause != null)
		{
			onPause();
		}
		ShowPauseButton(show: true);
		ShowLoadingIcon(show: false);
		thumbnail.gameObject.SetActive(value: true);
	}

	public void Pause()
	{
		if (onPause != null)
		{
			onPause();
		}
		ShowPauseButton(show: true);
		videoPlayer.Pause();
	}

	private void ShowPauseButton(bool show)
	{
		playButton.gameObject.SetActive(show);
	}

	private void ShowLoadingIcon(bool show)
	{
		loadingIcon.DOKill();
		if (show)
		{
			loadingIcon.color = loadingIcon.color.WithAlpha(0f);
			loadingIcon.DOFade(1f, 0.25f);
		}
		loadingIcon.gameObject.SetActive(show);
	}

	private void ShowNoInternet(bool show)
	{
		noInternet.gameObject.SetActive(show);
	}

	private void Update()
	{
		if (isSelected)
		{
			if (videoPlayer.isPlaying && RDInput.cancelPress)
			{
				Toggle();
			}
			loadingIcon.transform.eulerAngles += Vector3.forward * Time.deltaTime * 180f;
			float f = loadingIcon.transform.eulerAngles.z * ((float)Math.PI / 180f);
			loadingIcon.GetComponent<Shadow>().effectDistance = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * new Vector2(16f, -16f);
			videoPlayer.playbackSpeed = Time.timeScale;
		}
	}
}
