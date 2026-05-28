using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PracticeTimeline : ADOBase
{
	[Header("References")]
	public RectTransform timeline;

	public RectTransform leftArea;

	public RectTransform rightArea;

	public RectTransform playBar;

	public RectTransform border;

	public PauseLevelButton speedLeftButton;

	public PauseLevelButton speedButton;

	public PauseLevelButton speedRightButton;

	public PauseLevelButton startButton;

	public PauseLevelButton endButton;

	public TMP_InputField startInput;

	public TMP_InputField endInput;

	public TMP_Text speedText;

	public AudioSource audioSource;

	public RectTransform timelineWaveContainer;

	public RawImage waveRaw;

	public Gradient waveGradient;

	public CanvasScaler canvasScaler;

	public Material waveMaterial;

	[NonSerialized]
	public int practiceStart;

	[NonSerialized]
	public int practiceEnd;

	[Header("Private")]
	private RectTransform rt;

	private int difficultySegments = 100;

	private int levelLength;

	private int spoilerStart;

	private int speedPercent;

	private readonly int textureSize = 2048;

	private readonly int textureHeight = 256;

	private float[] segmentValues;

	private float[] floorTimings;

	private float levelDur;

	private bool dragging;

	private bool dragEndpoint;

	private bool initialized;

	private float defaultSongPitch = 1f;

	private const int minimumLength = 5;

	private PauseMenu pauseMenu => scrController.instance.pauseMenu;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
		waveMaterial = new Material(waveMaterial);
		Init();
	}

	private void AddListeners()
	{
		startInput.onSelect.AddListener(delegate
		{
			ADOBase.controller.pauseMenu.SelectVerticalFixed(1, -1);
		});
		startInput.onEndEdit.AddListener(delegate(string s)
		{
			ADOBase.controller.pauseMenu.SelectVerticalFixed(1, -1);
			if (int.TryParse(s, out var result))
			{
				practiceStart = result;
				UpdatePositions(changedEnd: false);
			}
		});
		endInput.onSelect.AddListener(delegate
		{
			ADOBase.controller.pauseMenu.SelectVerticalFixed(2, 1);
		});
		endInput.onEndEdit.AddListener(delegate(string s)
		{
			ADOBase.controller.pauseMenu.SelectVerticalFixed(2, 1);
			if (int.TryParse(s, out var result))
			{
				practiceEnd = result;
				UpdatePositions(changedEnd: true);
			}
		});
	}

	private void SetupSpeedButtons()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		int num2 = -1;
		bool flag = false;
		bool flag2 = false;
		if (ADOBase.isScnGame && !ADOBase.isOfficialLevel)
		{
			string[] customLevelPaths = GCS.customLevelPaths;
			string text = customLevelPaths[customLevelPaths.Length - 1];
			string json;
			if (ADOBase.isBundleLevel)
			{
				AsyncOperationHandle<TextAsset> val = Addressables.LoadAssetAsync<TextAsset>((object)text);
				json = val.WaitForCompletion().text;
				Addressables.Release<TextAsset>(val);
			}
			else
			{
				json = RDFile.ReadAllText(text);
			}
			Dictionary<string, object> rootDict = Json.DeserializePartially(json, "actions") as Dictionary<string, object>;
			LevelDataCLS levelDataCLS = new LevelDataCLS();
			levelDataCLS.Decode(rootDict);
			num2 = GCS.customLevelPaths.Length;
			int customLevelIndex = GCS.customLevelIndex;
			if (customLevelIndex == num2 - 1)
			{
				string iconPath = Path.Combine(Path.GetDirectoryName(text), levelDataCLS.previewIcon);
				StartCoroutine(PauseLevel.LoadIconCLS(iconPath, speedButton, levelDataCLS.previewIconColor, pauseMenu.unselectedIconColor));
			}
			else
			{
				num = customLevelIndex;
			}
		}
		else
		{
			string[] array = ADOBase.currentLevel.Split('-', StringSplitOptions.None);
			string text2 = array[0];
			flag = text2.IsCrownWorld();
			flag2 = text2.IsTaro();
			num2 = ADOBase.worldData[text2].levelCount;
			int num3 = ((array[1] == "X") ? num2 : int.Parse(array[1])) - 1;
			if (num3 == num2 - 1)
			{
				speedButton.background.sprite = Resources.Load<Sprite>("boss" + text2);
			}
			else
			{
				num = num3;
			}
		}
		if (num != -1)
		{
			speedButton.label.gameObject.SetActive(value: true);
			speedButton.label.text = (num + 1).ToString();
		}
		string variantKey = (flag ? "crown" : (flag2 ? "taro" : ""));
		Sprite orLoadSpriteVariant = PauseLevel.GetOrLoadSpriteVariant("submenu_blank", variantKey);
		speedLeftButton.button.onClick.AddListener(delegate
		{
			pauseMenu.SelectVerticalFixed(3, -1);
			ChangeSpeed(increase: false);
		});
		speedLeftButton.background.sprite = orLoadSpriteVariant;
		speedLeftButton.useIconColorForLabel = !flag2;
		speedRightButton.button.onClick.AddListener(delegate
		{
			pauseMenu.SelectVerticalFixed(3, 1);
			ChangeSpeed(increase: true);
		});
		speedRightButton.background.sprite = orLoadSpriteVariant;
		speedRightButton.useIconColorForLabel = !flag2;
		speedButton.restartLabel.text = RDString.Get("pauseMenu.restartPractice");
		speedButton.restartLabel.SetLocalizedFont();
		speedButton.button.onClick.AddListener(delegate
		{
			pauseMenu.SelectVerticalFixed(3, 0);
		});
		if (flag2)
		{
			speedLeftButton.icon.rectTransform.anchoredPosition = new Vector3(-0.7f, 0f);
			speedRightButton.icon.rectTransform.anchoredPosition = new Vector3(0.7f, 0f);
			Image icon = speedLeftButton.icon;
			Color color = (speedRightButton.icon.color = "D6D6D6".HexToColor());
			icon.color = color;
			RectTransform rectTransform = speedLeftButton.background.rectTransform;
			RectTransform rectTransform2 = speedRightButton.background.rectTransform;
			Vector2 vector = (speedButton.background.rectTransform.sizeDelta = PauseLevel.defaultButtonWidth * 1.3f * Vector2.one);
			Vector2 sizeDelta = (rectTransform2.sizeDelta = vector);
			rectTransform.sizeDelta = sizeDelta;
		}
		if (flag)
		{
			speedButton.background.DORainbow(10f, 0.5f, 1f, Ease.Linear).SetUpdate(isIndependentUpdate: true);
		}
	}

	private void Update()
	{
		rt.sizeDelta = rt.sizeDelta.WithX(rt.rect.width);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out var localPoint);
		float num = Mathf.InverseLerp(0f - rt.rect.size.x, rt.rect.size.x, localPoint.x * 2f);
		int num2 = TimeToFloor(num * levelDur);
		if (Input.GetMouseButtonDown(0))
		{
			Rect rect = timelineWaveContainer.rect;
			Vector2 vector = border.rect.size - rt.rect.size;
			rect.size += vector;
			rect.position -= vector / 2f;
			dragging = rect.Contains(localPoint);
			int num3 = (practiceStart + practiceEnd) / 2;
			dragEndpoint = num2 >= num3;
		}
		if (dragging)
		{
			if (!dragEndpoint)
			{
				practiceStart = num2;
			}
			else
			{
				practiceEnd = num2;
			}
			UpdatePositions(dragEndpoint);
			SongPlayback(play: false);
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (dragging)
			{
				SongPlayback(play: true);
			}
			dragging = false;
		}
		SongUpdate();
	}

	private void SongPlayback(bool play)
	{
		AudioSource audioSource = this.audioSource;
		if (play)
		{
			audioSource.clip = ADOBase.conductor.song.clip;
			if (!(audioSource.clip == null))
			{
				audioSource.volume = ADOBase.conductor.song.volume;
				audioSource.ignoreListenerPause = true;
				audioSource.Play();
				audioSource.time = (float)ADOBase.lm.listFloors[practiceStart].entryTime;
				float num = (float)speedPercent / 100f;
				audioSource.pitch = num * ((!ADOBase.isOfficialLevel) ? ((float)ADOBase.customLevel.levelData.pitch / 100f) : defaultSongPitch);
			}
		}
		else
		{
			audioSource.Pause();
		}
	}

	private void SongUpdate()
	{
		AudioSource audioSource = this.audioSource;
		if (practiceEnd < ADOBase.lm.listFloors.Count)
		{
			float num = (float)ADOBase.lm.listFloors[practiceEnd].entryTime;
			float num2 = (float)ADOBase.lm.listFloors.Last().entryTime;
			if (audioSource.time >= num)
			{
				SongPlayback(play: false);
			}
			playBar.gameObject.SetActive(audioSource.isPlaying);
			playBar.AnchorPosX(timelineWaveContainer.rect.size.x * (audioSource.time / num2));
		}
	}

	public void SetPositions()
	{
		if (GCS.practiceMode)
		{
			float num = (float)speedPercent / 100f;
			List<scrFloor> listFloors = ADOBase.lm.listFloors;
			while (practiceStart > 0 && (listFloors[practiceStart].midSpin || listFloors[practiceStart].freeroam))
			{
				practiceStart--;
			}
			while (practiceEnd < listFloors.Count - 1 && (listFloors[practiceEnd].midSpin || listFloors[practiceEnd].freeroam))
			{
				practiceEnd++;
			}
			int num2 = practiceEnd - practiceStart;
			if (GCS.checkpointNum != practiceStart || GCS.practiceLength != num2 || GCS.currentSpeedTrial != num)
			{
				ADOBase.controller.pauseMenu.requireRestart = true;
				GCS.checkpointNum = practiceStart;
				GCS.practiceLength = num2;
				GCS.nextSpeedRun = num;
				GCS.currentSpeedTrial = num;
			}
		}
	}

	public void ChangeSpeed(bool increase)
	{
		int num = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 1 : 5);
		int num2 = (increase ? 1 : (-1));
		speedPercent += num * num2;
		RectTransform target = (increase ? speedRightButton.icon.rectTransform : speedLeftButton.icon.rectTransform);
		target.DOComplete(withCallbacks: true);
		target.DOPunchAnchorPos(Vector2.zero.WithX(1f * (float)num2), 0.2f, 0).SetUpdate(isIndependentUpdate: true);
		SfxSound sfxSound = (increase ? SfxSound.MenuIncrement : SfxSound.MenuDecrement);
		scrSfx.instance.PlaySfx(sfxSound, MixerGroup.InterfaceParent);
		UpdateSpeed();
		SongPlayback(play: true);
	}

	private void UpdateSpeed()
	{
		speedPercent = Mathf.Clamp(speedPercent, 20, 1000);
		speedText.text = speedPercent + "%";
	}

	private int TimeToFloor(float pos)
	{
		return floorTimings.TakeWhile((float t) => t < pos).Count();
	}

	private float FloorToTime(int floor)
	{
		return floorTimings[Mathf.Clamp(floor, 0, levelLength - 1)];
	}

	public void UpdatePositions(bool changedEnd, bool updateVerticalIndex = true)
	{
		if (updateVerticalIndex)
		{
			ADOBase.controller.pauseMenu.SelectVerticalFixed((!changedEnd) ? 1 : 2, changedEnd ? 1 : (-1));
		}
		if (!changedEnd)
		{
			practiceStart = Mathf.Clamp(practiceStart, 0, levelLength - 1 - 5);
			practiceStart = Mathf.Min(practiceStart, spoilerStart - 5);
			practiceEnd = Mathf.Max(practiceEnd, practiceStart + 5);
		}
		else
		{
			practiceEnd = Mathf.Clamp(practiceEnd, 5, levelLength - 1);
			practiceEnd = Mathf.Min(practiceEnd, spoilerStart);
			practiceStart = Mathf.Min(practiceStart, practiceEnd - 5);
		}
		leftArea.SizeDeltaX(FloorToTime(practiceStart) / levelDur * rt.rect.size.x);
		waveMaterial.SetFloat("_ShadowLeft", FloorToTime(practiceStart) / levelDur);
		rightArea.SizeDeltaX((1f - FloorToTime(practiceEnd) / levelDur) * rt.rect.size.x);
		waveMaterial.SetFloat("_ShadowRight", FloorToTime(practiceEnd) / levelDur);
		startInput.text = practiceStart.ToString();
		endInput.text = practiceEnd.ToString();
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(FocusHack());
		}
		IEnumerator FocusHack()
		{
			yield return new WaitForEndOfFrame();
			startInput.interactable = false;
			startInput.interactable = true;
			endInput.interactable = false;
			endInput.interactable = true;
		}
	}

	public void Init()
	{
		bool flag = (bool)ADOBase.controller.currFloor && (ADOBase.controller.currFloor.freeroam || ADOBase.controller.currFloor.freeroamGenerated);
		if (!(ADOBase.controller.gameworld || flag) || !GCS.practiceMode)
		{
			timelineWaveContainer.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		timelineWaveContainer.gameObject.SetActive(value: true);
		playBar.gameObject.SetActive(value: false);
		floorTimings = ADOBase.lm.listFloors.Select((scrFloor f) => (float)f.entryTime).ToArray();
		levelLength = floorTimings.Count();
		levelDur = floorTimings.Last();
		practiceStart = GCS.checkpointNum;
		practiceEnd = practiceStart + GCS.practiceLength;
		speedPercent = Mathf.RoundToInt(GCS.currentSpeedTrial * 100f);
		if (ADOBase.controller.isbosslevel && ADOBase.isOfficialLevel && !Persistence.IsWorldComplete(scrController.currentWorld))
		{
			float num = Mathf.Max(Persistence.GetPercentCompletion(scrController.currentWorld), Persistence.GetPercentCompletion(scrController.currentWorld, coop: true));
			spoilerStart = Mathf.Max(20, Mathf.FloorToInt(num * (float)levelLength) + 10);
		}
		else
		{
			spoilerStart = levelLength;
		}
		UpdatePositions(changedEnd: false, updateVerticalIndex: false);
		UpdateSpeed();
		if (initialized)
		{
			return;
		}
		SetupSpeedButtons();
		AddListeners();
		initialized = true;
		segmentValues = new float[difficultySegments];
		float a = 999f;
		float a2 = 0f;
		float num2 = levelDur / (float)difficultySegments;
		int num3 = 0;
		float num4 = 0f;
		int num5 = 0;
		for (; num3 < levelLength - 1; num3++)
		{
			float num6 = floorTimings[num3];
			float num7 = (float)(num5 + 1) * num2;
			if (num6 > num7)
			{
				a = Mathf.Min(a, num4);
				a2 = Mathf.Max(a2, num4);
				segmentValues[num5] = num4;
				num5++;
				num4 = 0f;
			}
			else if (num6 >= (float)num5 * num2)
			{
				num4 += 1f;
			}
		}
		waveRaw.texture = CreateTexture();
		int num8 = Mathf.FloorToInt(waveRaw.rectTransform.rect.width * (float)Screen.width / canvasScaler.referenceResolution.x);
		int num9 = Mathf.FloorToInt(waveRaw.rectTransform.rect.height * (float)Screen.height / canvasScaler.referenceResolution.y);
		waveMaterial.SetFloat("_TextureWidth", num8);
		waveMaterial.SetFloat("_TextureHeight", num9);
		waveRaw.material = waveMaterial;
	}

	private float[] CreateArray()
	{
		float num = float.PositiveInfinity;
		float num2 = float.NegativeInfinity;
		float[] array = new float[segmentValues.Length];
		for (int i = 0; i < segmentValues.Length; i++)
		{
			float num3 = segmentValues[i];
			if (num3 < num)
			{
				num = num3;
			}
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		for (int j = 0; j < segmentValues.Length; j++)
		{
			float num4 = Mathf.InverseLerp(num, num2, segmentValues[j]);
			array[j] = num4;
		}
		return array;
	}

	private float[] ExpandArray(float[] values, int length)
	{
		float[] array = new float[length];
		for (int i = 0; i < length; i++)
		{
			float num = (float)i / (float)(length - 1) * (float)(values.Length - 1);
			int num2 = (int)num;
			int num3 = Mathf.Min(num2 + 1, values.Length - 1);
			float num4 = num - (float)num2;
			array[i] = values[num2] * (1f - num4) + values[num3] * num4;
		}
		return array;
	}

	private float[] BlurArray(float[] array, int radius, float sigma)
	{
		float[] array2 = new float[array.Length];
		float[] kernel = GetKernel(radius, sigma);
		for (int i = 0; i < array.Length; i++)
		{
			float num = 0f;
			float num2 = 0f;
			for (int j = -radius; j <= radius; j++)
			{
				int num3 = i + j;
				if (num3 >= 0 && num3 < array.Length)
				{
					float num4 = kernel[j + radius];
					num += array[num3] * num4;
					num2 += num4;
				}
			}
			array2[i] = num / num2;
		}
		return array2;
	}

	private float[] GetKernel(int radius, float sigma)
	{
		float[] array = new float[2 * radius + 1];
		float num = 1f / (Mathf.Sqrt((float)Math.PI * 2f) * sigma);
		float num2 = 0f;
		for (int i = -radius; i <= radius; i++)
		{
			array[i + radius] = num * Mathf.Exp((float)(-i * i / 2) * Mathf.Pow(sigma, 2f));
			num2 += array[i + radius];
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] /= num2;
		}
		return array;
	}

	private float[] SmoothArray(float[] array, int radius)
	{
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			float num = 0f;
			int num2 = 0;
			for (int j = Mathf.Max(0, i - radius); j <= Mathf.Min(array.Length - 1, i + radius); j++)
			{
				num += array[j];
				num2++;
			}
			array2[i] = num / (float)num2;
		}
		return array2;
	}

	private Texture2D CreateTexture()
	{
		float[] values = CreateArray();
		values = ExpandArray(values, textureSize);
		values = BlurArray(values, 50, 2f);
		values = SmoothArray(values, 15);
		Texture2D texture2D = new Texture2D(textureSize, textureHeight, TextureFormat.RGBA32, mipChain: false);
		for (int i = 0; i < textureSize; i++)
		{
			float num = Mathf.Lerp(0f, textureHeight, Mathf.Max(values[i], 0.1f));
			for (int j = 0; j < textureHeight; j++)
			{
				if (num >= (float)j)
				{
					float num2 = Mathf.InverseLerp(0f, textureHeight, j);
					Color color = waveGradient.Evaluate(1f - num2);
					texture2D.SetPixel(i, j, color);
				}
				else
				{
					texture2D.SetPixel(i, j, Color.clear);
				}
			}
		}
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.name = "Waveform";
		texture2D.Apply();
		return texture2D;
	}
}
