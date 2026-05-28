public class scrDisableIfSteamworksDisabled : ADOBase
{
	private void Start()
	{
		if (!SteamIntegration.initialized)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
