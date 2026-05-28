using System.Collections.Generic;
using ADOFAI;
using DG.Tweening;
using UnityEngine;

public class ffxScreenTilePlus : ffxPlusBase
{
	public float tileX;

	public float tileY;

	private static ScreenTile screenTile;

	private static Tween tweenX;

	private static Tween tweenY;

	protected override IEnumerable<Tween> eventTweens => new Tween[2] { tweenX, tweenY };

	public override void Awake()
	{
		base.Awake();
		if (screenTile == null)
		{
			screenTile = cam.GetComponent<ScreenTile>();
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (duration == 0f)
		{
			SetScreenTile(tileX, tileY);
			return;
		}
		tweenX?.Kill(complete: true);
		tweenY?.Kill(complete: true);
		bool enable = tileX != 1f || tileY != 1f;
		if (enable)
		{
			screenTile.enabled = true;
		}
		tweenX = DOTween.To(() => screenTile.tileX, delegate(float x)
		{
			screenTile.tileX = x;
		}, tileX, duration).SetEase(ease).Done();
		tweenY = DOTween.To(() => screenTile.tileY, delegate(float y)
		{
			screenTile.tileY = y;
		}, tileY, duration).SetEase(ease).OnComplete(delegate
		{
			screenTile.enabled = enable;
		})
			.Done();
	}

	private void SetScreenTile(float tileX, float tileY)
	{
		screenTile.enabled = tileX != 1f || tileY != 1f;
		screenTile.tileX = tileX;
		screenTile.tileY = tileY;
	}

	public override void Decode(LevelEvent evnt)
	{
		duration = evnt.GetFloat("duration") * crotchet;
		ease = (Ease)evnt["ease"];
		Vector2 vector = (Vector2)evnt["tile"];
		tileX = vector.x;
		tileY = vector.y;
	}
}
