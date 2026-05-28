using System;

public class SaveStateScope : IDisposable
{
	private scnEditor editor;

	public SaveStateScope(scnEditor editor, bool clearRedo = false, bool dataHasChanged = true, bool skipSaving = false)
	{
		this.editor = editor;
		if (!skipSaving)
		{
			editor.SaveState(clearRedo, dataHasChanged);
		}
		editor.changingState++;
	}

	public void Dispose()
	{
		editor.changingState--;
	}
}
