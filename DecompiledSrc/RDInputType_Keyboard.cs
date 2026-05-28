using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RDInputType_Keyboard : RDInputType
{
	private static readonly KeyCode[] pausedKeys = new KeyCode[5]
	{
		KeyCode.Q,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.UpArrow,
		KeyCode.DownArrow
	};

	private static readonly KeyCode[] SpecialKeys = new KeyCode[22]
	{
		KeyCode.Print,
		KeyCode.SysReq,
		KeyCode.LeftAlt,
		KeyCode.RightAlt,
		KeyCode.LeftWindows,
		KeyCode.RightWindows,
		KeyCode.LeftMeta,
		KeyCode.RightMeta,
		KeyCode.Tab,
		KeyCode.End,
		KeyCode.F1,
		KeyCode.F2,
		KeyCode.F3,
		KeyCode.F4,
		KeyCode.F5,
		KeyCode.F6,
		KeyCode.F7,
		KeyCode.F8,
		KeyCode.F9,
		KeyCode.F10,
		KeyCode.F11,
		KeyCode.F12
	};

	private static readonly KeyCode[] VirtualAvatarKeys = new KeyCode[10]
	{
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9
	};

	public static readonly KeyCode[] LevelSelectKeys = new KeyCode[15]
	{
		KeyCode.BackQuote,
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.AltGr,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.UpArrow,
		KeyCode.DownArrow
	};

	public static readonly KeyCode[] CLSKeys = new KeyCode[11]
	{
		KeyCode.UpArrow,
		KeyCode.DownArrow,
		KeyCode.LeftArrow,
		KeyCode.RightArrow,
		KeyCode.R,
		KeyCode.S,
		KeyCode.Delete,
		KeyCode.I,
		KeyCode.F,
		KeyCode.O,
		KeyCode.N
	};

	public static readonly KeyCode[] LeftKeys = new KeyCode[17]
	{
		KeyCode.Q,
		KeyCode.W,
		KeyCode.E,
		KeyCode.R,
		KeyCode.T,
		KeyCode.CapsLock,
		KeyCode.A,
		KeyCode.S,
		KeyCode.D,
		KeyCode.F,
		KeyCode.G,
		KeyCode.LeftShift,
		KeyCode.Z,
		KeyCode.X,
		KeyCode.C,
		KeyCode.V,
		KeyCode.B
	};

	public static readonly KeyCode[] RightKeys = new KeyCode[20]
	{
		KeyCode.Y,
		KeyCode.U,
		KeyCode.I,
		KeyCode.O,
		KeyCode.P,
		KeyCode.LeftBracket,
		KeyCode.RightBracket,
		KeyCode.H,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.Semicolon,
		KeyCode.Quote,
		KeyCode.N,
		KeyCode.M,
		KeyCode.Comma,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.RightShift,
		KeyCode.Return
	};

	public static readonly KeyCode[] ConfirmKeys = new KeyCode[3]
	{
		KeyCode.Space,
		KeyCode.Return,
		KeyCode.KeypadEnter
	};

	private readonly KeyCode[] mainKeys;

	public RDInputType_Keyboard(ControllerType type)
	{
		switch (type)
		{
		case ControllerType.KeyboardFull:
		{
			mainKeys = new KeyCode[321];
			for (int i = 0; i < 321; i++)
			{
				mainKeys[i] = (KeyCode)i;
			}
			break;
		}
		case ControllerType.KeyboardLeft:
			mainKeys = LeftKeys;
			break;
		case ControllerType.KeyboardRight:
			mainKeys = RightKeys;
			break;
		default:
			Debug.LogError("Wrong selected ControllerType for RDInputType_Keyboard!");
			break;
		}
	}

	internal static bool CheckKeyState(KeyCode key, ButtonState state = ButtonState.WentDown)
	{
		return state switch
		{
			ButtonState.WentDown => Input.GetKeyDown(key), 
			ButtonState.WentUp => Input.GetKeyUp(key), 
			ButtonState.IsDown => Input.GetKey(key), 
			ButtonState.IsUp => !Input.GetKey(key), 
			_ => false, 
		};
	}

	internal static bool CheckAnyKeyState(KeyCode[] keys, ButtonState state = ButtonState.WentDown)
	{
		return keys.Any((KeyCode key) => CheckKeyState(key, state));
	}

	public override int Main(ButtonState state)
	{
		if (!isActive)
		{
			return 0;
		}
		return MainIgnoreActive(state);
	}

	public int MainIgnoreActive(ButtonState state)
	{
		MainStateCount stateCount = GetStateCount(state);
		if (stateCount.lastFrameUpdated == Time.frameCount)
		{
			return stateCount.keys.Count;
		}
		stateCount.lastFrameUpdated = Time.frameCount;
		stateCount.keys = new List<AnyKeyCode>();
		List<KeyCode> keys = new List<KeyCode>();
		Array.ForEach(mainKeys, delegate(KeyCode key)
		{
			if (CheckKeyState(key, state))
			{
				keys.Add(key);
			}
		});
		if (state == ButtonState.WentDown)
		{
			foreach (KeyCode item in CountSpecialInput())
			{
				keys.Remove(item);
			}
		}
		HashSet<KeyCode> unityKeysCache = Persistence.keyLimiterKeys.unityKeysCache;
		foreach (KeyCode item2 in keys)
		{
			if (!RDInput.useKeyLimiter || unityKeysCache.Count <= 0 || unityKeysCache.Contains(item2))
			{
				stateCount.keys.Add(new AnyKeyCode(item2));
			}
		}
		return stateCount.keys.Count;
	}

	private MainStateCount GetStateCountIgnoreActive(ButtonState state)
	{
		return state switch
		{
			ButtonState.WentDown => pressCount, 
			ButtonState.IsDown => heldCount, 
			ButtonState.WentUp => releaseCount, 
			_ => isReleaseCount, 
		};
	}

	public List<KeyCode> CountSpecialInput()
	{
		List<KeyCode> keys = new List<KeyCode>();
		if (Cancel(ButtonState.WentDown))
		{
			keys.Add(KeyCode.Escape);
		}
		Array.ForEach(SpecialKeys, delegate(KeyCode key)
		{
			if (CheckKeyState(key))
			{
				keys.Add(key);
			}
		});
		if (VirtualAvatarCanvas.instance != null)
		{
			Array.ForEach(VirtualAvatarKeys, delegate(KeyCode key)
			{
				if (CheckKeyState(key))
				{
					keys.Add(key);
				}
			});
		}
		if (!base.controller.pauseMenu.settingsMenu.editingKeys)
		{
			if (!base.isPlaying)
			{
				Array.ForEach(pausedKeys, delegate(KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
			if (base.controller.currentState != States.PlayerControl && base.controller.currentState != States.Fail2 && !ADOBase.isLevelEditor && ADOBase.uiController.difficultyUIMode != DifficultyUIMode.DontShow)
			{
				if (Left(ButtonState.WentDown))
				{
					keys.Add(KeyCode.LeftArrow);
				}
				else if (Right(ButtonState.WentDown))
				{
					keys.Add(KeyCode.RightArrow);
				}
			}
			bool flag = ADOBase.sceneName.StartsWith("scnTaroMenu");
			if ((bool)scnLevelSelect.instance || flag)
			{
				Array.ForEach(LevelSelectKeys, delegate(KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
			if (ADOBase.isCLS && !scnCLS.instance.showingInitialMenu)
			{
				Array.ForEach(CLSKeys, delegate(KeyCode key)
				{
					if (CheckKeyState(key))
					{
						keys.Add(key);
					}
				});
			}
		}
		return keys;
	}

	public override bool Restart(ButtonState state)
	{
		return CheckKeyState(KeyCode.R, state);
	}

	public override bool Cancel(ButtonState state)
	{
		return CheckKeyState(KeyCode.Escape, state);
	}

	public override bool Back(ButtonState state)
	{
		if (!CheckKeyState(KeyCode.Escape, state))
		{
			if (Application.isEditor)
			{
				return CheckKeyState(KeyCode.Z, state);
			}
			return false;
		}
		return true;
	}

	public override bool Quit(ButtonState state)
	{
		return CheckKeyState(KeyCode.Q, state);
	}

	public override bool Left(ButtonState state)
	{
		return CheckKeyState(KeyCode.LeftArrow, state);
	}

	public override bool Right(ButtonState state)
	{
		return CheckKeyState(KeyCode.RightArrow, state);
	}

	public override bool Up(ButtonState state)
	{
		return CheckKeyState(KeyCode.UpArrow, state);
	}

	public override bool Down(ButtonState state)
	{
		return CheckKeyState(KeyCode.DownArrow, state);
	}

	public override bool LeftAlt(ButtonState state)
	{
		return CheckKeyState(KeyCode.LeftShift, state);
	}

	public override bool RightAlt(ButtonState state)
	{
		return CheckKeyState(KeyCode.RightShift, state);
	}

	public override bool UpAlt(ButtonState state)
	{
		return false;
	}

	public override bool DownAlt(ButtonState state)
	{
		return false;
	}

	public override bool Action1(ButtonState state)
	{
		return CheckAnyKeyState(LeftKeys, state);
	}

	public override bool Action2(ButtonState state)
	{
		return CheckAnyKeyState(RightKeys, state);
	}

	public override bool Confirm(ButtonState state)
	{
		return CheckAnyKeyState(ConfirmKeys, state);
	}

	public override bool FaceUp(ButtonState state)
	{
		return CheckKeyState(KeyCode.S, state);
	}

	public override bool FaceLeft(ButtonState state)
	{
		return CheckKeyState(KeyCode.A, state);
	}
}
