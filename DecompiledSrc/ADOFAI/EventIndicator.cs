using System;
using UnityEngine;

namespace ADOFAI;

public class EventIndicator : ADOBase
{
	public SpriteRenderer icon;

	public SpriteRenderer circle;

	public LevelEvent evnt;

	public bool editable;

	[NonSerialized]
	public scrFloor floor;

	[NonSerialized]
	public int order;

	public void Init(LevelEvent baseEvent, scrFloor baseFloor, int order)
	{
		this.order = order;
		evnt = baseEvent;
		floor = baseFloor;
		editable = true;
		icon.sprite = GCS.levelEventIcons[baseEvent.eventType];
		float num = (float)baseEvent["angleOffset"];
		float num2 = (float)baseFloor.entryangle;
		float num3 = (float)baseFloor.exitangle;
		float num4 = Mathf.Round((float)scrMisc.GetAngleMoved(num2, num3, !baseFloor.isCCW) * 57.29578f);
		if (num4 <= Mathf.Pow(10f, -6f) && !baseFloor.midSpin)
		{
			num4 = 360f;
		}
		if (num != Mathf.Clamp(num, 0f, num4))
		{
			editable = false;
			circle.color = new Color(1f, 0.72f, 0.72f);
		}
		float num5 = (0f - (float)baseFloor.entryangle) * 57.29578f;
		base.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, num5 + num * (float)(floor.isCCW ? 1 : (-1)));
	}

	private void LateUpdate()
	{
		circle.transform.rotation = Quaternion.identity;
		if (evnt != null)
		{
			bool num = ADOBase.editor.levelEventsPanel.selectedEvent == evnt;
			Color color = (num ? ADOBase.editor.vfxIconColor : Color.black);
			icon.color = color.WithAlpha(evnt.active ? 1f : 0.33f);
			int num2 = (num ? (ADOBase.editor.events.Count + 1) : order);
			circle.sortingOrder = 1 + num2 * 2;
			icon.sortingOrder = 1 + num2 * 2 + 1;
		}
	}
}
