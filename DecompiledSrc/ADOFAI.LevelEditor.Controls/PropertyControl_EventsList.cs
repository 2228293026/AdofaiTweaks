namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_EventsList : PropertyControl_List
{
	public static string stringItemNoTag;

	private const string FilterFloorKey = "floor:";

	private const string FilterTypeKey = "type:";

	private const string FilterTagKey = "tag:";

	private string[] filterKeys = new string[3] { "floor:", "tag:", "type:" };

	private bool applyOnEventsUpdate;

	protected override void Awake()
	{
		base.Awake();
		stringItemNoTag = string.Format("<i>({0})</i>", RDString.Get("editor.noTag").ToLower());
	}

	protected override void Start()
	{
		contentRT = ADOBase.editor.eventsListContent;
		base.Start();
		removeButton.onClick.AddListener(delegate
		{
			ADOBase.editor.DeleteMultiSelection();
		});
		listItemPool.Initialize();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (applyOnEventsUpdate)
		{
			ApplyOnEventsUpdate();
		}
	}

	public void OnEventsUpdate()
	{
		applyOnEventsUpdate = true;
	}

	protected override void FilterSearchResults(string search, bool adjustRect = true)
	{
		base.FilterSearchResults(search, adjustRect);
		bool flag = false;
		if (string.IsNullOrEmpty(search))
		{
			foreach (LevelEvent @event in ADOBase.editor.events)
			{
				filteredEvents.Add(@event);
			}
		}
		else
		{
			string text = search.ToLower();
			string[] array = filterKeys;
			foreach (string text2 in array)
			{
				if (!text.Contains(text2))
				{
					continue;
				}
				flag = true;
				switch (text2)
				{
				case "floor:":
				{
					string text3 = text.Substring("floor:".Length);
					if (!int.TryParse(text3, out var result))
					{
						break;
					}
					foreach (LevelEvent event2 in ADOBase.editor.events)
					{
						if (result == event2.floor)
						{
							filteredEvents.Add(event2);
						}
					}
					break;
				}
				case "tag:":
				{
					string text3 = text.Substring("tag:".Length);
					foreach (LevelEvent event3 in ADOBase.editor.events)
					{
						string text4 = (string)event3["tag"];
						if (string.IsNullOrEmpty(text4))
						{
							text4 = "";
						}
						if (string.Equals(text4.ToLower(), text3))
						{
							filteredEvents.Add(event3);
						}
					}
					break;
				}
				case "type:":
				{
					string text3 = text.Substring("type:".Length);
					foreach (LevelEventType value in RDUtils.GetValues<LevelEventType>())
					{
						if (!(text3 == value.ToString()))
						{
							continue;
						}
						foreach (LevelEvent event4 in ADOBase.editor.events)
						{
							if (event4.eventType == value)
							{
								filteredEvents.Add(event4);
							}
						}
					}
					break;
				}
				}
			}
			if (!flag)
			{
				foreach (LevelEvent event5 in ADOBase.editor.events)
				{
					string obj = event5.eventType.ToString().ToLower();
					string text5 = (string)event5["tag"];
					if (text5.IsNullOrEmpty())
					{
						text5 = stringItemNoTag;
					}
					string text6 = text5.ToLower();
					bool flag2 = text6 == stringItemNoTag;
					if (obj.Contains(text) || (!flag2 && text6.Contains(text)))
					{
						filteredEvents.Add(event5);
					}
				}
			}
		}
		if (adjustRect && filteredEvents.Count > 0)
		{
			RefreshScrollRectPosition(filteredEvents[0]);
		}
	}

	private void ApplyOnEventsUpdate()
	{
		if (searchField != null)
		{
			FilterSearchResults(searchField.text, adjustRect: false);
		}
		RefreshItemsList(forceRefreshAll: true);
		applyOnEventsUpdate = false;
	}
}
