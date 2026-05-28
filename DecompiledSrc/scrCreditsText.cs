using System;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class scrCreditsText : ADOBase
{
	public enum CreditsType
	{
		GameCredits,
		NeoCosmosCredits
	}

	public bool planetOnPosition;

	private bool forceScrolling;

	public const float translateSpeed = 75f;

	public const float creditsLoopSpacing = 450f;

	private const string creditsPrefix = "ADOCredits - ";

	private const string gameCreditsCSV = "Main Game";

	private const string neoCosmosCreditsCSV = "Neo Cosmos";

	private const string creditsFolder = "Credits";

	public CreditsType creditsType;

	public Transform scrollSection;

	public RectTransform content;

	public RectTransform title;

	public TMP_Text text;

	public Vector2Int planetsPosition;

	[NonSerialized]
	public RectTransform contentCopy;

	private Vector3 startPos;

	private Vector3 copyStartPos;

	private bool activateSwitch;

	public OverseerIdle os;

	public GameObject pigStatue;

	private bool initalized;

	private void Start()
	{
		Setup();
	}

	public void Setup()
	{
		if (initalized)
		{
			return;
		}
		initalized = true;
		string text = "";
		string text2 = ((creditsType == CreditsType.GameCredits) ? "Main Game" : "Neo Cosmos");
		text2 = "ADOCredits - " + text2;
		string[,] grid = new CSVReader(Resources.Load<TextAsset>(Path.Combine("Credits", text2))).grid;
		float num = 34f * RDString.fontData.fontScale;
		string text3 = "";
		for (int i = 0; i < grid.GetLength(1); i++)
		{
			if (grid[0, i] == null)
			{
				continue;
			}
			if (grid[0, i].StartsWith("::") || grid[0, i] == "levelSelect.subtitle")
			{
				if (text3 != "")
				{
					text += "\n";
				}
				text3 = grid[0, i].Remove(0, 2);
			}
			if (text3 == "")
			{
				continue;
			}
			for (int j = 0; j < grid.GetLength(0) && grid[j, i] != null && !(grid[j, i] == ""); j++)
			{
				bool flag = grid[0, i].StartsWith("::");
				string text4 = grid[j, i];
				if (text4.StartsWith("::"))
				{
					text4 = text4.Remove(0, 2);
				}
				string text5 = "";
				if (text3 == "localization")
				{
					if (text4 == "localization")
					{
						text5 = RDString.Get("credits." + text4);
						text5 = $"<color=#ffffffaa><size={Mathf.RoundToInt(num * 1.25f)}>{text5}</size></color>\n";
					}
					else if (j == 0)
					{
						text5 = RDString.Get("credits.language." + text4);
						text5 = $"<color=#ffffffaa><size={num}>{text5}{RDString.GetColon()} </size></color>";
					}
					else
					{
						text5 = RDString.Get("credits." + text4);
						text5 += "\n";
					}
				}
				else
				{
					text5 = (text4.Contains(".") ? RDString.Get(text4) : ((text4 != "") ? RDString.Get("credits." + text4) : ""));
					if (flag)
					{
						text5 = $"<color=#ffffffaa><size={num}>{text5}</size></color>";
					}
					text5 += "\n";
				}
				text += text5;
			}
		}
		this.text.text = text;
		this.text.SetLocalizedFont();
		this.text.rectTransform.SizeDeltaY(this.text.preferredHeight);
		content.SizeDeltaY(this.text.preferredHeight + title.rect.height);
		contentCopy = UnityEngine.Object.Instantiate(content, scrollSection.transform);
		content.localPosition += Vector3.down * 500f;
		startPos = content.localPosition + Vector3.up * 500f;
		copyStartPos = startPos + Vector3.down * (content.rect.height + 450f);
		contentCopy.transform.localPosition = copyStartPos + Vector3.down * 500f;
		if ((bool)ADOBase.controller)
		{
			ADOBase.controller.creditsText = this;
		}
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenPlanet.transform.position;
		int num = Mathf.RoundToInt(position.x);
		int num2 = Mathf.RoundToInt(position.y);
		bool flag = (planetOnPosition = num == planetsPosition.x && num2 == planetsPosition.y);
		if (forceScrolling || flag)
		{
			if (!activateSwitch && os != null)
			{
				os.FadeIn(0.5f);
			}
			activateSwitch = true;
			bool upIsPressed = RDInput.upIsPressed;
			bool downIsPressed = RDInput.downIsPressed;
			bool flag2 = content.localPosition.y >= startPos.y || contentCopy.localPosition.y >= startPos.y;
			bool flag3 = upIsPressed && flag2;
			Vector3 vector = (flag3 ? Vector3.down : Vector3.up) * (75f * (float)((!(downIsPressed || flag3)) ? 1 : 12) * Time.deltaTime);
			if (!flag3 && upIsPressed)
			{
				vector = Vector3.zero;
			}
			if (content.localPosition.y <= content.rect.height + 450f)
			{
				content.localPosition += vector;
			}
			else
			{
				content.localPosition = copyStartPos;
			}
			if (contentCopy.localPosition.y <= content.rect.height + 450f)
			{
				contentCopy.localPosition += vector;
			}
			else
			{
				contentCopy.localPosition = copyStartPos;
			}
		}
		else if (activateSwitch)
		{
			Reset();
		}
	}

	public void Reset(bool instant = false)
	{
		if (os != null)
		{
			os.FadeOut(instant ? 0f : 0.5f);
		}
		float y = content.localPosition.y;
		float y2 = contentCopy.localPosition.y;
		bool flag = y > y2;
		content.transform.DOLocalMoveY(flag ? startPos.y : copyStartPos.y, instant ? 0f : 1.5f);
		contentCopy.transform.DOLocalMoveY(flag ? copyStartPos.y : startPos.y, instant ? 0f : 1.5f);
		activateSwitch = false;
	}

	public void SetScroll(bool scroll)
	{
		forceScrolling = scroll;
	}
}
