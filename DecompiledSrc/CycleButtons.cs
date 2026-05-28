using System.Collections.Generic;
using ADOFAI;
using TMPro;
using UnityEngine.UI;

public class CycleButtons : ADOBase
{
	public Button prevButton;

	public Button nextButton;

	public InspectorTab tab;

	public InspectorPanel panel;

	public TMP_Text text;

	public void CycleEvent(bool next)
	{
		int eventIndex = tab.eventIndex;
		LevelEventType levelEventType = tab.levelEventType;
		List<LevelEvent> selectedFloorEvents = ADOBase.editor.GetSelectedFloorEvents(levelEventType);
		eventIndex = ((!next) ? ((eventIndex - 1 < 0) ? (selectedFloorEvents.Count - 1) : (eventIndex - 1)) : ((eventIndex + 1) % selectedFloorEvents.Count));
		tab.eventIndex = eventIndex;
		panel.ShowPanel(levelEventType, eventIndex);
	}
}
