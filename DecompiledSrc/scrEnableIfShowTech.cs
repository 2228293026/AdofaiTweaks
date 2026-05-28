public class scrEnableIfShowTech : ADOBase
{
	public bool invert;

	private void Awake()
	{
		bool flag = Persistence.ShowTechLevels();
		if (invert)
		{
			flag = !flag;
		}
		base.gameObject.SetActive(flag);
	}
}
