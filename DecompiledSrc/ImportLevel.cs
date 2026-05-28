using System;
using UnityEngine;
using UnityEngine.UI;

public class ImportLevel : MonoBehaviour
{
	public RectTransform rectTransform;

	public Image progressImage;

	public Text progressText;

	public Text infoText;

	public RectTransform infoTextRectTransform;

	[NonSerialized]
	public string folderPath;

	[NonSerialized]
	public bool isUrl;

	public void OnInstallSuccess()
	{
		progressText.gameObject.SetActive(value: false);
		progressImage.gameObject.SetActive(value: true);
		progressImage.color = Color.green;
	}

	public void OnInstallError()
	{
		progressText.gameObject.SetActive(value: false);
		progressImage.gameObject.SetActive(value: false);
	}

	public void BeginInstallProgress()
	{
		progressText.gameObject.SetActive(value: true);
	}

	public void StopInstallProgress()
	{
		progressText.gameObject.SetActive(value: false);
		progressImage.gameObject.SetActive(value: false);
		progressText.text = "0%";
	}

	public void UpdateHeight()
	{
		float num = infoText.preferredHeight;
		if (num < 72f)
		{
			num = 72f;
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, num);
		infoTextRectTransform.sizeDelta = new Vector2(infoTextRectTransform.sizeDelta.x, num);
	}
}
