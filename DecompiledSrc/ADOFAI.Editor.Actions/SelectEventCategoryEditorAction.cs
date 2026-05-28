using System;
using System.Collections.Generic;
using System.Linq;

namespace ADOFAI.Editor.Actions;

public class SelectEventCategoryEditorAction : EditorAction
{
	private int number;

	public override EditorTabKey sectionKey => EditorTabKey.None;

	public SelectEventCategoryEditorAction(int number)
	{
		this.number = ((number == 0) ? 10 : number);
	}

	public override void Execute(scnEditor editor)
	{
		List<LevelEventCategory> list = (from x in Enum.GetValues(typeof(LevelEventCategory)).Cast<LevelEventCategory>().ToList()
			where editor.eventButtons[x].Any((LevelEventButton y) => !editor.selectedFirstFloor || y.info.allowFirstFloorCheck) || x == LevelEventCategory.Favorites
			select x).ToList();
		if (number <= list.Count)
		{
			LevelEventCategory eventCategory = list[number - 1];
			editor.SetCategory(eventCategory);
		}
	}
}
