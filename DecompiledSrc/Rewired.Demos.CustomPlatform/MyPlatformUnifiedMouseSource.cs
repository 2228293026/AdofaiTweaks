using Rewired.Platforms.Custom;
using UnityEngine;

namespace Rewired.Demos.CustomPlatform;

public class MyPlatformUnifiedMouseSource : CustomPlatformUnifiedMouseSource
{
	public override Vector2 mousePosition => Input.mousePosition;

	protected override void Update()
	{
		((CustomPlatformUnifiedControllerSource)this).SetAxisValue(0, Input.GetAxis("MouseAxis1"));
		((CustomPlatformUnifiedControllerSource)this).SetAxisValue(1, Input.GetAxis("MouseAxis2"));
		((CustomPlatformUnifiedControllerSource)this).SetAxisValue(2, Input.GetAxis("MouseAxis3"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(0, Input.GetButton("MouseButton0"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(1, Input.GetButton("MouseButton1"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(2, Input.GetButton("MouseButton2"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(3, Input.GetButton("MouseButton3"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(4, Input.GetButton("MouseButton4"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(5, Input.GetButton("MouseButton5"));
		((CustomPlatformUnifiedControllerSource)this).SetButtonValue(6, Input.GetButton("MouseButton6"));
	}
}
