using UnityEngine;
using UnityEngine.UI;

public class scrVersionText : MonoBehaviour
{
	public RectTransform version;

	public Text text;

	private int page;

	private string page0 => $"v{Application.version} ({ADOBase.platform})";

	private string page1 => $"r{141} ({GCNS.buildCommit}, {GCNS.buildDate})";

	private void Awake()
	{
		text.text = page0;
	}

	public void Init()
	{
		bool flag = (bool)ADOBase.controller.currFloor && (ADOBase.controller.currFloor.freeroam || ADOBase.controller.currFloor.freeroamGenerated);
		bool flag2 = ADOBase.controller.gameworld || flag;
		bool flag3 = !ADOBase.sceneName.IsTaro() || ADOBase.controller.isPuzzleRoom || !ADOBase.controller.isbosslevel || !flag2 || GCS.speedTrialMode || GCS.practiceMode;
		RectTransform rectTransform = version;
		Vector2 anchorMin = (version.anchorMax = new Vector2((!flag3) ? 1 : 0, 0f));
		rectTransform.anchorMin = anchorMin;
		version.anchoredPosition = new Vector2(flag3 ? 65 : (-65), 13f);
		version.pivot = new Vector2(flag3 ? 1 : 0, 1f);
		text.alignment = (flag3 ? TextAnchor.LowerLeft : TextAnchor.LowerRight);
	}

	public void UpdatePage()
	{
		page = ((page == 0) ? 1 : 0);
		text.text = ((page == 0) ? page0 : page1);
	}
}
