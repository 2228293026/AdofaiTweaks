using UnityEngine;

public class scrEnableOnlyForLanguage : MonoBehaviour
{
	public bool english;

	public bool chinese;

	private void Start()
	{
		bool isChinese = RDString.isChinese;
		if (isChinese && !chinese)
		{
			base.gameObject.SetActive(value: false);
		}
		if (!isChinese && !english)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
