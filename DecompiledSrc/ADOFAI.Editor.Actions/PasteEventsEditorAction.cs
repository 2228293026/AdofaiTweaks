using System.Collections.Generic;
using System.Linq;

namespace ADOFAI.Editor.Actions;

public class PasteEventsEditorAction : EditorAction
{
	private bool alsoPasteDecorations;

	public override EditorTabKey sectionKey => EditorTabKey.AdvancedEditing;

	public PasteEventsEditorAction(bool alsoPasteDecorations = true)
	{
		this.alsoPasteDecorations = alsoPasteDecorations;
	}

	public override void Execute(scnEditor editor)
	{
		List<scrFloor> selectedFloors;
		using (new SaveStateScope(editor))
		{
			if (!editor.clipboard.Any() || editor.SelectionIsEmpty())
			{
				return;
			}
			if (editor.SelectionIsSingle())
			{
				if (editor.clipboard.Count() == 1)
				{
					if (editor.clipboardContent == scnEditor.ClipboardContent.Floors)
					{
						scnEditor.FloorData floorData = (scnEditor.FloorData)editor.clipboard[0];
						editor.PasteEvents(editor.selectedFloors[0], floorData, alsoPasteDecorations, overwrite: false);
					}
				}
				else if (editor.clipboardContent == scnEditor.ClipboardContent.Floors)
				{
					int seqID = editor.selectedFloors[0].seqID;
					scnEditor.FloorData floorData2 = (scnEditor.FloorData)editor.clipboard[0];
					editor.PasteEvents(editor.selectedFloors[0], floorData2, alsoPasteDecorations, overwrite: false);
					for (int i = 1; i < editor.clipboard.Count && seqID + i < editor.floors.Count; i++)
					{
						floorData2 = (scnEditor.FloorData)editor.clipboard[i];
						editor.PasteEvents(editor.floors[seqID + i], floorData2, alsoPasteDecorations, overwrite: false, selectAfterward: false);
					}
				}
			}
			else if (editor.clipboard.Count() == 1)
			{
				selectedFloors = new List<scrFloor>(editor.selectedFloors);
				if (selectedFloors.Count > 1000)
				{
					editor.ConfirmPopup(RDString.Get("editor.largeFloorsEditWarning"), Action);
				}
				else
				{
					Action();
				}
			}
		}
		void Action()
		{
			using (new SaveStateScope(editor))
			{
				scnEditor.FloorData floorData3 = (scnEditor.FloorData)editor.clipboard[0];
				for (int j = ((selectedFloors[0].seqID == 0) ? 1 : 0); j < selectedFloors.Count; j++)
				{
					editor.PasteEvents(selectedFloors[j], floorData3, alsoPasteDecorations, overwrite: false, selectAfterward: true, j == selectedFloors.Count - 1);
				}
			}
		}
	}
}
