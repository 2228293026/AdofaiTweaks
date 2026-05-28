namespace ADOFAI.Common.Platform;

public struct AudioDevice
{
	public string Name;

	public AudioOutputType Type;

	public static AudioDevice Default => new AudioDevice
	{
		Name = "*",
		Type = AudioOutputType.Speaker
	};
}
