using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GDMiniJSON;
using UnityEngine;

public class PlayerPrefsJson
{
	public enum FileLocation
	{
		Automatic,
		UserFolder,
		PersistentDataFolder
	}

	public enum FileType
	{
		General,
		CustomWorld
	}

	public static readonly Dictionary<FileType, PlayerPrefsJson> files = new Dictionary<FileType, PlayerPrefsJson>();

	private static readonly Dictionary<FileType, string> filesPath = new Dictionary<FileType, string>();

	public static readonly FileType[] LoadedFileTypes = files.Keys.ToArray();

	public static readonly FileType[] AllFileTypes = Enum.GetValues(typeof(FileType)) as FileType[];

	public static HashSet<string> nonSyncedKeys = new HashSet<string>();

	public static readonly Encoding DefaultLevelEncoding = Encoding.UTF8;

	public static readonly Dictionary<FileType, string> Filenames = new Dictionary<FileType, string>
	{
		{
			FileType.General,
			"data"
		},
		{
			FileType.CustomWorld,
			"custom_data"
		}
	};

	public readonly Dictionary<string, object> dict = new Dictionary<string, object>();

	private readonly FileType fileType;

	public static bool LoadFile(FileType fileType, bool loadBackup, out PlayerPrefsJson prefs)
	{
		prefs = null;
		string filePath = GetFilePath(fileType, loadBackup);
		if (RDFile.Exists(filePath))
		{
			string json = RDFile.ReadAllText(filePath, DefaultLevelEncoding);
			if (!ValidateJson(json))
			{
				return false;
			}
			Dictionary<string, object> dictionary = null;
			try
			{
				dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			}
			catch (Exception arg)
			{
				Debug.Log($"There was an error deserializing save file in path {filePath}.\nException: {arg}");
			}
			if (dictionary == null)
			{
				Debug.Log("Deserializing save in path " + filePath + " is null.");
				return false;
			}
			printe("JSON deserialized successfully. (" + filePath + ")");
			prefs = new PlayerPrefsJson(fileType, dictionary);
			return true;
		}
		return false;
	}

	private static bool ValidateJson(string json)
	{
		if (json.Length < 10)
		{
			Debug.LogWarning("PlayerPrefsJson: encoded JSON is way too short, seems corrupted: " + json);
			return false;
		}
		if (json[0] != '{' && json[1] != '{' && json[2] != '{')
		{
			Debug.LogWarning("PlayerPrefsJson: doesn't start with {");
			return false;
		}
		return true;
	}

	public static bool CreateFromBytes(byte[] bytes, out Dictionary<string, object> fileContent)
	{
		fileContent = null;
		if (bytes == null || bytes.Length == 0)
		{
			return false;
		}
		string json = DefaultLevelEncoding.GetString(bytes);
		if (!ValidateJson(json))
		{
			return false;
		}
		try
		{
			if (Json.Deserialize(json) is Dictionary<string, object> dictionary)
			{
				fileContent = dictionary;
				return true;
			}
			Debug.Log("Deserialized object is not a dictionary.");
		}
		catch (Exception arg)
		{
			Debug.Log($"There was an error deserializing file from bytes.\nException: {arg}");
		}
		return false;
	}

	public static string GetFilePath(FileType fileType, bool loadBackup = false, bool corruptedFile = false, FileLocation fileFolderLocation = FileLocation.Automatic)
	{
		string text = Filenames[fileType];
		string text2;
		if (!corruptedFile && filesPath.TryGetValue(fileType, out var value))
		{
			text2 = value;
		}
		else
		{
			if (fileFolderLocation == FileLocation.Automatic)
			{
				_ = ADOBase.appIsInSteamLibrary;
			}
			text2 = GetFileFolderPath() + Path.DirectorySeparatorChar + (corruptedFile ? $"corrupted_{text}_{DateTime.Now:yyyy'-'MM'-'dd'_'HH'-'mm'-'ss}" : text) + ".sav";
			if (!corruptedFile)
			{
				filesPath.Add(fileType, text2);
			}
		}
		if (loadBackup)
		{
			text2 += ".old";
		}
		return text2;
	}

	public static string GetFileFolderPath()
	{
		string text;
		if (ADOBase.appIsInSteamLibrary)
		{
			text = Application.dataPath;
			switch (ADOBase.platform)
			{
			case Platform.Linux:
			case Platform.Windows:
				text = Directory.GetParent(text).FullName;
				break;
			case Platform.Mac:
				text = Directory.GetParent(text).Parent.FullName;
				break;
			}
			text = text + Path.DirectorySeparatorChar + "User";
			if (!RDDirectory.Exists(text))
			{
				RDDirectory.CreateDirectory(text);
			}
		}
		else
		{
			text = Persistence.DataPath;
		}
		return text;
	}

	public static bool FileExists(FileType fileType, FileLocation location = FileLocation.Automatic)
	{
		return RDFile.Exists(GetFilePath(fileType, loadBackup: false, corruptedFile: false, location));
	}

	public static void AddFile(PlayerPrefsJson data)
	{
		if (!files.ContainsKey(data.fileType))
		{
			files.Add(data.fileType, data);
		}
	}

	public static PlayerPrefsJson CreateFile(FileType fileType)
	{
		PlayerPrefsJson playerPrefsJson = new PlayerPrefsJson(fileType);
		AddFile(playerPrefsJson);
		return playerPrefsJson;
	}

	public static bool LoadAllFiles()
	{
		bool result = true;
		FileType[] allFileTypes = AllFileTypes;
		foreach (FileType fileType in allFileTypes)
		{
			PlayerPrefsJson prefs2;
			if (LoadFile(fileType, loadBackup: false, out var prefs))
			{
				AddFile(prefs);
				prefs.SaveBackup();
			}
			else if (LoadFile(fileType, loadBackup: true, out prefs2))
			{
				MarkCorruptFile(fileType, isBackup: false);
				printe($"Main save file {fileType} seems corrupted, attempting to load backup.");
				AddFile(prefs2);
			}
			else
			{
				MarkCorruptFile(fileType, isBackup: false);
				MarkCorruptFile(fileType, isBackup: true);
				result = false;
				AddFile(CreateFile(fileType));
			}
		}
		return result;
	}

	public static void SaveAllFiles()
	{
		foreach (PlayerPrefsJson value in files.Values)
		{
			value.Save();
		}
	}

	public static void MarkCorruptFile(FileType fileType, bool isBackup)
	{
		string filePath = GetFilePath(fileType, isBackup);
		if (RDFile.Exists(filePath))
		{
			RDFile.Copy(filePath, GetFilePath(fileType, isBackup, corruptedFile: true));
			RDFile.Delete(filePath);
		}
	}

	public static PlayerPrefsJson Get(FileType fileType)
	{
		if (!files.TryGetValue(fileType, out var value))
		{
			return CreateFile(fileType);
		}
		return value;
	}

	public PlayerPrefsJson(FileType fileType, Dictionary<string, object> data = null)
	{
		this.fileType = fileType;
		dict = data ?? dict;
	}

	public void Save()
	{
		if (dict == null)
		{
			Debug.LogWarning("PlayerPrefsJson: 'dict' is null");
			return;
		}
		if (dict.Keys.Count == 0)
		{
			Debug.LogWarning("PlayerPrefsJson: 'dict' is empty");
			return;
		}
		string text = Json.Serialize(dict);
		if (text == null)
		{
			Debug.LogWarning("PlayerPrefsJson: encoded JSON is null");
		}
		else if (!ValidateJson(text))
		{
			Debug.LogWarning("PlayerPrefsJson: Json is not valid");
		}
		else
		{
			RDFile.WriteAllText(GetFilePath(fileType), text, DefaultLevelEncoding);
		}
	}

	public void SaveBackup()
	{
		RDFile.Copy(GetFilePath(fileType), GetFilePath(fileType, loadBackup: true), overwrite: true);
	}

	public string GetString(string key, string defaultValue = "")
	{
		return Get(key, defaultValue);
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		return Get(key, defaultValue);
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		return Get(key, defaultValue);
	}

	public float GetFloat(string key, float defaultValue = 0f)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return Convert.ToSingle(value);
	}

	public Dictionary<string, object> GetDict(string key)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return new Dictionary<string, object>();
		}
		return value as Dictionary<string, object>;
	}

	public List<object> GetList(string key)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return new List<object>();
		}
		return value as List<object>;
	}

	private T Get<T>(string key, T defaultValue)
	{
		if (nonSyncedKeys.Contains(key))
		{
			object obj = null;
			if (defaultValue is float defaultValue2)
			{
				obj = PlayerPrefs.GetFloat(key, defaultValue2);
			}
			if (defaultValue is int defaultValue3)
			{
				obj = PlayerPrefs.GetInt(key, defaultValue3);
			}
			if (defaultValue is string defaultValue4)
			{
				obj = PlayerPrefs.GetString(key, defaultValue4);
			}
			if (defaultValue is bool flag)
			{
				obj = PlayerPrefs.GetInt(key, flag ? 1 : 0) == 1;
			}
			if (obj != null)
			{
				return (T)obj;
			}
			Debug.LogError($"nonSyncedKey {key} type {typeof(T)} not supported!");
		}
		if (dict.TryGetValue(key, out var value) && value is T)
		{
			return (T)value;
		}
		return defaultValue;
	}

	public void SetList(string key, List<object> values)
	{
		Set(key, values);
	}

	public void SetDict(string key, Dictionary<string, object> value)
	{
		Set(key, value);
	}

	public void SetInt(string key, int value)
	{
		Set(key, value);
	}

	public void SetFloat(string key, float value)
	{
		Set(key, value);
	}

	public void SetString(string key, string value)
	{
		Set(key, value);
	}

	public void SetBool(string key, bool value)
	{
		Set(key, value);
	}

	private void Set<T>(string key, T value)
	{
		if (nonSyncedKeys.Contains(key))
		{
			if (value is float value2)
			{
				PlayerPrefs.SetFloat(key, value2);
			}
			else if (value is int value3)
			{
				PlayerPrefs.SetInt(key, value3);
			}
			else if (value is string value4)
			{
				PlayerPrefs.SetString(key, value4);
			}
			else if (value is bool flag)
			{
				PlayerPrefs.SetInt(key, flag ? 1 : 0);
			}
		}
		else
		{
			dict[key] = value;
		}
	}

	public void RemoveKey(string key)
	{
		if (dict.ContainsKey(key))
		{
			dict.Remove(key);
		}
	}

	public static void printe(object obj)
	{
		if (Application.isEditor)
		{
			Debug.Log(obj);
		}
	}
}
