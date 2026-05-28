using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrPercentageComplete : MonoBehaviour
{
	public Text newBestText;

	private float percent;

	public bool custommessage;

	private Text text;

	private void Awake()
	{
	}

	public void UpdatePercent()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		if (!custommessage)
		{
			percent = scrController.instance.percentComplete * 100f;
			int num = Mathf.FloorToInt(percent);
			text.text = RDString.Get("status.complete", new Dictionary<string, object> { { "pctComplete", num } });
			bool flag = scrController.instance.endLevelInfo.endLevelType == EndLevelType.NewBest;
			RectTransform obj = base.transform as RectTransform;
			newBestText.text = RDString.Get("status.newbest");
			newBestText.gameObject.SetActive(flag);
			obj.pivot = obj.pivot.WithY(flag ? 0.275f : 0.5f);
		}
	}

	public void ShowMessage(string msg)
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		GetComponent<Text>().text = msg;
		custommessage = true;
	}
}
