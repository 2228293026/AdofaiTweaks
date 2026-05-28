using System;

public class scrDisableIfPastDate : ADOBase
{
	private void Update()
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((new DateTime(2020, 6, 28, 17, 0, 0) - utcNow).CompareTo(TimeSpan.Zero) < 0)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
