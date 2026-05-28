using System;
using System.Runtime.InteropServices;
using System.Text;

public static class MacUtils
{
	public static string downloadsPath;

	private const string RDMacPluginName = "RDMacPlugin";

	[DllImport("RDMacPlugin")]
	private static extern IntPtr GetDownloadsPath(out int stringLength);

	[DllImport("RDMacPlugin")]
	public static extern IntPtr GetDummyString(out int stringLength);

	[DllImport("RDMacPlugin")]
	public static extern void DisposeOutString();

	public static void SetupDownloadsPath()
	{
		int stringLength = 0;
		IntPtr source = GetDownloadsPath(out stringLength);
		byte[] array = new byte[stringLength];
		Marshal.Copy(source, array, 0, stringLength);
		DisposeOutString();
		downloadsPath = Encoding.UTF8.GetString(array);
	}
}
