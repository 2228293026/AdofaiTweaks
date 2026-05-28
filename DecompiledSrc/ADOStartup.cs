using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using Rewired;
using Steamworks;
using UnityEngine;

public static class ADOStartup
{
	public static ErrorCanvas errorCanvas;

	public static List<string> addedMods = new List<string>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Startup()
	{
		Application.logMessageReceived += LogMessageReceived;
		StartupLog("GetPlatform");
		GetPlatform();
		StartupLog("DetermineAppLocation");
		DetermineAppLocation();
		StartupLog("SetBuildDateAndCommit");
		SetBuildDateAndCommit();
		StartupLog("SetLocale");
		SetLocale();
		StartupLog("LoadSaveData");
		LoadSaveData();
		StartupLog("SetupSystems");
		SetupSystems();
		StartupLog("Initialize Steam SDK");
		InitializeSteamSDK();
		if (ADOBase.isExpo)
		{
			Persistence.Complete100();
		}
		GameServices.Instance.Initialize();
		StartupLog("FixResolution");
		FixResolution();
		StartupLog("SetupLevelEventsInfo");
		SetupLevelEventsInfo();
		StartupLog("SetSettings");
		SetSettings();
		StartupLog("ForceResolutionOnLofi");
		ForceResolutionOnLofi();
		StartupLog("LoadCalibration");
		LoadCalibration();
		StartupLog("SetupSfxHandler");
		SetupSfxHandler();
		StartupLog("GetSteamBranch");
		InitializeSteamIntegration();
		StartupLog("SetupDLCs");
		SetupDLCs();
		StartupLog("CacheShaderPropertyIDs");
		CacheShaderPropertyIDs();
		StartupLog("DeleteTempUnzippingFolder");
		DeleteTempUnzippingFolder();
		if (!ADOBase.isMobile)
		{
			StartupLog("Setup Discord");
			new GameObject("DiscordController").AddComponent<DiscordController>();
		}
		StartupLog("Give Steam Achievements");
		Persistence.GiveAchievements();
		StartupLog("Rewired Init");
		InputManager rewiredManager = UnityEngine.Object.Instantiate<InputManager>(RDConstants.data.prefab_rewiredManager);
		StartupLog("RDInput Setup");
		RDInput.Setup(rewiredManager);
		scrPlayerManager.Setup();
		Analytics.LimitAnalyticsIfModded();
		Analytics.UploadBranchToUnity();
		QualitySettings.antiAliasing = Persistence.antiAliasing;
		StartupLog("Setup Audio Settings");
		AudioConfiguration configuration = AudioSettings.GetConfiguration();
		configuration.dspBufferSize = Persistence.audioBufferSize;
		AudioSettings.Reset(configuration);
		StartupLog("Setup Loading Screen");
		SetupLoader();
		StartupLog("<color=green>ADOStartup done!</color>");
		static void FixResolution()
		{
			Vector2Int vector2Int = new Vector2Int(640, 360);
			if (Screen.width < vector2Int.x || Screen.height < vector2Int.y)
			{
				Screen.SetResolution(vector2Int.x, vector2Int.y, FullScreenMode.Windowed, new RefreshRate
				{
					numerator = 60u,
					denominator = 1u
				});
			}
			if (RDC.runningOnSteamDeck)
			{
				Resolution[] resolutions = Screen.resolutions;
				Resolution resolution = resolutions[0];
				int num = resolution.width * resolution.height;
				Resolution[] array = resolutions;
				for (int i = 0; i < array.Length; i++)
				{
					Resolution resolution2 = array[i];
					int num2 = resolution2.width * resolution2.height;
					if (num2 > num)
					{
						resolution = resolution2;
						num = num2;
					}
					else if (num2 == num && resolution2.refreshRateRatio.value > resolution.refreshRateRatio.value)
					{
						resolution = resolution2;
					}
				}
				if (resolution.width == 1280 && resolution.height == 800)
				{
					Screen.SetResolution(1280, 800, FullScreenMode.MaximizedWindow);
					StartupLog($"Setting game resolution to {1280}x{800} (Steam Deck)");
				}
			}
		}
		static void ForceResolutionOnLofi()
		{
			if (GCS.lofiVersion && !ADOBase.isMobile)
			{
				Screen.fullScreen = false;
				if (ADOBase.playerIsOnIntroScene && !GCS.webHasSetResolution)
				{
					Screen.SetResolution(800, 400, FullScreenMode.Windowed);
				}
			}
		}
		static void GetPlatform()
		{
			Platform platform;
			switch (Application.platform)
			{
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				platform = Platform.Windows;
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				platform = Platform.Mac;
				break;
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				platform = Platform.Linux;
				break;
			case RuntimePlatform.Android:
				platform = Platform.Android;
				break;
			case RuntimePlatform.IPhonePlayer:
				platform = Platform.iOS;
				break;
			case RuntimePlatform.Switch:
				platform = Platform.Switch;
				break;
			case RuntimePlatform.WebGLPlayer:
				platform = Platform.WebGL;
				break;
			default:
				platform = Platform.Windows;
				break;
			}
			StartupLog($"Setting platform to {ADOBase.platform = platform}.");
		}
		static void InitializeSteamIntegration()
		{
			if (!ADOBase.appIsInSteamLibrary)
			{
				foreach (DLCManager dLCManager in DLCManager.DLCManagers)
				{
					dLCManager.own = true;
				}
			}
			string steamBranchName = default(string);
			if (SteamIntegration.initialized && SteamApps.GetCurrentBetaName(ref steamBranchName, 20))
			{
				GCS.steamBranchName = steamBranchName;
			}
		}
		static void InitializeSteamSDK()
		{
			SteamIntegration.Setup();
		}
		static void LoadCalibration()
		{
			scrConductor.defaultPresets = CalibrationPreset.LoadDefaults();
			scrConductor.UpdateCurrentAudioOutput();
		}
		static void LoadSaveData()
		{
			Persistence.Load();
		}
		static void SetBuildDateAndCommit()
		{
			GCNS.buildDate = Resources.Load<TextAsset>("buildDate")?.text;
			GCNS.buildCommit = Resources.Load<TextAsset>("buildCommit")?.text;
			StartupLog($"Version r{141}, ({GCNS.buildCommit}, {GCNS.buildDate})");
		}
		static void SetLocale()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
		}
		static void SetSettings()
		{
			DOTween.SetTweensCapacity(500, 50);
			GCS.d_vibrate = Persistence.vibration;
			scrController.volume = Persistence.globalVolume;
			RDC.forceUnlockAllLevels = Persistence.unlockAllLevels;
			Application.runInBackground = true;
			RDUtils.targetFrameRate = Persistence.targetFrameRate;
			QualitySettings.vSyncCount = Persistence.vSync;
			RDC.forceNoSteamworks = false;
			GCS.hitMarginLimit = Persistence.hitMarginLimit;
			scrController.showDetailedResults = Persistence.showDetailedResults;
		}
		static void SetupLoader()
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(RDConstants.data.prefab_loader));
		}
		static void SetupSfxHandler()
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(RDConstants.data.prefab_sfxHandler));
		}
	}

	public static void SetupDLCs()
	{
		StartupLog($"DLCManagers count: {DLCManager.DLCManagers.Count}");
		foreach (DLCManager dLCManager in DLCManager.DLCManagers)
		{
			dLCManager.CheckInstalled();
		}
		StartupLog("DLC: CheckUpToDate");
		foreach (DLCManager dLCManager2 in DLCManager.DLCManagers)
		{
			dLCManager2.CheckUpToDate();
		}
		StartupLog($"Neo Cosmos: owns {NeoCosmosManager.instance.own}, installed {NeoCosmosManager.instance.installed}");
		if (GCS.isDev)
		{
			StartupLog($"Vega DLC: owns {VegaDLCManager.instance.own}, installed {VegaDLCManager.instance.installed}");
		}
	}

	public static void CacheShaderPropertyIDs()
	{
		scrFloor.ShaderProperty_Color = Shader.PropertyToID("_Color");
		scrFloor.ShaderProperty_Alpha = Shader.PropertyToID("_Alpha");
		FloorRenderer.ShaderProperty_Flash = Shader.PropertyToID("_Flash");
		scrDecorationManager.ShaderProperty_RepeatX = Shader.PropertyToID("RepeatX");
		scrDecorationManager.ShaderProperty_RepeatY = Shader.PropertyToID("RepeatY");
		scrDecorationManager.tileShader = Shader.Find("Sprites/Tile");
		scrDecorationManager.visualDecoShader = Shader.Find("Hidden/BlendModes/VisualDeco/Grab");
		scrDecorationManager.ShaderProperty_MainTex = Shader.PropertyToID("_MainTex");
		scrDecorationManager.ShaderProperty_Color = scrFloor.ShaderProperty_Color;
		scrDecorationManager.ShaderProperty_Opacity = Shader.PropertyToID("_Opacity");
		scrDecorationManager.ShaderProperty_Tile = Shader.PropertyToID("_Tile");
	}

	public static void SetupLevelEventsInfo()
	{
		Dictionary<string, object> obj = Json.Deserialize(Resources.Load<TextAsset>("LevelEditorProperties").text) as Dictionary<string, object>;
		GCS.levelEventsInfo = DecodeLevelEventInfoList(obj["levelEvents"] as List<object>);
		GCS.settingsInfo = DecodeLevelEventInfoList(obj["settings"] as List<object>);
		DecodeLevelEventCategoryList(obj["categories"] as List<object>);
		LevelEventType[] obj2 = (LevelEventType[])Enum.GetValues(typeof(LevelEventType));
		GCS.levelEventTypeString = new Dictionary<LevelEventType, string>();
		LevelEventType[] array = obj2;
		for (int i = 0; i < array.Length; i++)
		{
			LevelEventType key = array[i];
			GCS.levelEventTypeString.Add(key, key.ToString());
		}
	}

	public static Dictionary<string, LevelEventInfo> DecodeLevelEventInfoList(List<object> eventInfoList)
	{
		Dictionary<string, LevelEventInfo> dictionary = new Dictionary<string, LevelEventInfo>();
		foreach (Dictionary<string, object> eventInfo in eventInfoList)
		{
			if (eventInfo.TryGetValue("enabled", out var value) && !(bool)value)
			{
				continue;
			}
			LevelEventInfo levelEventInfo = new LevelEventInfo();
			levelEventInfo.name = eventInfo["name"] as string;
			levelEventInfo.stretchViewport = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)eventInfo, "stretchViewport", (object)false);
			levelEventInfo.type = RDUtils.ParseEnum(levelEventInfo.name, LevelEventType.None);
			levelEventInfo.pro = eventInfo.TryGetValue("pro", out var value2) && (bool)value2;
			levelEventInfo.taroDLC = eventInfo.TryGetValue("taroDLC", out var value3) && (bool)value3;
			levelEventInfo.allowFirstFloor = (eventInfo.TryGetValue("allowFirstFloor", out var value4) ? new bool?((bool)value4) : ((bool?)null));
			levelEventInfo.isDecoration = eventInfo.TryGetValue("isDecoration", out value4) && (bool)value4;
			if (eventInfo.TryGetValue("groups", out var value5) && value5 is List<object> { Count: >0 } list)
			{
				levelEventInfo.useGroups = true;
				levelEventInfo.groups.Clear();
				foreach (Dictionary<string, object> item in list)
				{
					levelEventInfo.groups.Add(new LevelEventInfo.Group
					{
						name = (string)item["name"],
						icon = (string)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)item, "icon", (object)string.Format("{0}/{1}", levelEventInfo.name, item["name"])),
						isDefault = (bool)CollectionExtensions.GetValueOrDefault<string, object>((IReadOnlyDictionary<string, object>)item, "isDefault", (object)false)
					});
				}
			}
			levelEventInfo.categories = new List<LevelEventCategory>();
			levelEventInfo.executionTime = RDUtils.ParseEnum(eventInfo["executionTime"] as string, LevelEventExecutionTime.OnBar);
			levelEventInfo.propertiesInfo = new Dictionary<string, PropertyInfo>();
			List<object> obj = eventInfo["properties"] as List<object>;
			int order = 0;
			foreach (Dictionary<string, object> item2 in obj)
			{
				if (!item2.ContainsKey("enabled") || (bool)item2["enabled"])
				{
					PropertyInfo propertyInfo = new PropertyInfo(item2, levelEventInfo);
					propertyInfo.order = order;
					levelEventInfo.propertiesInfo.Add(propertyInfo.name, propertyInfo);
				}
			}
			dictionary.Add(levelEventInfo.name, levelEventInfo);
		}
		return dictionary;
	}

	public static void DecodeLevelEventCategoryList(List<object> categoryInfoList)
	{
		foreach (Dictionary<string, object> categoryInfo in categoryInfoList)
		{
			List<object> list = categoryInfo["events"] as List<object>;
			LevelEventCategory item = RDUtils.ParseEnum(categoryInfo["name"] as string, LevelEventCategory.Gameplay);
			foreach (string item2 in list)
			{
				if (GCS.levelEventsInfo.ContainsKey(item2))
				{
					GCS.levelEventsInfo[item2].categories.Add(item);
				}
			}
		}
	}

	public static void LogMessageReceived(string logString, string stackTrace, LogType type)
	{
		if (!Application.isPlaying || (type != LogType.Error && type != LogType.Exception && type != LogType.Assert))
		{
			return;
		}
		string text = $"{type}:\n{logString}\n{stackTrace}";
		string[] array = new string[5] { "GLSL link error", "No cloud project ID was found by the Analytics SDK.", "The system is running out of memory.", "AABB", "IsFinite" };
		foreach (string value in array)
		{
			if (text.Contains(value))
			{
				return;
			}
		}
		if (errorCanvas == null)
		{
			errorCanvas = UnityEngine.Object.Instantiate(RDConstants.data.prefab_errorCanvas).GetComponent<ErrorCanvas>();
		}
		errorCanvas.ShowError(text);
	}

	public static void ModWasAdded(string modName)
	{
		if (!string.IsNullOrEmpty(modName))
		{
			addedMods.Add(modName);
		}
		Debug.Log("Mod was added: " + modName);
	}

	public static void DetermineAppLocation()
	{
		bool appIsInSteamLibrary = false;
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath)?.Parent;
		if (ADOBase.platform == Platform.Mac)
		{
			directoryInfo = directoryInfo?.Parent;
		}
		if (directoryInfo != null && directoryInfo.Parent?.Name == "common" && directoryInfo.Parent?.Parent?.Name == "steamapps")
		{
			appIsInSteamLibrary = true;
		}
		StartupLog("isInSteamLibrary: " + appIsInSteamLibrary);
		ADOBase.appIsInSteamLibrary = appIsInSteamLibrary;
	}

	public static void StartupLog(object log)
	{
		Debug.Log($"Startup: {log}");
	}

	public static void StartupLogError(object log)
	{
		Debug.LogError($"Startup Error: {log}");
	}

	public static void SetupSystems()
	{
		RDString.Setup();
	}

	private static void DeleteTempUnzippingFolder()
	{
		string tempLevelsFolder = scnCLS.tempLevelsFolder;
		if (!Directory.Exists(tempLevelsFolder))
		{
			return;
		}
		Utils.SetDirectoryAttributes(tempLevelsFolder);
		Task.Factory.StartNew((Func<Task>)async delegate
		{
			if (!(await Utils.TryDeleteDirectory(tempLevelsFolder)).IsNullOrEmpty())
			{
				Debug.LogWarning("Error deleting temps folder");
			}
		});
	}
}
