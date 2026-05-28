using System;
using System.Collections;
using System.Collections.Generic;
using ADOFAI.LevelEditor.Controls;
using UnityEngine;
using UnityEngine.UI;

public class scrTextDecoration : scrDecoration
{
	public Text text;

	public Canvas canvas;

	[NonSerialized]
	public Image bordersRenderer;

	[NonSerialized]
	public Vector2 bordersSetSize;

	private int startingSize;

	private float startingSpacing;

	private int startingResizeTextMaxSize;

	private int startingResizeTextMinSize;

	private FontData fontData;

	private float width;

	private float height;

	public static Dictionary<FontName, Font> nameToFont;

	public override string gameObjectName => sourceLevelEvent.GetString("decText")?.RemoveRichTags();

	public override string decorationName => gameObjectName.NullIfEmpty() ?? PropertyControl_DecorationsList.stringItemNoText;

	private new void Awake()
	{
		base.Awake();
		InitText();
		if (selectionBordersObject != null)
		{
			bordersRenderer = selectionBordersObject.GetComponent<Image>();
		}
	}

	public void InitText()
	{
		startingSize = text.fontSize;
		startingSpacing = text.lineSpacing;
		startingResizeTextMaxSize = text.resizeTextMaxSize;
		startingResizeTextMinSize = text.resizeTextMinSize;
	}

	public void SetText(string newText)
	{
		text.text = newText;
		StartCoroutine(SetCollider());
	}

	public void SetFont(FontName fontName)
	{
		text.fontSize = startingSize;
		text.lineSpacing = startingSpacing;
		text.resizeTextMaxSize = startingResizeTextMaxSize;
		text.resizeTextMinSize = startingResizeTextMinSize;
		fontData = ((fontName == FontName.Default) ? RDString.fontData : RDString.enFontData);
		text.font = nameToFont[fontName];
		float fontScale = fontData.fontScale;
		float lineSpacing = fontData.lineSpacing;
		text.fontSize = Mathf.RoundToInt((float)text.fontSize * fontScale);
		text.lineSpacing *= lineSpacing;
		text.resizeTextMaxSize = Mathf.RoundToInt((float)text.resizeTextMaxSize * fontScale);
	}

	public override void SetDepth(int depth)
	{
		string sortingLayerName = ((depth >= 0) ? "Bg" : "Default");
		int layer = ((depth >= 0) ? 9 : 7);
		canvas.sortingLayerName = sortingLayerName;
		canvas.gameObject.layer = layer;
		int sortingOrder = -depth;
		canvas.sortingOrder = sortingOrder;
	}

	public override Vector2 GetDecorationWorldSize()
	{
		return Vector2.zero;
	}

	protected override void ApplyColor()
	{
		rendererColor = color.WithAlpha(color.a * opacity);
		text.color = rendererColor;
	}

	public override float GetAlpha()
	{
		return text.color.a;
	}

	private IEnumerator SetCollider()
	{
		yield return null;
		TextGenerator textGenerator = new TextGenerator();
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
		width = textGenerator.GetPreferredWidth(text.text, generationSettings);
		height = textGenerator.GetPreferredHeight(text.text, generationSettings);
		bordersSetSize = new Vector2(width, height);
		editorCollider.size = bordersSetSize;
		selectionBordersObject.GetComponent<RectTransform>().sizeDelta = bordersSetSize;
	}

	public override void SetVisible(bool visible)
	{
		rendererEnabled = visible;
		text.enabled = visible;
	}

	public static void MakeNewFontDictionary()
	{
		nameToFont = new Dictionary<FontName, Font>
		{
			{
				FontName.Default,
				RDString.fontData.font
			},
			{
				FontName.Arial,
				RDConstants.data.arialFont
			},
			{
				FontName.ComicSansMS,
				RDConstants.data.comicSansMSFont
			},
			{
				FontName.CourierNew,
				RDConstants.data.courierNewFont
			},
			{
				FontName.Georgia,
				RDConstants.data.georgiaFont
			},
			{
				FontName.Impact,
				RDConstants.data.impactFont
			},
			{
				FontName.TimesNewRoman,
				RDConstants.data.timesNewRomanFont
			}
		};
	}
}
