using DG.Tweening;
using UnityEngine;

public class scrPAPPart3BG : ADOBase
{
	public Transform border;

	public SpriteRenderer patternRenderer;

	public Sprite pattern1;

	public Sprite pattern2;

	public Sprite pattern3;

	public Sprite pattern4;

	public float pulseLength = 0.5f;

	private scrConductor cond;

	private Tween pulseTween;

	public Vector2 scrollSpeed = Vector2.zero;

	private Vector2 currentOffset = Vector2.zero;

	private void Start()
	{
		cond = scrConductor.instance;
	}

	public void Pulse()
	{
		if (ADOBase.conductor.hasSongStarted && ADOBase.conductor.beatNumber >= 0 && ADOBase.controller.currentState == States.PlayerControl)
		{
			if (pulseTween != null)
			{
				pulseTween.Kill();
			}
			float duration = pulseLength * (float)cond.crotchetAtStart / cond.song.pitch;
			border.localScale = 1.2f * Vector3.one;
			pulseTween = border.DOScale(Vector3.one, duration).SetEase(Ease.OutCubic);
		}
	}

	public void SetScrollSpeed(Vector2 dir)
	{
		scrollSpeed = dir;
	}

	public void ResetOffset()
	{
		currentOffset = Vector2.zero;
	}

	public void SetPattern(int pattern)
	{
		switch (pattern)
		{
		case 1:
			patternRenderer.sprite = pattern1;
			break;
		case 2:
			patternRenderer.sprite = pattern2;
			break;
		case 3:
			patternRenderer.sprite = pattern3;
			break;
		case 4:
			patternRenderer.sprite = pattern4;
			break;
		}
	}

	public void SetPolar(bool polar)
	{
		int value = 0;
		if (polar)
		{
			value = 1;
		}
		patternRenderer.material.SetInt("_Polar", value);
	}

	public void SetTileSize(int tileSize)
	{
		patternRenderer.material.SetFloat("_Tile", tileSize);
	}

	private void Update()
	{
		currentOffset += Time.deltaTime * scrollSpeed;
		patternRenderer.material.SetFloat("_XOffset", currentOffset.x);
		patternRenderer.material.SetFloat("_YOffset", currentOffset.y);
	}
}
