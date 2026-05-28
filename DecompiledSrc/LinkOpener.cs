using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : ADOBase, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		TMP_Text component = GetComponent<TMP_Text>();
		int num = TMP_TextUtilities.FindIntersectingLink(component, eventData.position, null);
		if (num != -1)
		{
			TMP_LinkInfo tMP_LinkInfo = component.textInfo.linkInfo[num];
			ADOBase.platformHelper.OpenURL(tMP_LinkInfo.GetLinkID());
		}
	}
}
