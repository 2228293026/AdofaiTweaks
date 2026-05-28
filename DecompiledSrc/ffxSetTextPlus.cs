using System;
using System.Collections.Generic;
using ADOFAI;

public class ffxSetTextPlus : ffxPlusBase
{
	[NonSerialized]
	public scrDecorationManager decManager;

	public string targetString;

	public List<string> targetTags = new List<string>();

	public override void Awake()
	{
		base.Awake();
		if (decManager == null)
		{
			decManager = ADOBase.controller.decorationManager;
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low && (ADOBase.isOfficialLevel || Persistence.forceVisualSettings))
		{
			return;
		}
		AdjustDurationForHardbake();
		foreach (string targetTag in targetTags)
		{
			if (!decManager.taggedDecorations.ContainsKey(targetTag))
			{
				continue;
			}
			foreach (scrTextDecoration taggedDecoration in decManager.GetTaggedDecorations<scrTextDecoration>(targetTags))
			{
				taggedDecoration.SetText(targetString);
			}
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		decManager = scnGame.suitableDecManager;
		targetString = evnt.GetStringLocalized("decText");
		string[] array = ((string)evnt["tag"]).Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			array = new string[1] { "NO TAG" };
		}
		targetTags.AddRange(array);
	}
}
