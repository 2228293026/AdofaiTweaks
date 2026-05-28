using DG.Tweening;
using UnityEngine;

public class scrPAPPart1BG : ADOBase
{
	public SpriteRenderer bgRenderer;

	public SpriteRenderer bgDarken;

	public Transform bgTrans;

	public float pulseLength = 1f;

	public Transform hexesTrans;

	public scrPAPRopeHex[] hexes;

	private scrConductor cond;

	private Tween pulseTween;

	public Vector2 scrollSpeed = Vector2.zero;

	private Vector2 currentOffset = Vector2.zero;

	private void Start()
	{
		cond = scrConductor.instance;
		bgRenderer.material.SetFloat("_XOffset", 0f);
		bgRenderer.material.SetFloat("_YOffset", 0f);
		bgRenderer.material.SetFloat("_Tile", 1f);
		bgRenderer.material.SetFloat("_Polar", 0f);
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
			bgTrans.localScale = 1.15f * Vector3.one;
			pulseTween = bgTrans.DOScale(Vector3.one, duration).SetEase(Ease.OutCubic);
		}
	}

	public void DarkenCrosses(bool darken)
	{
		bgDarken.color = bgDarken.color.WithAlpha(darken ? 0.5f : 0.05f);
	}

	public void ToggleHexes(bool show)
	{
		hexesTrans.localScale = (show ? Vector3.one : Vector3.zero);
	}

	public void SetHexShape(int shape)
	{
		scrPAPRopeHex[] array = hexes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetAnim(shape);
		}
	}

	private void Update()
	{
		currentOffset += Time.deltaTime * scrollSpeed;
		bgRenderer.material.SetFloat("_XOffset", currentOffset.x);
		bgRenderer.material.SetFloat("_YOffset", currentOffset.y);
	}
}
