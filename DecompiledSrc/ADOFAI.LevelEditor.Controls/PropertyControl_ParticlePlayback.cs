using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_ParticlePlayback : PropertyControl
{
	public TMP_Text editText;

	public Button playPauseButton;

	public Button stopButton;

	public Button resetButton;

	public Button editButton;

	public GameObject playIcon;

	public GameObject pauseIcon;

	[NonSerialized]
	public scrParticleDecoration Decoration;

	private bool _playing;

	private void Awake()
	{
		editText.color = Color.black;
		playPauseButton.onClick.AddListener(OnPlayPause);
		stopButton.onClick.AddListener(OnStop);
		resetButton.onClick.AddListener(OnReset);
		editButton.onClick.AddListener(OnEdit);
	}

	private void OnPlayPause()
	{
		ParticleSystem particleSystem = Decoration.particleSystem;
		ParticleSystem.MainModule main = particleSystem.main;
		main.useUnscaledTime = true;
		if (!particleSystem.isPlaying)
		{
			particleSystem.Play();
		}
		else
		{
			particleSystem.Pause();
		}
	}

	private void Update()
	{
		bool isPlaying = Decoration.particleSystem.isPlaying;
		if (_playing != isPlaying)
		{
			playIcon.gameObject.SetActive(!isPlaying);
			pauseIcon.gameObject.SetActive(isPlaying);
			_playing = isPlaying;
		}
	}

	private void OnReset()
	{
		Decoration.particleSystem.Clear();
	}

	private void OnStop()
	{
		Decoration.particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	private void OnEdit()
	{
		ADOBase.editor.ShowParticleEditor(propertiesPanel.inspectorPanel.selectedEvent);
	}
}
