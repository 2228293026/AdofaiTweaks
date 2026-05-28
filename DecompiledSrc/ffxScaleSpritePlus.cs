using UnityEngine;

public class ffxScaleSpritePlus : ffxPlusBase
{
	public GameObject objScale;

	public Vector3 scale;

	public float time;

	public override void StartEffect(scrPlanet planet)
	{
		float num = time / cond.song.pitch;
		ffxSpriteScale.StartEffectStatic(objScale, scale, num, ease);
	}

	public override void ScrubToTime(float t)
	{
	}
}
