public class DisableAtAwake : ADOBase
{
	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}
}
