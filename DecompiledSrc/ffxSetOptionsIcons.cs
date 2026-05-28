using System.Collections.Generic;

public class ffxSetOptionsIcons : ffxPlusBase
{
	private scrOptionsWindows opWinRef;

	public OptionsShape shape;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public void Start()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			opWinRef = scrOptionsWindows.opWinRef;
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low)
		{
			return;
		}
		if (scrController.coopMode && planet == null)
		{
			foreach (scrPlayer activePlayer in ADOBase.controller.playerManager.GetActivePlayers())
			{
				StartEffect(activePlayer.planetarySystem.chosenPlanet);
			}
			return;
		}
		if (opWinRef == null)
		{
			opWinRef = scrOptionsWindows.opWinRef;
		}
		if (scrController.coopMode)
		{
			List<scrPlayer> activePlayers = ADOBase.controller.playerManager.GetActivePlayers();
			if (activePlayers.Count > 0)
			{
				opWinRef.SetIcons(shape, staticTime: false, activePlayers.IndexOf(planet.player), activePlayers.Count, planet.planetRenderer.planetColor.ToRealColor());
			}
		}
		else
		{
			opWinRef.SetIcons(shape);
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
