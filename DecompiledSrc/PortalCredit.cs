using UnityEngine;
using UnityEngine.UI;

public class PortalCredit : ADOBase
{
	public Text titleText;

	public Text peopleText;

	public scrButtonURL soundcloudButton;

	public scrButtonURL youtubeButton;

	public scrButtonURL twitterButton;

	public Color soundcloudColor;

	public Color youtubeColor;

	public Color twitterColor;

	public void Load(PortalCreditData data)
	{
		if (data == null)
		{
			return;
		}
		base.gameObject.SetActive(value: true);
		data.Localize();
		titleText.SetLocalizedFont();
		peopleText.SetLocalizedFont();
		titleText.text = data.credit;
		peopleText.text = data.people;
		GetComponent<VerticalLayoutGroup>().enabled = true;
		if (!ADOBase.isSwitch)
		{
			scrButtonURL scrButtonURL2;
			switch (data.linkType)
			{
			case PortalCreditData.LinkType.Soundcloud:
				scrButtonURL2 = soundcloudButton;
				peopleText.color = soundcloudColor;
				break;
			case PortalCreditData.LinkType.Youtube:
				scrButtonURL2 = youtubeButton;
				peopleText.color = youtubeColor;
				break;
			case PortalCreditData.LinkType.Twitter:
				scrButtonURL2 = twitterButton;
				peopleText.color = twitterColor;
				break;
			default:
				scrButtonURL2 = null;
				break;
			}
			if (scrButtonURL2 != null)
			{
				scrButtonURL2.gameObject.SetActive(value: true);
				scrButtonURL2.link = data.link;
			}
		}
	}
}
