using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ADOFAI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public static class RDUtils
{
	[Flags]
	public enum UnCamelCaseOptions
	{
		None = 0,
		SpaceBeforeNumbers = 1,
		UnderbarToSpace = 2
	}

	private static readonly string[] noneTags = new string[6] { "NONE", "无", "無", "AUCUN", "ŽÁDNÝ", "NICHTS" };

	public static int targetFrameRate
	{
		get
		{
			return Application.targetFrameRate;
		}
		set
		{
			Application.targetFrameRate = value;
		}
	}

	private static bool TryParseJPEG(FileStream fs, out int width, out int height)
	{
		width = 0;
		height = 0;
		fs.Seek(0L, SeekOrigin.Begin);
		while (true)
		{
			int num;
			if ((num = fs.ReadByte()) != 255 && (num < 208 || num > 216))
			{
				if (num == 217 || num == -1)
				{
					break;
				}
				int num2 = (fs.ReadByte() << 8) | fs.ReadByte();
				if (num == 192 || num == 194)
				{
					fs.Seek(1L, SeekOrigin.Current);
					height |= fs.ReadByte() << 8;
					height |= fs.ReadByte();
					width |= fs.ReadByte() << 8;
					width |= fs.ReadByte();
					return true;
				}
				fs.Seek(num2 - 2, SeekOrigin.Current);
			}
		}
		return false;
	}

	private static bool TryParseGIF(FileStream fs, out int width, out int height)
	{
		width = 0;
		height = 0;
		fs.Seek(6L, SeekOrigin.Begin);
		width |= fs.ReadByte();
		width |= fs.ReadByte() << 8;
		height |= fs.ReadByte();
		height |= fs.ReadByte() << 8;
		return true;
	}

	private static bool TryParsePNG(FileStream fs, out int width, out int height)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		width = 0;
		height = 0;
		fs.Seek(12L, SeekOrigin.Begin);
		byte[] array = new byte[4];
		((Stream)fs).Read(Span<byte>.op_Implicit(array));
		if (!array.SequenceEqual(Encoding.ASCII.GetBytes("IHDR")))
		{
			return false;
		}
		width |= fs.ReadByte() << 24;
		width |= fs.ReadByte() << 16;
		width |= fs.ReadByte() << 8;
		width |= fs.ReadByte();
		height |= fs.ReadByte() << 24;
		height |= fs.ReadByte() << 16;
		height |= fs.ReadByte() << 8;
		height |= fs.ReadByte();
		return true;
	}

	public static Vector2Int GetImageDimensions(string path)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			using FileStream fileStream = File.OpenRead(path);
			byte[] array = new byte[4];
			((Stream)fileStream).Read(Span<byte>.op_Implicit(array));
			int width3;
			int height3;
			if (array.SequenceEqual(new byte[4] { 137, 80, 78, 71 }))
			{
				if (TryParsePNG(fileStream, out var width, out var height))
				{
					return new Vector2Int(width, height);
				}
			}
			else if (array.SequenceEqual(new byte[4] { 255, 216, 255, 224 }) || array.SequenceEqual(new byte[4] { 255, 216, 255, 225 }) || array.SequenceEqual(new byte[4] { 255, 216, 255, 238 }))
			{
				if (TryParseJPEG(fileStream, out var width2, out var height2))
				{
					return new Vector2Int(width2, height2);
				}
			}
			else if (array.SequenceEqual(new byte[4] { 71, 73, 70, 56 }) && TryParseGIF(fileStream, out width3, out height3))
			{
				return new Vector2Int(width3, height3);
			}
		}
		catch (Exception)
		{
		}
		return Vector2Int.zero;
	}

	public static AudioMixerGroup GetMixerGroup(string groupPath)
	{
		AudioMixerGroup[] array = AudioManager.Instance.Mixer.FindMatchingGroups(groupPath);
		if (array != null && array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static AudioMixerGroup GetMixerGroup(MixerGroup group)
	{
		AudioMixerGroup[] array = AudioManager.Instance.Mixer.FindMatchingGroups(group.ToString());
		if (array != null && array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static void SetMixerParameter(string exposedParameter, float value)
	{
		if (!AudioManager.Instance.Mixer.SetFloat(exposedParameter, value))
		{
			UnityEngine.Debug.LogWarning("Audio Parameter not found: $" + exposedParameter + "!");
		}
	}

	public static void SetMixerVolume(string exposedParameterVolume, float value)
	{
		float value2 = ((value > 1f) ? ((value - 1f) * 10f) : ((!(value > 0f)) ? (-80f) : ((value - 1f) * 20f)));
		SetMixerParameter(exposedParameterVolume, value2);
	}

	public static float GetMixerParameter(string exposedParameterName)
	{
		AudioManager.Instance.Mixer.GetFloat(exposedParameterName, out var value);
		return value;
	}

	public static float PitchPercentToSemitones(float percent)
	{
		return (float)Math.Log(percent, 2.0) * 12f;
	}

	public static float PitchSemitonestoPercent(float semitones)
	{
		return (float)Math.Pow(2.0, (double)semitones / 12.0);
	}

	public static string GetAvailableDirectoryName(string directoryPath)
	{
		int num = -1;
		string text = "";
		do
		{
			string text2 = ((num >= 0) ? num.ToString() : "");
			text = directoryPath + text2;
			num++;
		}
		while (RDDirectory.Exists(text) || RDFile.Exists(text));
		return text;
	}

	public static bool IsHex(string s)
	{
		foreach (char c in s)
		{
			if ((c < '0' || c > '9') && (c < 'a' || c > 'f') && (c < 'A' || c > 'F'))
			{
				return false;
			}
		}
		return true;
	}

	public static Color HexToColor(this string hex)
	{
		TryHexToColor(hex, out var color);
		return color;
	}

	public static bool TryHexToColor(string hex, out Color color)
	{
		if (hex != null && hex.Length >= 6 && byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out var result) && byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out var result2) && byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out var result3))
		{
			byte result4 = byte.MaxValue;
			if (hex.Length < 8 || byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, null, out result4))
			{
				color = new Color32(result, result2, result3, result4);
				return true;
			}
		}
		color = Color.black;
		return false;
	}

	public static string ToHex(this Color c, bool useAlpha = false, bool hash = true)
	{
		string text = (hash ? "#" : "");
		if (!useAlpha)
		{
			return string.Format(text + "{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
		}
		return string.Format(text + "{0:X2}{1:X2}{2:X2}{3:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a));
	}

	private static byte ToByte(float f)
	{
		f = Mathf.Clamp01(f);
		return (byte)(f * 255f);
	}

	public static Vector2 StringToVector2(string sVector)
	{
		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}
		string[] array = sVector.Split(',', StringSplitOptions.None);
		return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}

	public static Tuple<float, float> StringToFloatPair(string sTuple)
	{
		if (sTuple.StartsWith("(") && sTuple.EndsWith(")"))
		{
			sTuple = sTuple.Substring(1, sTuple.Length - 2);
		}
		string[] array = sTuple.Split(',', StringSplitOptions.None);
		return new Tuple<float, float>(float.Parse(array[0]), float.Parse(array[1]));
	}

	public static Tuple<string, string> StringToStringTuple(string sTuple)
	{
		if (sTuple.StartsWith("(") && sTuple.EndsWith(")"))
		{
			sTuple = sTuple.Substring(1, sTuple.Length - 2);
		}
		string[] array = sTuple.Split(',', StringSplitOptions.None);
		return new Tuple<string, string>(array[0], array[1]);
	}

	public static float GetRandomFloat(LevelEvent evnt, string key)
	{
		return evnt.GetFloat(key);
	}

	public static int GetRandomInt(LevelEvent evnt, string key)
	{
		return evnt.GetInt(key);
	}

	public static Vector2 GetRandomVector2(LevelEvent evnt, string key)
	{
		return (Vector2)evnt[key];
	}

	public static T ParseEnum<T>(string str, T defaultValue = default(T)) where T : struct
	{
		if (Enum.TryParse<T>(str, ignoreCase: true, out var result))
		{
			return result;
		}
		UnityEngine.Debug.LogWarning("ParseEnum(): Returned default value " + defaultValue.ToString() + " because couldn't find string value " + str);
		return defaultValue;
	}

	public static T ToEnum<T>(this string str, T defaultValue = default(T), bool showWarning = true)
	{
		try
		{
			return (T)Enum.Parse(typeof(T), str, ignoreCase: true);
		}
		catch (Exception)
		{
			if (showWarning)
			{
				UnityEngine.Debug.LogWarning("ParseEnum(): Returned default value " + defaultValue.ToString() + " because couldn't find string value " + str);
			}
			return defaultValue;
		}
	}

	public static Array GetValues<T>()
	{
		return Enum.GetValues(typeof(T));
	}

	public static GameObject SpawnIfNotFound(string GOname, GameObject prefab = null)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = GameObject.Find(GOname);
		if (gameObject2 == null)
		{
			if (prefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab);
				gameObject.name = GOname;
			}
			else
			{
				gameObject = new GameObject(GOname);
			}
			return gameObject;
		}
		return gameObject2;
	}

	public static float OutQuad(float t, float d)
	{
		return 1f - Mathf.Pow(1f - t / d, 2f);
	}

	public static void ShowDebugLabel(string s)
	{
		GUI.Label(new Rect(0f, 0f, 400f, 400f), s);
	}

	public static int Clamp(int value, int min, int max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static int Clamp01(int value)
	{
		return Clamp(value, 0, 1);
	}

	public static Color CycleColor(Color currentcolour)
	{
		uint[] array = new uint[11]
		{
			4291189578u, 4291198794u, 4283090599u, 4283081414u, 4283070918u, 4283058886u, 4285745862u, 4288039622u, 4290136774u, 4291185316u,
			4291185279u
		};
		Color color = currentcolour;
		while (currentcolour == color)
		{
			int num = Mathf.FloorToInt(UnityEngine.Random.value * (float)array.Length);
			color = array[num].ARGBToColor();
		}
		return color;
	}

	public static void Rotate2D(Transform trans, float rotation, bool relative = false)
	{
		if (!relative)
		{
			trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, rotation);
		}
		else
		{
			trans.localEulerAngles = new Vector3(trans.localEulerAngles.x, trans.localEulerAngles.y, trans.localEulerAngles.z + rotation);
		}
	}

	public static float CeilToNearestMultiple(float n, float multiple)
	{
		float num = Mathf.Round(n / multiple) * multiple;
		if (WithinSmallMargin(num, n))
		{
			return num;
		}
		return Mathf.Ceil(n / multiple) * multiple;
	}

	public static bool WithinSmallMargin(float a, float b)
	{
		return (double)Mathf.Abs(a - b) < 0.001;
	}

	public static T Create<T>(string name, bool makeContainer = false, int suffixNumber = -1, string customContainer = null) where T : Component
	{
		GameObject gameObject = new GameObject();
		GameObject gameObject2 = gameObject.Instantiate(name, makeContainer, customContainer, suffixNumber);
		UnityEngine.Object.Destroy(gameObject);
		return gameObject2.AddComponent<T>();
	}

	private static bool CheckForProcess(string processName)
	{
		if (Process.GetProcessesByName(processName).Length == 0)
		{
			return false;
		}
		return true;
	}

	public static bool IsDigitsOnly(string str)
	{
		foreach (char c in str)
		{
			if (c < '0' || c > '9')
			{
				return false;
			}
		}
		return true;
	}

	public static T Pop<T>(this List<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static float GetHue(Color color)
	{
		int num = Mathf.RoundToInt(color.r * 255f);
		int num2 = Mathf.RoundToInt(color.g * 255f);
		int num3 = Mathf.RoundToInt(color.b * 255f);
		float num4 = Mathf.Min(Mathf.Min(num, num2), num3);
		float num5 = Mathf.Max(Mathf.Max(num, num2), num3);
		if (num4 == num5)
		{
			return 0f;
		}
		float num6 = 0f;
		num6 = ((num5 == (float)num) ? ((float)(num2 - num3) / (num5 - num4)) : ((num5 != (float)num2) ? (4f + (float)(num - num2) / (num5 - num4)) : (2f + (float)(num3 - num) / (num5 - num4))));
		num6 *= 60f;
		if (num6 < 0f)
		{
			num6 += 360f;
		}
		return num6;
	}

	public static Color GetColor(this PlanetColorPreset planetColor)
	{
		return RDConstants.data.GetPlanetColor(planetColor);
	}

	public static void Add<T>(this List<T> list, params T[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			list.Add(items[i]);
		}
	}

	public static string BreakRichText(string text)
	{
		if (text.Length <= 1)
		{
			return text;
		}
		int num = 0;
		char[] array = text.ToCharArray();
		for (int i = 1; i < text.Length; i++)
		{
			if ((array[i] == '<' || array[i] == '>') && array[i - 1] != '\\')
			{
				text.Insert(i + num, "\\");
			}
		}
		return text;
	}

	public static string RemoveRichTags(string text)
	{
		int num = 0;
		char[] array = text.ToCharArray();
		string text2 = text;
		bool flag = false;
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			if (array[i] == '<')
			{
				flag = true;
				num2 = i;
			}
			else if (array[i] == '>' && flag)
			{
				int num3 = i - num2 + 1;
				text2 = text2.Remove(num2 - num, num3);
				num += num3;
				flag = false;
			}
		}
		return text2;
	}

	public static bool IsXtra(this string s)
	{
		if (GCNS.worldData.ContainsKey(s))
		{
			if (!GCNS.worldData[s].isXtra)
			{
				return GCNS.worldData[s].isCrown;
			}
			return true;
		}
		return false;
	}

	public static bool IsCrownWorld(this string s)
	{
		if (GCNS.worldData.ContainsKey(s))
		{
			return GCNS.worldData[s].isCrown;
		}
		return false;
	}

	public static bool IsMuseDashWorld(this string s)
	{
		if (GCNS.worldData.ContainsKey(s))
		{
			return GCNS.worldData[s].isMuseDash;
		}
		return false;
	}

	public static bool IsTechWorld(this string s)
	{
		if (!s.IsNullOrEmpty() && GCNS.worldData.ContainsKey(s))
		{
			return GCNS.worldData[s].isTech;
		}
		return false;
	}

	public static bool Approximately(this Vector2 v, Vector2 v2)
	{
		if (Mathf.Approximately(v.x, v2.x))
		{
			return Mathf.Approximately(v.y, v2.y);
		}
		return false;
	}

	public static bool ApproximatelyXY(this Vector3 v, Vector3 v2)
	{
		if (Mathf.Approximately(v.x, v2.x))
		{
			return Mathf.Approximately(v.y, v2.y);
		}
		return false;
	}

	public static string TrimAllSpaces(this string value)
	{
		return value.Replace(" ", string.Empty);
	}

	public static bool IsTaro(this string s)
	{
		return NeoCosmosManager.instance.IsDLCLevel(s);
	}

	public static bool IsTaroScene(this string s)
	{
		return NeoCosmosManager.instance.IsDLCScene(s);
	}

	public static bool IsVega(this string s)
	{
		return VegaDLCManager.instance.IsDLCLevel(s);
	}

	public static bool IsVegaScene(this string s)
	{
		return VegaDLCManager.instance.IsDLCScene(s);
	}

	public static bool CheckDLCLevelPlayable(string name)
	{
		foreach (DLCManager dLCManager in DLCManager.DLCManagers)
		{
			if (dLCManager.IsDLCLevel(name))
			{
				return dLCManager.installed;
			}
		}
		return true;
	}

	public static bool IsBossLevel(this string levelName)
	{
		return levelName.EndsWith("-X");
	}

	public static (int year, int month, int day) GetDateOfChineseNewYear()
	{
		ChineseLunisolarCalendar chineseLunisolarCalendar = new ChineseLunisolarCalendar();
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		DateTime time = chineseLunisolarCalendar.ToDateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, 0);
		int year = gregorianCalendar.GetYear(time);
		int month = gregorianCalendar.GetMonth(time);
		int dayOfMonth = gregorianCalendar.GetDayOfMonth(time);
		return (year: year, month: month, day: dayOfMonth);
	}

	public static bool IsSubDirectoryOf(this string candidate, string other)
	{
		bool result = false;
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(candidate);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(other);
			while (directoryInfo.Parent != null)
			{
				if (directoryInfo.Parent.FullName.ToLower() == directoryInfo2.FullName.ToLower())
				{
					result = true;
					break;
				}
				directoryInfo = directoryInfo.Parent;
			}
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.Log($"Unable to check directories {candidate} and {other}: {arg}");
		}
		return result;
	}

	public static bool HasConnectionError(this UnityWebRequest webRequest)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		Result result = webRequest.result;
		if ((int)result != 2)
		{
			return (int)result == 3;
		}
		return true;
	}

	public static string SanitizeProtocolPath(string path)
	{
		return Uri.EscapeDataString(path).Replace(Uri.HexEscape(Path.PathSeparator), Path.PathSeparator.ToString()).Replace(Uri.HexEscape(Path.DirectorySeparatorChar), Path.DirectorySeparatorChar.ToString())
			.Replace(Uri.HexEscape(Path.AltDirectorySeparatorChar), Path.AltDirectorySeparatorChar.ToString())
			.Replace(Uri.HexEscape(Path.VolumeSeparatorChar), Path.VolumeSeparatorChar.ToString());
	}

	public static string ToFileUri(this string path)
	{
		Uri uri = new Uri(path);
		if (!uri.IsFile)
		{
			return null;
		}
		return uri.ToString();
	}

	public static int FlooredModulo(this int a, int b)
	{
		return a - b * (int)Math.Floor((float)a * 1f / (float)b);
	}

	public static float NormalizedSin(this float v)
	{
		return (Mathf.Sin(v) + 1f) / 2f;
	}

	public static string GenerateHash(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return null;
		}
		Hash128 hash = default(Hash128);
		using (FileStream fileStream = File.OpenRead(filePath))
		{
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			while (fileStream.Position != fileStream.Length)
			{
				hash.Append(binaryReader.ReadBytes(33554432));
			}
		}
		return hash.ToString();
	}

	public static bool IsNoneConditionalTag(this string s)
	{
		return noneTags.Contains(s);
	}

	public static string NullIfEmptyConditionalTag(this string s)
	{
		if (!string.IsNullOrEmpty(s) && !s.IsNoneConditionalTag())
		{
			return s;
		}
		return null;
	}

	public static bool GameIsModded()
	{
		try
		{
			return AppDomain.CurrentDomain.GetAssemblies().Any((Assembly assembly) => assembly.FullName.Contains("0Harmony"));
		}
		catch
		{
			return true;
		}
	}

	public static string UnCamelCase(this string s, UnCamelCaseOptions options = UnCamelCaseOptions.None)
	{
		s = Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");
		s = Regex.Replace(s, "\\b([A-Z]+)([A-Z])([a-z])", "$1 $2$3");
		string text = char.ToUpper(s[0]).ToString();
		string text2 = s;
		s = text + text2.Substring(1, text2.Length - 1);
		if (options.HasFlag(UnCamelCaseOptions.SpaceBeforeNumbers))
		{
			s = Regex.Replace(s, "(\\d+)", " $1");
		}
		if (options.HasFlag(UnCamelCaseOptions.UnderbarToSpace))
		{
			s = s.Replace("_", " ");
		}
		s = Regex.Replace(s, "\\s{2,}", " ");
		return s.Trim();
	}

	public static string NullIfEmpty(this string s)
	{
		if (!string.IsNullOrEmpty(s))
		{
			return s;
		}
		return null;
	}

	public static float SetIfNaN(this float f, float value)
	{
		if (!float.IsNaN(f))
		{
			return f;
		}
		return value;
	}

	public static bool CheckTouched(Vector2 obj1Pos, Vector2 obj1Size, Vector2 obj2Pos, Vector2 obj2Size)
	{
		float num = obj1Pos.x - obj1Size.x / 2f;
		float num2 = obj1Pos.x + obj1Size.x / 2f;
		float num3 = obj1Pos.y + obj1Size.y / 2f;
		float num4 = obj1Pos.y - obj1Size.y / 2f;
		float num5 = obj2Pos.x - obj2Size.x / 2f;
		float num6 = obj2Pos.x + obj2Size.x / 2f;
		float num7 = obj2Pos.y + obj2Size.y / 2f;
		float num8 = obj2Pos.y - obj2Size.y / 2f;
		if (num < num6 && num2 > num5 && num3 > num8)
		{
			return num4 < num7;
		}
		return false;
	}

	public static ParticleSystem.MinMaxCurve ToRandomCurve(this Tuple<float, float> tuple, float multiplier = 1f)
	{
		return new ParticleSystem.MinMaxCurve(tuple.Item1 * multiplier, tuple.Item2 * multiplier);
	}

	public static ParticleSystem.MinMaxCurve ToLinearCurve(this Tuple<float, float> tuple, float multiplier = 1f)
	{
		return new ParticleSystem.MinMaxCurve(multiplier, AnimationCurve.Linear(0f, tuple.Item1, 1f, tuple.Item2));
	}

	public static scrFloor GetFloorAtPosition(Vector2 position)
	{
		return ((Component)(object)Physics2D.OverlapPoint(position, 1 << LayerMask.NameToLayer("Floor")))?.GetComponent<scrFloor>();
	}
}
