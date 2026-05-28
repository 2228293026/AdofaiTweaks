using UnityEngine;
using UnityEngine.UI;

public class scrShowIfDebug : ADOBase
{
	public bool hideWithNoAuto;

	private Text txt;

	private Color defaultColor = new Color(0.106f, 1f, 0f, 1f);

	private Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	private void Awake()
	{
		txt = GetComponent<Text>();
		txt.SetLocalizedFont();
	}

	private void Update()
	{
		if (hideWithNoAuto)
		{
			txt.enabled = !RDC.noAutoHud;
			if (RDC.noAutoHud)
			{
				return;
			}
		}
		if (RDC.noHud)
		{
			txt.enabled = false;
			return;
		}
		if (GCS.d_recording)
		{
			txt.enabled = false;
			return;
		}
		txt.color = defaultColor;
		if (RDC.auto && RDC.debug)
		{
			txt.enabled = true;
			txt.text = string.Empty;
		}
		else if (RDC.auto)
		{
			txt.enabled = true;
			if (RDC.useOldAuto)
			{
				txt.text = RDString.Get("status.autoplay") + " (old)";
			}
			else
			{
				txt.text = RDString.Get("status.autoplay");
			}
			if ((bool)ADOBase.editor && ADOBase.editor.pausedInPlayMode)
			{
				Text text = txt;
				text.text = text.text + " + " + RDString.Get("status.paused");
			}
		}
		else if (!RDC.auto && (bool)scrController.instance.currFloor && (bool)scrController.instance.currFloor.nextfloor && scrController.instance.currFloor.nextfloor.showStatusText && !ADOBase.sceneName.IsTaro())
		{
			txt.enabled = true;
			txt.text = RDString.Get("status.autoTile");
			txt.color = orangeColor;
		}
		else if (RDC.debug)
		{
			txt.enabled = true;
			txt.text = "Debug Mode";
		}
		else if (txt.enabled)
		{
			txt.enabled = false;
			txt.text = string.Empty;
		}
	}
}
