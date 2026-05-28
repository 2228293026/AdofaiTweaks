using System;
using System.Collections.Generic;
using Discord;
using GDMiniJSON;
using UnityEngine;

public class DiscordController : ADOBase
{
	public static DiscordController instance;

	public static bool shouldUpdatePresence = true;

	public static string currentUsername = null;

	public static long currentUserID = -1L;

	public static bool isBirthday = false;

	private global::Discord.Discord discord;

	private Dictionary<string, object> devPresenceStringDict;

	public void UpdatePresence()
	{
		if (discord == null)
		{
			return;
		}
		string s = string.Empty;
		string s2 = string.Empty;
		string text = string.Empty;
		scrController scrController2 = scrController.instance;
		if (Persistence.hideRichPresenceDetails)
		{
			s2 = "Clearing space dust ☄\ufe0f";
			s = "Hello Bin Guy \ud83d\udc40";
			foreach (string item in devPresenceStringDict?.Keys)
			{
				switch (item)
				{
				case "largeImageText":
					s = (string)devPresenceStringDict[item];
					break;
				case "details":
					s2 = (string)devPresenceStringDict[item];
					break;
				case "state":
					text = (string)devPresenceStringDict[item];
					break;
				}
			}
		}
		else if (ADOBase.sceneName == GCNS.sceneLevelSelect)
		{
			s2 = RDString.Get("discord.inLevelSelect");
			int overallProgressStage = Persistence.GetOverallProgressStage();
			string text2 = ((overallProgressStage >= 9) ? ((overallProgressStage < 10) ? RDString.Get("levelSelect.GameCompleteFull") : RDString.Get("levelSelect.GameCompleteFullPure")) : ((overallProgressStage < 5) ? text : RDString.Get("levelSelect.GameComplete")));
			text = text2;
		}
		else if (scrController2 != null && (bool)ADOBase.customLevel && !ADOBase.isOfficialLevel)
		{
			string text3 = ADOBase.customLevel.levelData.fullCaption;
			if (!ADOBase.isLevelEditor)
			{
				s2 = RDString.Get("discord.playing");
				if (!scrMisc.ApproximatelyFloor(GCS.speedTrialMode ? GCS.currentSpeedTrial : (ADOBase.isLevelEditor ? ADOBase.editor.playbackSpeed : 1f), 1.0))
				{
					string text4 = RDString.Get("levelSelect.multiplier", new Dictionary<string, object> { 
					{
						"multiplier",
						ADOBase.conductor.song.pitch.ToString("0.0#")
					} });
					text3 = text3 + " (" + text4 + ")";
				}
				text = text3;
			}
			else
			{
				s2 = RDString.Get("discord.inLevelEditor");
				if (!ADOBase.editor.customLevel.levelPath.IsNullOrEmpty())
				{
					text = RDString.Get("discord.editedLevel", new Dictionary<string, object> { { "level", text3 } });
				}
			}
		}
		else if (ADOBase.sceneName == "scnCLS")
		{
			s2 = RDString.Get("discord.inCustomLevelSelect");
		}
		else if (scrController2 != null && scrController2.gameworld)
		{
			string levelName = scrController2.levelName;
			string text5 = ADOBase.GetLocalizedLevelName(ADOBase.isInternalLevel ? GCS.internalLevelName : levelName).RemoveRichTags();
			if (!scrMisc.ApproximatelyFloor(GCS.speedTrialMode ? GCS.currentSpeedTrial : (ADOBase.isLevelEditor ? ADOBase.editor.playbackSpeed : 1f), 1.0))
			{
				string text6 = RDString.Get("levelSelect.multiplier", new Dictionary<string, object> { 
				{
					"multiplier",
					ADOBase.conductor.song.pitch.ToString("0.0#")
				} });
				text5 = text5 + " (" + text6 + ")";
			}
			s2 = RDString.Get("discord.playing");
			text = text5;
			s = text5;
		}
		s = Validate(s);
		text = Validate(text);
		s2 = Validate(s2);
		Activity activity = new Activity
		{
			State = text,
			Details = s2,
			Assets = 
			{
				LargeImage = "planets_icon_stars",
				LargeText = s
			}
		};
		try
		{
			discord.GetActivityManager().UpdateActivity(activity, delegate
			{
			});
		}
		catch (Exception)
		{
		}
		shouldUpdatePresence = false;
	}

	private string Validate(string s)
	{
		if (s.Length <= 60)
		{
			return s;
		}
		return s.Substring(0, 57) + "...";
	}

	private void CheckForBirthday()
	{
		string[] array = Resources.Load<TextAsset>("birthdays").text.Split('\n', StringSplitOptions.None);
		isBirthday = false;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(',', StringSplitOptions.None);
			string[] array4 = array3[0].Split('/', StringSplitOptions.None);
			if (long.TryParse(array3[1], out var result) && result == currentUserID)
			{
				DateTime now = DateTime.Now;
				int num = int.Parse(array4[0]);
				int num2 = int.Parse(array4[1]);
				int month = now.Month;
				int day = now.Day;
				if (num == month && num2 == day)
				{
					isBirthday = true;
					break;
				}
			}
		}
	}

	private void Update()
	{
		if (discord != null)
		{
			try
			{
				discord.RunCallbacks();
			}
			catch (ResultException ex)
			{
				_ = ex.Result;
			}
			if (shouldUpdatePresence)
			{
				UpdatePresence();
			}
		}
	}

	private void OnEnable()
	{
		if (instance != null)
		{
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		try
		{
			discord = new global::Discord.Discord(537047684993777686L, 1uL);
		}
		catch (Exception ex)
		{
			Debug.Log("Discord was not initialized: " + ex.ToString());
		}
		if (discord == null)
		{
			return;
		}
		try
		{
			discord.GetActivityManager().RegisterSteam(977950u);
		}
		catch (ResultException ex2)
		{
			Debug.LogWarning("Discord: failed to register steam launch method: " + ex2.Message);
		}
		discord.GetUserManager().OnCurrentUserUpdate += delegate
		{
			User currentUser = discord.GetUserManager().GetCurrentUser();
			currentUsername = currentUser.Username;
			currentUserID = currentUser.Id;
			CheckForBirthday();
			if (devPresenceStringDict != null && devPresenceStringDict.ContainsKey(currentUsername))
			{
				devPresenceStringDict = devPresenceStringDict[currentUsername] as Dictionary<string, object>;
				shouldUpdatePresence = true;
			}
		};
	}

	private void OnDisable()
	{
		if (discord != null)
		{
			discord.Dispose();
		}
	}

	private void Awake()
	{
		string text = Resources.Load<TextAsset>("DevDiscordStrings").text;
		devPresenceStringDict = Json.Deserialize(text) as Dictionary<string, object>;
	}
}
