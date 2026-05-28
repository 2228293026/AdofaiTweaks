using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace ADOFAI.Common.Platform.Mac;

public class PlatformHelperMac : IPlatformHelper
{
	private static class NativeMethods
	{
		public delegate void SpeakerChangeCallback();

		private const string Lib = "adofaiplatformhelper";

		[DllImport("adofaiplatformhelper", EntryPoint = "get_speaker_name")]
		public static extern string GetSpeakerName();

		[DllImport("adofaiplatformhelper", EntryPoint = "get_speaker_type")]
		public static extern AudioOutputType GetSpeakerType();

		[DllImport("adofaiplatformhelper", EntryPoint = "set_speaker_change_callback")]
		public static extern void SetSpeackerChangeCallback(SpeakerChangeCallback callback);

		[DllImport("adofaiplatformhelper", EntryPoint = "init")]
		public static extern void Initialize();
	}

	private static string _audioOutputDeviceName = "*";

	private static AudioOutputType _audioOutputDeviceType = AudioOutputType.Speaker;

	[MonoPInvokeCallback(typeof(NativeMethods.SpeakerChangeCallback))]
	private static void SpeakerChanged()
	{
		_audioOutputDeviceName = NativeMethods.GetSpeakerName();
		_audioOutputDeviceType = NativeMethods.GetSpeakerType();
		scrConductor.isAudioOutputDeviceChanged = true;
	}

	public PlatformHelperMac()
	{
		NativeMethods.Initialize();
		NativeMethods.SetSpeackerChangeCallback(SpeakerChanged);
	}

	public string GetActiveAudioDeviceName()
	{
		return _audioOutputDeviceName;
	}

	public AudioOutputType GetActiveAudioDeviceType()
	{
		return _audioOutputDeviceType;
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void Update()
	{
	}
}
