using UnityEngine.UI;

public class scrTextBonusColor : ADOBase
{
	private void Update()
	{
		if (Persistence.GetIsHighestPossibleAcc(6))
		{
			GetComponent<Text>().color = ADOBase.gc.goldTextColor;
		}
	}
}
