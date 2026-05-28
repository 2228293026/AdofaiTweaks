using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOFAI.FloorFX;

public class ffxEmitParticlePlus : ffxPlusBase
{
	public List<string> targetTags;

	public scrDecorationManager decManager;

	public int count;

	public override void StartEffect(scrPlanet planet)
	{
		foreach (scrParticleDecoration taggedDecoration in decManager.GetTaggedDecorations<scrParticleDecoration>(targetTags))
		{
			taggedDecoration.particleSystem.Emit(count);
		}
	}

	public override void Decode(LevelEvent evnt)
	{
		decManager = ADOBase.controller.decorationManager;
		count = evnt.GetInt("count");
		string[] array = ((string)evnt["tag"]).Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			array = new string[1] { "NO TAG" };
		}
		targetTags = array.ToList();
	}
}
