namespace ADOFAI.Common.Platform;

public interface IPlatformHelper
{
	string GetActiveAudioDeviceName();

	AudioOutputType GetActiveAudioDeviceType();

	void Update();

	void OpenURL(string url);
}
