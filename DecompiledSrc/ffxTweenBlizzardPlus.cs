using DG.Tweening;

public class ffxTweenBlizzardPlus : ffxPlusBase
{
	public float durationBeats = 1f;

	public float startOpacity;

	public float endOpacity;

	private CameraFilterPackLegacy_Blizzard blizzFilter;

	public override void Awake()
	{
		base.Awake();
		SetStartTime(cond.bpm);
		blizzFilter = cam.GetComponent<CameraFilterPackLegacy_Blizzard>();
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		float num = durationBeats * (float)cond.crotchetAtStart / (floor.speed * cond.song.pitch);
		blizzFilter._Fade = startOpacity;
		DOTween.To(() => blizzFilter._Fade, delegate(float b)
		{
			blizzFilter._Fade = b;
		}, endOpacity, num);
	}

	public override void ScrubToTime(float t)
	{
	}
}
