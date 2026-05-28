using System.Linq;
using TMPro;

public class scrEnableIfBeta : ADOBase
{
	public bool setBuildText;

	private void Awake()
	{
		base.gameObject.SetActive(value: false);
		if (!RDC.debug && SteamIntegration.initialized && !GCNS.stableBranches.Contains(GCS.steamBranchName) && !GCS.steamBranchName.IsNullOrEmpty())
		{
			base.gameObject.SetActive(value: true);
			if (setBuildText)
			{
				GetComponent<TMP_Text>().text = char.ToUpper(GCS.steamBranchName[0]) + GCS.steamBranchName.Substring(1) + " Build";
			}
		}
	}
}
