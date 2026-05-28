using System.Collections.Generic;
using Rewired;
using Rewired.Data;
using UnityEngine;

public class RDInputType_Joystick : RDInputType
{
	private Player player;

	private Dictionary<string, string[]> mapCategoryMains;

	private string currentMapCategoryName;

	private string restartActionName;

	private string cancelActionName;

	private string leftActionName;

	private string rightActionName;

	private string upActionName;

	private string downActionName;

	private string leftAltActionName;

	private string rightAltActionName;

	private string upAltActionName;

	private string downAltActionName;

	private string confirmActionName;

	public RDInputType_Joystick(int playerIndex)
	{
		player = ReInput.players.GetPlayer(playerIndex);
		mapCategoryMains = new Dictionary<string, string[]>();
		UserData userData = ((InputManager_Base)RDInput.rewiredManager).userData;
		foreach (Mapping defaultJoystickMap in userData.GetPlayer(playerIndex + 1).defaultJoystickMaps)
		{
			string[] sortedActionNamesInCategory = userData.GetSortedActionNamesInCategory(defaultJoystickMap.categoryId);
			List<string> list = new List<string>();
			string[] array = sortedActionNamesInCategory;
			foreach (string text in array)
			{
				if (text.StartsWith("Main"))
				{
					list.Add(text);
				}
			}
			string mapCategoryNameById = userData.GetMapCategoryNameById(defaultJoystickMap.categoryId);
			mapCategoryMains.Add(mapCategoryNameById, list.ToArray());
			if (defaultJoystickMap.enabled)
			{
				SetMapCategoryName(userData.GetMapCategoryNameById(defaultJoystickMap.categoryId));
			}
		}
	}

	public void SetMapping(string newMapName)
	{
		MapHelper maps = player.controllers.maps;
		maps.SetAllMapsEnabled(false);
		maps.SetMapsEnabled(true, newMapName);
		SetMapCategoryName(newMapName);
	}

	private void SetMapCategoryName(string newMapName)
	{
		currentMapCategoryName = newMapName;
		restartActionName = "Restart" + currentMapCategoryName;
		cancelActionName = "Cancel" + currentMapCategoryName;
		leftActionName = "Left" + currentMapCategoryName;
		rightActionName = "Right" + currentMapCategoryName;
		upActionName = "Up" + currentMapCategoryName;
		downActionName = "Down" + currentMapCategoryName;
		leftAltActionName = "LeftAlt" + currentMapCategoryName;
		rightAltActionName = "RightAlt" + currentMapCategoryName;
		upAltActionName = "UpAlt" + currentMapCategoryName;
		downAltActionName = "DownAlt" + currentMapCategoryName;
		confirmActionName = "Confirm" + currentMapCategoryName;
	}

	private bool GetActionState(string actionName, ButtonState state = ButtonState.WentDown)
	{
		if (!isActive)
		{
			return false;
		}
		return state switch
		{
			ButtonState.WentDown => player.GetButtonDown(actionName), 
			ButtonState.WentUp => player.GetButtonUp(actionName), 
			ButtonState.IsUp => !player.GetButton(actionName), 
			ButtonState.IsDown => player.GetButton(actionName), 
			_ => false, 
		};
	}

	public override int Main(ButtonState state)
	{
		if (!isActive)
		{
			return 0;
		}
		MainStateCount stateCount = GetStateCount(state);
		if (player.controllers.Joysticks.Count == 0)
		{
			return 0;
		}
		if (stateCount.lastFrameUpdated == Time.frameCount)
		{
			return stateCount.keys.Count;
		}
		stateCount.lastFrameUpdated = Time.frameCount;
		stateCount.keys = new List<AnyKeyCode>();
		string[] array = mapCategoryMains[currentMapCategoryName];
		foreach (string text in array)
		{
			if (GetActionState(text, state))
			{
				stateCount.keys.Add(new AnyKeyCode(text));
			}
		}
		return stateCount.keys.Count;
	}

	public override bool Restart(ButtonState state)
	{
		return GetActionState(restartActionName, state);
	}

	public override bool Cancel(ButtonState state)
	{
		return GetActionState(cancelActionName, state);
	}

	public override bool Back(ButtonState state)
	{
		return GetActionState(cancelActionName, state);
	}

	public override bool Quit(ButtonState state)
	{
		return false;
	}

	public override bool Left(ButtonState state)
	{
		return GetActionState(leftActionName, state);
	}

	public override bool Right(ButtonState state)
	{
		return GetActionState(rightActionName, state);
	}

	public override bool Up(ButtonState state)
	{
		return GetActionState(upActionName, state);
	}

	public override bool Down(ButtonState state)
	{
		return GetActionState(downActionName, state);
	}

	public override bool LeftAlt(ButtonState state)
	{
		return GetActionState(leftAltActionName, state);
	}

	public override bool RightAlt(ButtonState state)
	{
		return GetActionState(rightAltActionName, state);
	}

	public override bool UpAlt(ButtonState state)
	{
		return GetActionState(upAltActionName, state);
	}

	public override bool DownAlt(ButtonState state)
	{
		return GetActionState(downAltActionName, state);
	}

	public override bool Action1(ButtonState state)
	{
		return false;
	}

	public override bool Action2(ButtonState state)
	{
		return false;
	}

	public override bool Confirm(ButtonState state)
	{
		return GetActionState(confirmActionName, state);
	}

	public override bool FaceUp(ButtonState state)
	{
		return false;
	}

	public override bool FaceLeft(ButtonState state)
	{
		return false;
	}
}
