using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class scrRing : ADOBase
{
	public scrPlanet planet;

	public LineRenderer line;

	public int lineSegments = 24;

	public float lineGap = 0.4f;

	private float targetScale;

	private float scalePercent;

	private Tween scaleTween;

	private bool lastFrameWasChosen = true;

	private bool scaleHasResetOnDeath;

	private float rotationOffset;

	public Color color
	{
		get
		{
			return line.startColor;
		}
		set
		{
			LineRenderer lineRenderer = line;
			Color startColor = (line.endColor = value);
			lineRenderer.startColor = startColor;
		}
	}

	private void Awake()
	{
		line.material = Object.Instantiate(line.material);
	}

	private void Update()
	{
		if (!(planet == null))
		{
			float timeSinceLevelLoad = Time.timeSinceLevelLoad;
			float num = (planet.planetarySystem.isCW ? 1f : (-1f));
			base.transform.eulerAngles = Vector3.back * (num * timeSinceLevelLoad * 30f + rotationOffset);
			if (planet.dead && !scaleHasResetOnDeath)
			{
				scaleHasResetOnDeath = true;
				Switch(chosen: false);
			}
			if (planet.isChosen != lastFrameWasChosen)
			{
				lastFrameWasChosen = planet.isChosen;
				scaleHasResetOnDeath = false;
				Switch(planet.isChosen);
			}
			UpdateScale();
		}
	}

	private void UpdateScale()
	{
		if (planet.transform.localScale.x != 0f)
		{
			base.transform.localScale = Vector3.one * (scalePercent * targetScale / planet.transform.localScale.x);
		}
	}

	public void Switch(bool chosen, bool instant = false)
	{
		if (chosen)
		{
			float num = scrController.instance.tileSize;
			if (planet.currfloor != null && !planet.dead)
			{
				num *= (planet.currfloor.nextfloor ?? planet.currfloor).radiusScale;
			}
			targetScale = num;
			if (scrController.coopMode)
			{
				UpdateRingSegments();
			}
		}
		scaleTween?.Kill();
		scaleTween = DOTween.To(() => scalePercent, delegate(float x)
		{
			scalePercent = x;
		}, chosen ? 1f : 0f, instant ? 0f : 0.1f).SetEase(Ease.Linear).Done();
		UpdateScale();
	}

	public void UpdateRingSegments()
	{
		List<scrPlayer> activePlayers = ADOBase.controller.playerManager.GetActivePlayers();
		int b = activePlayers.Count;
		int num = activePlayers.IndexOf(planet.player);
		if (ADOBase.controller.independentPlayers)
		{
			b = 1;
			num = 1;
		}
		float num2 = (1f - lineGap) / (float)lineSegments;
		int num3 = lineSegments / Mathf.Max(1, b);
		int num4 = lineSegments - num3;
		float value = lineGap + (float)num4 * num2;
		line.material.SetFloat("_TileX", num3);
		line.material.SetFloat("_GapX", value);
		float num5 = 360f / (float)lineSegments;
		rotationOffset = (float)num * num5;
	}

	public Tween DOColor(Color toColor, float duration)
	{
		return DOTween.To(() => color, delegate(Color x)
		{
			color = x;
		}, toColor, duration);
	}

	public Tween DOFade(float toAlpha, float duration)
	{
		return DOTween.To(() => color.a, delegate(float x)
		{
			color = color.WithAlpha(x);
		}, toAlpha, duration);
	}
}
