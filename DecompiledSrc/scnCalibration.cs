using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scnCalibration : ADOBase
{
	private enum InputType
	{
		None,
		Keyboard,
		Mouse,
		Joystick,
		Touch,
		Mixed
	}

	private enum Feedback
	{
		OK,
		Inconsistent,
		Sparse
	}

	private enum Rank
	{
		E,
		D,
		C,
		B,
		A,
		S
	}

	[Serializable]
	public struct OffsetPair(double offset, scrCalibrax calibrationX)
	{
		public double offset = offset;

		public scrCalibrax calibrationX = calibrationX;
	}

	[NonSerialized]
	public List<OffsetPair> listOffsets = new List<OffsetPair>();

	[NonSerialized]
	public float radius = 1f;

	[NonSerialized]
	public double angleRadians;

	[NonSerialized]
	public double averageTimeOffset;

	[NonSerialized]
	public double averageAngleOffset;

	[NonSerialized]
	public double standardDeviation;

	[NonSerialized]
	public float keyCooldown;

	public new scrConductor conductor;

	public EventSystem eventSystem;

	public GameObject calibrationPlanet;

	public SpriteRenderer calibrationPlanetSpriteRenderer;

	public TrailRenderer calibrationPlanetTrailRenderer;

	public ParticleSystem calibrationPlanetDeathExplosion;

	public GameObject otherPlanet;

	public scrCalibrax calibrax;

	public Transform ring;

	public Text txtMessage;

	public Text txtOffsetIndicator;

	public Text txtResults;

	public AudioClip desktopCalibrationMusic;

	public AudioClip applause;

	public AudioClip finishSound;

	public AudioClip explosion;

	public GameObject quitButton;

	public ScrCallibrationBGLines BGLines;

	[Header("Details Panel")]
	public RectTransform detailsPanel;

	public RectTransform detailsTab;

	public Button detailsTabButton;

	public Text calibrationInfo;

	public Text privacyPolicyText;

	public Button sendButton;

	public Image sendImage;

	public Text sendButtonLabel;

	public Color loadingColor;

	public Color successColor;

	public Color errorColor;

	private InputType inputType;

	private bool showingInformationPanel;

	private int currentMessageNumber;

	private float overloadCounter;

	private float overloadDamagePerPress = 0.3f;

	private float overloadCooldown = 1f;

	private bool dead;

	private bool calibrated;

	private bool quitting;

	private string exportInputOffset;

	private string exportStandardDeviation;

	private string exportOutputName;

	private bool firstTimeCalibration;

	private void Start()
	{
		if (GCS.lastVisitedScene == "scnSplash")
		{
			firstTimeCalibration = true;
			quitButton.SetActive(value: false);
		}
		detailsPanel.gameObject.SetActive(value: false);
		detailsPanel.AnchorPosX(0f - detailsTab.sizeDelta.x);
		txtMessage.text = "";
		txtOffsetIndicator.gameObject.SetActive(value: false);
		txtResults.text = "";
		SetMessageNumber(0);
		calibrationInfo.SetLocalizedFont();
		conductor = scrConductor.instance;
		if (!ADOBase.isMobile && !ADOBase.isSwitch)
		{
			conductor.song.clip = desktopCalibrationMusic;
			conductor.bpm = 130f;
		}
		Material material = calibrationPlanetSpriteRenderer.material;
		Color color = material.color;
		color.a = 0f;
		material.color = color;
		calibrationPlanetTrailRenderer.enabled = false;
		txtMessage.SetLocalizedFont();
		txtOffsetIndicator.SetLocalizedFont();
		txtResults.SetLocalizedFont();
		scrController.CheckForAudioOutputChange();
	}

	private void SetMessageNumber(int n)
	{
		currentMessageNumber = n;
		if (n == 3)
		{
			txtMessage.text = RDString.Get("status.overload");
			return;
		}
		txtMessage.text = RDString.Get("calibration." + currentMessageNumber);
		if (currentMessageNumber <= 2)
		{
			txtMessage.text = txtMessage.text.Replace("[output]", "<b>" + scrConductor.currentPreset.ReadableOutputName() + "</b>");
		}
	}

	private void FadeInPlanet(float delay, float duration, float startAlpha = 0f, bool shouldScale = false)
	{
		Sequence sequence = DOTween.Sequence();
		Material material = calibrationPlanetSpriteRenderer.material;
		Color color = material.color;
		color.a = 1f;
		material.color = new Color(1f, 1f, 1f, startAlpha);
		if (shouldScale)
		{
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = new Vector3(0.055f, 0.055f, 1f);
			sequence.Append(base.transform.DOScale(localScale, 0.5f));
		}
		sequence.Insert(delay, material.DOColor(color, duration).SetEase(Ease.InOutQuad));
		sequence.Play();
	}

	private void FadeOutPlanet(float delay, float duration, bool shouldScale)
	{
		Sequence sequence = DOTween.Sequence();
		Material material = calibrationPlanetSpriteRenderer.material;
		Color color = material.color;
		color.a = 0f;
		if (shouldScale)
		{
			sequence.Append(ShortcutExtensions.DOScale(endValue: new Vector3(5f, 5f, 1f), target: base.transform, duration: 0.5f));
		}
		sequence.Insert(delay, material.DOColor(color, duration).SetEase(Ease.InOutQuad));
		sequence.Play();
	}

	private void Calibrated(Rank rank)
	{
		calibrated = true;
		conductor.song2.PlayOneShot(finishSound);
		SetMessageNumber(2);
		FadeOutPlanet(0f, 0.5f, shouldScale: false);
		calibrationPlanetTrailRenderer.enabled = false;
		conductor.song.DOFade(0f, 3f).SetEase(Ease.OutExpo);
		PlayerPrefs.SetInt("maxcalibrationrank", (int)rank);
		PlayerPrefs.Save();
		double num = Math.Round(averageTimeOffset * 1000.0);
		string text = num + RDString.Get("editor.unit.ms");
		string text2 = rank.ToString();
		txtResults.text = RDString.Get("calibration.offset") + " <b>" + text + "</b>\n" + RDString.Get("calibration.skill") + " <b>" + text2 + "</b>";
		txtOffsetIndicator.gameObject.SetActive(value: false);
		exportOutputName = scrConductor.currentPreset.outputName;
		string[] array = new string[3] { "AirPods Pro", "AirPods", "Powerbeats Pro" };
		foreach (string value in array)
		{
			if (exportOutputName.Contains(value))
			{
				exportOutputName = value;
				break;
			}
		}
		exportInputOffset = num.ToString();
		exportStandardDeviation = (standardDeviation * 1000.0).ToString("0.000");
		calibrationInfo.text = RDString.Get("calibration.inputOffset") + ": " + exportInputOffset + "ms\n" + RDString.Get("calibration.standardDeviation") + ": " + exportStandardDeviation + "ms\n" + RDString.Get("calibration.operatingSystem") + ": " + SystemInfo.operatingSystem + "\n" + RDString.Get("calibration.deviceModel") + ": " + SystemInfo.deviceModel + "\n" + string.Format("{0}: {1}\n", RDString.Get("calibration.inputType"), inputType) + string.Format("{0}: {1}\n", RDString.Get("calibration.outputType"), scrConductor.currentPreset.outputType) + RDString.Get("calibration.outputName") + ": " + exportOutputName + "\n";
		scrConductor.currentPreset.inputOffset = Mathf.RoundToInt((float)(averageTimeOffset * 1000.0));
		scrConductor.SaveCurrentPreset();
		if (!ADOBase.isSwitch)
		{
			ShowDetailsTab(show: true);
		}
		keyCooldown = 1f;
		BGLines.MakeAxisLines();
	}

	private void Update()
	{
		angleRadians = 1.5707963705062866 + (conductor.songposition_minusi + (double)(scrConductor.calibration_i * conductor.song.pitch)) / conductor.crotchetAtStart * 3.1415927410125732;
		if (!dead)
		{
			Vector3 position = otherPlanet.transform.position;
			calibrationPlanet.transform.position = new Vector3(position.x + Mathf.Sin((float)angleRadians) * radius, position.y + Mathf.Cos((float)angleRadians) * radius, position.z);
		}
		ring.Rotate(Vector3.back, -30f * Time.unscaledDeltaTime);
		if (keyCooldown <= 0f)
		{
			InputType inputTypeForDown = GetInputTypeForDown();
			bool num = inputTypeForDown == InputType.Keyboard;
			bool flag = inputTypeForDown == InputType.Mouse && eventSystem.currentSelectedGameObject == null;
			bool flag2 = inputTypeForDown == InputType.Touch && eventSystem.currentSelectedGameObject == null;
			if (num || flag || flag2)
			{
				if (currentMessageNumber == 0)
				{
					conductor.StartMusic();
					FadeInPlanet(0f, 0.5f, 0.5f, shouldScale: true);
					calibrationPlanetTrailRenderer.enabled = true;
					SetMessageNumber(currentMessageNumber + 1);
				}
				else if (currentMessageNumber == 1)
				{
					if (inputType == InputType.None)
					{
						inputType = inputTypeForDown;
					}
					else if (inputType != InputType.Mixed && inputType != inputTypeForDown)
					{
						inputType = InputType.Mixed;
					}
					PutDataPoint();
					overloadCounter += overloadDamagePerPress;
				}
				else if (currentMessageNumber == 2)
				{
					Quit();
				}
				else if (currentMessageNumber == 3)
				{
					CleanSlate();
				}
			}
		}
		if (RDInput.cancelPress && !firstTimeCalibration)
		{
			scrSfx.instance.PlaySfx(SfxSound.MenuBack, MixerGroup.InterfaceParent);
			Quit();
		}
		if (keyCooldown > 0f)
		{
			keyCooldown -= Time.unscaledDeltaTime;
		}
		overloadCounter -= (float)((double)overloadCooldown * conductor.deltaSongPos / conductor.crotchetAtStart);
		overloadCounter = Mathf.Max(overloadCounter, 0f);
		if (overloadCounter > 1f && !dead && !calibrated)
		{
			SetMessageNumber(3);
			conductor.song.DOFade(0f, 3f).SetEase(Ease.OutExpo);
			scrCalibrationLine.instance.FadeOut();
			keyCooldown = 1f;
			ExplodePlanet();
		}
	}

	private void ExplodePlanet()
	{
		dead = true;
		FadeOutPlanet(0f, 0.1f, shouldScale: false);
		calibrationPlanetTrailRenderer.enabled = false;
		calibrationPlanetDeathExplosion.gameObject.SetActive(value: true);
		conductor.song2.volume = 0.75f;
		conductor.song2.PlayOneShot(explosion);
		txtOffsetIndicator.gameObject.SetActive(value: false);
	}

	private void CleanSlate()
	{
		SetMessageNumber(1);
		conductor.song.Stop();
		conductor.StartMusic();
		conductor.song.DOKill();
		conductor.song.volume = 1f;
		FadeInPlanet(0f, 0.5f, 0.5f, shouldScale: true);
		calibrationPlanetTrailRenderer.enabled = true;
		calibrationPlanetDeathExplosion.gameObject.SetActive(value: false);
		dead = false;
		overloadCounter = 0f;
		inputType = InputType.None;
		foreach (OffsetPair listOffset in listOffsets)
		{
			UnityEngine.Object.Destroy(listOffset.calibrationX.gameObject);
		}
		listOffsets.Clear();
		ShowDetailsTab(show: false);
	}

	private void PutDataPoint()
	{
		scrMisc.Vibrate(50L);
		double offset = GetOffset(angleRadians, conductor.bpm);
		listOffsets.Add(new OffsetPair(offset, UnityEngine.Object.Instantiate(calibrax, calibrationPlanet.transform.position, Quaternion.identity)));
		if (listOffsets.Count == 4)
		{
			scrCalibrationLine.instance.FadeIn();
			txtOffsetIndicator.gameObject.SetActive(value: true);
			txtOffsetIndicator.DOFade(1f, 0.5f).SetEase(Ease.InQuad).From(0f);
		}
		else if (listOffsets.Count > 15)
		{
			listOffsets[0].calibrationX.FadeAndDestroy();
			listOffsets.RemoveAt(0);
		}
		Rank rank = CheckConsistency();
		if (listOffsets.Count > 10 && rank >= Rank.A)
		{
			conductor.song2.volume = 1f;
			conductor.song2.PlayOneShot(applause);
			Calibrated(rank);
		}
		else if (listOffsets.Count >= 15 && rank >= Rank.D)
		{
			Calibrated(rank);
		}
	}

	private double GetOffset(double angleRad, double bpm)
	{
		angleRad %= 6.2831854820251465;
		double num = 0.0;
		double num2 = ((!ADOBase.isMobile) ? 0.7853981852531433 : 1.5707963705062866);
		double num3 = num2;
		double num4 = 3.1415927410125732 + num2;
		if (angleRad < num4 && angleRad >= num3)
		{
			num = 1.5707963705062866;
		}
		if (angleRad >= num4)
		{
			num = 4.71238899230957;
		}
		if (angleRad < num3)
		{
			num = -1.5707963705062866;
		}
		return (angleRad - num) * (60.0 / bpm) / 3.1415927410125732;
	}

	private double StandardDeviation(List<OffsetPair> offsetPairs)
	{
		double num = offsetPairs.Sum((OffsetPair x) => x.offset) / (double)offsetPairs.Count;
		double num2 = 0.0;
		foreach (OffsetPair offsetPair in offsetPairs)
		{
			num2 += Math.Pow(offsetPair.offset - num, 2.0);
		}
		return Math.Sqrt(num2 / (double)offsetPairs.Count);
	}

	private Rank CheckConsistency()
	{
		List<OffsetPair> list = new List<OffsetPair>(listOffsets);
		int num = 0;
		foreach (OffsetPair item in list)
		{
			if (item.offset < 0.0)
			{
				num++;
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (num > list.Count / 2)
			{
				if (list[i].offset > 0.0)
				{
					OffsetPair value = list[i];
					value.offset = list[i].offset - (double)(60f / conductor.bpm);
					list[i] = value;
				}
			}
			else if (list[i].offset < 0.0)
			{
				OffsetPair value2 = list[i];
				value2.offset = list[i].offset + (double)(60f / conductor.bpm);
				list[i] = value2;
			}
		}
		double num2 = StandardDeviation(listOffsets);
		double num3 = StandardDeviation(list);
		List<OffsetPair> list2 = new List<OffsetPair>((num2 < num3) ? listOffsets : list);
		list2.Sort((OffsetPair s1, OffsetPair s2) => s1.offset.CompareTo(s2.offset));
		double num4 = 0.0;
		num4 = ((list2.Count % 2 != 0) ? list2[(list2.Count - 1) / 2].offset : list2[list2.Count / 2].offset);
		double num5 = (double)(60f / conductor.bpm) * 0.25;
		int num6 = 0;
		for (int num7 = 0; num7 < list2.Count; num7++)
		{
			if (Math.Abs(list2[num7].offset - num4) > num5)
			{
				list2[num7].calibrationX.SetOutlier(isOutlier: true);
				list2.RemoveAt(num7);
				num7--;
				num6++;
			}
			else
			{
				list2[num7].calibrationX.SetOutlier(isOutlier: false);
			}
		}
		averageTimeOffset = list2.Sum((OffsetPair x) => x.offset) / (double)list2.Count;
		averageAngleOffset = averageTimeOffset / (double)(60f / conductor.bpm) * 3.1415927410125732;
		txtOffsetIndicator.text = Math.Round(averageTimeOffset * 1000.0) + RDString.Get("editor.unit.ms");
		if (num6 < 3)
		{
			standardDeviation = StandardDeviation(list2);
			if (standardDeviation < 0.01)
			{
				return Rank.S;
			}
			if (standardDeviation < 0.015)
			{
				return Rank.A;
			}
			if (standardDeviation < 0.02)
			{
				return Rank.B;
			}
			if (standardDeviation < 0.025)
			{
				return Rank.C;
			}
			if (standardDeviation < ((ADOBase.isMobile || ADOBase.isSwitch) ? 0.06 : 0.05))
			{
				return Rank.D;
			}
			return Rank.E;
		}
		return Rank.E;
	}

	private void Quit()
	{
		if (!quitting)
		{
			quitting = true;
			DOTween.KillAll();
			string scene = (GCS.webVersion ? "scnIntro" : GCNS.sceneLevelSelect);
			if (GCS.lastVisitedScene != "")
			{
				scene = GCS.lastVisitedScene;
			}
			ADOBase.loader.LoadSceneWithTransition(WipeDirection.StartsFromLeft, scene);
		}
	}

	public void ShowDetailsTab(bool show)
	{
		if (show)
		{
			detailsPanel.gameObject.SetActive(value: true);
		}
		float endValue = (show ? 0f : (0f - detailsPanel.sizeDelta.x));
		detailsPanel.DOKill();
		detailsPanel.DOAnchorPosX(endValue, 0.3f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		detailsTabButton.enabled = show;
		if (!ADOBase.isMobile)
		{
			privacyPolicyText.gameObject.SetActive(value: false);
			sendButton.gameObject.SetActive(value: false);
		}
	}

	public void DetailsTabClick()
	{
		showingInformationPanel = !showingInformationPanel;
		float endValue = (showingInformationPanel ? (detailsPanel.sizeDelta.x - 10f) : 0f);
		detailsPanel.DOKill();
		detailsPanel.DOAnchorPosX(endValue, 0.3f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
	}

	public void UploadData()
	{
		StartCoroutine(UploadDataCo());
	}

	private IEnumerator UploadDataCo()
	{
		sendButton.enabled = false;
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add((IMultipartFormSection)new MultipartFormDataSection("platform", ValidateString(ADOBase.platform.ToString())));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("operating_system", ValidateString(SystemInfo.operatingSystem.ToString())));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("device_model", ValidateString(SystemInfo.deviceModel.ToString())));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("input_type", ValidateString(inputType.ToString())));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("output_type", ValidateString(scrConductor.currentPreset.outputType.ToString())));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("output_name", ValidateString(exportOutputName)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("input_offset", ValidateString(exportInputOffset)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("priority", "1"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("confident", "true"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("standard_deviation", ValidateString(exportStandardDeviation)));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("game", "adofai"));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("release", 141.ToString()));
		list.Add((IMultipartFormSection)new MultipartFormDataSection("build", GCNS.buildCommit));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/submit-calibration", list);
		sendImage.color = loadingColor;
		sendButtonLabel.text = RDString.Get("calibration.uploadingProcess");
		sendButtonLabel.color = Color.black;
		yield return request.SendWebRequest();
		if (request.HasConnectionError())
		{
			sendImage.color = errorColor;
			sendButton.enabled = true;
			sendButtonLabel.text = RDString.Get("calibration.uploadingError", new Dictionary<string, object> { { "error", request.error } });
			sendButtonLabel.color = Color.white;
			Debug.Log(request.error);
		}
		else
		{
			sendImage.color = successColor;
			sendButtonLabel.text = RDString.Get("calibration.uploadingSuccess");
			sendButtonLabel.color = Color.black;
			Debug.Log("Form upload complete!");
		}
		static string ValidateString(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "[Not Found]";
			}
			return s;
		}
	}

	private InputType GetInputTypeForDown()
	{
		if (Input.touchCount > 0)
		{
			Touch[] touches = Input.touches;
			foreach (Touch touch in touches)
			{
				if (touch.phase == TouchPhase.Began)
				{
					return InputType.Touch;
				}
			}
		}
		else if (Input.anyKeyDown)
		{
			for (int j = 1; j < 600; j++)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					return InputType.Keyboard;
				}
				if (Input.GetKeyDown(KeyCode.Mouse0))
				{
					return InputType.Mouse;
				}
				if (Input.GetKeyDown((KeyCode)j))
				{
					if (j < 323)
					{
						return InputType.Keyboard;
					}
					if (j < 350)
					{
						return InputType.Mouse;
					}
					return InputType.Joystick;
				}
			}
		}
		return InputType.None;
	}

	public void OpenPrivacyPolicy()
	{
		ADOBase.platformHelper.OpenURL("https://7thbe.at/privacy");
	}

	public void QuitButton()
	{
		scrSfx.instance.PlaySfx(SfxSound.MenuBack, MixerGroup.InterfaceParent);
		Quit();
	}
}
