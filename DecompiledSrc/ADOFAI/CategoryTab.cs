using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI;

public class CategoryTab : ADOBase, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("UI")]
	public Button button;

	public Image icon;

	public RectTransform background;

	[Header("Other")]
	public LevelEventCategory levelEventCategory;

	private const float initialY = 0f;

	private RectTransform rectTransform;

	public void Init(LevelEventCategory category)
	{
		levelEventCategory = category;
		string text = category.ToString();
		icon.sprite = GCS.eventCategoryIcons[category];
		button.name = text;
		SetSelected(selected: false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		ADOBase.editor.SetCategory(levelEventCategory);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ADOBase.editor.categoryText.text = RDString.Get($"editor.{levelEventCategory}");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ADOBase.editor.categoryText.text = "";
	}

	public void SetSelected(bool selected)
	{
		background.DOAnchorPosY(0f + (selected ? (-3f) : 0f), 0.05f).SetUpdate(isIndependentUpdate: true);
		float alpha = (selected ? 0.7f : 0.45f);
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white.WithAlpha(alpha);
		button.colors = colors;
		icon.DOKill();
		float alpha2 = (selected ? 1f : 0.6f);
		icon.DOColor(Color.white.WithAlpha(alpha2), 0.05f).SetUpdate(isIndependentUpdate: true);
		if (selected)
		{
			ADOBase.editor.categoryText.text = RDString.Get($"editor.{levelEventCategory}");
		}
	}
}
