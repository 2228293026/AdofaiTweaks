public class scrSetEmojiMode : ffxPlusBase
{
	public bool emojiMode;

	public override void Awake()
	{
		base.Awake();
		floor.dontChangeMySprite = true;
		floor.topGlow.gameObject.SetActive(value: false);
		if ((bool)floor.bottomGlow)
		{
			floor.bottomGlow.gameObject.SetActive(value: false);
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (scrController.coopMode)
		{
			planet.planetRenderer.SetEmojiMode(emojiMode, pulseOnEnable: true);
			planet.other.planetRenderer.SetEmojiMode(emojiMode, pulseOnEnable: true);
			scrPlayerManager.playerEmoji[planet.player.playerID] = emojiMode;
		}
		else
		{
			planet.planetRenderer.SetEmojiMode(emojiMode, pulseOnEnable: true);
			Persistence.SetEmojiMode(emojiMode, planet.isRed);
		}
	}
}
