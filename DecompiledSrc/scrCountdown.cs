using UnityEngine;
using UnityEngine.UI;

public class scrCountdown : ADOBase
{
	public new scrConductor conductor;

	public new scrController controller;

	private Text text;

	private float timeGoTween;

	private void Awake()
	{
		conductor = GameObject.Find("Conductor").GetComponent<scrConductor>();
		controller = GameObject.Find("Controller").GetComponent<scrController>();
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		text.text = string.Empty;
	}

	private void Update()
	{
		if (((bool)ADOBase.customLevel || controller.currentState == States.PlayerControl) && (controller.goShown || conductor.fastTakeoff))
		{
			double num = AudioSettings.dspTime - (double)scrConductor.calibration_i;
			if (controller.curCountdown < conductor.extraTicksCountdown.Count && num > conductor.extraTicksCountdown[controller.curCountdown].time)
			{
				if (num < conductor.extraTicksCountdown[controller.curCountdown].time + 1.0)
				{
					if (conductor.extraTicksCountdown[controller.curCountdown].count > 0)
					{
						this.text.text = conductor.extraTicksCountdown[controller.curCountdown].count.ToString();
					}
					else
					{
						this.text.text = RDString.Get("status.go");
						timeGoTween = (float)(conductor.crotchetAtStart / (double)conductor.extraTicksCountdown[controller.curCountdown].speed) / conductor.song.pitch;
					}
				}
				controller.curCountdown++;
			}
		}
		if (ADOBase.isLevelEditor && ADOBase.editor.inStrictlyEditingMode)
		{
			return;
		}
		if (!controller.goShown && !conductor.fastTakeoff && (controller.state == States.PlayerControl || controller.state == States.Countdown || controller.state == States.Checkpoint))
		{
			double num2 = AudioSettings.dspTime - (double)scrConductor.calibration_i;
			int countdownTicks = conductor.countdownTicks;
			if (num2 > conductor.GetCountdownTime(countdownTicks - 1) && !controller.goShown)
			{
				this.text.text = RDString.Get("status.go");
				controller.goShown = true;
				timeGoTween = (float)conductor.crotchetAtStart;
			}
			else if (num2 > conductor.GetCountdownTime(0))
			{
				int num3 = countdownTicks - 1;
				for (int i = 1; i < countdownTicks - 1; i++)
				{
					if (num2 > conductor.GetCountdownTime(i))
					{
						num3 = countdownTicks - 1 - i;
					}
				}
				this.text.text = num3.ToString();
			}
			else
			{
				string text = (GCS.speedTrialMode ? "levelSelect.SpeedTrial" : "status.getReady");
				text = (GCS.practiceMode ? "status.practiceMode" : text);
				this.text.text = RDString.Get(text);
			}
		}
		if (!controller.goShown && conductor.fastTakeoff && (controller.state == States.PlayerControl || controller.state == States.Countdown || controller.state == States.Checkpoint))
		{
			this.text.text = RDString.Get("status.go");
			controller.goShown = true;
			timeGoTween = (float)conductor.crotchetAtStart;
		}
		if (timeGoTween > 0f)
		{
			timeGoTween -= Time.unscaledDeltaTime;
			if (timeGoTween <= 0f)
			{
				CancelGo();
			}
		}
	}

	public void ShowOverload()
	{
		text.text = RDString.Get("status.overload");
	}

	public void ShowGetReady()
	{
		string text = (GCS.speedTrialMode ? "levelSelect.SpeedTrial" : "status.getReady");
		text = (GCS.practiceMode ? "status.practiceMode" : text);
		this.text.text = RDString.Get(text);
		controller.goShown = false;
	}

	public void CancelGo()
	{
		text.text = "";
		if (base.bb)
		{
			BBManager.instance.ShowCountdown(show: false);
		}
	}
}
