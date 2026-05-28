public class scrDisableIfNotMobile : ADOBase
{
	private void Start()
	{
		if (!ADOBase.isMobile)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
