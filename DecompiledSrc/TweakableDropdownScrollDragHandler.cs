using UnityEngine;
using UnityEngine.EventSystems;

public class TweakableDropdownScrollDragHandler : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler
{
	public TweakableDropdown dropdown;

	public bool isDragging { get; set; }

	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
		dropdown.releasedDragging = true;
	}
}
