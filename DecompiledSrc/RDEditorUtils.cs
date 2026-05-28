using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using ADOFAI;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityFileDialog;

public static class RDEditorUtils
{
	public static string Indentation = "\t";

	private const string DoubleQuote = "\"";

	private static readonly string[] logDirectories = new string[2]
	{
		Persistence.DataPath,
		Application.dataPath
	};

	private static readonly string[] logFileNames = new string[2] { "output_log.txt", "Player.log" };

	private static string lastLogDirectory = null;

	private const float ScreenEdgeRange = 10f;

	private static string[] macDirectories => new string[1] { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/Library/Logs/7th Beat Games/A Dance of Fire and Ice/" };

	public static bool MouseOnScreenEdge
	{
		get
		{
			Vector3 mousePosition = Input.mousePosition;
			bool flag = mousePosition.x < 10f;
			bool flag2 = mousePosition.x > (float)Screen.width - 10f;
			bool flag3 = mousePosition.y > (float)Screen.height - 10f;
			bool flag4 = mousePosition.y < 10f;
			return flag || flag2 || flag3 || flag4;
		}
	}

	public static bool IsValidHexColor(this string s, bool hasAlpha)
	{
		int num = (hasAlpha ? 8 : 6);
		int result;
		if (s.Length == num)
		{
			return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}
		return false;
	}

	public static object[] EncodeVector2(Vector2 vecValue)
	{
		decimal? num = (float.IsNaN(vecValue.x) ? ((decimal?)null) : new decimal?((decimal)vecValue.x));
		decimal? num2 = (float.IsNaN(vecValue.y) ? ((decimal?)null) : new decimal?((decimal)vecValue.y));
		return new object[2] { num, num2 };
	}

	public static object[] EncodeTile(Tuple<int, TileRelativeTo> tupValue)
	{
		int item = tupValue.Item1;
		string text = tupValue.Item2.ToString();
		return new object[2] { item, text };
	}

	public static Dictionary<string, object> EncodeFilterProperties(Dictionary<string, object> dict, Dictionary<string, bool> disabled)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> item in dict)
		{
			if (item.Key.StartsWith("filter_") && !(disabled.TryGetValue(item.Key, out var value) && value))
			{
				if (item.Value is float num)
				{
					dictionary.Add(item.Key, num);
				}
				else if (item.Value is int num2)
				{
					dictionary.Add(item.Key, num2);
				}
				else if (item.Value is string value2)
				{
					dictionary.Add(item.Key, value2);
				}
				else if (item.Value is Vector2 vecValue)
				{
					dictionary.Add(item.Key, EncodeVector2(vecValue));
				}
			}
		}
		return dictionary;
	}

	public static bool DecodeBool(object dictValue)
	{
		return (bool)dictValue;
	}

	public static float DecodeFloat(object dictValue)
	{
		return Convert.ToSingle(dictValue);
	}

	public static int DecodeInt(object dictValue)
	{
		return Convert.ToInt32(dictValue);
	}

	public static string DecodeString(object dictValue)
	{
		return dictValue as string;
	}

	public static T DecodeEnum<T>(object dictValue) where T : struct
	{
		return RDUtils.ParseEnum<T>(dictValue as string);
	}

	public static T[] DecodeEnumArray<T>(object dictValue) where T : struct
	{
		if (!(dictValue is List<object> list))
		{
			return null;
		}
		T[] array = new T[list.Count];
		int num = 0;
		foreach (object item in list)
		{
			array[num] = RDUtils.ParseEnum<T>(item as string);
			num++;
		}
		return array;
	}

	public static int[] DecodeIntArray(object dictValue)
	{
		if (!(dictValue is List<object> list))
		{
			return null;
		}
		int[] array = new int[list.Count];
		int num = 0;
		foreach (object item in list)
		{
			array[num] = Convert.ToInt32(item);
			num++;
		}
		return array;
	}

	public static float[] DecodeFloatArray(object dictValue)
	{
		if (!(dictValue is List<object> list))
		{
			return null;
		}
		float[] array = new float[list.Count];
		int num = 0;
		foreach (object item in list)
		{
			array[num] = Convert.ToSingle(item);
			num++;
		}
		return array;
	}

	public static string[] DecodeStringArray(object dictValue)
	{
		if (!(dictValue is List<object> list))
		{
			return null;
		}
		string[] array = new string[list.Count];
		int num = 0;
		foreach (object item in list)
		{
			array[num] = item as string;
			num++;
		}
		return array;
	}

	public static object[] DecodeModsArray(object dictValue)
	{
		return ((List<object>)dictValue).ToArray();
	}

	public static string ShowFileSelectorForAudio(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.audioFileFormat"), GCS.SupportedAudioFiles, RDString.Get("editor.dialog.saveBeforeImportingSounds"), maximumSize);
	}

	public static string ShowFileSelectorForImage(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.imageFileFormat"), GCS.SupportedImageFiles, RDString.Get("editor.dialog.saveBeforeImportingImages"), maximumSize);
	}

	public static string ShowFileSelectorForVideo(string title, long maximumSize = -1L)
	{
		return ShowFileSelector(title, RDString.Get("editor.dialog.videoFileFormat"), GCS.SupportedVideoFiles, RDString.Get("editor.dialog.saveBeforeImportingVideos"), maximumSize);
	}

	public static string ShowFileSelector(string title, string extensionDescription, string[] extensions, string levelNotSavedMessage, long maximumSize = -1L)
	{
		scnGame instance = scnGame.instance;
		if (string.IsNullOrEmpty(instance.levelPath))
		{
			return null;
		}
		string text = FileBrowser.PickFile(Persistence.GetLastUsedFolder(), extensionDescription, extensions, title);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		string directoryName = Path.GetDirectoryName(text);
		string fileName = Path.GetFileName(text);
		string directoryName2 = Path.GetDirectoryName(instance.levelPath);
		string text2 = Path.Combine(directoryName2, fileName);
		long length = new FileInfo(text).Length;
		if (directoryName != directoryName2 && !RDFile.Exists(text2))
		{
			if (maximumSize != -1)
			{
				if (length >= maximumSize)
				{
					return maximumSize.ToString();
				}
				RDFile.Copy(text, text2);
			}
			else
			{
				RDFile.Copy(text, text2);
			}
		}
		return fileName;
	}

	public static bool HasAudioFileExtension(this string filename)
	{
		for (int i = 0; i < GCS.SupportedAudioFiles.Length; i++)
		{
			string value = "." + GCS.SupportedAudioFiles[i];
			if (filename.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasImageFileExtension(this string filename)
	{
		for (int i = 0; i < GCS.SupportedImageFiles.Length; i++)
		{
			string value = "." + GCS.SupportedImageFiles[i];
			if (filename.EndsWith(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static RDAudioLoadType FindClip(string filename)
	{
		if (!filename.HasAudioFileExtension())
		{
			string clipName = "";
			if (AudioManager.Instance.FindOrLoadAudioClip(clipName) != null)
			{
				return RDAudioLoadType.SuccessInternalClipLoaded;
			}
		}
		else if (RDFile.Exists(Path.GetDirectoryName(ADOBase.levelPath) + Path.DirectorySeparatorChar + filename))
		{
			return RDAudioLoadType.SuccessExternalClipLoaded;
		}
		UnityEngine.Debug.LogWarning("RDEditorUtils - Audio not found: " + filename);
		return RDAudioLoadType.ErrorFileNotFound;
	}

	public static string GetCurrentLevelFolderPath()
	{
		_ = scnEditor.instance;
		return Path.GetDirectoryName(ADOBase.levelPath);
	}

	public static IEnumerator AudioClipFromFilename(string filename)
	{
		bool num = filename.HasAudioFileExtension();
		bool flag = false;
		if (!num)
		{
			string clipName = "" + "/" + filename;
			AudioClip audioClip = AudioManager.Instance.FindOrLoadAudioClip(clipName);
			if (audioClip != null)
			{
				yield return new RDAudioLoadResult(RDAudioLoadType.SuccessInternalClipLoaded, audioClip);
				flag = true;
			}
		}
		else
		{
			string path = GetCurrentLevelFolderPath() + Path.DirectorySeparatorChar + filename;
			if (RDFile.Exists(path))
			{
				MonoBehaviour instance = scnEditor.instance;
				CoroutineWithData loadAudio = new CoroutineWithData(instance, AudioManager.Instance.FindOrLoadAudioClipExternal(path, mp3Streaming: false));
				yield return loadAudio.coroutine;
				yield return (RDAudioLoadResult)loadAudio.result;
				flag = true;
			}
		}
		if (!flag)
		{
			UnityEngine.Debug.LogWarning("AudioClip doesn't exist: " + filename);
			yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
		}
	}

	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	public static bool CheckForKeyCombo(bool control, bool shift, KeyCode key)
	{
		bool flag = ControlIsPressed();
		bool holdingShift = RDInput.holdingShift;
		bool keyDown = Input.GetKeyDown(key);
		if (control && shift)
		{
			return flag && holdingShift && keyDown;
		}
		if (control && !shift)
		{
			return flag && keyDown;
		}
		if (shift && !control)
		{
			return holdingShift && keyDown;
		}
		return false;
	}

	public static string KeyComboToString(bool control, bool shift, KeyCode keyCode, bool ctrlIsCmd = true)
	{
		return KeyComboToString(control, shift, alt: false, keyCode, ctrlIsCmd);
	}

	public static string KeyComboToString(bool control, bool shift, bool alt, KeyCode keyCode, bool ctrlIsCmd = true)
	{
		bool flag = ADOBase.platform == Platform.Mac && !Application.isEditor;
		string text = "";
		if (control)
		{
			text += ((flag && ctrlIsCmd) ? "cmd-" : "ctrl-");
		}
		if (shift)
		{
			text += "shift-";
		}
		if (alt)
		{
			text += (flag ? "opt-" : "alt-");
		}
		string text2;
		if (keyCode == (KeyCode)(-1))
		{
			text2 = RDString.Get("KeyCode.Scroll");
		}
		else
		{
			bool exists = false;
			text2 = RDString.GetWithCheck("KeyCode." + keyCode, out exists);
			if (!exists)
			{
				text2 = keyCode.ToString();
			}
		}
		return text + text2.ToLower();
	}

	public static bool ControlIsPressed()
	{
		if (ADOBase.platform != Platform.Mac || Application.isEditor)
		{
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				return Input.GetKey(KeyCode.RightControl);
			}
			return true;
		}
		if (!Input.GetKey(KeyCode.LeftMeta))
		{
			return Input.GetKey(KeyCode.RightMeta);
		}
		return true;
	}

	public static string CombinePaths(params string[] paths)
	{
		if (paths == null)
		{
			throw new ArgumentNullException("paths");
		}
		return paths.Aggregate(Path.Combine);
	}

	public static string LogPath()
	{
		return CombinePaths(Environment.GetEnvironmentVariable("AppData"), "..", "LocalLow", Application.companyName, Application.productName, "Player.log");
	}

	public static void RevealInExplorer(string path, bool selectInExplorer = false)
	{
		if (!string.IsNullOrEmpty(path))
		{
			RuntimePlatform platform = Application.platform;
			switch (platform)
			{
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				OpenInWinFileBrowser(path, selectInExplorer);
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				OpenInMacFileBrowser(path, selectInExplorer);
				break;
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				OpenInLinuxFileBrowser(path, selectInExplorer);
				break;
			default:
				UnityEngine.Debug.LogError("RevealInExplorer not implemented for " + platform.ToString() + " platform, path: " + path);
				break;
			}
		}
	}

	public static void OpenInLinuxFileBrowser(string path, bool selectInExplorer = false)
	{
		string text = path.Replace("\\", "/");
		if (!selectInExplorer && Directory.Exists(text))
		{
			Process.Start("xdg-open", "\"file://" + text + "\"");
			return;
		}
		string arguments = "--session --dest=org.freedesktop.FileManager1 --type=method_call /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems array:string:\"file://" + text + "\" string:\"\"";
		Process.Start("dbus-send", arguments);
	}

	public static void OpenInMacFileBrowser(string path, bool selectInExplorer = false)
	{
		bool flag = false;
		string text = path.Replace("\\", "/");
		if (!selectInExplorer && Directory.Exists(text))
		{
			flag = true;
		}
		if (!text.StartsWith("\""))
		{
			text = "\"" + text;
		}
		if (!text.EndsWith("\""))
		{
			text += "\"";
		}
		string arguments = (flag ? "" : "-R ") + text;
		try
		{
			Process.Start("open", arguments);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static void OpenInWinFileBrowser(string path, bool selectInExplorer = false)
	{
		bool flag = false;
		string text = path.Replace("/", "\\");
		if (!selectInExplorer && Directory.Exists(text))
		{
			flag = true;
		}
		try
		{
			Process.Start("explorer.exe", (flag ? "/root," : "/select,") + text);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static bool CheckModsDependency(object[] mods)
	{
		return mods != null && mods.Length != 0;
	}

	public static void OpenLogDirectory()
	{
		if (!string.IsNullOrEmpty(lastLogDirectory) && RDDirectory.Exists(lastLogDirectory))
		{
			RevealInExplorer(lastLogDirectory);
			return;
		}
		string text = null;
		string[] array = ((ADOBase.platform == Platform.Mac) ? macDirectories : logDirectories);
		foreach (string text2 in array)
		{
			UnityEngine.Debug.Log("trying to find dir: " + text2);
			string[] array2 = logFileNames;
			foreach (string path in array2)
			{
				if (RDFile.Exists(Path.Combine(text2, path)))
				{
					lastLogDirectory = text2;
					RevealInExplorer(text2);
					return;
				}
			}
			if (string.IsNullOrEmpty(text) && RDDirectory.Exists(text2))
			{
				text = text2;
			}
		}
		if (text != null)
		{
			lastLogDirectory = text;
			RevealInExplorer(text);
		}
	}

	public static bool CheckPlayerLogKeyCombo()
	{
		bool flag = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		bool flag2 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		if (CheckForKeyCombo(control: true, shift: false, KeyCode.L))
		{
			return flag || flag2;
		}
		return false;
	}

	public static bool CheckPointerInObject(GameObject obj)
	{
		RectTransform component = obj.GetComponent<RectTransform>();
		Rect rect = component.rect;
		Vector2 point = component.InverseTransformPoint(Input.mousePosition);
		return rect.Contains(point);
	}

	public static bool CheckPointerInObject(UnityEngine.Component obj)
	{
		return CheckPointerInObject(obj.gameObject);
	}

	public static GameObject[] ObjectsAtPointer()
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = Input.mousePosition;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		return list.Select((RaycastResult r) => r.gameObject).ToArray();
	}

	public static T GetComponentInAllParents<T>(GameObject gameObject) where T : UnityEngine.Component
	{
		T val = null;
		Transform transform = gameObject.transform;
		while (transform != null && val == null)
		{
			val = transform.GetComponent<T>();
			transform = transform.parent;
		}
		return val;
	}

	public static string FilterFieldToUnit(PropertyType type, string name)
	{
		if (name.ToLower().Contains("rotation"))
		{
			return "°";
		}
		if (type != PropertyType.Float)
		{
			return "";
		}
		return "%";
	}

	public static float UnitMultiplier(string unit)
	{
		if (unit == "%")
		{
			return 100f;
		}
		return 1f;
	}

	public static PropertyType TypeToPropertyType(Type type)
	{
		if (type == typeof(int))
		{
			return PropertyType.Int;
		}
		if (type == typeof(float))
		{
			return PropertyType.Float;
		}
		if (type == typeof(Color))
		{
			return PropertyType.Color;
		}
		if (type == typeof(Vector2))
		{
			return PropertyType.Vector2;
		}
		return PropertyType.String;
	}

	public static Dictionary<string, string> ParseQueryString(string query)
	{
		if (query.StartsWith("?"))
		{
			string text = query;
			query = text.Substring(1, text.Length - 1);
		}
		if (query.Length == 0)
		{
			return new Dictionary<string, string>();
		}
		return (from q in query.Split('&', StringSplitOptions.None)
			select q.Split('=', StringSplitOptions.None)).ToDictionary((string[] q) => q.FirstOrDefault(), (string[] q) => WebUtility.UrlDecode(q.Skip(1).FirstOrDefault()));
	}

	public static string BuildQueryString(Dictionary<string, string> query)
	{
		return string.Join("&", query.Select((KeyValuePair<string, string> q) => q.Key + "=" + WebUtility.UrlEncode(q.Value)));
	}

	public static IEnumerator ParseLevelURL(string url)
	{
		Uri uri = new Uri(url);
		string host = uri.Host;
		string absolutePath = uri.AbsolutePath;
		Dictionary<string, string> dictionary = ParseQueryString(uri.Query);
		if (host.EndsWith("youtube.com") && absolutePath.StartsWith("/redirect"))
		{
			url = dictionary["q"];
			Uri uri2 = new Uri(url);
			host = uri2.Host;
			absolutePath = uri2.AbsolutePath;
			dictionary = ParseQueryString(uri2.Query);
		}
		string text = absolutePath;
		string[] array = text.Substring(1, text.Length - 1).Split("/", StringSplitOptions.None);
		string value;
		ulong result;
		if (host == "drive.google.com")
		{
			string text2 = "";
			if (absolutePath.StartsWith("/file/d/"))
			{
				text2 = array[2];
			}
			else if (absolutePath.StartsWith("/open") || absolutePath.StartsWith("/uc"))
			{
				text2 = dictionary["id"];
			}
			url = "https://drive.google.com/uc?id=" + text2 + "&export=download&confirm=t";
		}
		else if (host.EndsWith("dropbox.com"))
		{
			dictionary["dl"] = "1";
			url = "https://www.dropbox.com" + absolutePath + "?" + BuildQueryString(dictionary);
		}
		else if (SteamIntegration.initialized && host == "steamcommunity.com" && absolutePath.StartsWith("/sharedfiles/filedetails") && dictionary.TryGetValue("id", out value) && ulong.TryParse(value, out result))
		{
			PublishedFileId_t fileId = new PublishedFileId_t(result);
			EItemState val = (EItemState)SteamUGC.GetItemState(fileId);
			if (!((Enum)val).HasFlag((Enum)(object)(EItemState)1))
			{
				SteamWorkshop.Subscribe(fileId);
			}
			yield return new WaitUntil(() => SteamWorkshop.ItemIsUsable(fileId));
			ulong num = default(ulong);
			string text3 = default(string);
			uint num2 = default(uint);
			if (!SteamUGC.GetItemInstallInfo(fileId, ref num, ref text3, 1024u, ref num2))
			{
				yield break;
			}
			url = "file://" + text3;
		}
		yield return url;
	}

	public static object[] EncodeFloatPair(Tuple<float, float> pair)
	{
		float? num = (float.IsNaN(pair.Item1) ? ((float?)null) : new float?(pair.Item1));
		float? num2 = (float.IsNaN(pair.Item2) ? ((float?)null) : new float?(pair.Item2));
		return new object[2] { num, num2 };
	}

	public static object[][] EncodeVector2Range(Tuple<Vector2, Vector2> pair)
	{
		var (vecValue, vecValue2) = pair;
		return new object[2][]
		{
			EncodeVector2(vecValue),
			EncodeVector2(vecValue2)
		};
	}

	public static string ToFeaturedDLCPath(this string path)
	{
		return Path.ChangeExtension(path, null).Replace("\\", "/");
	}
}
