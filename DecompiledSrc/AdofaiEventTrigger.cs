using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdofaiEventTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IMoveHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
	public bool isDragging;

	[NonSerialized]
	public AdofaiPointerEventData onPointerClick;

	[NonSerialized]
	public AdofaiPointerEventData onPointerEnter;

	[NonSerialized]
	public AdofaiPointerEventData onPointerDown;

	[NonSerialized]
	public AdofaiPointerEventData onPointerUp;

	[NonSerialized]
	public AdofaiPointerEventData onPointerExit;

	[NonSerialized]
	public AdofaiPointerEventData onBeginDrag;

	[NonSerialized]
	public AdofaiPointerEventData onEndDrag;

	[NonSerialized]
	public AdofaiPointerEventData onDrag;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (onBeginDrag != null)
		{
			onBeginDrag(eventData);
		}
		isDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (onEndDrag != null)
		{
			onEndDrag(eventData);
		}
		isDragging = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (onPointerDown != null)
		{
			onPointerDown(eventData);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (onPointerUp != null)
		{
			onPointerUp(eventData);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (onPointerEnter != null)
		{
			onPointerEnter(eventData);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (onPointerExit != null)
		{
			onPointerExit(eventData);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (onPointerClick != null)
		{
			onPointerClick(eventData);
		}
	}

	public void OnMove(AxisEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (onDrag != null && isDragging)
		{
			onDrag(eventData);
		}
	}
}
