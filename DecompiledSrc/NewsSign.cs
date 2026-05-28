using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GDMiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class NewsSign : ADOBase
{
	private class NewsData
	{
		public string text;

		public string url;

		public string defaultColor;

		public string clickableColor;

		public string hoveredColor;
	}

	private class NewsFetcher
	{
		private const string jsonURL = "https://7thbe.at/adofai-news.json";

		public NewsFetcher(MonoBehaviour owner, SystemLanguage language, Action<NewsData> callback)
		{
			owner.StartCoroutine(FetchNews(language, callback));
		}

		private IEnumerator FetchNews(SystemLanguage language, Action<NewsData> callback)
		{
			UnityWebRequest www = UnityWebRequest.Get("https://7thbe.at/adofai-news.json");
			yield return www.SendWebRequest();
			if ((int)www.result != 2)
			{
				string text = www.downloadHandler.text;
				if (!text.IsNullOrEmpty() && Json.Deserialize(text) is Dictionary<string, object> dictionary)
				{
					string key = language.ToString();
					if (dictionary.TryGetValueAs<string, object, Dictionary<string, object>>(key, out var valueAs))
					{
						NewsData newsData = new NewsData();
						valueAs.TryGetValueAs<string, object, string>("text", out newsData.text);
						valueAs.TryGetValueAs<string, object, string>("url", out newsData.url);
						valueAs.TryGetValueAs<string, object, string>("defaultColor", out newsData.defaultColor);
						valueAs.TryGetValueAs<string, object, string>("clickableColor", out newsData.clickableColor);
						valueAs.TryGetValueAs<string, object, string>("hoveredColor", out newsData.hoveredColor);
						callback(newsData);
						yield break;
					}
				}
			}
			callback(null);
		}
	}

	public TMP_Text text;

	public SpriteRenderer loadingIcon;

	public scrButtonURL button;

	public Color defaultColor;

	public Color clickableColor;

	public Color hoveredColor;

	private Tween clickableTween;

	private Tween hoverTween;

	private SpriteRenderer[] spriteRenderers;

	private NewsFetcher newsFetcher;

	private float alpha;

	private float loadingAlpha = 1f;

	private float textAlpha;

	private bool isFaded;

	private void Awake()
	{
		if (Persistence.GetOverallProgressStage() == 0)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		loadingIcon.gameObject.SetActive(value: true);
		text.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		FadeOutSign(fade: false);
	}

	private void FetchNews()
	{
		_ = Persistence.language;
		newsFetcher = new NewsFetcher(this, RDString.language, ShowNews);
	}

	private void ShowNews(NewsData news)
	{
		if (news == null)
		{
			return;
		}
		if (RDUtils.TryHexToColor(news.defaultColor, out var color))
		{
			defaultColor = color;
		}
		text.color = defaultColor;
		float duration = 0.25f;
		DOTween.Sequence().Append(DOTween.To(() => loadingAlpha, delegate(float x)
		{
			loadingAlpha = x;
		}, 0f, duration)).AppendCallback(delegate
		{
			text.gameObject.SetActive(value: true);
			loadingIcon.gameObject.SetActive(value: false);
		})
			.Append(DOTween.To(() => textAlpha, delegate(float x)
			{
				textAlpha = x;
			}, 1f, duration));
		text.text = news.text;
		text.SetLocalizedFont();
		button.link = news.url;
		if (!button.link.IsNullOrEmpty())
		{
			if (RDUtils.TryHexToColor(news.hoveredColor, out var color2))
			{
				hoveredColor = color2;
			}
			if (RDUtils.TryHexToColor(news.clickableColor, out var color3))
			{
				clickableColor = color3;
			}
			clickableTween = text.DOColor(clickableColor, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo)
				.Play();
			AddHoverListeners();
		}
	}

	private void Update()
	{
		bool num = ADOBase.levelSelectBase.menuPhase == 0;
		if (num && newsFetcher == null)
		{
			FetchNews();
		}
		bool flag = !num;
		if (flag != isFaded)
		{
			FadeOutSign(flag);
		}
		text.alpha = textAlpha * alpha;
		loadingIcon.SetAlpha(loadingAlpha * alpha);
		SpriteRenderer[] array = spriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetAlpha(alpha);
		}
		loadingIcon.transform.eulerAngles += Vector3.forward * Time.deltaTime * 360f;
	}

	private void FadeOutSign(bool fade)
	{
		isFaded = fade;
		float endValue = (fade ? 0f : 1f);
		float fadeDuration = scrShowOnlyInPhase.FadeDuration;
		DOTween.To(() => alpha, delegate(float x)
		{
			alpha = x;
		}, endValue, fadeDuration);
		button.gameObject.SetActive(!fade);
	}

	private void AddHoverListeners()
	{
		EventTrigger eventTrigger = button.button.gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerEnter
		};
		EventTrigger.Entry entry2 = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerExit
		};
		entry.callback.AddListener(delegate
		{
			HoverText(hover: true);
		});
		entry2.callback.AddListener(delegate
		{
			HoverText(hover: false);
		});
		eventTrigger.triggers.AddMany(entry, entry2);
	}

	private void HoverText(bool hover)
	{
		hoverTween?.Kill();
		hoverTween = text.DOColor(hover ? hoveredColor : defaultColor, 0.25f);
		if (hover)
		{
			clickableTween.Pause();
		}
		else
		{
			hoverTween.OnComplete(delegate
			{
				clickableTween.Restart();
			});
		}
		hoverTween.Play();
		text.fontStyle = (hover ? FontStyles.Underline : FontStyles.Normal);
	}
}
