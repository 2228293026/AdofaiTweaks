using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class TaroCutsceneScript : ADOBase
{
	[Header("Base Cutscene Fields")]
	public scrDialogBox scene_assets;

	public TextMeshProUGUI scene_text;

	private TMP_LetterMovement scene_text_movement;

	private Image scene_text_bg;

	private Image scene_text_nxt;

	public List<string> dialog = new List<string>();

	public List<TextEvent> textEvents = new List<TextEvent>();

	private int curTextEvent;

	public List<SpeechEvent> speechEvents = new List<SpeechEvent>();

	private int curSpeechEvent;

	public Dictionary<int, int> charaSwitch = new Dictionary<int, int>();

	public List<Mawaru_Sprite> characters = new List<Mawaru_Sprite>();

	public Dictionary<string, Action> runnables = new Dictionary<string, Action>();

	private float uptime;

	private float charDisplayTime = 0.04f;

	private bool displayingText;

	private float textWait;

	private bool skip;

	public bool canSkip = true;

	public bool canAdvance = true;

	public bool displayBox = true;

	public bool scene_ended;

	public float scene_end_delay = 0.05f;

	private float speech_sound_delay = 0.07f;

	public float speech_sound_timer;

	public SfxSound speech_previous_talking = SfxSound.SpeechOverseer;

	public SfxSound speech_current_talking = SfxSound.SpeechOverseer;

	private float speech_default_volume = 0.5f;

	private float speech_volume = 0.5f;

	public float fontScale = 1f;

	private string lineNoRich;

	private int actualLength;

	private string scene_text_noRich = "";

	public int curTextString;

	public Color whiteClear = new Color(1f, 1f, 1f, 0f);

	public int curSwitch;

	private Color notTalking = new Color(0.5f, 0.5f, 0.5f, 1f);

	private bool biggerTextForMobile
	{
		get
		{
			if (ADOBase.isMobileMenu)
			{
				return !IsTabletAspectRatio();
			}
			return false;
		}
	}

	public void SetupFonts()
	{
		FontData fontData = RDString.fontData;
		fontScale = ((fontData.fontScale < 1f) ? fontData.fontScale : (fontData.fontScale / 1.25f));
		scene_text.font = fontData.fontTMP;
		scene_text.lineSpacing = -40f;
		if (RDString.language == SystemLanguage.Korean)
		{
			scene_text.lineSpacing = -25f;
		}
		else if (RDString.isChinese)
		{
			scene_text.lineSpacing = -25f;
		}
		else if (RDString.language == SystemLanguage.Japanese)
		{
			scene_text.lineSpacing = -25f;
		}
		float num = (biggerTextForMobile ? 1.2f : 1f);
		scene_text.fontSize = Mathf.RoundToInt(scene_text.fontSize * fontScale * num);
	}

	private string ParseText(string line)
	{
		int num = (biggerTextForMobile ? 60 : 45);
		string newValue = $"<size={num}>";
		string newValue2 = "</size>";
		line = line.Replace("<small>", newValue).Replace("</small>", newValue2);
		StringBuilder stringBuilder = new StringBuilder();
		lineNoRich = Regex.Replace(line, "<.*?>", string.Empty);
		List<string> list = new List<string>();
		string text = "";
		int num2 = 0;
		int num3 = -1;
		int num4 = 0;
		bool flag = false;
		int num5 = 0;
		for (int i = 0; i < lineNoRich.Length; i++)
		{
			char c = lineNoRich[i];
			if (!flag)
			{
				if (i < lineNoRich.Length - 1)
				{
					if (c == ',')
					{
						textEvents.Add(new TextEvent(num4 + 1, 0, new List<string> { "p", "0.15" }));
					}
					if (c == '.' && i < lineNoRich.Length - 1)
					{
						textEvents.Add(new TextEvent(num4 + 1, 0, new List<string> { "p", "0.25" }));
					}
					if (c == '!' && i < lineNoRich.Length - 1 && lineNoRich[i + 1] != '!')
					{
						textEvents.Add(new TextEvent(num4 + 1, 0, new List<string> { "p", "0.25" }));
					}
					if (c == '?' && i < lineNoRich.Length - 1 && lineNoRich[i + 1] != '?')
					{
						textEvents.Add(new TextEvent(num4 + 1, 0, new List<string> { "p", "0.25" }));
					}
				}
				if (c == '`')
				{
					flag = true;
					num3 = -1;
					text = "";
					num2 = 0;
					list = new List<string>();
					continue;
				}
				switch (num5)
				{
				case 1:
					scene_text_movement.AddCharIndexToShake(num4);
					break;
				case 2:
					scene_text_movement.AddCharIndexToWave(num4);
					break;
				}
				num4++;
				continue;
			}
			switch (c)
			{
			case ';':
				flag = false;
				if (text != "")
				{
					list.Add(text);
				}
				if (num3 > -1)
				{
					textEvents.Add(new TextEvent(num4, num3, list));
				}
				break;
			case ',':
				if (text != "")
				{
					list.Add(text);
				}
				text = "";
				num2++;
				break;
			case 'c':
				if (list.Count == 0)
				{
					num3 = 1;
				}
				text += c;
				break;
			case 'p':
				if (list.Count == 0)
				{
					num3 = 0;
				}
				text += c;
				break;
			case 'f':
				if (list.Count == 0)
				{
					num3 = 2;
				}
				text += c;
				break;
			case 's':
				if (list.Count == 0)
				{
					num3 = 3;
				}
				text += c;
				break;
			case 'v':
				if (list.Count == 0)
				{
					num5 = 1;
				}
				text += c;
				break;
			case 'w':
				if (list.Count == 0)
				{
					num5 = 2;
				}
				text += c;
				break;
			case 'n':
				if (list.Count == 0)
				{
					num5 = 0;
				}
				text += c;
				break;
			default:
				text += c;
				break;
			}
		}
		string text2 = line;
		foreach (char c2 in text2)
		{
			if (!flag)
			{
				if (c2 == '`')
				{
					flag = true;
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			else if (c2 == ';')
			{
				flag = false;
			}
		}
		actualLength = num4;
		return stringBuilder.ToString();
	}

	private string ParseForVoiceBlips(string line)
	{
		StringBuilder stringBuilder = new StringBuilder();
		lineNoRich = Regex.Replace(line, "<.*?>", string.Empty);
		new List<string>();
		string text = "";
		int num = 0;
		bool flag = false;
		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];
			if (!flag)
			{
				if (c == '<')
				{
					flag = true;
					text = "";
				}
				else
				{
					num++;
				}
			}
			else if (c == '>')
			{
				flag = false;
				speechEvents.Add(new SpeechEvent(num, text));
			}
			else
			{
				text += c;
			}
		}
		foreach (char c2 in line)
		{
			if (!flag)
			{
				if (c2 == '<')
				{
					flag = true;
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			else if (c2 == '>')
			{
				flag = false;
			}
		}
		return stringBuilder.ToString();
	}

	private bool IsTabletAspectRatio()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 1.5f;
		return num <= num2 + float.Epsilon;
	}

	private void DisplayText(int i)
	{
		speech_sound_timer = 0f;
		textWait = 0f;
		curTextString = i;
		scene_text_movement.ClearTextEffects();
		textEvents.Clear();
		speechEvents.Clear();
		curTextEvent = 0;
		curSpeechEvent = 0;
		if (curTextString < dialog.Count)
		{
			for (int j = 0; j < scene_text.transform.childCount; j++)
			{
				scene_text.transform.GetChild(j).gameObject.SetActive(value: false);
				scene_text.transform.GetChild(j).gameObject.SetActive(value: true);
			}
			scene_text.text = ParseText(dialog[curTextString]);
			scene_text_noRich = ParseForVoiceBlips(scene_text.text);
			if (displayBox)
			{
				scene_text.transform.localPosition = Vector3.up * -325f;
				scene_assets.transform.localPosition = Vector3.up * 0f;
				if (biggerTextForMobile)
				{
					scene_assets.transform.PositionY(scene_assets.transform.position.y * 1.2f);
				}
				if ((bool)ADOBase.controller.errorMeter)
				{
					ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			if (ADOBase.controller.gameworld && displayBox && (bool)ADOBase.controller.errorMeter)
			{
				ADOBase.controller.errorMeter.wrapperRectTransform.gameObject.SetActive(value: true);
			}
			scene_assets.transform.localPosition = Vector3.up * -9999f;
			scene_text.text = "";
			scene_text_noRich = "";
			scene_text.transform.localPosition = Vector3.up * -9999f;
			scene_ended = true;
			dialog.Clear();
			displayingText = false;
			if (runnables.ContainsKey("OnComplete"))
			{
				runnables["OnComplete"]();
			}
		}
		scene_text.maxVisibleCharacters = 0;
		ResetSpeech();
		CheckTextEvents(0);
		CheckSpeechEvents(0);
	}

	public void AdvanceText()
	{
		if (curTextString < dialog.Count)
		{
			curTextString++;
			DisplayText(curTextString);
		}
	}

	public void Awake()
	{
		scene_text = scene_assets.text;
		scene_text_bg = scene_assets.bg;
		scene_text_nxt = scene_assets.nxt;
		scene_text_movement = scene_text.GetComponent<TMP_LetterMovement>();
		scene_text_nxt.enabled = false;
		SetupFonts();
		scene_text.text = "";
		scene_assets.transform.localPosition = Vector3.up * -9999f;
		scene_text.transform.localPosition = Vector3.up * -9999f;
		if (biggerTextForMobile)
		{
			scene_assets.transform.ScaleXY(1.2f);
			scene_text.fontSize *= 0.8f;
		}
	}

	private void Start()
	{
		ADOBase.controller.isCutscene = true;
	}

	private void CheckTextEvents(int c)
	{
		if (textEvents.Count > 0)
		{
			while (curTextEvent < textEvents.Count && c >= textEvents[curTextEvent].iTime)
			{
				RunTextEvent(textEvents[curTextEvent]);
				curTextEvent++;
			}
		}
	}

	private void CheckSpeechEvents(int c)
	{
		if (speechEvents.Count > 0)
		{
			while (curSpeechEvent < speechEvents.Count && c >= speechEvents[curSpeechEvent].iTime)
			{
				RunSpeechEvent(speechEvents[curSpeechEvent]);
				curSpeechEvent++;
			}
		}
	}

	private void RunTextEvent(TextEvent t)
	{
		string text = $"Run Text Event @ {t.iTime}: {t.iEvent}";
		for (int i = 0; i < t.args.Count; i++)
		{
			text = text + "," + t.args[i];
		}
		switch (t.iEvent)
		{
		case 0:
			textWait -= float.Parse(t.args[1]);
			break;
		case 1:
		{
			int index = int.Parse(t.args[1]);
			int state = int.Parse(t.args[2]);
			characters[index].SetState(state);
			break;
		}
		case 2:
			runnables[t.args[1]]();
			break;
		case 3:
			charDisplayTime = float.Parse(t.args[1]);
			break;
		}
	}

	private void RunSpeechEvent(SpeechEvent s)
	{
		_ = $"Run Speech Event @ {s.iTime}: {s.sTag}";
		if (s.sTag.ToLower().StartsWith("color"))
		{
			speech_previous_talking = speech_current_talking;
		}
		if (s.sTag.ToLower() == "color=#ffffff")
		{
			speech_current_talking = SfxSound.SpeechOverseer;
		}
		if (s.sTag.ToLower() == "color=#ff9999")
		{
			speech_current_talking = SfxSound.SpeechCharlie;
		}
		if (s.sTag.ToLower() == "color=#cc6600")
		{
			speech_current_talking = SfxSound.SpeechSef;
		}
		if (s.sTag.ToLower() == "color=#cbe1ff")
		{
			speech_current_talking = SfxSound.PowerUp;
		}
		if (s.sTag.ToLower() == "/color")
		{
			speech_current_talking = speech_previous_talking;
		}
		if (s.sTag.ToLower() == "size=45")
		{
			speech_volume = 0.2f;
		}
		if (s.sTag.ToLower() == "/size")
		{
			speech_volume = speech_default_volume;
		}
	}

	public void StartScene()
	{
		DisplayText(0);
		displayingText = true;
		scene_ended = false;
	}

	public void Update()
	{
		uptime += Time.deltaTime;
		if (speech_sound_timer >= 0f)
		{
			speech_sound_timer -= Time.deltaTime;
		}
		if (ADOBase.controller.playerManager.AnyValidInputWasTriggered() && uptime > 1f)
		{
			if (!displayingText)
			{
				DisplayText(0);
				displayingText = true;
			}
			else if (scene_text.maxVisibleCharacters < actualLength + 1)
			{
				if (canSkip)
				{
					skip = true;
				}
			}
			else if (canAdvance)
			{
				AdvanceText();
			}
		}
		if (scene_ended && scrController.instance.isCutscene)
		{
			scene_end_delay -= Time.deltaTime;
			if (scene_end_delay < 0f)
			{
				scrController.instance.isCutscene = false;
			}
		}
		scene_text_nxt.transform.localPosition = Vector3.right * 590f + Vector3.up * -436f + Vector3.up * 3f * Mathf.Sin(Time.time * 4f * (float)Math.PI);
		if (!displayingText)
		{
			return;
		}
		if (!skip)
		{
			if (scene_text.maxVisibleCharacters < actualLength + 2)
			{
				textWait += Time.deltaTime;
				if (textWait >= charDisplayTime)
				{
					CheckSpeechEvents(scene_text.maxVisibleCharacters);
					if (scene_text_noRich != "" && scene_text.maxVisibleCharacters < scene_text_noRich.Length)
					{
						VoiceBlip(scene_text_noRich[scene_text.maxVisibleCharacters]);
					}
					scene_text.maxVisibleCharacters++;
					CheckTextEvents(scene_text.maxVisibleCharacters);
					textWait -= charDisplayTime;
				}
			}
			if (scene_text.maxVisibleCharacters >= actualLength + 1)
			{
				if (!scene_text_nxt.enabled && canAdvance)
				{
					scene_text_nxt.enabled = true;
				}
			}
			else if (scene_text_nxt.enabled)
			{
				scene_text_nxt.enabled = false;
			}
		}
		else
		{
			while (scene_text.maxVisibleCharacters < actualLength + 1)
			{
				scene_text.maxVisibleCharacters++;
				CheckTextEvents(scene_text.maxVisibleCharacters);
			}
			skip = false;
		}
	}

	public void CharFadeIn(Mawaru_Sprite c, float dur, float delay = 0f)
	{
		c.render.enabled = true;
		DOTween.Sequence().AppendInterval(delay).Append(c.render.material.DOColor(Color.white, dur).SetEase(Ease.Linear));
	}

	public void CharFadeOut(Mawaru_Sprite c, float dur, float delay = 0f)
	{
		DOTween.Sequence().AppendInterval(delay).Append(c.render.material.DOColor(whiteClear, dur).SetEase(Ease.Linear).OnComplete(delegate
		{
			c.render.enabled = false;
		}));
	}

	public void SetSpeaking(Mawaru_Sprite c)
	{
		c.render.material.DOColor(Color.white, 0.3f).SetEase(Ease.Linear);
	}

	public void SetNotSpeaking(Mawaru_Sprite c)
	{
		c.render.material.DOColor(notTalking, 0.3f).SetEase(Ease.Linear);
	}

	private void ResetSpeech()
	{
		speech_previous_talking = SfxSound.SpeechOverseer;
		speech_current_talking = SfxSound.SpeechOverseer;
		speech_volume = speech_default_volume;
	}

	private void VoiceBlip(char c)
	{
		if (speech_sound_timer <= 0f && char.IsLetterOrDigit(c) && speech_current_talking > SfxSound.PowerUp)
		{
			scrSfx.instance.PlaySfx(speech_current_talking, MixerGroup.InterfaceParent, speech_volume, UnityEngine.Random.Range(1f, 1.1f));
			speech_sound_timer = speech_sound_delay;
		}
	}
}
