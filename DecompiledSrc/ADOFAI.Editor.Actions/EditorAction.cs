namespace ADOFAI.Editor.Actions;

public abstract class EditorAction
{
	public abstract EditorTabKey sectionKey { get; }

	public virtual string descriptionKey => GetType().Name.Replace("EditorAction", "");

	public abstract void Execute(scnEditor editor);
}
