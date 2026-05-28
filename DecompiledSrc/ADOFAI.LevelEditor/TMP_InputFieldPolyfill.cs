using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ADOFAI.LevelEditor;

[RequireComponent(typeof(TMP_InputField))]
public class TMP_InputFieldPolyfill : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	private IScrollHandler _parentRect;

	private void Start()
	{
		Transform parent = base.transform;
		while ((bool)parent)
		{
			SmoothScrollRect component = parent.GetComponent<SmoothScrollRect>();
			if ((bool)component)
			{
				_parentRect = component;
				break;
			}
			parent = parent.parent;
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		_parentRect?.OnScroll(eventData);
	}
}
