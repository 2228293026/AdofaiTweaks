public class scrMenuContinueDisable : ADOBase
{
	private void Awake()
	{
		string text = Persistence.savedCurrentLevel;
		if (!RDUtils.CheckDLCLevelPlayable(text) || GCS.FOOL_JOKER)
		{
			text = "0-0";
		}
		if (text == "0-0")
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
