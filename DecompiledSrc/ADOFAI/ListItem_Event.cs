namespace ADOFAI;

public class ListItem_Event : ListItem
{
	public override void SetEvent(LevelEvent ev)
	{
		propertyControlList = ADOBase.editor.propertyControlEventsList;
		base.SetEvent(ev);
		LevelEventType eventType = ev.eventType;
		string text = eventType.ToString();
		itemName.text = text;
		base.transform.name = text;
		itemTypeImage.sprite = GCS.levelEventIcons[ev.eventType];
	}

	protected override void SelectItemButton()
	{
		base.SelectItemButton();
	}
}
