using ADOFAI.Common.Platform.Linux;
using ADOFAI.Common.Platform.Mac;
using ADOFAI.Common.Platform.Windows;

namespace ADOFAI.Common.Platform;

public static class PlatformHelper
{
	private static IPlatformHelper _instance;

	public static IPlatformHelper instance => _instance ?? (_instance = Init());

	public static IPlatformHelper Init()
	{
		return ADOBase.platform switch
		{
			global::Platform.Linux => new PlatformHelperLinux(), 
			global::Platform.Windows => new PlatformHelperWindows(), 
			global::Platform.Mac => new PlatformHelperMac(), 
			_ => new PlatformHelperDefault(), 
		};
	}
}
