using System;
using Steamworks;
using UnityEngine;

public class SteamIntegration
{
	public struct SteamAchievement(Achievement id)
	{
		public Achievement id = id;

		public string name = string.Empty;

		public string description = string.Empty;

		public bool achieved = false;
	}

	public struct SteamStat(Stat name, bool valueType)
	{
		public Stat name = name;

		public float value = 0f;

		public bool isValueInt = valueType;
	}

	public enum Stat
	{
		stat_test
	}

	public enum Achievement
	{
		World0Complete,
		World1Complete,
		World2Complete,
		World3Complete,
		World4Complete,
		World5Complete,
		World7Complete,
		World8Complete,
		World9Complete,
		World0Perfect,
		World1Perfect,
		World2Perfect,
		World3Perfect,
		World4Perfect,
		World5Perfect,
		World7Perfect,
		World8Perfect,
		World9Perfect,
		World0Trial,
		World1Trial,
		World2Trial,
		World3Trial,
		World4Trial,
		World5Trial,
		World7Trial,
		World8Trial,
		World9Trial,
		BonusComplete,
		Game100PercentComplete
	}

	private static SteamIntegration _instance;

	public CGameID gameID;

	private SteamStat[] steamStatArray = new SteamStat[1]
	{
		new SteamStat(Stat.stat_test, valueType: false)
	};

	private SteamAchievement[] steamAchievementArray = new SteamAchievement[29]
	{
		new SteamAchievement(Achievement.World0Complete),
		new SteamAchievement(Achievement.World1Complete),
		new SteamAchievement(Achievement.World2Complete),
		new SteamAchievement(Achievement.World3Complete),
		new SteamAchievement(Achievement.World4Complete),
		new SteamAchievement(Achievement.World5Complete),
		new SteamAchievement(Achievement.World7Complete),
		new SteamAchievement(Achievement.World8Complete),
		new SteamAchievement(Achievement.World9Complete),
		new SteamAchievement(Achievement.World0Perfect),
		new SteamAchievement(Achievement.World1Perfect),
		new SteamAchievement(Achievement.World2Perfect),
		new SteamAchievement(Achievement.World3Perfect),
		new SteamAchievement(Achievement.World4Perfect),
		new SteamAchievement(Achievement.World5Perfect),
		new SteamAchievement(Achievement.World7Perfect),
		new SteamAchievement(Achievement.World8Perfect),
		new SteamAchievement(Achievement.World9Perfect),
		new SteamAchievement(Achievement.World0Trial),
		new SteamAchievement(Achievement.World1Trial),
		new SteamAchievement(Achievement.World2Trial),
		new SteamAchievement(Achievement.World3Trial),
		new SteamAchievement(Achievement.World4Trial),
		new SteamAchievement(Achievement.World5Trial),
		new SteamAchievement(Achievement.World7Trial),
		new SteamAchievement(Achievement.World8Trial),
		new SteamAchievement(Achievement.World9Trial),
		new SteamAchievement(Achievement.BonusComplete),
		new SteamAchievement(Achievement.Game100PercentComplete)
	};

	protected Callback<UserStatsReceived_t> userStatsReceived;

	protected Callback<UserStatsStored_t> userStatsStored;

	protected Callback<UserAchievementStored_t> userAchievementStored;

	private static bool everInitialized;

	public static bool initialized;

	private const string CLS_Entered = "cls_entered";

	private const string Editor_Entered = "editor_entered";

	private const string LevelSelect_Entered = "levelselect_entered";

	public static SteamIntegration instance
	{
		get
		{
			if (_instance == null)
			{
				Setup();
			}
			return _instance;
		}
	}

	public static void Setup()
	{
		_instance = new SteamIntegration();
		_instance.OpenConnection();
		if (initialized)
		{
			SteamWorkshop.Setup();
			RDC.runningOnSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
			RDC.isSteamDeckOnSteamOS = RDC.runningOnSteamDeck && SteamUtils.IsSteamInBigPictureMode();
			if (RDC.runningOnSteamDeck)
			{
				Debug.Log("runningOnSteamDeck = true");
			}
			if (RDC.isSteamDeckOnSteamOS)
			{
				Debug.Log("isSteamDeckOnSteamOS = true");
			}
		}
	}

	public SteamIntegration()
	{
		RDC.forceNoSteamworks = false;
		if (RDC.forceNoSteamworks)
		{
			Debug.Log("<color=yellow>Steamworks integration is disabled by RDC.forceNoSteamworks</color>");
		}
		else if (_instance == null)
		{
			_instance = this;
			if (everInitialized)
			{
				throw new Exception("Tried to Initialize the SteamAPI twice in one session");
			}
			if (!Packsize.Test())
			{
				Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
			}
			if (!DllCheck.Test())
			{
				Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
			}
			initialized = SteamAPI.Init();
			if (!initialized)
			{
				Debug.Log("SteamIntegration: Steamworks initialization error.");
				return;
			}
			everInitialized = true;
			Debug.Log("SteamIntegration: Steamworks initialized successfully");
		}
	}

	public void CheckCallbacks()
	{
		if (initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	public bool OpenConnection()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			return false;
		}
		gameID = new CGameID(SteamUtils.GetAppID());
		userStatsReceived = Callback<UserStatsReceived_t>.Create((DispatchDelegate<UserStatsReceived_t>)OnUserStatsReceived);
		userStatsStored = Callback<UserStatsStored_t>.Create((DispatchDelegate<UserStatsStored_t>)OnUserStatsStored);
		userAchievementStored = Callback<UserAchievementStored_t>.Create((DispatchDelegate<UserAchievementStored_t>)OnAchievementStored);
		SteamUser.GetSteamID();
		AppId_t val = default(AppId_t);
		foreach (DLCManager dLCManager in DLCManager.DLCManagers)
		{
			((AppId_t)(ref val))._002Ector(dLCManager.steamAppId);
			dLCManager.own = SteamApps.BIsDlcInstalled(val);
			Debug.Log($"Has license for {dLCManager.steamAppId}: {dLCManager.own}");
		}
		if (SteamUserStats.RequestCurrentStats())
		{
			for (int i = 0; i < steamStatArray.Length; i++)
			{
				SteamUserStats.GetStat(steamStatArray[i].name.ToString(), ref steamStatArray[i].value);
			}
		}
		return true;
	}

	public void CloseConnection()
	{
		if (_instance == this)
		{
			_instance = null;
			if (initialized)
			{
				SteamAPI.Shutdown();
				everInitialized = false;
				initialized = false;
				_instance = null;
			}
		}
	}

	public bool GetStatValue(Stat statName, ref int value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = (int)steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public bool GetStatValue(Stat statName, ref float value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public bool SetStatValue(Stat statName, int addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				Debug.Log((int)steamStatArray[i].value);
				SteamUserStats.SetStat(statName.ToString(), (int)steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public bool SetStatValue(Stat statName, float addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				SteamUserStats.SetStat(statName.ToString(), steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public string[] GetSteamFriends()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (initialized)
		{
			string[] array = new string[SteamFriends.GetFriendCount((EFriendFlags)4)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, (EFriendFlags)4));
			}
			return array;
		}
		return new string[0];
	}

	public string GetPlayersName()
	{
		if (!initialized)
		{
			return string.Empty;
		}
		return SteamFriends.GetPersonaName();
	}

	public bool GetAchievementByName(Achievement achievementID, ref SteamAchievement steamAchievement)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamAchievementArray.Length; i++)
		{
			if (steamAchievementArray[i].id == achievementID)
			{
				steamAchievement = steamAchievementArray[i];
				return true;
			}
		}
		return false;
	}

	public void UnlockAchievement(SteamAchievement achievement)
	{
		if (initialized)
		{
			SteamUserStats.SetAchievement(achievement.ToString());
		}
	}

	public void UnlockAchievementWithName(string achievement)
	{
		if (initialized)
		{
			SteamUserStats.SetAchievement(achievement);
		}
	}

	private unsafe void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between I4 and Unknown
		if (!initialized || (ulong)gameID != pCallback.m_nGameID)
		{
			return;
		}
		if (1 == (int)pCallback.m_eResult)
		{
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				if (SteamUserStats.GetAchievement(steamAchievementArray[i].id.ToString(), ref steamAchievementArray[i].achieved))
				{
					steamAchievementArray[i].name = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "name");
					steamAchievementArray[i].description = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "desc");
				}
				else
				{
					Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + steamAchievementArray[i].id.ToString() + "\nIs it registered in the Steam Partner site?");
				}
			}
			for (int j = 0; j < steamStatArray.Length; j++)
			{
				SteamUserStats.GetStat(steamStatArray[j].name.ToString(), ref steamStatArray[j].value);
			}
		}
		else
		{
			Debug.Log("RequestStats - failed, " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString());
		}
	}

	private unsafe void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between I4 and Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between I4 and Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if ((ulong)gameID == pCallback.m_nGameID && 1 != (int)pCallback.m_eResult)
		{
			if (8 == (int)pCallback.m_eResult)
			{
				Debug.Log("StoreStats - some failed to validate");
				OnUserStatsReceived(new UserStatsReceived_t
				{
					m_eResult = (EResult)1,
					m_nGameID = (ulong)gameID
				});
			}
			else
			{
				Debug.Log("StoreStats - failed, " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString());
			}
		}
	}

	private void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((ulong)gameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + ((UserAchievementStored_t)(ref pCallback)).m_rgchAchievementName + "' unlocked!");
				return;
			}
			Debug.Log("Achievement '" + ((UserAchievementStored_t)(ref pCallback)).m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
		}
	}

	public void ClearAllAchievements()
	{
		if (initialized)
		{
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				SteamUserStats.ClearAchievement(steamAchievementArray[i].id.ToString());
			}
		}
	}

	public void ResetAllData()
	{
		if (initialized)
		{
			SteamUserStats.ResetAllStats(true);
		}
	}

	public static void IncrementCLSEnteredStat()
	{
		if (initialized)
		{
			int num = default(int);
			SteamUserStats.GetStat("cls_entered", ref num);
			num++;
			SteamUserStats.SetStat("cls_entered", num);
		}
	}

	public static void CLSEntered()
	{
		if (initialized)
		{
			SteamUserStats.SetStat("cls_entered", 1);
		}
	}

	public static void EditorEntered()
	{
		if (initialized)
		{
			SteamUserStats.SetStat("editor_entered", 1);
		}
	}

	public static void LevelSelectEntered()
	{
		if (initialized)
		{
			SteamUserStats.SetStat("levelselect_entered", 1);
		}
	}
}
