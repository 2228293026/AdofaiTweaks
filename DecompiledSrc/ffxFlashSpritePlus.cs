using UnityEngine;

public class ffxFlashSpritePlus : ffxPlusBase
{
	public GameObject objFade;

	public float time;

	public override void StartEffect(scrPlanet planet)
	{
		float num = time / cond.song.pitch;
		ffxFlashSprite.StartEffectStatic(objFade, num);
	}

	public override void ScrubToTime(float t)
	{
	}
}
