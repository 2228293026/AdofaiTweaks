using System;
using UnityEngine;
using UnityEngine.UI;

namespace MobileMenu;

public class MobileMenuDebug : ADOBase
{
	[Header("References")]
	public MobileMenuController menuController;

	public GameObject debugPanel;

	public InputField resolutionInputField;

	public Text resolutionLabel;

	public InputField playScheduledInputField;

	public AudioSource soundTest;

	private void Awake()
	{
		resolutionInputField.text = Screen.width + "x" + Screen.height;
		debugPanel.SetActive(RDC.debug);
	}

	public void CompleteFirst5()
	{
		Persistence.CompleteFirst5();
		ADOBase.GoToLevelSelect();
	}

	public void CompleteMain()
	{
		Persistence.CompleteAllMainLevels();
		ADOBase.GoToLevelSelect();
	}

	public void UnlockBonus()
	{
		Persistence.CompleteAllMainLevelsAndSpeedTrials();
		ADOBase.GoToLevelSelect();
	}

	public void BeatSelectedWorld()
	{
		string world = (menuController.currentScreen as MobileMenuPortalScreen).world;
		Persistence.CompleteWorld(GCNS.worldData[world].index);
		Persistence.passedMobileMenuTutorial = true;
		Persistence.Save();
		MobileMenuMap map = menuController.map;
		map.EvaluateAllConditions();
		map.Build();
		foreach (MobileMenuPortalScreen value in map.portalLUT.Values)
		{
			if (value.visible)
			{
				value.CheckLocked(speedTrial: false);
			}
		}
	}

	public void BeatFirstWorld()
	{
		Persistence.CompleteFirst();
		ADOBase.GoToLevelSelect();
	}

	public void Complete100Percent()
	{
		Persistence.Complete100();
		ADOBase.GoToLevelSelect();
	}

	public void StartBenchmark()
	{
		GCS.turnOnBenchmarkMode = true;
	}

	public void SetResolution()
	{
		string[] array = resolutionInputField.text.Split('x', StringSplitOptions.None);
		if (array.Length == 2)
		{
			int result = 0;
			int result2 = 0;
			bool flag = int.TryParse(array[0], out result);
			bool flag2 = int.TryParse(array[1], out result2);
			if (flag && flag2 && result > 0 && result2 > 0)
			{
				Screen.SetResolution(result, result2, fullscreen: true);
				resolutionLabel.color = Color.black;
			}
			else
			{
				resolutionLabel.color = Color.red;
			}
		}
		else
		{
			resolutionLabel.color = Color.red;
		}
	}

	public void ClearAchievements()
	{
		ADOBase.controller.ClearAllAchievements();
	}

	public void TurnOffDebugMode()
	{
		RDC.debug = false;
		debugPanel.SetActive(value: false);
	}

	public void PlayScheduled(bool usingAudioManager)
	{
		if (float.TryParse(playScheduledInputField.text, out var result))
		{
			double time = AudioSettings.dspTime + (double)result;
			if (usingAudioManager)
			{
				AudioManager.Play("sndPowerDown", time, scrConductor.instance.hitSoundGroup);
			}
			else
			{
				soundTest.PlayScheduled(time);
			}
		}
	}

	public void LoadGame()
	{
		GameServices.Instance.LoadGame();
	}

	public void SaveGame()
	{
		GameServices.Instance.SaveGame();
	}

	public void DeleteGame()
	{
		GameServices.Instance.DeleteGame();
	}

	public void ShowDashboard()
	{
		GameServices.Instance.ShowAchievements();
	}

	public void ClearKey()
	{
		PlayerPrefs.DeleteKey("achievementsFirstTime");
	}
}
