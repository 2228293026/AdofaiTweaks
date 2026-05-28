using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class GameServices
{
	public enum LoadStatus
	{
		Nothing,
		Authenticate,
		Data,
		Successful,
		Failed
	}

	private static GameServices instance;

	private bool initialized;

	private bool retroactiveCompleted;

	private string achievementsFirstTimeKey = "achievementsFirstTime";

	private DateTime initDateTime;

	private bool timeOut;

	protected bool debugIsPossible;

	protected List<int> frameRates = new List<int>();

	protected int maxFramerate = 60;

	private LoadStatus loadStatus;

	protected List<string> unlockedAchievements = new List<string>();

	public List<string> achievementsQueue = new List<string>();

	public bool showRetroactiveAchievements;

	public static GameServices Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameServicesEmpty();
			}
			return instance;
		}
	}

	public virtual bool IsLoadStatusComplete
	{
		get
		{
			if (loadStatus != LoadStatus.Failed)
			{
				if (loadStatus == LoadStatus.Successful)
				{
					return retroactiveCompleted;
				}
				return false;
			}
			return true;
		}
	}

	public virtual bool Initialized => initialized;

	public virtual bool TimeOut => timeOut;

	public virtual bool IsFirstLoading
	{
		get
		{
			if (loadStatus != LoadStatus.Failed)
			{
				return loadStatus != LoadStatus.Successful;
			}
			return false;
		}
	}

	public virtual bool DebugIsPosible => debugIsPossible;

	public virtual List<int> FrameRates => frameRates;

	public virtual int MaxFrameRate => maxFramerate;

	private void CheckTimeOut()
	{
		while (!((DateTime.Now - initDateTime).TotalSeconds >= 10.0) || !retroactiveCompleted)
		{
		}
		if (!IsLoadStatusComplete)
		{
			timeOut = true;
		}
	}

	public virtual void Initialize()
	{
		retroactiveCompleted = Convert.ToBoolean(PlayerPrefs.GetInt(achievementsFirstTimeKey, 0));
		CheckIfDebugIsPossible();
		initDateTime = DateTime.Now;
		new Thread(CheckTimeOut).Start();
		loadStatus = LoadStatus.Authenticate;
		Authenticate(delegate(bool success)
		{
			if (success)
			{
				OnInitialized();
			}
			else
			{
				initialized = false;
				retroactiveCompleted = true;
				loadStatus = LoadStatus.Failed;
				Debug.Log("[GameServices] Initialization failed");
			}
		});
	}

	protected virtual void Authenticate(Action<bool> authenticateCallback)
	{
	}

	protected virtual void OnInitialized()
	{
		initialized = true;
		LoadAchievements();
		loadStatus = LoadStatus.Data;
		LoadGame();
	}

	protected virtual void LoadAchievements()
	{
	}

	protected void AddUnlockAchievement(string achievement)
	{
		if (!unlockedAchievements.Contains(achievement))
		{
			unlockedAchievements.Add(achievement);
		}
	}

	protected void CheckRetroactiveAchievements()
	{
		if (!retroactiveCompleted)
		{
			retroactiveCompleted = true;
			PlayerPrefs.SetInt(achievementsFirstTimeKey, 1);
			Persistence.GiveAchievements();
			if (unlockedAchievements.Count > 0)
			{
				AddAchievementsToQueue(unlockedAchievements);
				showRetroactiveAchievements = true;
			}
		}
	}

	protected void AddAchievementToQueue(string achievement)
	{
		if (!achievementsQueue.Contains(achievement))
		{
			achievementsQueue.Add(achievement);
		}
	}

	protected virtual void AddAchievementsToQueue(List<string> unlockedAchievements)
	{
	}

	public virtual void HasUnlockedAchievement(string name, out bool unlocked)
	{
		unlocked = false;
	}

	public virtual void UnlockAchievementWithName(string name)
	{
		HasUnlockedAchievement(name, out var unlocked);
		if (!unlocked)
		{
			AddAchievementToQueue(name);
		}
	}

	public virtual void ClearAllAchievements()
	{
	}

	public virtual void ShowAchievements()
	{
	}

	public virtual void LoadGame()
	{
	}

	public virtual void SaveGame()
	{
	}

	public virtual void DeleteGame()
	{
	}

	protected void LoadGameToDisk(byte[] bytes)
	{
		bool flag = false;
		Dictionary<string, object> fileContent;
		if (bytes == null || bytes.Length == 0)
		{
			flag = true;
		}
		else if (PlayerPrefsJson.CreateFromBytes(bytes, out fileContent))
		{
			PlayerPrefsJson cloudPrefs = new PlayerPrefsJson(PlayerPrefsJson.FileType.General, fileContent);
			if (Persistence.IsCompatibleWithCloud(cloudPrefs))
			{
				Persistence.CheckWithCloud(cloudPrefs, Instance.IsFirstLoading);
				Persistence.WriteSaveToDisk();
				flag = true;
			}
			else
			{
				Notification.instance.ShowIncompatibleCloud();
			}
		}
		else
		{
			Debug.Log("[GameServices] LoadGameToDisk Error: Could not create from bytes");
		}
		if (flag)
		{
			SaveGame();
			if (Instance.IsFirstLoading && timeOut)
			{
				Notification.instance.ShowGameServicesComplete();
			}
		}
		loadStatus = LoadStatus.Successful;
	}

	protected virtual void InitializeVibration()
	{
	}

	public virtual void Vibrate(long ms)
	{
		InitializeVibration();
	}

	public virtual void CancelVibration()
	{
		InitializeVibration();
	}

	public virtual bool HasVibrator()
	{
		return false;
	}

	public virtual void SetupFrameRate()
	{
	}

	public virtual double GetAvailableDiskSpace()
	{
		return 0.0;
	}

	public virtual void CheckIfDebugIsPossible()
	{
	}
}
