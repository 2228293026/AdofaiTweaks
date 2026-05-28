using UnityEngine;

public class scrBackgroundBars : MonoBehaviour
{
	public enum BarStatus
	{
		idle,
		hit,
		miss,
		special
	}

	public static scrBackgroundBars instance;

	public scrController controller;

	public scrBgbar[] arrBgbars;

	public Camera camy;

	public int indexMultiplier;

	public float heightMultiplier;

	public float failHeightMultiplier;

	public float firstBarMultiplier;

	public bool warning;

	private void Awake()
	{
		instance = this;
		controller = scrController.instance;
	}

	private void Start()
	{
		Flash(BarStatus.hit);
		Timer.Add(delegate
		{
			Flash(BarStatus.hit);
		}, 0.3f);
		for (int num = 0; num < arrBgbars.Length; num++)
		{
			arrBgbars[num].index = num * indexMultiplier;
			arrBgbars[num].heightMultiplier = heightMultiplier * ((num == 0) ? firstBarMultiplier : 1f);
			arrBgbars[num].failHeightMultiplier = failHeightMultiplier;
		}
	}

	public void Flash(BarStatus status)
	{
		Color start = default(Color);
		ColourScheme currentColourScheme = scrVfx.instance.currentColourScheme;
		switch (status)
		{
		case BarStatus.hit:
			start = currentColourScheme.colourBarsHit;
			break;
		case BarStatus.miss:
			start = currentColourScheme.colourBarsMiss;
			break;
		}
		Color colourBarsIdle = currentColourScheme.colourBarsIdle;
		for (int i = 0; i < arrBgbars.Length; i++)
		{
			arrBgbars[i].Flash(start, colourBarsIdle);
		}
	}

	public void Damage()
	{
		Flash(BarStatus.miss);
	}

	public float GetFailMultiplier()
	{
		return (controller?.playerOne?.failBar.overloadCounter).GetValueOrDefault() * 10f;
	}
}
