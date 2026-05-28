using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ADOFAI.Editor.Components.Gradients;

public class GradientMarker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public Image colorImage;

	public Image background;

	public Color color;

	public UnityEvent onClick;

	public UnityEvent onDelete;

	public void SetSelected(bool selected)
	{
		background.color = (selected ? InspectorPanel.selectionColor : Color.white);
	}

	public void UpdateColor()
	{
		colorImage.color = color;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			onClick.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			onDelete.Invoke();
		}
	}
}
