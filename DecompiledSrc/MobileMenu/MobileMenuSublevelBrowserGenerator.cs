using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuSublevelBrowserGenerator : MonoBehaviour
{
	public static Dictionary<string, Sprite> spriteVariantsDict = new Dictionary<string, Sprite>();

	public GameObject sublevelButtonPrefab;

	public GameObject submenuPrefab;

	public RectTransform container;

	public Dictionary<string, GameObject> submenu = new Dictionary<string, GameObject>();

	private static Sprite GetOrLoadSpriteVariant(string spriteName, string variantKey)
	{
		if (variantKey != "")
		{
			spriteName = spriteName + "_" + variantKey;
		}
		if (spriteVariantsDict.TryGetValue(spriteName, out var value))
		{
			return value;
		}
		return Resources.Load<Sprite>(spriteName);
	}

	public void GenerateSubmenu(string world)
	{
		Dictionary<string, GCNS.WorldData> worldData = GCNS.worldData;
		int levelCount = worldData[world].levelCount;
		if (levelCount < 1)
		{
			return;
		}
		int index = worldData[world].index;
		int levelTutorialProgress = Persistence.GetLevelTutorialProgress(index);
		bool num = Persistence.GetWorldAttempts(index) > 0 || levelTutorialProgress >= levelCount - 1;
		bool flag = world.IsTechWorld();
		bool flag2 = world.IsCrownWorld();
		bool flag3 = world.IsTaro();
		int num2 = (num ? levelCount : (levelTutorialProgress + 1));
		GameObject gameObject = Object.Instantiate(submenuPrefab, base.transform);
		submenu.Add(world, gameObject);
		float num3 = 135f;
		float num4 = 1.25f;
		float num5 = 1f;
		if (num2 >= 11)
		{
			num3 *= 0.88f;
			num5 = 0.8f;
		}
		else if (num2 >= 10)
		{
			num5 *= 0.88f;
		}
		for (int i = (ADOBase.isExpo ? (-1) : 0); i < num2; i++)
		{
			if (i == -1 && world != "1")
			{
				continue;
			}
			bool isBoss = i == levelCount - 1;
			bool flag4 = (i == num2 - 1 && i > 0) || levelCount == 1;
			GameObject gameObject2 = Object.Instantiate(sublevelButtonPrefab, gameObject.transform);
			Image component = gameObject2.transform.GetChild(0).GetComponent<Image>();
			Image component2 = gameObject2.transform.GetChild(1).GetComponent<Image>();
			RectTransform rectTransform = gameObject2.transform as RectTransform;
			_ = component2.transform;
			rectTransform.localScale = Vector2.one * (flag4 ? num4 : 1f) * num5;
			rectTransform.sizeDelta = rectTransform.sizeDelta.WithX(num3);
			if (flag4 && !isBoss)
			{
				rectTransform.localEulerAngles = Vector3.forward * 5f;
				rectTransform.DOLocalRotate(Vector3.forward * -5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
				rectTransform.DOScale(num5, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
			}
			Sprite orLoadSpriteVariant = GetOrLoadSpriteVariant("submenu_blank_glow", flag ? "tech" : (flag3 ? "taro" : ""));
			Sprite sprite;
			if (isBoss)
			{
				sprite = Resources.Load<Sprite>("boss" + world);
			}
			else
			{
				string variantKey = (flag ? "tech" : (flag2 ? "crown" : (flag3 ? "taro" : "")));
				sprite = GetOrLoadSpriteVariant("submenu_blank", variantKey);
			}
			component2.sprite = sprite;
			component.sprite = orLoadSpriteVariant;
			int lvl = i;
			gameObject2.GetComponent<Button>().onClick.AddListener(delegate
			{
				string text = (isBoss ? "X" : (lvl + 1).ToString());
				MobileMenuController.EnterLevel(world + "-" + text, speedTrial: false);
			});
			gameObject2.gameObject.SetActive(value: true);
			if (flag3)
			{
				component2.rectTransform.sizeDelta = Vector2.one * 130f;
			}
			if (!isBoss)
			{
				TMP_Text componentInChildren = gameObject2.transform.GetComponentInChildren<TMP_Text>(includeInactive: true);
				componentInChildren.gameObject.SetActive(value: true);
				componentInChildren.text = (i + 1).ToString();
				if (flag2)
				{
					componentInChildren.color = "333333".HexToColor();
				}
				if (flag3)
				{
					componentInChildren.color = "D6D6D6".HexToColor();
				}
			}
			if (isBoss && flag2)
			{
				component2.DORainbow(10f, 0.5f, 1f, Ease.Linear);
			}
		}
	}
}
