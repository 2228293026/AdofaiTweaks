using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ADOFAI;

public class InspectorTab : ADOBase, IPointerClickHandler, IEventSystemHandler
{
	[Header("UI")]
	public Button button;

	public Image icon;

	public CycleButtons cycleButtons;

	[Header("Other")]
	public LevelEventType levelEventType;

	[Header("Runtime")]
	public InspectorPanel panel;

	private bool isEventTab;

	public int eventIndex;

	public void Init(LevelEventType type, InspectorPanel panel)
	{
		this.panel = panel;
		levelEventType = type;
		string text = type.ToString();
		icon.sprite = GCS.levelEventIcons[type];
		button.name = text;
		isEventTab = !type.IsSetting();
		if (isEventTab)
		{
			FlipTab();
		}
		cycleButtons.panel = panel;
		SetSelected(selected: false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (panel.selectedEventType == levelEventType || !panel.showInspector)
			{
				panel.ShowInspector(!panel.showInspector, forceAction: true);
			}
			else
			{
				ADOBase.editor.DecideInspectorTabsAtSelected();
				panel.selectedEventType = levelEventType;
			}
			panel.ShowPanel(levelEventType, eventIndex);
		}
		if (panel.floorPanel && eventData.button == PointerEventData.InputButton.Right)
		{
			ADOBase.editor.RemoveEventAtSelected(levelEventType);
		}
	}

	private void FlipTab()
	{
		button.transform.ScaleX(-1f);
		icon.transform.ScaleX(-1f);
		cycleButtons.transform.ScaleX(-1f);
	}

	public void SetSelected(bool selected)
	{
		if (!selected)
		{
			eventIndex = 0;
		}
		bool flag = false;
		if (ADOBase.editor.SelectionIsSingle() && !Array.Exists(EditorConstants.soloTypes, (LevelEventType t) => t == levelEventType) && !Array.Exists(EditorConstants.settingsTypes, (LevelEventType t) => t == levelEventType))
		{
			flag = selected;
			if (ADOBase.editor.GetSelectedFloorEvents(levelEventType).Count <= 1)
			{
				flag = false;
			}
		}
		cycleButtons.gameObject.SetActive(flag);
		RectTransform component = GetComponent<RectTransform>();
		float num = (flag ? 120f : 0f);
		Vector2 endValue = new Vector2(num, component.sizeDelta.y);
		component.DOKill();
		component.DOSizeDelta(endValue, 0.05f).SetUpdate(isIndependentUpdate: true);
		float num2 = (selected ? 0f : 3f);
		num2 *= (isEventTab ? (-1f) : 1f);
		num2 -= num / 2f;
		component.DOAnchorPosX(num2, 0.05f).SetUpdate(isIndependentUpdate: true);
		float alpha = (selected ? 0.7f : 0.45f);
		ColorBlock colors = button.colors;
		colors.normalColor = Color.white.WithAlpha(alpha);
		button.colors = colors;
		icon.DOKill();
		float alpha2 = (selected ? 1f : 0.6f);
		icon.DOColor(Color.white.WithAlpha(alpha2), 0.05f).SetUpdate(isIndependentUpdate: true);
	}
}
