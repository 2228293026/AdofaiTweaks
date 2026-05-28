using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CoopPainter : ADOBase
{
	public Text[] colorTexts;

	public scrColorPlanet[] colorFloors;

	private scrColorPlanet[] takenColors = new scrColorPlanet[4];

	private bool init;

	private int playerCount => scrPlayerManager.playerCount;

	private void PaintPlayer(scrPlayer player, scrColorPlanet colorFloor, bool playSound = false)
	{
		scrColorPlanet scrColorPlanet2 = takenColors[player.playerID];
		if (scrColorPlanet2 != null)
		{
			scrColorPlanet2.floor.legacyFloorSpriteRenderer.color = scrColorPlanet2.planetColor.GetColor();
			scrColorPlanet2.floor.isLandable = true;
		}
		takenColors[player.playerID] = colorFloor;
		Color color = colorFloor.planetColor.GetColor();
		colorFloor.floor.legacyFloorSpriteRenderer.color = color.WithAlpha(0.5f);
		colorFloor.floor.isLandable = false;
		Text obj = colorTexts[player.playerID];
		obj.color = color;
		obj.transform.parent.SetParent(colorFloor.transform);
		obj.transform.parent.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutSine);
		obj.enabled = true;
		PlanetColor planetColor = new PlanetColor(colorFloor.planetColor);
		foreach (scrPlanet allPlanet in player.planetarySystem.allPlanets)
		{
			allPlanet.planetRenderer.SetColor(planetColor);
		}
		scrPlayerManager.playerColors[player.playerID] = planetColor;
		if (playSound)
		{
			scrSfx.instance.PlaySfx(SfxSound.PlanetPaint, MixerGroup.SfxParent);
		}
	}

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		if (init)
		{
			return;
		}
		init = true;
		if (playerCount <= 1)
		{
			return;
		}
		foreach (scrPlayer player in ADOBase.controller.playerManager)
		{
			scrColorPlanet[] array = colorFloors;
			foreach (scrColorPlanet scrColorPlanet2 in array)
			{
				if (scrColorPlanet2.planetColor == scrPlayerManager.playerColors[player.playerID].preset)
				{
					PaintPlayer(player, scrColorPlanet2);
					break;
				}
			}
			player.planetarySystem.chosenPlanet.planetRenderer.ringComp.UpdateRingSegments();
			scrPlayer obj = player;
			obj.onHit = (Action<scrFloor>)Delegate.Combine(obj.onHit, (Action<scrFloor>)delegate(scrFloor floor)
			{
				OnHit(player, floor);
			});
		}
	}

	private void OnHit(scrPlayer player, scrFloor floor)
	{
		if (floor.TryGetComponent<scrColorPlanet>(out var component))
		{
			PaintPlayer(player, component, playSound: true);
			floor.topGlow.color = Color.clear;
		}
	}
}
