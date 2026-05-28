using Rewired.Platforms.Custom;
using UnityEngine;

namespace Rewired.Demos.CustomPlatform;

public sealed class CustomPlatformManager : MonoBehaviour, ICustomPlatformInitializer
{
	public CustomPlatformHardwareJoystickMapProvider mapProvider;

	public CustomPlatformInitOptions GetCustomPlatformInitOptions()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		CustomPlatformInitOptions val = new CustomPlatformInitOptions
		{
			platformId = 0,
			platformIdentifierString = "MyPlatform",
			hardwareJoystickMapCustomPlatformMapProvider = (IHardwareJoystickMapCustomPlatformMapProvider)(object)mapProvider
		};
		CustomPlatformConfigVars configVars = new CustomPlatformConfigVars
		{
			ignoreInputWhenAppNotInFocus = true,
			useNativeKeyboard = true,
			useNativeMouse = true
		};
		val.inputSource = (CustomInputSource)(object)new MyPlatformInputSource(configVars);
		return val;
	}
}
