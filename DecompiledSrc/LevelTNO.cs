using System;
using DG.Tweening;

public class LevelTNO : Level
{
	public void TransitionPalette(int palette, float durBeats)
	{
		Level.FindDecorationComponent<TNOBG_controller>("bgController")?.TransitionPalette(palette, durBeats);
	}

	public void ShrinkPlanetRadius(float durBeats, string easeStr)
	{
		float num = 60f / (base.conductor.bpm * base.conductor.song.pitch * base.controller.currFloor.speed);
		float duration = durBeats * num;
		Ease ease = (Ease)Enum.Parse(typeof(Ease), easeStr);
		foreach (scrPlayer item in base.controller.playerManager)
		{
			scrPlanet planet = item.planetarySystem.chosenPlanet;
			planet.planetRenderer.ring.gameObject.SetActive(value: false);
			DOTween.To(() => planet.cosmeticRadius, delegate(float r)
			{
				planet.cosmeticRadius = r;
			}, 0f, duration).SetEase(ease);
		}
	}
}
