using UnityEngine;

public class WiderScreen : MonoBehaviour
{
	private void Awake()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.78f)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x * (num / 1.7777778f), base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
