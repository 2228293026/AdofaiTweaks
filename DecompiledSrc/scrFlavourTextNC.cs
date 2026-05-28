using System;
using UnityEngine;
using UnityEngine.UI;

public class scrFlavourTextNC : ADOBase
{
	private const int startX = 13;

	private const int spacing = 7;

	private const int bonusSpeedTrialX = 52;

	[NonSerialized]
	public Text text;

	private float ballX;

	private float ballY;

	private int lastRoundedX = -9999;

	private int lastRoundedY = -9999;

	private bool unlockedWorldT3;

	private bool beatWorldT4;

	private bool bottomRow;

	private void Awake()
	{
		text = GetComponent<Text>();
		text.SetLocalizedFont();
		unlockedWorldT3 = Persistence.IsWorldComplete("T1") && Persistence.IsWorldComplete("T2");
		beatWorldT4 = Persistence.IsWorldComplete("T4");
	}

	private void Update()
	{
		Vector3 position = ADOBase.controller.chosenPlanet.transform.position;
		int num = Mathf.RoundToInt(position.x);
		int num2 = Mathf.RoundToInt(position.y);
		Vector2Int vector2Int = new Vector2Int(num, num2);
		if (lastRoundedX == num && lastRoundedY == num2)
		{
			return;
		}
		lastRoundedX = num;
		lastRoundedY = num2;
		bottomRow = scrCamera.instance.positionState == PositionState.TaroMenu0BottomLane || scrCamera.instance.positionState == PositionState.TaroMenu2BottomLane || scrCamera.instance.positionState == PositionState.TaroMenu3BottomLane;
		UpdateAnchors(bottomRow);
		string[] allWorlds = GCNS.allWorlds;
		foreach (string text in allWorlds)
		{
			if (!text.IsTaro() || !scrPortal.portals.ContainsKey(text) || scrPortal.portals[text] == null || scrPortal.portals[text].jumpPosition != vector2Int)
			{
				continue;
			}
			if (ADOBase.sceneName == "scnTaroMenu0")
			{
				UpdateAnchors(text.EndsWith("EX"));
				ShowText(text);
				return;
			}
			if (ADOBase.sceneName == "scnTaroMenu1")
			{
				if ((text == "T3" && unlockedWorldT3) || text == "T1" || text == "T2")
				{
					ShowText(text);
					return;
				}
				continue;
			}
			if (ADOBase.sceneName == "scnTaroMenu2")
			{
				if (!(text == "T4") || !beatWorldT4)
				{
					switch (text)
					{
					case "T3":
					case "T1":
					case "T2":
						break;
					case "T4":
						if (beatWorldT4)
						{
							continue;
						}
						UpdateAnchors(atTop: false);
						ShowTextRaw("???");
						return;
					default:
						continue;
					}
				}
				UpdateAnchors(atTop: true);
				ShowText(text);
				return;
			}
			if (ADOBase.sceneName == "scnTaroMenu3")
			{
				switch (text)
				{
				case "T5":
				case "T4":
				case "T3":
				case "T1":
				case "T2":
					UpdateAnchors(text != "T5");
					ShowText(text);
					return;
				}
			}
		}
		ShowText(null);
	}

	private void UpdateAnchors(bool atTop)
	{
		RectTransform component = GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, atTop ? 1 : 0);
		component.anchorMax = new Vector2(1f, atTop ? 1 : 0);
		component.pivot = new Vector2(0.5f, atTop ? 0.85f : 0f);
	}

	public void ShowTextRaw(string t)
	{
		if (text == null)
		{
			text = GetComponent<Text>();
		}
		text.text = t;
	}

	public void ShowText(string world)
	{
		if (!(world == "B") || Persistence.GetOverallProgressStage() >= 7)
		{
			if (text == null)
			{
				text = GetComponent<Text>();
			}
			text.text = ((world == null) ? "" : (scrPortal.portals[world].locked ? RDString.Get("levelSelect.locked") : RDString.Get("world" + world + ".description")));
		}
	}
}
