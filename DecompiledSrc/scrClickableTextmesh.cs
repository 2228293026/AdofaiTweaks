using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class scrClickableTextmesh : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		TMP_Text component = GetComponent<TMP_Text>();
		int num = TMP_TextUtilities.FindIntersectingLink(component, eventData.position, null);
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = component.textInfo.linkInfo[num];
			Application.OpenURL(tMP_LinkInfo.GetLinkID());
		}
	}
}
