using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrBadgeContainer : ADOBase
{
	public int updateInterval = 3;

	public float animationDuration = 0.5f;

	public Dictionary<string, GameObject> badges = new Dictionary<string, GameObject>();

	public List<string> activeBadges = new List<string>();

	private int badgeIndex = -1;

	private float lastUpdate = -1000f;

	public void ResetBadges()
	{
		activeBadges.Clear();
		lastUpdate = -1000f;
		badgeIndex = -1;
	}

	private void Update()
	{
		if (Time.time - lastUpdate < (float)updateInterval)
		{
			return;
		}
		lastUpdate = Time.time;
		int count = activeBadges.Count;
		if (count == 0)
		{
			return;
		}
		int num = badgeIndex;
		badgeIndex = (badgeIndex + 1) % count;
		if (num == badgeIndex)
		{
			return;
		}
		string text = activeBadges[badgeIndex];
		GameObject obj = badges[text];
		Image component = obj.GetComponent<Image>();
		obj.SetActive(value: true);
		if (num != -1)
		{
			GameObject oldTag = badges[activeBadges[num]];
			oldTag.GetComponent<Image>().DOFade(0f, animationDuration).OnComplete(delegate
			{
				oldTag.SetActive(value: false);
			});
			component.color = component.color.WithAlpha(0f);
			component.DOFade(1f, animationDuration);
		}
		else
		{
			component.color = component.color.WithAlpha(1f);
		}
		scnCLS.instance.badgeText.text = RDString.GetWithCheck("cls." + text, out var exists);
		ADOBase.cls.badgeText.SetLocalizedFont();
		ADOBase.cls.badgeText.enabled = exists;
		ADOBase.cls.badgeButton.clickable = exists;
		ADOBase.cls.badgeButton.UpdateHoverState();
		ADOBase.cls.badgeButton.url = null;
		if (text == "CommunityFeatured")
		{
			ADOBase.cls.badgeButton.url = "https://7thbe.at/featured-mailbox";
		}
	}
}
