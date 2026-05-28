using System;
using System.Collections.Generic;
using System.Linq;
using ADOFAI;

public class ffxSetInputEventPlus : ffxPlusBase
{
	public InputEventTarget target;

	public InputEventState state;

	public new string tag;

	public bool ignoreInput;

	public HashSet<ffxPlusBase> inputEvents = new HashSet<ffxPlusBase>();

	private bool initialized;

	public override void PrepVfx()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		foreach (ffxPlusBase plusEffect in floor.plusEffects)
		{
			LevelEvent levelEvent = plusEffect.sourceLevelEvent;
			if (levelEvent != null)
			{
				string text = levelEvent.GetString("eventTag") ?? "";
				if (!text.IsNullOrEmpty() && text.Split(" ", StringSplitOptions.None).Contains(tag))
				{
					inputEvents.Add(plusEffect);
					plusEffect.triggered = true;
					plusEffect.runManually = true;
				}
			}
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		ADOBase.controller.inputEventFfx[(int)state][(int)target] = this;
	}

	public override void Decode(LevelEvent evnt)
	{
		target = (InputEventTarget)evnt["target"];
		state = (InputEventState)evnt["state"];
		tag = evnt.GetString("targetEventTag");
		ignoreInput = evnt.GetBool("ignoreInput");
	}
}
