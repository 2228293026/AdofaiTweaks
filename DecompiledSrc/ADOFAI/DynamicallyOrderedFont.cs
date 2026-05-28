using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ADOFAI;

[Serializable]
public struct DynamicallyOrderedFont
{
	public TMP_FontAsset sourceFont;

	[Header("Language Specific Fonts")]
	public TMP_FontAsset japaneseFont;

	public TMP_FontAsset koreanFont;

	public TMP_FontAsset simplifiedChineseFont;

	public TMP_FontAsset traditionalChineseFont;

	public TMP_FontAsset[] extraFonts;

	public void ApplyForLanguage(SystemLanguage language)
	{
		List<TMP_FontAsset> list = new List<TMP_FontAsset> { japaneseFont, koreanFont, simplifiedChineseFont, traditionalChineseFont };
		TMP_FontAsset item = language switch
		{
			SystemLanguage.Korean => koreanFont, 
			SystemLanguage.Japanese => japaneseFont, 
			SystemLanguage.ChineseSimplified => simplifiedChineseFont, 
			SystemLanguage.ChineseTraditional => traditionalChineseFont, 
			_ => japaneseFont, 
		};
		list.Remove(item);
		list.Insert(0, item);
		list.AddRange(extraFonts);
		sourceFont.fallbackFontAssetTable = list;
	}
}
