public class scrDisableIfMinimumVFX : ADOBase
{
	private void Awake()
	{
		if (ADOBase.controller.visualEffects == VisualEffects.Minimum)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
