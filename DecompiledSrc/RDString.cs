using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SA.GoogleDoc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class RDString
{
	private static readonly string[] editorFontWeights = new string[3] { "Normal", "Medium", "Bold" };

	public static SystemLanguage language = SystemLanguage.English;

	public const SystemLanguage FallbackLanguage = SystemLanguage.English;

	public const string StringsFolder = "Strings/";

	public const string StringsFilePrefix = "RDStrings_";

	public const string GamepadKeySuffix = ".gamepad";

	public const string MobileKeySuffix = ".mobile";

	public const string BoothButtonKeySuffix = ".boothButton";

	public const string DivekickKeySuffix = ".divekick";

	public const string NintendoSwitchKeySuffix = ".nx";

	public const string NintendoSwitchFaceRightCharacter = "\ue0ab";

	public static FontData enFontData;

	public static bool initialized = false;

	public static TMP_FontAsset[] editorFonts;

	public static FontData fontData { get; private set; }

	public static SystemLanguage[] AvailableLanguages => Localization.AvailableLanguages;

	public static string languageSuffix
	{
		get
		{
			Setup();
			if (SystemLanguage.English != language)
			{
				return "_" + language;
			}
			return "";
		}
	}

	public static bool isCJK
	{
		get
		{
			SystemLanguage systemLanguage = language;
			return systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.ChineseTraditional || systemLanguage == SystemLanguage.Chinese || systemLanguage == SystemLanguage.Japanese || systemLanguage == SystemLanguage.Korean;
		}
	}

	public static bool isChinese
	{
		get
		{
			SystemLanguage systemLanguage = language;
			return systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.ChineseTraditional || systemLanguage == SystemLanguage.Chinese;
		}
	}

	public static void Setup()
	{
		if (initialized)
		{
			return;
		}
		string text = Persistence.language.ToString();
		bool flag = false;
		if (text == SystemLanguage.Chinese.ToString())
		{
			flag = true;
			language = SystemLanguage.ChineseSimplified;
		}
		else
		{
			SystemLanguage[] availableLanguages = AvailableLanguages;
			for (int i = 0; i < availableLanguages.Length; i++)
			{
				SystemLanguage systemLanguage = availableLanguages[i];
				if (systemLanguage.ToString() == text)
				{
					flag = true;
					language = systemLanguage;
					break;
				}
			}
		}
		if (!flag)
		{
			language = SystemLanguage.English;
		}
		Localization.SetLanguage(language);
		initialized = true;
		fontData = GetFontDataForLanguage(language);
		enFontData = GetFontDataForLanguage(SystemLanguage.English);
	}

	public static void LoadLevelEditorFonts()
	{
		if (editorFonts == null)
		{
			SystemLanguage systemLanguage = language;
			string text = (((uint)(systemLanguage - 22) > 1u && (uint)(systemLanguage - 40) > 1u) ? "Latin" : language.ToString());
			string text2 = text;
			editorFonts = new TMP_FontAsset[editorFontWeights.Length];
			int num = 0;
			string[] array = editorFontWeights;
			foreach (string text3 in array)
			{
				TMP_FontAsset item = Resources.Load<TMP_FontAsset>("SourceHanSans/" + text2 + "/" + text2 + "-" + text3);
				TMP_FontAsset tMP_FontAsset = Resources.Load<TMP_FontAsset>("SourceHanSans/" + text3);
				tMP_FontAsset.fallbackFontAssetTable = new List<TMP_FontAsset> { item };
				editorFonts[num] = tMP_FontAsset;
				num++;
			}
		}
	}

	public static void CleanEditorFontFallbacks()
	{
		string[] array = editorFontWeights;
		foreach (string text in array)
		{
			Resources.Load<TMP_FontAsset>("SourceHanSans/" + text).fallbackFontAssetTable = new List<TMP_FontAsset>();
		}
	}

	public static void SetLocalizedFont(this Text text)
	{
		Setup();
		if (fontData.font != text.font)
		{
			float fontScale = fontData.fontScale;
			float lineSpacing = fontData.lineSpacing;
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
			text.lineSpacing *= lineSpacing;
			text.resizeTextMaxSize = Mathf.RoundToInt((float)text.resizeTextMaxSize * fontScale);
			text.resizeTextMinSize = Mathf.RoundToInt((float)text.resizeTextMinSize * fontScale);
			text.font = fontData.font;
		}
	}

	public static void SetLocalizedFont(this TMP_Text text)
	{
		Setup();
		if (fontData.fontTMP != text.font)
		{
			float num = ((fontData.fontScale < 1f) ? fontData.fontScale : (fontData.fontScale / 1.25f));
			float lineSpacingTMP = fontData.lineSpacingTMP;
			if (language != SystemLanguage.Japanese)
			{
				num *= 1.2f;
			}
			text.fontSize = Mathf.RoundToInt(text.fontSize * num);
			text.lineSpacing *= lineSpacingTMP;
			Material fontMaterial = text.fontMaterial;
			text.font = fontData.fontTMP;
			Material fontMaterial2 = text.fontMaterial;
			fontMaterial2.SetColor("_UnderlayColor", fontMaterial.GetColor("_UnderlayColor"));
			fontMaterial2.SetFloat("_UnderlayDilate", fontMaterial.GetFloat("_UnderlayDilate"));
		}
	}

	public static void SetLocalizedFont(this TextMesh text)
	{
		Setup();
		if (fontData.font != text.font)
		{
			float fontScale = fontData.fontScale;
			float lineSpacing = fontData.lineSpacing;
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
			text.lineSpacing *= lineSpacing;
			text.font = fontData.font;
			text.GetComponent<MeshRenderer>().material = text.font.material;
		}
	}

	public static string GetInlinedFontForString(string s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Font[] array = new Font[4]
		{
			RDConstants.data.latinFont,
			RDConstants.data.koreanFont,
			RDConstants.data.chineseFont,
			RDConstants.data.japaneseFont
		};
		int num = 0;
		bool flag = false;
		foreach (char c in s)
		{
			int num2 = ((!char.IsWhiteSpace(c)) ? num : (IsKoreanCharacter(c) ? 1 : (IsChineseCharacter(c) ? 2 : (IsJapaneseCharacter(c) ? 3 : 0))));
			if (num != num2)
			{
				if (array[num].name != fontData.font.name)
				{
					stringBuilder.Append("</font>");
					flag = false;
				}
				if (array[num2].name != fontData.font.name)
				{
					stringBuilder.Append("<font=\"" + array[num2].name + "\">");
					flag = true;
				}
			}
			num = num2;
			stringBuilder.Append(c);
		}
		if (flag)
		{
			stringBuilder.Append("</font>");
		}
		return stringBuilder.ToString();
	}

	public static FontData GetFontDataForLanguage(SystemLanguage language)
	{
		FontData result = default(FontData);
		RDConstants data = RDConstants.data;
		result.fontScale = 1f;
		switch (language)
		{
		case SystemLanguage.Korean:
			result.lineSpacing = 0.75f;
			result.lineSpacingTMP = 0.75f;
			result.font = data.koreanFont;
			result.fontTMP = data.koreanFontTMPro;
			break;
		case SystemLanguage.Japanese:
			result.lineSpacing = 1.1f;
			result.lineSpacingTMP = 1.1f;
			result.font = data.japaneseFont;
			result.fontTMP = data.japaneseFontTMPro;
			break;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			result.lineSpacing = 1f;
			result.lineSpacingTMP = 1f;
			result.font = data.chineseFont;
			result.fontTMP = data.chineseFontTMPro;
			break;
		default:
			result.lineSpacing = 0.75f;
			result.lineSpacingTMP = 1f;
			result.font = (GCS.bb ? BBManager.instance.font : data.latinFont);
			result.fontTMP = data.latinFontTMPro;
			break;
		}
		result.fontScale = 1f;
		return result;
	}

	public static void SetRDString(this Text text, string key)
	{
		Setup();
		text.SetLocalizedFont();
		text.text = Get(key);
	}

	public static string Get(string key, Dictionary<string, object> parameters = null)
	{
		Setup();
		bool exists = false;
		return GetWithCheck(key, out exists, parameters);
	}

	public static string GetEnumValue(string type, string value)
	{
		string text = value.ToString();
		string key = "enum." + type + "." + text;
		bool exists = false;
		string result = GetWithCheck(key, out exists);
		if (!exists)
		{
			result = Get("enum.common." + text);
		}
		return result;
	}

	public static string GetEnumValue<T>(T value)
	{
		return GetEnumValue(typeof(T).Name, value.ToString());
	}

	public static string GetWithCheck(string key, out bool exists, Dictionary<string, object> parameters = null)
	{
		Setup();
		exists = false;
		string text = "";
		string token = key + ".gamepad";
		string token2 = key + ".mobile";
		string token3 = key + ".nx";
		if (ADOBase.isSwitch && Localization.ExistsLocalizedString(token3))
		{
			text = Localization.GetLocalizedString(token3);
			exists = true;
		}
		else if (ADOBase.isGamepad && Localization.ExistsLocalizedString(token))
		{
			text = Localization.GetLocalizedString(token);
			exists = true;
		}
		else if (ADOBase.isMobile && Localization.ExistsLocalizedString(token2))
		{
			text = Localization.GetLocalizedString(token2);
			exists = true;
		}
		else if (Localization.ExistsLocalizedString(key))
		{
			text = Localization.GetLocalizedString(key);
			exists = true;
		}
		if (text == "RDNull")
		{
			text = Localization.GetLocalizedString(key, SystemLanguage.English);
			exists = true;
		}
		if (exists)
		{
			text = ReplaceParameters(text, parameters);
		}
		return text;
	}

	public static string Join(params object[] _string)
	{
		return string.Join("", (string[])_string);
	}

	public static void ChangeLanguage(SystemLanguage language)
	{
		RDConstants.data.forceLanguage = false;
		Persistence.language = language;
		print("changed language to: " + language);
		initialized = false;
		Setup();
	}

	private static void print(object message)
	{
		Debug.Log(message);
	}

	public static string AddSpacesToChineseString(string s)
	{
		if (s.IsNullOrEmpty())
		{
			return s;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < s.Length - 1; i++)
		{
			char c = s[i];
			char c2 = s[i + 1];
			bool flag = !IsChineseCharacter(c) && !IsChineseCharacter(c2);
			stringBuilder.Append(s[i]);
			if (!CharIsCJKPunctuation(c2) && !flag)
			{
				stringBuilder.Append('\u200b');
			}
		}
		stringBuilder.Append(s[s.Length - 1]);
		return stringBuilder.ToString();
	}

	private static bool CharIsCJKPunctuation(char c)
	{
		uint[] array = new uint[12]
		{
			65292u, 65281u, 65311u, 65307u, 65306u, 65288u, 65289u, 65339u, 65341u, 12304u,
			12305u, 12290u
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == c)
			{
				return true;
			}
		}
		return false;
	}

	public static Font GetAppropiateFontForString(string s)
	{
		if (s.Any((char c) => IsKoreanCharacter(c)))
		{
			return RDConstants.data.koreanFont;
		}
		if (s.Any((char c) => IsJapaneseCharacter(c)))
		{
			return RDConstants.data.japaneseFont;
		}
		if (s.Any((char c) => IsChineseCharacter(c)))
		{
			return RDConstants.data.chineseFont;
		}
		return RDConstants.data.latinFont;
	}

	public static bool IsChineseCharacter(char c)
	{
		if (((uint)c < 19968u || (uint)c > 40959u) && ((uint)c < 13312u || (uint)c > 19903u) && ((uint)c < 131072u || (uint)c > 173791u) && ((uint)c < 173824u || (uint)c > 191471u) && ((uint)c < 196608u || (uint)c > 201551u) && ((uint)c < 13056u || (uint)c > 13311u) && ((uint)c < 65072u || (uint)c > 65103u) && ((uint)c < 63744u || (uint)c > 64255u))
		{
			if ((uint)c >= 194560u)
			{
				return (uint)c <= 195103u;
			}
			return false;
		}
		return true;
	}

	public static bool IsKoreanCharacter(char c)
	{
		if (((uint)c < 44032u || (uint)c > 55203u) && ((uint)c < 4352u || (uint)c > 4607u) && ((uint)c < 12592u || (uint)c > 12687u) && ((uint)c < 43360u || (uint)c > 43391u))
		{
			if ((uint)c >= 55216u)
			{
				return (uint)c <= 55295u;
			}
			return false;
		}
		return true;
	}

	public static bool IsJapaneseCharacter(char c)
	{
		if ((uint)c >= 12288u)
		{
			return (uint)c <= 12543u;
		}
		return false;
	}

	public static void DownloadStringsFromWeb()
	{
		print("download strings from web");
		API.RetrievePublicSheetData(Settings.Instance.GetDocByKey(LocalizationConfig.Instance.LocalizationDocKey));
		Localization.SetLanguage(Localization.CurrentLanguage);
	}

	public static string GetColon()
	{
		if (!isCJK)
		{
			return ":";
		}
		return "：";
	}

	public static string ReplaceParameters(string str, Dictionary<string, object> parameters)
	{
		if (parameters != null)
		{
			foreach (string key in parameters.Keys)
			{
				str = Regex.Replace(str, "(\\[" + key + "\\])", parameters[key].ToString());
			}
		}
		return str;
	}
}
