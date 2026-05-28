using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI;

public class ListItem_Decoration : ListItem
{
	private scrDecoration sourceDecoration;

	[Header("Buttons")]
	public Button eyeButton;

	public Button lockButton;

	[Header("Image Components")]
	public Image eyeButtonImage;

	public Image lockButtonImage;

	[Header("Sprites")]
	public Sprite eyeOpenSprite;

	public Sprite eyeClosedSprite;

	public Sprite decorationSprite;

	public Sprite textSprite;

	public Sprite objectSprite;

	public Sprite particleSprite;

	public Sprite lockOpenSprite;

	public Sprite lockClosedSprite;

	private bool visible => sourceLevelEvent.visible;

	private bool locked => sourceLevelEvent.locked;

	protected override void Awake()
	{
		base.Awake();
		propertyControlList = ADOBase.editor.propertyControlDecorationsList;
		eyeButton.onClick.AddListener(delegate
		{
			PowerButton();
		});
		lockButton.onClick.AddListener(delegate
		{
			LockButton();
		});
		displayedButtonColorMultiplier = eyeButton.colors.colorMultiplier;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!(sourceDecoration == null))
		{
			eyeButtonImage.sprite = (visible ? eyeOpenSprite : eyeClosedSprite);
			lockButtonImage.sprite = (locked ? lockClosedSprite : lockOpenSprite);
			ColorBlock colors = eyeButton.colors;
			colors.colorMultiplier = displayedButtonColorMultiplier * (sourceDecoration.forceHide ? 0.5f : 1f);
			eyeButton.colors = colors;
			ColorBlock colors2 = lockButton.colors;
			colors2.colorMultiplier = displayedButtonColorMultiplier * (sourceDecoration.forceLock ? 0.5f : 1f);
			lockButton.colors = colors2;
		}
	}

	public override void SetEvent(LevelEvent ev)
	{
		base.SetEvent(ev);
		sourceDecoration = scrDecorationManager.GetDecoration(ev);
		propertyControlList = ADOBase.editor.propertyControlDecorationsList;
		ShowSideButtons();
		itemName.text = sourceDecoration.decorationName;
		base.transform.name = ((sourceDecoration.transform.name == "") ? "" : sourceDecoration.decorationName);
		itemTagText.text = sourceDecoration.decorationTag;
		switch (ev.eventType)
		{
		case LevelEventType.AddDecoration:
			itemTypeImage.sprite = decorationSprite;
			break;
		case LevelEventType.AddText:
			itemTypeImage.sprite = textSprite;
			break;
		case LevelEventType.AddObject:
			itemTypeImage.sprite = objectSprite;
			break;
		case LevelEventType.AddParticle:
			itemTypeImage.sprite = particleSprite;
			break;
		}
		bool flag = ev.visible;
		eyeButtonImage.sprite = (flag ? eyeOpenSprite : eyeClosedSprite);
	}

	public override void OnHover(bool enteredCursor)
	{
		base.OnHover(enteredCursor);
		ShowSideButtons(enteredCursor);
	}

	protected override void ShowSelectionBackground(bool selected)
	{
		base.ShowSelectionBackground(selected);
		eyeButtonImage.color = (selected ? Color.black : Color.white);
		lockButtonImage.color = (selected ? Color.black : Color.white);
	}

	public void ShowSideButtons(bool forceShow = false)
	{
		bool flag = sourceLevelEvent.visible;
		bool flag2 = sourceLevelEvent.locked;
		eyeButton.gameObject.SetActive(forceShow || !flag);
		lockButton.gameObject.SetActive(forceShow || flag2);
	}

	protected override void SelectItemButton()
	{
		base.SelectItemButton();
		if (RDInput.holdingShift)
		{
			propertyControlList.SelectItemsInRange(sourceLevelEvent);
			return;
		}
		int decorationIndex = scrDecorationManager.GetDecorationIndex(sourceLevelEvent);
		ADOBase.editor.SelectDecoration(decorationIndex);
	}

	private void PowerButton()
	{
		ADOBase.editor.ShowEvent(sourceLevelEvent, !sourceLevelEvent.visible);
	}

	private void LockButton()
	{
		ADOBase.editor.LockEvent(sourceLevelEvent, !sourceLevelEvent.locked);
	}
}
