public class scrDisableIfCoop : ADOBase
{
	private void Start()
	{
		if (scrController.coopMode)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
