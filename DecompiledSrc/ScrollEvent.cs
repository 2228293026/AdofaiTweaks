using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollEvent : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public static bool inside;

	public virtual void OnPointerEnter(PointerEventData data)
	{
		inside = true;
	}

	public virtual void OnPointerExit(PointerEventData data)
	{
		inside = false;
	}
}
