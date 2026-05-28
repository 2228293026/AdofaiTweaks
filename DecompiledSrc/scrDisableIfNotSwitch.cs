public class scrDisableIfNotSwitch : ADOBase
{
	private void Start()
	{
		if (!ADOBase.isSwitch)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
