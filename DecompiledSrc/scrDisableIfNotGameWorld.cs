public class scrDisableIfNotGameWorld : ADOBase
{
	private void Start()
	{
		if (!ADOBase.controller.gameworld)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
