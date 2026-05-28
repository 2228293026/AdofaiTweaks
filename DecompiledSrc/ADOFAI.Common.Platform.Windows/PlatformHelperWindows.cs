using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace ADOFAI.Common.Platform.Windows;

public class PlatformHelperWindows : IPlatformHelper
{
	private delegate void SpeakerNameChangeCallback();

	private const string Lib = "adofaiplatformhelper";

	[DllImport("adofaiplatformhelper")]
	private static extern string get_speaker_name();

	[DllImport("adofaiplatformhelper")]
	private static extern AudioOutputType get_speaker_type();

	[DllImport("adofaiplatformhelper")]
	private static extern void init();

	[DllImport("adofaiplatformhelper")]
	private static extern void update();

	[DllImport("adofaiplatformhelper")]
	private static extern void set_speaker_name_callback(SpeakerNameChangeCallback callback);

	public PlatformHelperWindows()
	{
		try
		{
			init();
			set_speaker_name_callback(OnSpeakerNameChange);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public string GetActiveAudioDeviceName()
	{
		try
		{
			return get_speaker_name() ?? "*";
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return "*";
		}
	}

	public AudioOutputType GetActiveAudioDeviceType()
	{
		try
		{
			return get_speaker_type();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return AudioOutputType.Speaker;
		}
	}

	[MonoPInvokeCallback(typeof(SpeakerNameChangeCallback))]
	private static void OnSpeakerNameChange()
	{
		scrConductor.isAudioOutputDeviceChanged = true;
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void Update()
	{
		try
		{
			update();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}
}
