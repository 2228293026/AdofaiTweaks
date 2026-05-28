using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TMP_TextHyperlinks : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private TMP_Text _text;

	private void Awake()
	{
		_text = GetComponent<TMP_Text>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		int num = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, eventData.pressEventCamera);
		if (num >= 0)
		{
			TMP_LinkInfo tMP_LinkInfo = _text.textInfo.linkInfo[num];
			Application.OpenURL(tMP_LinkInfo.GetLinkID());
		}
	}
}
