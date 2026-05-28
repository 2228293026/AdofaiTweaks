using DG.Tweening;
using UnityEngine;

public class scrColorPlanet : ffxPlusBase
{
	public PlanetColorPreset planetColor;

	[Header("Obsolete")]
	public Color color = Color.white;

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

	private void Start()
	{
		if (planetColor == PlanetColorPreset.Rainbow)
		{
			GetComponent<SpriteRenderer>().DORainbow(5f).SetUpdate(isIndependentUpdate: true);
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (scrController.coopMode)
		{
			PlanetColor planetColor = new PlanetColor(this.planetColor);
			planet.planetRenderer.SetColor(planetColor);
			planet.other.planetRenderer.SetColor(planetColor);
			scrPlayerManager.playerColors[planet.player.playerID] = planetColor;
			return;
		}
		scrPlanet other = scrController.instance.chosenPlanet.other;
		other.planetRenderer.SetColorAndSave(new PlanetColor(this.planetColor), other.isRed);
		if ((bool)scrLogoText.instance)
		{
			scrLogoText.instance.UpdateColors();
		}
	}
}
