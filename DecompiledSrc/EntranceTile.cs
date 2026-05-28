using UnityEngine;

public class EntranceTile : ADOBase
{
	private void LateUpdate()
	{
		scnCLS instance = scnCLS.instance;
		float num = ADOBase.controller.camy.pos.y;
		if ((float)instance.levelCount >= instance.levelCountForLoop)
		{
			int num2 = instance.gemBottomY + 1;
			int num3 = instance.gemTopY - 1;
			num = Mathf.Clamp(num, num2, num3);
		}
		if (base.transform.position.y != num)
		{
			base.transform.MoveY(num);
		}
	}
}
