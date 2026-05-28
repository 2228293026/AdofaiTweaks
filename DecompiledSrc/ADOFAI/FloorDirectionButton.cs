using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI;

public class FloorDirectionButton : ADOBase
{
	[Header("Tweakables")]
	public FloorDirectionButtonType btnType;

	[Header("Components")]
	public Button button;

	public Image deleteIcon;

	public TMP_Text text;

	public TMP_Text textShifted;

	public Color colorDeleteIdle;

	public Color colorDeleteHover;

	public Color colorDeletePressed;

	public Color colorNormalIdle;

	public Color colorNormalHover;

	public Color colorNormalPressed;

	public Sprite spriteButtonLong;

	public Sprite spriteButtonShort;

	public RectTransform hitbox;

	public bool delete;

	public void Start()
	{
		Init();
	}

	public void Init()
	{
		Vector3 eulerAngles = new Vector3(this.text.transform.eulerAngles.x, this.text.transform.eulerAngles.y, 0f * button.transform.eulerAngles.z);
		this.text.transform.eulerAngles = eulerAngles;
		textShifted.transform.eulerAngles = eulerAngles;
		deleteIcon.transform.eulerAngles = eulerAngles;
		string text = btnType.ToString().Replace("BackQuote", "");
		this.text.text = text;
		textShifted.text = text;
		ColorBlock colors = button.colors;
		if (delete)
		{
			colors.normalColor = colorDeleteIdle;
			colors.highlightedColor = colorDeleteHover;
			colors.pressedColor = colorDeletePressed;
			button.image.sprite = spriteButtonLong;
			hitbox.anchoredPosition = new Vector2(8.52f, 0f);
			hitbox.sizeDelta = new Vector2(111.38f, 66f);
		}
		else
		{
			colors.normalColor = colorNormalIdle;
			colors.highlightedColor = colorNormalHover;
			colors.pressedColor = colorNormalPressed;
			button.image.sprite = spriteButtonShort;
			hitbox.anchoredPosition = new Vector2(0.826f, 0f);
			hitbox.sizeDelta = new Vector2(96f, 66f);
		}
		this.text.gameObject.SetActive(!delete);
		textShifted.gameObject.SetActive(delete);
		deleteIcon.gameObject.SetActive(delete);
		button.colors = colors;
	}
}
