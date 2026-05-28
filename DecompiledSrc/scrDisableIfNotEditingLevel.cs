public class scrDisableIfNotEditingLevel : ADOBase
{
	private void Update()
	{
		if (!ADOBase.editor || !ADOBase.editor.inStrictlyEditingMode)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
