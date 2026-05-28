using DG.Tweening;
using UnityEngine;

public class ffxBgColor : ffxPlusBase
{
	public Color color;

	public float fadeTime;

	public override void StartEffect(scrPlanet planet)
	{
		cam.Bgcamstatic.DOColor(color, fadeTime).Done();
	}
}
