using UnityEngine;

namespace ADOFAI.Common.Platform;

public class PlatformHelperDefault : IPlatformHelper
{
	public string GetActiveAudioDeviceName()
	{
		return "*";
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
