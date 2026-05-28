using UnityEngine;

public class ffxSetOptionsText : ffxPlusBase
{
	public scrOptionsExperiencingText experiencingText;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			scrFloor currFloor = ADOBase.controller.currFloor;
			if (currFloor != null && currFloor.seqID <= ADOBase.lm.listFloors.Count - 2)
			{
				float f = (float)currFloor.angleLength * 57.29578f;
				experiencingText.SetAngleText(Mathf.Round(f));
			}
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
