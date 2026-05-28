using UnityEngine;
using UnityEngine.EventSystems;

public class SecretTree : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler
{
	public static GameObject DraggedInstance;

	private Vector3 _startPosition;

	private Vector3 _offsetToMouse;

	public void OnBeginDrag(PointerEventData eventData)
	{
		DraggedInstance = base.gameObject;
		_startPosition = base.transform.position;
		_offsetToMouse = _startPosition - new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (Input.touchCount <= 1)
		{
			base.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f) + _offsetToMouse;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		DraggedInstance = null;
		_offsetToMouse = Vector3.zero;
	}
}
