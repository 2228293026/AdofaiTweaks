using UnityEngine;

public class scrPortalParticles : ADOBase
{
	[Header("Properties")]
	public Color color;

	public bool speedTrial;

	public bool disabled;

	[Header("Components")]
	public ParticleSystem ps;

	public SpriteRenderer glowSprite;

	public SpriteRenderer innerSprite;

	public SpriteRenderer cap;

	public SpriteRenderer disabledCap;

	public SpriteRenderer icon;

	private FloorRenderer floor;

	private scrFloor floorComp;

	private void Start()
	{
		floor = base.transform.parent.GetComponent<FloorRenderer>();
		floorComp = base.transform.parent.GetComponent<scrFloor>();
		if (floorComp != null && floorComp.iconsprite != null)
		{
			floorComp.iconsprite.sprite = null;
		}
		Refresh();
		ps.Simulate(1f);
		ps.Play();
	}

	private void LateUpdate()
	{
		Refresh();
	}

	private void Refresh()
	{
		if (floor == null)
		{
			return;
		}
		ps.gameObject.SetActive(!disabled);
		icon.enabled = !disabled;
		glowSprite.enabled = !disabled;
		cap.enabled = !disabled;
		disabledCap.enabled = disabled;
		if (!disabled)
		{
			float num = ((!floor.material.HasProperty(scrFloor.ShaderProperty_Alpha)) ? floor.color.a : floor.material.GetFloat(scrFloor.ShaderProperty_Alpha));
			float x = floor.transform.localScale.x;
			if ((num < 1f || x < 1f) && ps.isEmitting)
			{
				ps.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			else if (num == 1f && x == 1f && ps.isStopped)
			{
				ps.Play();
			}
			Color color = (speedTrial ? ADOBase.gc.speedPortalColor : this.color);
			icon.enabled = speedTrial;
			ParticleSystem.MainModule main = ps.main;
			main.startColor = color;
			float alpha = num * color.a;
			glowSprite.color = color.WithAlpha(alpha);
			cap.color = cap.color.WithAlpha(num);
			icon.color = color.WithAlpha(num * 0.45f);
		}
	}
}
