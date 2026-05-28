using UnityEngine.UI;

public class scrPressToStart : ADOBase
{
	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
		text.text = string.Empty;
		text.SetLocalizedFont();
	}

	public void ShowText()
	{
		text.text = RDString.Get(ADOBase.isMobile ? "status.tapToBegin" : "status.pressToBegin");
		if (GCS.d_drumcontroller)
		{
			text.text = RDString.Get("status.drumToBegin");
		}
		DifficultyUIMode mode = ((!ADOBase.lm.hideDifficultyUI) ? scrMisc.DetermineDifficultyUIMode(ADOBase.lm.highestBPM) : DifficultyUIMode.DontShow);
		ADOBase.uiController.ShowDifficultyContainer(mode);
	}

	public void HideText()
	{
		text.text = string.Empty;
		if (base.bb)
		{
			BBManager.instance.ShowLevelName(show: false);
		}
	}
}
