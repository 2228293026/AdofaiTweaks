using DG.Tweening;

public class ffxMenuPlanetSpeedChange : ffxPlusBase
{
	public override void Awake()
	{
		base.Awake();
		floor.topGlow.gameObject.SetActive(value: false);
		floor.floorIcon = FloorIcon.Rabbit;
		floor.UpdateIconSprite();
	}

	public void Start()
	{
		floor.topGlow.gameObject.SetActive(value: false);
		floor.bottomGlow.gameObject.SetActive(value: false);
	}

	public override void StartEffect(scrPlanet planet)
	{
		float num;
		if (ADOBase.controller.playerOne.planetarySystem.speed == 1.0)
		{
			num = 2f;
			floor.floorIcon = FloorIcon.Snail;
			ADOBase.conductor.song2.DOFade(ADOBase.IsHalloweenWeek() ? 0.7f : 0.7f, 0.2f);
		}
		else
		{
			num = 1f;
			floor.floorIcon = FloorIcon.Rabbit;
			ADOBase.conductor.song2.DOFade(0f, 0.2f);
		}
		floor.UpdateIconSprite();
		foreach (scrPlayer item in ADOBase.controller.playerManager)
		{
			item.planetarySystem.speed = num;
		}
	}
}
