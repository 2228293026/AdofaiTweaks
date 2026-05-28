using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : ADOBase
{
	private class Achievement
	{
		public string id;

		public string name;

		public string description;

		public Sprite icon;
	}

	[Header("Tweakables")]
	public float bubbleOpenDuration;

	public float bubbleCloseDuration;

	public float contentOpenDuration;

	public float iconsOpenDuration;

	public float iconsCloseDuration;

	public float iconsDelay;

	public float infoOpenDuration;

	public float infoCloseDuration;

	public float infoFinalX;

	[Header("Components")]
	public Transform bubble;

	public TMP_Text title;

	public TMP_Text retroactiveTitle;

	public TMP_Text retroactiveDescription;

	public TMP_Text retroactiveTap;

	public new TMP_Text name;

	public TMP_Text description;

	public SpriteRenderer icon;

	public Transform content;

	public Transform info;

	public Transform retroactive;

	public Transform retroactiveContent;

	public Transform icons;

	private Vector3 bubbleFinalScale;

	private Vector3 contentFinalScale;

	private Vector3 retroactiveFinalScale;

	private Vector3 iconsFinalScale;

	private Achievement[] achievements;

	private int achievementIndex = -1;

	private Action onComplete;

	private bool canTap;

	private Button skipButton;

	private void Awake()
	{
		bubbleFinalScale = bubble.localScale;
		bubble.localScale = Vector3.zero;
		contentFinalScale = content.localScale;
		content.localScale = Vector3.zero;
		retroactiveFinalScale = retroactive.localScale;
		retroactive.localScale = Vector3.zero;
		iconsFinalScale = icons.localScale;
		icons.localScale = Vector3.zero;
		scrUIController instance = scrUIController.instance;
		if ((bool)instance)
		{
			skipButton = instance.achievementSkip;
			skipButton.onClick.AddListener(Finish);
		}
	}

	private void CreateAchievements(string[] ids)
	{
		achievements = new Achievement[ids.Length];
		int num = 0;
		foreach (string text in ids)
		{
			Achievement achievement = new Achievement
			{
				id = text,
				name = RDString.Get("achievement." + text + ".title"),
				description = RDString.Get("achievement." + text + ".description"),
				icon = Resources.Load<Sprite>("Achievements/" + text)
			};
			achievements[num] = achievement;
			num++;
		}
	}

	private void EnableSkipButton()
	{
		if (!skipButton.gameObject.activeSelf && achievements.Length >= 3)
		{
			skipButton.gameObject.SetActive(value: true);
		}
	}

	private void SetInfo()
	{
		Achievement achievement = achievements[achievementIndex];
		name.text = achievement.name;
		description.text = achievement.description;
		icon.sprite = achievement.icon;
	}

	public void ShowAchievements(string[] ids, Action onComplete)
	{
		this.onComplete = onComplete;
		content.gameObject.SetActive(value: true);
		bubble.DOScale(bubbleFinalScale, bubbleOpenDuration).SetEase(Ease.InCubic).SetUpdate(isIndependentUpdate: true);
		title.text = RDString.Get("achievement.unlocked");
		if (achievements == null || achievements.Length == 0)
		{
			CreateAchievements(ids);
			scrSfx.instance.PlaySfx(SfxSound.AchievementBubbleOpen, MixerGroup.InterfaceParent);
			scrSfx.instance.PlaySfx(SfxSound.AchievementJingle, MixerGroup.InterfaceParent);
		}
		Advance();
	}

	public void ShowRetroactiveAchievements(string[] ids, Action onComplete)
	{
		this.onComplete = onComplete;
		CreateAchievements(ids);
		List<string> copyIds = new List<string>();
		int childCount = icons.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = icons.GetChild(i);
			child.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>()
				.sprite = GetRandomIcon();
			scrGfxFloat component = child.GetComponent<scrGfxFloat>();
			component.amplitude = UnityEngine.Random.Range(0.1f, 0.2f);
			component.period = UnityEngine.Random.Range(0.1f, 0.5f);
		}
		bubble.DOScale(bubbleFinalScale, bubbleOpenDuration).OnStart(delegate
		{
			scrSfx.instance.PlaySfx(SfxSound.AchievementBubbleOpen, MixerGroup.InterfaceParent);
			scrSfx.instance.PlaySfx(SfxSound.AchievementJingle, MixerGroup.InterfaceParent);
		}).SetEase(Ease.InCubic)
			.SetUpdate(isIndependentUpdate: true);
		retroactive.gameObject.SetActive(value: true);
		retroactiveTitle.text = RDString.Get("achievement.newUpdate.title");
		retroactiveDescription.text = RDString.Get("achievement.newUpdate.description", new Dictionary<string, object> { { "x", ids.Length } });
		retroactiveTap.text = RDString.Get("calibration.finish.mobile");
		retroactive.DOScale(retroactiveFinalScale, contentOpenDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				canTap = true;
			});
		icons.DOScale(iconsFinalScale, iconsOpenDuration).SetDelay(iconsDelay).SetEase(Ease.OutCubic)
			.SetUpdate(isIndependentUpdate: true);
		Sprite GetRandomIcon()
		{
			if (copyIds.Count == 0)
			{
				copyIds.AddRange(ids);
			}
			string text = copyIds[UnityEngine.Random.Range(0, copyIds.Count)];
			copyIds.Remove(text);
			return Resources.Load<Sprite>("Achievements/" + text);
		}
	}

	public void Advance()
	{
		achievementIndex++;
		if (achievementIndex >= achievements.Length)
		{
			Finish();
			return;
		}
		if (achievementIndex == 0)
		{
			SetInfo();
			content.DOScale(contentFinalScale, contentOpenDuration).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
				.OnComplete(delegate
				{
					canTap = true;
				});
			return;
		}
		EnableSkipButton();
		info.DOLocalMoveX(0f - infoFinalX, infoCloseDuration).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				info.localPosition = new Vector2(infoFinalX, 0f);
				SetInfo();
				info.DOLocalMoveX(0f, infoOpenDuration).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true)
					.OnComplete(delegate
					{
						canTap = true;
						scrSfx.instance.PlaySfx(SfxSound.AchievementJingleSmall, MixerGroup.InterfaceParent);
					});
			});
	}

	private void Update()
	{
		if (skipButton.gameObject.activeInHierarchy && RDInput.rightPress)
		{
			Finish();
		}
		else
		{
			if ((!Input.GetMouseButtonDown(0) && (RDInput.rightPress || !RDInput.confirmPress)) || !canTap)
			{
				return;
			}
			canTap = false;
			if (achievementIndex == -1)
			{
				icons.DOScale(Vector3.one * 2f, iconsCloseDuration).OnStart(delegate
				{
					scrSfx.instance.PlaySfx(SfxSound.AchievementIconBurst, MixerGroup.InterfaceParent);
				}).SetEase(Ease.InBack)
					.SetUpdate(isIndependentUpdate: true)
					.OnComplete(delegate
					{
						icons.gameObject.SetActive(value: false);
						retroactiveContent.DOLocalMoveX(0f - infoFinalX, infoCloseDuration).SetDelay(0.3f).SetEase(Ease.OutQuad)
							.SetUpdate(isIndependentUpdate: true)
							.OnComplete(delegate
							{
								retroactive.gameObject.SetActive(value: false);
								ShowAchievements(null, onComplete);
								content.localScale = contentFinalScale;
								content.localPosition = new Vector2(infoFinalX, 0f);
								content.DOLocalMoveX(0f, infoOpenDuration).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true)
									.OnComplete(delegate
									{
										canTap = true;
										scrSfx.instance.PlaySfx(SfxSound.AchievementJingleSmall, MixerGroup.InterfaceParent);
									});
							});
					});
			}
			else if (achievementIndex >= 0 && achievementIndex < achievements.Length)
			{
				Advance();
			}
		}
	}

	public void Finish()
	{
		if (skipButton.gameObject.activeSelf)
		{
			skipButton.gameObject.SetActive(value: false);
			icons.DOKill();
			content.DOKill();
			info.DOKill();
			scrSfx.instance.PlaySfx(SfxSound.MobileButton, MixerGroup.InterfaceParent);
		}
		bubble.DOKill();
		content.DOKill();
		bubble.DOScale(Vector3.zero, bubbleCloseDuration).OnStart(delegate
		{
			scrSfx.instance.PlaySfx(SfxSound.AchievementBubbleClose, MixerGroup.InterfaceParent);
		}).SetEase(Ease.InOutCubic)
			.SetUpdate(isIndependentUpdate: true)
			.OnComplete(delegate
			{
				DOVirtual.DelayedCall(0.3f, delegate
				{
					onComplete?.Invoke();
					UnityEngine.Object.Destroy(base.gameObject);
				});
			});
	}
}
