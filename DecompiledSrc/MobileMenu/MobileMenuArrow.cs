using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuArrow : MonoBehaviour
{
	public Button button;

	public Image buttonImage;

	public Image glow;

	public TMP_Text caption;

	public MoveDirection direction;

	public Image museDashIcon;

	public Image cr2024Icon;

	public RectTransform rt => base.transform as RectTransform;

	private void Awake()
	{
		caption.SetLocalizedFont();
	}

	public void Show(bool showButton, MobileMenuGroup toGroup, MobileMenuGroup fromGroup = null)
	{
		if (toGroup != null && toGroup.inaccessible)
		{
			showButton = false;
		}
		string text = ((!(toGroup?.captionKey != fromGroup.captionKey)) ? null : toGroup?.captionKey);
		if (fromGroup.id == "mainGroup")
		{
			text = null;
		}
		Show(showButton, text);
		bool active = true;
		if (direction == MoveDirection.Up)
		{
			rt.sizeDelta = Vector2.one * 200f;
			rt.anchoredPosition = Vector2.left * 420f;
			string text2 = (ADOBase.isMobile ? "homeScreen" : "moreScreen");
			if (text == "adofai" || (text == "neoCosmos" && fromGroup.captionKey == text2))
			{
				active = false;
				rt.sizeDelta = new Vector2(650f, 200f);
				rt.anchoredPosition = Vector2.zero;
			}
		}
		buttonImage.gameObject.SetActive(active);
	}

	public void Show(bool showButton, string captionKey = null)
	{
		base.gameObject.SetActive(showButton);
		if (!showButton)
		{
			return;
		}
		bool flag = true;
		if (museDashIcon != null)
		{
			museDashIcon.gameObject.SetActive(value: false);
		}
		if (cr2024Icon != null)
		{
			cr2024Icon.gameObject.SetActive(value: false);
		}
		if (captionKey == "museDash")
		{
			flag = false;
			if (museDashIcon != null)
			{
				museDashIcon.gameObject.SetActive(value: true);
			}
		}
		if (captionKey == "cosmicRadio2024")
		{
			flag = false;
			if (cr2024Icon != null)
			{
				cr2024Icon.gameObject.SetActive(value: true);
			}
		}
		if (flag && captionKey != null)
		{
			bool exists;
			string withCheck = RDString.GetWithCheck("levelSelect." + captionKey, out exists);
			if (!exists)
			{
				withCheck = RDString.GetWithCheck(captionKey, out var _);
			}
			caption.text = withCheck;
		}
		caption.gameObject.SetActive(flag && captionKey != null);
	}
}
