using UnityEngine;
using UnityEngine.UI;

public class OttoButtonController : ADOBase
{
	public Button button;

	public Button beatLevelButton;

	private void Awake()
	{
		button.onClick.AddListener(ToggleAuto);
		beatLevelButton.onClick.AddListener(BeatLevel);
	}

	private void Update()
	{
		bool flag = ADOBase.controller != null && ADOBase.controller.gameworld;
		button.gameObject.SetActive(RDC.debug && !RDC.noHud && flag);
		button.image.color = (RDC.auto ? Color.white : Color.gray);
		beatLevelButton.gameObject.SetActive(RDC.debug && !RDC.noHud && flag);
	}

	private void ToggleAuto()
	{
		RDC.auto = !RDC.auto;
	}

	private void BeatLevel()
	{
		scrController.instance.levelWasSkipped = true;
		scrController.instance.BeatLevel();
	}
}
