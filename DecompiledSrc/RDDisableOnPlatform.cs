public class RDDisableOnPlatform : ADOBase
{
	public enum RDPlatform
	{
		Mobile,
		Desktop
	}

	public RDPlatform disableIfPlatformIs;

	private void Awake()
	{
		if ((ADOBase.isMobile && disableIfPlatformIs == RDPlatform.Mobile) || (!ADOBase.isMobile && disableIfPlatformIs == RDPlatform.Desktop))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
