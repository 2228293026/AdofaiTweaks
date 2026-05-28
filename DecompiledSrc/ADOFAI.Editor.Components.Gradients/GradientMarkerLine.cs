using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ADOFAI.Editor.Components.Gradients;

public class GradientMarkerLine : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public UnityEvent<PointerEventData> onClick;

	public void OnPointerDown(PointerEventData eventData)
	{
		onClick.Invoke(eventData);
	}
}
