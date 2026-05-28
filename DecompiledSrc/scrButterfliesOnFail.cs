using UnityEngine;

public class scrButterfliesOnFail : ADOBase
{
	private void Update()
	{
		if (ADOBase.controller.state == States.Fail2)
		{
			ADOBase.controller.currFloor.gameObject.AddComponent<ffxButterflyCircle>();
			ffxButterflyCircle component = ADOBase.controller.currFloor.GetComponent<ffxButterflyCircle>();
			component.planetColors = true;
			component.StartEffect();
			Object.Destroy(this);
		}
	}
}
