using UnityEngine;
using UnityEngine.UI;

public class scrImageByLanguage : ADOBase
{
	public string path;

	private void Awake()
	{
		Sprite sprite = Resources.Load<Sprite>($"{path}{Persistence.language}");
		if (!(sprite == null))
		{
			SpriteRenderer component2;
			if (TryGetComponent<Image>(out var component))
			{
				RectTransform rectTransform = component.rectTransform;
				Rect rect = component.sprite.rect;
				float num = rectTransform.sizeDelta.x / rect.width;
				float num2 = rectTransform.sizeDelta.y / rect.height;
				component.sprite = sprite;
				component.rectTransform.sizeDelta = new Vector2(sprite.rect.width * num, sprite.rect.height * num2);
			}
			else if (TryGetComponent<SpriteRenderer>(out component2))
			{
				component2.sprite = sprite;
			}
		}
	}
}
