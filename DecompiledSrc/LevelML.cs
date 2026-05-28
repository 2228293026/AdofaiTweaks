using DG.Tweening;
using UnityEngine;

public class LevelML : Level
{
	public void Shatter(string decoTag)
	{
		Level.FindDecorationComponent<Shatter>(decoTag)?.StartShatter();
	}

	public void StartMob()
	{
		Level.FindDecorationComponent<MonsterChase>("monsterChase")?.StartMob();
	}

	public void StopMob()
	{
		Level.FindDecorationComponent<MonsterChase>("monsterChase")?.StopMob();
	}

	public void HideLaNuitScenery()
	{
		Level.FindDecorationComponent<LaNuitScenery>("laNuitScenery")?.FadeOut();
	}

	public void ShowSkeletons()
	{
		Level.FindDecorationComponent<Skeletons>("skeletons")?.Show();
	}

	public void HideSkeletons()
	{
		Level.FindDecorationComponent<Skeletons>("skeletons")?.Hide();
	}

	public void FadeInDarkness(float alpha)
	{
		Level.FindDecorationComponent<SpriteRenderer>("darkness")?.DOFade(alpha, 1f);
	}

	public void DontLightUpFloors()
	{
		scrVfx.instance.tileFlashStyle = TileFlashStyle.MoveToTopLayer;
	}
}
