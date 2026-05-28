using System;
using System.Collections.Generic;
using Rewired.Platforms.Custom;
using UnityEngine;

namespace Rewired.Demos.CustomPlatform;

public class MyPlatformUnifiedKeyboardSource : CustomPlatformUnifiedKeyboardSource
{
	private static readonly KeyboardKeyCode[] keyCodes = (KeyboardKeyCode[])Enum.GetValues(typeof(KeyboardKeyCode));

	protected override void OnInitialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		((CustomPlatformUnifiedControllerSource)this).OnInitialize();
		KeyPropertyMap val = new KeyPropertyMap();
		val.Set(new Key
		{
			keyCode = (KeyboardKeyCode)97,
			label = "[A]"
		});
		val.Set((ICollection<Key>)(object)new Key[3]
		{
			new Key
			{
				keyCode = (KeyboardKeyCode)98,
				label = "[B]"
			},
			new Key
			{
				keyCode = (KeyboardKeyCode)99,
				label = "[C]"
			},
			new Key
			{
				keyCode = (KeyboardKeyCode)100,
				label = "[D]"
			}
		});
		((CustomPlatformUnifiedKeyboardSource)this).keyPropertyMap = val;
	}

	protected override void Update()
	{
		for (int i = 0; i < keyCodes.Length; i++)
		{
			((CustomPlatformUnifiedKeyboardSource)this).SetKeyValue(keyCodes[i], Input.GetKey((KeyCode)keyCodes[i]));
		}
	}
}
