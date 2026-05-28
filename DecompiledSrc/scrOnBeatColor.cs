using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrOnBeatColor : ADOBase
{
	public Image image;

	public Color colorOn;

	public Color colorOff;

	public float transitionDuration;

	private void Start()
	{
		ADOBase.conductor.onBeats.Add(this);
	}

	public override void OnBeat()
	{
		image.color = colorOn;
		image.DOKill();
		image.DOColor(colorOff, transitionDuration).SetUpdate(isIndependentUpdate: true);
	}
}
