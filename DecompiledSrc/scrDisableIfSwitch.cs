public class scrDisableIfSwitch : ADOBase
{
	private void Start()
	{
		if (ADOBase.isSwitch)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
