using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBubble : ADOBase
{
	[Header("Properties")]
	public int floor;

	public int appearStartOffset = 8;

	public int appearEndOffset = 2;

	public int disappearOffset = 4;

	public int spawnOffset = 3;

	[Header("Components")]
	public SpinningPlanets spinningPlanets;

	public Transform bubbleContainer;

	public SpriteRenderer bubbleSprite;

	public CircleCollider2D collider;

	[NonSerialized]
	[Header("Variables")]
	public scrPlayer player;

	private Tween tween;

	private bool popped;

	private int appearFloor => floor - appearStartOffset;

	private int lastAppearFloor => floor - appearEndOffset;

	private int disappearFloor => floor + disappearOffset;

	private int spawnFloor => floor + spawnOffset;

	private List<scrPlayer> deadPlayerQueue => ADOBase.controller.playerManager.deadPlayersQueue;

	private void Start()
	{
		if (!scrController.coopMode)
		{
			base.enabled = false;
			popped = true;
			bubbleContainer.gameObject.SetActive(value: false);
		}
		else
		{
			ResetBubble(!ADOBase.isEditingLevel);
		}
	}

	public void ResetBubble(bool playmode)
	{
		bubbleContainer.gameObject.SetActive(!playmode);
		spinningPlanets.gameObject.SetActive(playmode);
		base.enabled = playmode;
		popped = false;
		bubbleContainer.localScale = Vector3.one;
		bubbleSprite.color = Color.white;
	}

	public void Appear(scrPlayer player)
	{
		this.player = player;
		bubbleContainer.gameObject.SetActive(value: true);
		spinningPlanets.SetAppearance(player);
		spinningPlanets.clockwise = !ADOBase.lm.listFloors[spawnFloor].isCCW;
		tween?.Kill();
		tween = bubbleContainer.DOScale(1f, 0.5f).From(0f).SetEase(Ease.OutBack);
	}

	public void TryAppear()
	{
		if (deadPlayerQueue.Count != 0 && ADOBase.controller.playerManager.GetActivePlayers().Count != 0)
		{
			scrPlayer scrPlayer2 = deadPlayerQueue[0];
			deadPlayerQueue.RemoveAt(0);
			Appear(scrPlayer2);
		}
	}

	public void Disappear()
	{
		tween?.Kill();
		tween = bubbleContainer.DOScale(0f, 0.5f).SetEase(Ease.InBack);
		if (player != null)
		{
			deadPlayerQueue.Add(player);
		}
		((Behaviour)(object)collider).enabled = false;
		base.enabled = false;
	}

	public void Pop(scrPlayer byPlayer = null)
	{
		if (!popped)
		{
			spinningPlanets.gameObject.SetActive(value: false);
			tween?.Kill();
			tween = DOTween.Sequence().Insert(0f, bubbleContainer.DOScale(2f, 0.125f).SetEase(Ease.OutSine)).Insert(0f, bubbleSprite.DOFade(0f, 0.25f).SetEase(Ease.OutCubic).From(0.75f));
			popped = true;
			base.enabled = false;
			player.Revive(spawnFloor, byPlayer);
			scrSfx.instance.PlaySfx(SfxSound.PlanetBubblePop, MixerGroup.SfxParent, 0.5f);
			scrSfx.instance.PlaySfx(SfxSound.PlanetRevive, MixerGroup.SfxParent, 0.5f);
		}
	}

	private void Update()
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		Vector2 vector = new Vector2(Mathf.Sin(timeSinceLevelLoad * 1f + 1.5f), Mathf.Sin(timeSinceLevelLoad * 1f * 0.6f)) * 0.1f;
		bubbleContainer.localPosition = vector;
		if (ADOBase.controller.state == States.PlayerControl && ADOBase.lm.listFloors.Count >= disappearFloor)
		{
			double songposition_minusi = ADOBase.conductor.songposition_minusi;
			double entryTime = ADOBase.lm.listFloors[appearFloor].entryTime;
			double entryTime2 = ADOBase.lm.listFloors[disappearFloor].entryTime;
			if (songposition_minusi >= entryTime2)
			{
				Disappear();
			}
			else if (songposition_minusi >= entryTime && !bubbleContainer.gameObject.activeSelf)
			{
				TryAppear();
			}
		}
	}

	public void Validate(int floorCount)
	{
		floor = Mathf.Clamp(floor, 0, floorCount - 1);
		appearStartOffset = Mathf.Max(appearStartOffset, 0);
		appearEndOffset = Mathf.Max(appearEndOffset, 0);
		spawnOffset = Mathf.Max(spawnOffset, 0);
		disappearOffset = Mathf.Max(disappearOffset, 0);
		appearEndOffset = Mathf.Min(appearEndOffset, appearStartOffset);
	}
}
