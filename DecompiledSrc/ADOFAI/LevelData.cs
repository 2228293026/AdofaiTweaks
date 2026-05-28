using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ADOFAI.Serialization;
using DG.Tweening;
using GDMiniJSON;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ADOFAI;

[Serializable]
public class LevelData
{
	private string _hash;

	public string pathData;

	public List<float> angleData;

	public EventsArray<LevelEvent> levelEvents = new EventsArray<LevelEvent>();

	public DecorationsArray<LevelEvent> decorations = new DecorationsArray<LevelEvent>();

	[NonSerialized]
	public LevelEvent songSettings;

	[NonSerialized]
	public LevelEvent levelSettings;

	[NonSerialized]
	public LevelEvent trackSettings;

	[NonSerialized]
	public LevelEvent backgroundSettings;

	[NonSerialized]
	public LevelEvent cameraSettings;

	[NonSerialized]
	public LevelEvent miscSettings;

	[NonSerialized]
	public LevelEvent eventSettings;

	[NonSerialized]
	public LevelEvent decorationSettings;

	public int version;

	public bool legacyFlash;

	public bool legacyCamRelativeTo;

	public bool isOldLevel;

	public bool oldCameraFollowStyle;

	public bool legacyTween;

	public bool disableV15Features;

	public bool legacyPause;

	public static bool shouldTryMigrate;

	private DLCManager[] _requiredDLC;

	public string Hash
	{
		get
		{
			if (artist.IsNullOrEmpty() && song.IsNullOrEmpty() && author.IsNullOrEmpty())
			{
				Debug.LogError("Hash needs data to generate hash");
				return null;
			}
			if (string.IsNullOrEmpty(_hash))
			{
				_hash = MD5Hash.GetHash(author + artist + song);
			}
			return _hash;
		}
	}

	public LevelEvent[] settings => new LevelEvent[8] { songSettings, levelSettings, trackSettings, backgroundSettings, cameraSettings, miscSettings, eventSettings, decorationSettings };

	public DLCManager[] requiredDLC => _requiredDLC;

	public string fullCaption => RDUtils.RemoveRichTags(fullCaptionTagged);

	public string fullCaptionTagged
	{
		get
		{
			if (song.IsNullOrEmpty())
			{
				return "";
			}
			if (artist.IsNullOrEmpty())
			{
				return song;
			}
			string text = artist.Trim();
			if (text.EndsWith(")"))
			{
				int num = text.IndexOf("(");
				if (num > 0)
				{
					text = text.Substring(0, num).Trim();
				}
			}
			return text + " - " + song;
		}
	}

	public string artist
	{
		get
		{
			return (string)levelSettings["artist"];
		}
		set
		{
			levelSettings["artist"] = value;
		}
	}

	public SpecialArtistType specialArtistType => (SpecialArtistType)levelSettings["specialArtistType"];

	public string song => (string)levelSettings["song"];

	public string author => (string)levelSettings["author"];

	public string previewImage
	{
		get
		{
			return (string)levelSettings["previewImage"];
		}
		set
		{
			levelSettings["previewImage"] = value;
		}
	}

	public string previewIcon
	{
		get
		{
			return levelSettings.GetString("previewIcon");
		}
		set
		{
			levelSettings["previewIcon"] = value;
		}
	}

	public Color previewIconColor => levelSettings.GetColor("previewIconColor");

	public int previewSongStart => (int)levelSettings["previewSongStart"];

	public int previewSongDuration => (int)levelSettings["previewSongDuration"];

	public bool seizureWarning => levelSettings.GetBool("seizureWarning");

	public string levelDesc => (string)levelSettings["levelDesc"];

	public string levelTags => (string)levelSettings["levelTags"];

	public string artistPermission
	{
		get
		{
			return (string)levelSettings["artistPermission"];
		}
		set
		{
			levelSettings["artistPermission"] = value;
		}
	}

	public string artistLinks => (string)levelSettings["artistLinks"];

	public int difficulty => levelSettings.GetInt("difficulty");

	public string songFilename
	{
		get
		{
			return (string)songSettings["songFilename"];
		}
		set
		{
			songSettings["songFilename"] = value;
		}
	}

	public float bpm => (float)songSettings["bpm"];

	public int volume => (int)songSettings["volume"];

	public int pitch => (int)songSettings["pitch"];

	public int offset => (int)songSettings["offset"];

	public HitSound hitsound => (HitSound)songSettings["hitsound"];

	public int hitsoundVolume => (int)songSettings["hitsoundVolume"];

	public bool separateCountdownTime => levelSettings.GetBool("separateCountdownTime");

	public int countdownTicks => (int)songSettings["countdownTicks"];

	public TrackColorType trackColorType => (TrackColorType)trackSettings["trackColorType"];

	public Color trackColor => trackSettings.GetColor("trackColor");

	public Color? trackShadowColor => trackSettings.GetNullableColor("trackShadowColor");

	public Color secondaryTrackColor => trackSettings.GetColor("secondaryTrackColor");

	public float trackColorAnimDuration => (float)trackSettings["trackColorAnimDuration"];

	public TrackColorPulse trackColorPulse => (TrackColorPulse)trackSettings["trackColorPulse"];

	public int trackPulseLength => (int)trackSettings["trackPulseLength"];

	public TrackStyle trackStyle => (TrackStyle)trackSettings["trackStyle"];

	public float trackTextureScale => (float)trackSettings["trackTextureScale"];

	public float trackGlowIntensity => (float)trackSettings["trackGlowIntensity"];

	public TileShape tileShape => (TileShape)trackSettings["tileShape"];

	public Texture2D trackTexture
	{
		get
		{
			scnGame instance = scnGame.instance;
			if (instance == null)
			{
				return null;
			}
			string text = (string)trackSettings["trackTexture"];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string filePath = Path.Combine(Path.GetDirectoryName(instance.levelPath), text);
			LoadResult status;
			Texture2D texture2D = instance.imgHolder.AddTexture(text, out status, filePath)?.GetTexture(TextureManager.ImageOptions.None);
			if (!texture2D)
			{
				return null;
			}
			texture2D.wrapMode = TextureWrapMode.Repeat;
			return texture2D;
		}
	}

	public TrackAnimationType trackAnimation => (TrackAnimationType)trackSettings["trackAnimation"];

	public TrackAnimationType2 trackDisappearAnimation => (TrackAnimationType2)trackSettings["trackDisappearAnimation"];

	public float trackBeatsAhead => (float)trackSettings["beatsAhead"];

	public float trackBeatsBehind => (float)trackSettings["beatsBehind"];

	public Color backgroundColor => backgroundSettings.GetColor("backgroundColor");

	public string bgImage => (string)backgroundSettings["bgImage"];

	public Color bgImageColor => backgroundSettings.GetColor("bgImageColor");

	public Vector2 bgParallax => (Vector2)backgroundSettings["parallax"] / 100f;

	public bool bgTiling => (BgDisplayMode)backgroundSettings["bgDisplayMode"] == BgDisplayMode.Tiled;

	public bool bgLooping => backgroundSettings.GetBool("loopBG");

	public bool bgFitScreen => (BgDisplayMode)backgroundSettings["bgDisplayMode"] != BgDisplayMode.Unscaled;

	public bool bgLockRot => backgroundSettings.GetBool("lockRot");

	public bool bgSmoothing => backgroundSettings.GetBool("imageSmoothing");

	public bool bgShowDefaultBGIfNoImage => backgroundSettings.GetBool("showDefaultBGIfNoImage");

	public bool bgShowDefaultBGTile => backgroundSettings.GetBool("showDefaultBGTile");

	public Color bgDefaultBGTileColor => backgroundSettings.GetColor("defaultBGTileColor");

	public bool showBGShape => (BGShapeType)backgroundSettings["defaultBGShapeType"] != BGShapeType.Disabled;

	public BGShapeType bgShapeType => (BGShapeType)backgroundSettings["defaultBGShapeType"];

	public Color bgDefaultBGShapeColor => backgroundSettings.GetColor("defaultBGShapeColor");

	public float scalingRatio => (float)(int)backgroundSettings["scalingRatio"] / 100f;

	public CamMovementType camRelativeTo => (CamMovementType)cameraSettings["relativeTo"];

	public Vector2 camPosition => (Vector2)cameraSettings["position"];

	public float camRotation => Convert.ToSingle(cameraSettings["rotation"].ToString());

	public float camZoom => (float)cameraSettings["zoom"];

	public bool pulseCamOnLandingFloor => cameraSettings.GetBool("pulseOnFloor");

	public bool camEnabledOnLowVFX
	{
		get
		{
			bool output;
			return cameraSettings.TryGet<bool>("startCamLowVFX", out output) && output;
		}
	}

	public string bgVideo => (string)miscSettings["bgVideo"];

	public bool floorIconOutlines => miscSettings.GetBool("floorIconOutlines");

	public bool stickToFloors => miscSettings.GetBool("stickToFloors");

	public Ease planetEase => (Ease)miscSettings["planetEase"];

	public int planetEaseParts => (int)miscSettings["planetEaseParts"];

	public EasePartBehavior planetEasePartBehavior => (EasePartBehavior)miscSettings["planetEasePartBehavior"];

	public Color defaultTextColor => miscSettings.GetColor("defaultTextColor");

	public Color defaultTextShadowColor => miscSettings.GetColor("defaultTextShadowColor");

	public static string GetCustomLevelName(string path)
	{
		string text = null;
		Dictionary<string, object> obj = (Json.Deserialize(RDFile.ReadAllText(path)) as Dictionary<string, object>)["settings"] as Dictionary<string, object>;
		string text2 = obj["song"] as string;
		string text3 = obj["artist"] as string;
		if (text2.IsNullOrEmpty())
		{
			text = "";
		}
		else if (text3.IsNullOrEmpty())
		{
			text = text2;
		}
		else
		{
			string text4 = text3.Trim();
			if (text4.EndsWith(")"))
			{
				int num = text4.IndexOf("(");
				if (num > 0)
				{
					text4 = text4.Substring(0, num).Trim();
				}
			}
			text = text4 + " - " + text2;
		}
		return RDUtils.RemoveRichTags(text);
	}

	public LevelData(bool setup = true)
	{
		if (setup)
		{
			Setup();
		}
	}

	public void Setup()
	{
		if (scnGame.instance != null)
		{
			isOldLevel = scnGame.instance.forceOldLevelStyle;
		}
		angleData = new List<float>(new float[10]);
		pathData = "RRRRRRRRRR";
		Dictionary<string, LevelEventInfo> settingsInfo = GCS.settingsInfo;
		songSettings = new LevelEvent(0, LevelEventType.SongSettings, settingsInfo["SongSettings"]);
		levelSettings = new LevelEvent(0, LevelEventType.LevelSettings, settingsInfo["LevelSettings"]);
		trackSettings = new LevelEvent(0, LevelEventType.TrackSettings, settingsInfo["TrackSettings"]);
		backgroundSettings = new LevelEvent(0, LevelEventType.BackgroundSettings, settingsInfo["BackgroundSettings"]);
		cameraSettings = new LevelEvent(0, LevelEventType.CameraSettings, settingsInfo["CameraSettings"]);
		miscSettings = new LevelEvent(0, LevelEventType.MiscSettings, settingsInfo["MiscSettings"]);
		eventSettings = new LevelEvent(0, LevelEventType.EventSettings, settingsInfo["EventSettings"]);
		decorationSettings = new LevelEvent(0, LevelEventType.DecorationSettings, settingsInfo["DecorationSettings"]);
	}

	public bool LoadLevel(string levelPath, out LoadResult status)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		status = LoadResult.Error;
		string text5;
		if (ADOBase.isInternalLevel)
		{
			string[] source = levelPath.Split('-', StringSplitOptions.None);
			string text = source.First();
			GCNS.WorldData worldData = GCNS.worldData[text];
			_ = worldData.index;
			string text2 = source.Last();
			int levelCount = worldData.levelCount;
			bool num = text2 == "X";
			int num2 = (num ? levelCount : int.Parse(text2)) - 1;
			string text3 = (num ? "main" : ("sub" + (num2 + 1)));
			string text4 = "InternalLevels/" + text + "/" + text3;
			if (worldData.isDLC)
			{
				AsyncOperationHandle<TextAsset> val = Addressables.LoadAssetAsync<TextAsset>((object)text4);
				text5 = val.WaitForCompletion().text;
				Addressables.Release<TextAsset>(val);
			}
			else
			{
				text5 = Resources.Load<TextAsset>(text4).text;
			}
		}
		else if (ADOBase.isBundleLevel)
		{
			AsyncOperationHandle<TextAsset> val2 = Addressables.LoadAssetAsync<TextAsset>((object)levelPath);
			text5 = val2.WaitForCompletion().text;
			Addressables.Release<TextAsset>(val2);
		}
		else
		{
			text5 = RDFile.ReadAllText(levelPath);
		}
		if (Json.Deserialize(text5) is Dictionary<string, object> dict)
		{
			Decode(dict, out status);
			bool flag = status == LoadResult.Successful;
			if (flag)
			{
				shouldTryMigrate = true;
				if (version == 7)
				{
					text5 = text5.Replace("\"enabled\": true", "\"active\": true").Replace("\"enabled\": false", "\"active\": false");
					if (!(Json.Deserialize(text5) is Dictionary<string, object> dict2))
					{
						return false;
					}
					Decode(dict2, out status);
				}
			}
			return flag;
		}
		return false;
	}

	public string Encode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		return JsonSerializer.Serialize<Dictionary<string, object>>(EncodeToDictionary(), new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			DefaultIgnoreCondition = (JsonIgnoreCondition)3,
			Converters = { (JsonConverter)(object)new LevelArrayConverter() },
			WriteIndented = true
		});
	}

	public Dictionary<string, object> EncodeToDictionary()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (isOldLevel)
		{
			dictionary.Add("pathData", pathData);
		}
		else
		{
			dictionary.Add("angleData", angleData.ToArray());
		}
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		dictionary2.Add("version", 18);
		LevelEvent[] array = new LevelEvent[6] { levelSettings, songSettings, trackSettings, backgroundSettings, cameraSettings, miscSettings };
		string text = default(string);
		object obj = default(object);
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<string, object> item in array[i].Encode(settings: true))
			{
				item.Deconstruct(ref text, ref obj);
				string key = text;
				object value = obj;
				dictionary2[key] = value;
			}
		}
		dictionary2.Add("legacyFlash", legacyFlash);
		dictionary2.Add("legacyCamRelativeTo", legacyCamRelativeTo);
		dictionary2.Add("legacySpriteTiles", isOldLevel);
		dictionary2.Add("legacyTween", legacyTween);
		dictionary2.Add("disableV15Features", disableV15Features);
		dictionary2.Add("legacyPause", legacyPause);
		dictionary.Add("settings", dictionary2);
		List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
		foreach (LevelEvent item2 in levelEvents.OrderBy((LevelEvent x) => x.floor).ToList())
		{
			if (item2.info.isActive)
			{
				list.Add(item2.Encode());
			}
		}
		dictionary.Add("actions", list);
		List<Dictionary<string, object>> list2 = new List<Dictionary<string, object>>();
		foreach (LevelEvent decoration in decorations)
		{
			if (decoration.info.isActive)
			{
				list2.Add(decoration.Encode());
			}
		}
		dictionary.Add("decorations", list2);
		return dictionary;
	}

	public void Decode(Dictionary<string, object> dict, out LoadResult status)
	{
		status = LoadResult.Error;
		Dictionary<string, object> dictionary = dict["settings"] as Dictionary<string, object>;
		string key = "version";
		version = ((!dictionary.ContainsKey(key)) ? 1 : Convert.ToInt32(dictionary[key]));
		if (version > 18)
		{
			status = LoadResult.FutureVersion;
			return;
		}
		if (version < 4)
		{
			legacyFlash = true;
		}
		else
		{
			string key2 = "legacyFlash";
			legacyFlash = dictionary.ContainsKey(key2) && RDEditorUtils.DecodeBool(dictionary[key2]);
		}
		if (version < 5)
		{
			isOldLevel = true;
		}
		else
		{
			isOldLevel = dictionary.ContainsKey("legacySpriteTiles") && RDEditorUtils.DecodeBool(dictionary["legacySpriteTiles"]);
		}
		if (version < 11)
		{
			legacyCamRelativeTo = true;
		}
		else
		{
			string key3 = "legacyCamRelativeTo";
			legacyCamRelativeTo = dictionary.ContainsKey(key3) && RDEditorUtils.DecodeBool(dictionary[key3]);
		}
		levelSettings.Decode(dictionary, "LevelSettings", isGlobal: true);
		songSettings.Decode(dictionary, "SongSettings", isGlobal: true);
		trackSettings.Decode(dictionary, "TrackSettings", isGlobal: true);
		backgroundSettings.Decode(dictionary, "BackgroundSettings", isGlobal: true);
		cameraSettings.Decode(dictionary, "CameraSettings", isGlobal: true);
		miscSettings.Decode(dictionary, "MiscSettings", isGlobal: true);
		eventSettings.Decode(dictionary, "EventSettings", isGlobal: true);
		decorationSettings.Decode(dictionary, "DecorationSettings", isGlobal: true);
		if (Application.isPlaying && RDEditorUtils.CheckModsDependency(levelSettings["requiredMods"] as object[]))
		{
			status = LoadResult.ModRequired;
			return;
		}
		if (dict.ContainsKey("pathData"))
		{
			if (isOldLevel)
			{
				pathData = RDEditorUtils.DecodeString(dict["pathData"]);
				angleData = new List<float>();
			}
			else
			{
				angleData = new List<float>(scrLevelMaker.StringToAngleArray(dict["pathData"] as string));
				pathData = "";
			}
		}
		else if (!Application.isPlaying || (scnGame.instance != null && !scnGame.instance.forceOldLevelStyle))
		{
			angleData = new List<float>(RDEditorUtils.DecodeFloatArray(dict["angleData"]));
			pathData = "";
			isOldLevel = false;
		}
		int num = Math.Max(pathData.Length, angleData.Count);
		levelEvents = new EventsArray<LevelEvent>();
		decorations = new DecorationsArray<LevelEvent>();
		List<object> obj = dict["actions"] as List<object>;
		List<object> list = new List<object>();
		if (dict.TryGetValue("decorations", out var value))
		{
			list = value as List<object>;
		}
		List<LevelEvent> list2 = new List<LevelEvent>();
		foreach (object item in obj)
		{
			LevelEvent levelEvent = new LevelEvent((item as Dictionary<string, object>) ?? new Dictionary<string, object>());
			if (Application.isPlaying && !levelEvent.info.taroDLCCheck)
			{
				status = LoadResult.TaroDLCRequired;
				return;
			}
			if (levelEvent.info.isActive && levelEvent.floor <= num && levelEvent.floor >= 0)
			{
				if (levelEvent.IsDecoration)
				{
					decorations.Add(levelEvent);
				}
				else
				{
					levelEvents.Add(levelEvent);
				}
			}
		}
		int num2 = version;
		if (num2 >= 9 && num2 <= 16)
		{
			List<MinimizedFloorData> list3 = new List<MinimizedFloorData>
			{
				new MinimizedFloorData(4.71238899230957, 0.0, ccw: false, midspin: false, turnaround: false, 2, -1)
			};
			HashSet<int> hashSet = new HashSet<int>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
			foreach (LevelEvent levelEvent5 in levelEvents)
			{
				int num3 = levelEvent5.floor - 1;
				switch (levelEvent5.eventType)
				{
				case LevelEventType.Twirl:
					if (!hashSet.Add(num3))
					{
						hashSet.Remove(num3);
					}
					break;
				case LevelEventType.MultiPlanet:
				{
					int? nullable = levelEvent5.GetNullable<int>("planets");
					if (nullable.HasValue)
					{
						int valueOrDefault = nullable.GetValueOrDefault();
						dictionary2[num3] = valueOrDefault;
					}
					break;
				}
				case LevelEventType.Hold:
				{
					int num4 = (int)levelEvent5["duration"];
					dictionary3[num3] = ((num > num3) ? num4 : (-1));
					break;
				}
				}
			}
			MinimizedFloorData minimizedFloorData = list3[0];
			bool flag = false;
			int numPlanets = 2;
			int holdLength = -1;
			for (int i = 0; i < num; i++)
			{
				double num5 = 0.0;
				double num6 = (isOldLevel ? scrLevelMaker.StringToAngle(pathData[i]) : ((double)angleData[i]));
				num5 = (minimizedFloorData.exitangle = ((num6 == 999.0) ? ((double)(float)minimizedFloorData.entryangle) : ((0.0 - num6 + 90.0) * 0.01745329238474369)));
				if (hashSet.Contains(i))
				{
					flag = !flag;
				}
				if (dictionary2.TryGetValue(i, out var value2))
				{
					numPlanets = value2;
				}
				if (dictionary3.TryGetValue(i, out var value3))
				{
					holdLength = value3;
				}
				MinimizedFloorData minimizedFloorData2 = new MinimizedFloorData((num5 + 3.1415927410125732) % 6.2831854820251465, 0.0, flag, midspin: false, turnaround: false, numPlanets, holdLength);
				if (num6 == 999.0)
				{
					minimizedFloorData.midspin = true;
				}
				list3.Add(minimizedFloorData2);
				minimizedFloorData = minimizedFloorData2;
			}
			for (int j = 0; j < list3.Count; j++)
			{
				MinimizedFloorData minimizedFloorData3 = list3[j];
				MinimizedFloorData minimizedFloorData4 = list3[Math.Max(0, j - 1)];
				double num7 = scrMisc.GetInverseAnglePerBeatMultiplanet(minimizedFloorData3.numPlanets) * (double)((!minimizedFloorData3.ccw) ? 1 : (-1));
				if (minimizedFloorData3.midspin)
				{
					num7 = 0.0;
				}
				if (minimizedFloorData4.midspin && minimizedFloorData3.numPlanets > 2)
				{
					num7 -= (6.2831854820251465 + scrMisc.GetInverseAnglePerBeatMultiplanet(minimizedFloorData3.numPlanets)) * (double)((!minimizedFloorData3.ccw) ? 1 : (-1));
				}
				double num8 = scrMisc.GetAngleMoved(minimizedFloorData3.entryangle + num7, minimizedFloorData3.exitangle + (minimizedFloorData3.midspin ? num7 : 0.0), !minimizedFloorData3.ccw);
				double num9 = Math.Abs(num8);
				if (num9 <= 1E-06 || num9 >= 6.283184482025146)
				{
					if (minimizedFloorData3.midspin)
					{
						num8 = 0.0;
					}
					else
					{
						num8 = 6.2831854820251465;
						minimizedFloorData3.turnaround = true;
					}
				}
				else
				{
					minimizedFloorData3.turnaround = false;
				}
				if (minimizedFloorData3.holdLength > 0)
				{
					num8 += (double)((float)(minimizedFloorData3.holdLength * 2) * (float)Math.PI);
				}
				minimizedFloorData3.angleLength = num8;
			}
			foreach (LevelEvent levelEvent2 in levelEvents)
			{
				switch (levelEvent2.eventType)
				{
				case LevelEventType.Pause:
				{
					float? nullable2 = levelEvent2.GetNullable<float>("duration");
					if (!nullable2.HasValue)
					{
						break;
					}
					float valueOrDefault3 = nullable2.GetValueOrDefault();
					MinimizedFloorData minimizedFloorData5 = list3[levelEvent2.floor];
					if (version == 9)
					{
						if (list3[levelEvent2.floor].turnaround)
						{
							levelEvent2["duration"] = valueOrDefault3 + 1f;
						}
						break;
					}
					bool flag2 = Math.Abs(scrMisc.GetAngleMoved(minimizedFloorData5.entryangle, minimizedFloorData5.exitangle, !minimizedFloorData5.ccw) - 6.2831854820251465) < 0.0001;
					if (list3[levelEvent2.floor - 1].midspin && minimizedFloorData5.turnaround && !flag2)
					{
						levelEvent2["duration"] = Math.Max(0f, valueOrDefault3 - 1f);
					}
					break;
				}
				case LevelEventType.FreeRoam:
				{
					object obj2 = levelEvent2.Get<object>("duration");
					if (obj2 == null)
					{
						break;
					}
					_ = list3[levelEvent2.floor];
					float num11;
					if (obj2 is int num10)
					{
						num11 = num10;
					}
					else
					{
						if (!(obj2 is float num12))
						{
							throw new Exception("Invalid freeroam duration type");
						}
						num11 = num12;
					}
					float num13 = num11 % 1f;
					if (num13 != 0f)
					{
						LevelEvent levelEvent3 = levelEvents.FirstOrDefault((LevelEvent e) => e.floor == levelEvent2.floor && e.eventType == LevelEventType.Pause);
						if (levelEvent3 != null)
						{
							float? nullable2 = levelEvent3.GetNullable<float>("duration");
							if (nullable2.HasValue)
							{
								float valueOrDefault2 = nullable2.GetValueOrDefault();
								levelEvent3["duration"] = valueOrDefault2 + num13;
							}
						}
						else
						{
							list2.Add(new LevelEvent(levelEvent2.floor, LevelEventType.Pause, null, new Dictionary<string, object>
							{
								["duration"] = num13,
								["countdownTicks"] = levelEvent2["countdownTicks"],
								["angleCorrectionDir"] = levelEvent2["angleCorrectionDir"]
							}, new Dictionary<string, bool>(), levelEvent2.active, visible: true, locked: false));
						}
					}
					levelEvent2["duration"] = Math.Max(0, Mathf.RoundToInt(num11 - num13));
					break;
				}
				}
			}
		}
		levelEvents.AddRange(list2);
		foreach (object item2 in list)
		{
			LevelEvent levelEvent4 = new LevelEvent(item2 as Dictionary<string, object>);
			if (levelEvent4.info.isActive)
			{
				decorations.Add(levelEvent4);
			}
		}
		if (version < 2)
		{
			songSettings["pitch"] = (int)songSettings.info.propertiesInfo["pitch"].value_default;
		}
		if (version < 4)
		{
			legacyFlash = true;
		}
		if (version < 11)
		{
			legacyCamRelativeTo = true;
		}
		if (version < 14)
		{
			legacyTween = true;
		}
		else
		{
			legacyTween = dictionary.TryGetValue("legacyTween", out var value4) && RDEditorUtils.DecodeBool(value4);
		}
		if (version < 15)
		{
			disableV15Features = true;
		}
		else
		{
			disableV15Features = dictionary.TryGetValue("disableV15Features", out var value5) && RDEditorUtils.DecodeBool(value5);
		}
		if (version < 18)
		{
			legacyPause = true;
		}
		else
		{
			legacyPause = dictionary.TryGetValue("legacyPause", out var value6) && RDEditorUtils.DecodeBool(value6);
		}
		status = LoadResult.Successful;
	}

	public LevelData Copy()
	{
		LevelData levelData = new LevelData(setup: false);
		levelData.pathData = pathData;
		levelData.angleData = new List<float>(angleData);
		levelData.isOldLevel = isOldLevel;
		levelData.legacyFlash = legacyFlash;
		levelData.legacyCamRelativeTo = legacyCamRelativeTo;
		levelData.legacyTween = legacyTween;
		levelData.levelEvents = new EventsArray<LevelEvent>();
		levelData.version = version;
		foreach (LevelEvent levelEvent in levelEvents)
		{
			levelData.levelEvents.Add(levelEvent.Copy());
		}
		foreach (LevelEvent decoration in decorations)
		{
			levelData.decorations.Add(decoration.Copy());
		}
		levelData.songSettings = songSettings.Copy();
		levelData.levelSettings = levelSettings.Copy();
		levelData.trackSettings = trackSettings.Copy();
		levelData.backgroundSettings = backgroundSettings.Copy();
		levelData.cameraSettings = cameraSettings.Copy();
		levelData.miscSettings = miscSettings.Copy();
		levelData.eventSettings = eventSettings.Copy();
		levelData.decorationSettings = decorationSettings.Copy();
		return levelData;
	}

	public void RefreshRequiredDLC()
	{
		List<DLCManager> list = new List<DLCManager>();
		bool flag = false;
		foreach (LevelEvent levelEvent in levelEvents)
		{
			if (levelEvent.info.taroDLC)
			{
				flag = true;
			}
		}
		if (flag)
		{
			list.Add(NeoCosmosManager.instance);
		}
		_requiredDLC = list.ToArray();
	}

	public List<string> GetMissingParams()
	{
		List<string> list = new List<string>();
		if (artist.Trim() == "")
		{
			list.Add("editor.artist");
		}
		if (song == "")
		{
			list.Add("editor.song");
		}
		if (author == "")
		{
			list.Add("editor.author");
		}
		if (previewImage == "")
		{
			list.Add("editor.previewImage");
		}
		return list;
	}
}
