using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomLevelTile : ffxPlusBase
{
	[NonSerialized]
	public string levelKey;

	public CustomLevelTile customLevelTile;

	public Text title;

	public Text artist;

	public Text removedText;

	public RawImage image;

	public RectTransform InfoContainer;

	public scrBlur blur;

	public bool selected;

	private Coroutine loadTextureCoroutine;

	public bool didStartLoadingIcon { get; private set; }

	public bool didProcessIcon { get; private set; }

	public override void StartEffect(scrPlanet planet)
	{
		if (!scnCLS.instance.initializing)
		{
			scnCLS.instance.SelectLevel(customLevelTile, snap: false);
			Highlight();
		}
	}

	public void Highlight(bool highlight = true, bool instant = false)
	{
		float num = (instant ? 0f : 0.25f);
		float num2 = (highlight ? 1f : 0.7f);
		float alpha = (highlight ? 1f : Mathf.Clamp(0.8f - Mathf.Abs(scrController.instance.chosenPlanet.transform.position.y - base.transform.position.y) * 0.15f, 0.1f, 1f));
		Ease ease = (highlight ? Ease.OutBack : Ease.OutSine);
		InfoContainer.DOScale(new Vector3(num2, num2, 1f), num).SetEase(ease);
		title.DOColor(title.color.WithAlpha(alpha), num).SetEase(ease);
		artist.DOColor(artist.color.WithAlpha(alpha), num).SetEase(ease);
		removedText.DOColor(removedText.color.WithAlpha(alpha), num).SetEase(ease);
	}

	public void SetDeleted()
	{
		title.gameObject.SetActive(value: false);
		artist.gameObject.SetActive(value: false);
		removedText.gameObject.SetActive(value: true);
		image.gameObject.SetActive(value: false);
		MarkUnavailable();
	}

	public void MarkUnavailable()
	{
		GetComponent<SpriteRenderer>().color = Color.gray;
	}

	public void LoadTileIcon(string iconPath, Color iconColor)
	{
		if (!didStartLoadingIcon)
		{
			didStartLoadingIcon = true;
			loadTextureCoroutine = StartCoroutine(LoadTexture(iconPath, iconColor));
		}
	}

	private IEnumerator LoadTexture(string iconPath, Color iconColor)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
		UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(iconPath.ToFileUri());
		Texture2D texture;
		try
		{
			yield return imageRequest.SendWebRequest();
			if ((int)imageRequest.result == 2 || (int)imageRequest.result == 3)
			{
				yield break;
			}
			texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
		}
		finally
		{
			((IDisposable)imageRequest)?.Dispose();
		}
		ProcessIconTexture(texture, iconColor);
	}

	public void ProcessIconTexture(Texture2D icon, Color iconColor)
	{
		if (!didProcessIcon)
		{
			didProcessIcon = true;
			image.enabled = true;
			image.color = Color.clear;
			TextureManager.ShrinkImage(icon, 128);
			image.texture = icon;
			blur.baseTint = iconColor;
			blur.blurTint = Color.black;
			blur.UpdateTexture();
			image.DOColor(Color.white, 0.5f);
		}
	}

	private void OnDestroy()
	{
		DOTween.Kill(title);
		DOTween.Kill(artist);
		DOTween.Kill(image);
		DOTween.Kill(removedText);
		DOTween.Kill(InfoContainer);
		if (loadTextureCoroutine != null)
		{
			StopCoroutine(loadTextureCoroutine);
		}
	}

	private void Start()
	{
		removedText.text = RDString.Get("cls.worldRemoved");
	}
}
