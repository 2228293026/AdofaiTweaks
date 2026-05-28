using System.Collections.Generic;
using System.Linq;

namespace ADOFAI.Editor.Actions;

public class PasteFloorEditorAction : EditorAction
{
	public override EditorTabKey sectionKey => EditorTabKey.BasicEditing;

	public override void Execute(scnEditor editor)
	{
		if (!editor.clipboard.Any())
		{
			return;
		}
		if (editor.clipboardContent == scnEditor.ClipboardContent.Floors && !editor.SelectionIsEmpty())
		{
			if (!editor.SelectionIsSingle())
			{
				if (editor.clipboard.Count == 1)
				{
					using (new SaveStateScope(editor))
					{
						scnEditor.FloorData floorData = (scnEditor.FloorData)editor.clipboard[0];
						List<scrFloor> list = new List<scrFloor>(editor.selectedFloors);
						for (int i = ((list[0].seqID == 0) ? 1 : 0); i < list.Count; i++)
						{
							editor.PasteEvents(list[i], floorData);
						}
						return;
					}
				}
				using (new SaveStateScope(editor))
				{
					editor.DeleteMultiSelection();
					editor.PasteFloors();
					return;
				}
			}
			if (editor.clipboard.Count == 1)
			{
				scnEditor.FloorData floorData2 = (scnEditor.FloorData)editor.clipboard[0];
				editor.PasteEvents(editor.selectedFloors[0], floorData2);
			}
			else
			{
				editor.PasteFloors();
			}
		}
		else if (editor.clipboardContent == scnEditor.ClipboardContent.Decorations)
		{
			editor.PasteDecorations();
		}
	}
}
