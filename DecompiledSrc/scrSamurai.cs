using DG.Tweening;
using UnityEngine;

public class scrSamurai : ADOBase
{
	private Sequence bobTimer;

	public Sprite baseSprite;

	public Sprite bobSprite;

	public SpriteRenderer spriteRenderer;

	public SpriteMask spriteMask;

	public bool updateOnPause;

	private Camera mainCamera;

	private void Awake()
	{
		mainCamera = ADOBase.controller.camy.camobj;
	}

	public void Setup()
	{
		ADOBase.conductor.onBeats.Add(this);
	}

	private void Update()
	{
		scrMisc.RotateWorld2DCW(base.transform, 0f - mainCamera.transform.eulerAngles.z);
		if (Time.frameCount != ADOBase.conductor.onBeatFrame && !updateOnPause)
		{
			return;
		}
		updateOnPause = false;
		if (bobTimer != null)
		{
			bobTimer.Kill();
		}
		spriteRenderer.sprite = bobSprite;
		spriteMask.sprite = bobSprite;
		float num = 60f / (ADOBase.conductor.bpm * ADOBase.conductor.song.pitch);
		bobTimer = DOTween.Sequence().AppendInterval(num / 4f).OnComplete(delegate
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.sprite = baseSprite;
				spriteMask.sprite = baseSprite;
			}
		})
			.SetUpdate(isIndependentUpdate: true)
			.OnKill(delegate
			{
				if (!(spriteRenderer == null))
				{
					spriteRenderer.sprite = baseSprite;
					spriteMask.sprite = baseSprite;
				}
			});
	}
}
