using UnityEngine.UI;

public class scrMaxLevelTracker : ADOBase
{
	private Text maxLevel;

	private void Start()
	{
		maxLevel = GetComponent<Text>();
		maxLevel.text = "jjj";
		maxLevel.SetLocalizedFont();
	}

	private void Update()
	{
		if (ADOBase.isMobileMenu)
		{
			maxLevel.text = "Highest Level Reached: " + GCS.maxLevel;
		}
		else
		{
			maxLevel.text = RDString.Get("webgl.maxLevel") + GCS.maxLevel + "\n" + RDString.Get("webgl.reset");
		}
	}
}
