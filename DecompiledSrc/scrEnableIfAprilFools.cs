public class scrEnableIfAprilFools : ADOBase
{
	public bool inJoker;

	public bool invert;

	private void Awake()
	{
		bool flag = (inJoker ? GCS.FOOL_JOKER : ADOBase.IsAprilFools());
		if (invert)
		{
			flag = !flag;
		}
		base.gameObject.SetActive(flag);
	}
}
