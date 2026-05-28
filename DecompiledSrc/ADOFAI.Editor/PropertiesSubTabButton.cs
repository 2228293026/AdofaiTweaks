using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.Editor;

public class PropertiesSubTabButton : MonoBehaviour
{
	public Button button;

	public Image icon;

	public RectTransform backgroundTransform;

	[Header("Tweaks")]
	public float selectedHeight;

	public float unselectedHeight;

	public Button.ButtonClickedEvent onClick => button.onClick;

	public string groupName { get; set; }

	public void SetIcon(Sprite sprite)
	{
		icon.sprite = sprite;
	}

	public void SetSelected(bool selected)
	{
		button.interactable = !selected;
		icon.color = icon.color.WithAlpha(selected ? 1f : 0.6f);
		backgroundTransform.DOKill();
		backgroundTransform.DOSizeDelta(backgroundTransform.sizeDelta.WithY(selected ? selectedHeight : unselectedHeight), 0.2f).SetEase(Ease.OutQuad);
	}
}
