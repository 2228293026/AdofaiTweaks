using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace ADOFAI.Common.Platform.Linux;

public class PlatformHelperLinux : IPlatformHelper
{
	private string _audioOutputDeviceName = "*";

	private Thread _audioOutputCheckThread;

	private bool _shouldStopOutputCheckThread;

	[DllImport("adofaipulse")]
	private static extern string get_device_name();

	public PlatformHelperLinux()
	{
		RefreshDevice();
		_audioOutputCheckThread = new Thread(AudioOutputCheckLoop);
		_audioOutputCheckThread.Start();
	}

	private void AudioOutputCheckLoop()
	{
		while (!_shouldStopOutputCheckThread)
		{
			Thread.Sleep(1000);
			RefreshDevice();
		}
	}

	private void RefreshDevice()
	{
		try
		{
			string device_name = get_device_name();
			if (_audioOutputDeviceName != "*" && _audioOutputDeviceName != device_name)
			{
				scrConductor.isAudioOutputDeviceChanged = true;
			}
			_audioOutputDeviceName = device_name;
		}
		catch (DllNotFoundException)
		{
		}
	}

	public string GetActiveAudioDeviceName()
	{
		return _audioOutputDeviceName;
	}

	public AudioOutputType GetActiveAudioDeviceType()
	{
		return AudioOutputType.Speaker;
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void Update()
	{
	}
}
