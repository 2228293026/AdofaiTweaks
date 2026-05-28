using System;
using UnityEngine;
using UnityEngine.UI;

public class scrLanternShake : ADOBase
{
	public float startOffset;

	private float swingoffsetpct;

	private float c;

	private void Start()
	{
		swingoffsetpct = UnityEngine.Random.value;
		if (!(ADOBase.controller.levelName == "1-X"))
		{
			return;
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		Image component2 = GetComponent<Image>();
		if (component != null)
		{
			string text = component.sprite.name;
			if (ADOBase.IsHalloweenWeek())
			{
				component.sprite = (text.Contains("lanterns_2") ? RDC.data.sprite_halloween_lantern_big : RDC.data.sprite_halloween_lantern_small);
			}
			else if (ADOBase.IsCNY())
			{
				component.sprite = RDC.data.sprite_cny_lantern;
			}
		}
		else if (component2 != null)
		{
			string text2 = component2.sprite.name;
			if (ADOBase.IsHalloweenWeek())
			{
				component2.sprite = (text2.Contains("lanterns_2") ? RDC.data.sprite_halloween_lantern_big : RDC.data.sprite_halloween_lantern_small);
			}
			else if (ADOBase.IsCNY())
			{
				component2.sprite = RDC.data.sprite_cny_lantern;
			}
		}
	}

	private void Update()
	{
		c += (float)(scrConductor.instance.deltaSongPos * (double)ADOBase.controller.d_speed);
		scrMisc.Rotate2DCW(base.transform, 10f * Mathf.Sin(c * 5f + swingoffsetpct * (float)Math.PI) + startOffset);
	}
}
