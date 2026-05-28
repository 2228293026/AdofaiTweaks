using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnMinesweeper : ADOBase
{
	private struct SweeperStage(int width, int height, int bombCount, float zoom = 1f)
	{
		public Vector2Int gridSize = new Vector2Int(width, height);

		public int bombCount = bombCount;

		public float zoom = zoom;
	}

	public static int stage = 0;

	public static string sceneToReturnTo;

	private static readonly SweeperStage[] stageData = new SweeperStage[3]
	{
		new SweeperStage(7, 7, 7),
		new SweeperStage(15, 9, 25),
		new SweeperStage(41, 25, 0, 2.56f)
	};

	private static readonly string[] colors = new string[9] { "FFFFFF", "A2BAFF", "AFFFAC", "FF999F", "D6A5FF", "AD7072", "00EEEE", "353535", "FFFFFF" };

	private const string colorIdle = "C7C7E2";

	private const string colorPressed = "ABABC2";

	public GameObject floorPrefab;

	public Transform floorParent;

	public GameObject minePrefab;

	public AudioClip music;

	public AudioClip musicTutorial;

	public GameObject dokuroCredits;

	private int[] tileValue;

	private bool[] hasBomb;

	private scrFloor[] floors;

	private int selectedFloor;

	private Vector2 lastPlanetPos;

	private int lastBeat;

	private int timerStartPosition = 9999;

	private int revealedFloors;

	private bool readySetGo;

	private bool madeFirstMove;

	private bool transitioningToNextLevel;

	private AudioSource[] countdownTicks = new AudioSource[20];

	private float[] endingDistances;

	private float endingTimer;

	private bool doingEnding;

	private bool won;

	private bool dead;

	private static readonly Vector2Int[] neighbourOffsets = new Vector2Int[8]
	{
		Vector2Int.up,
		Vector2Int.right,
		Vector2Int.down,
		Vector2Int.left,
		Vector2Int.left + Vector2Int.up,
		Vector2Int.up + Vector2Int.right,
		Vector2Int.right + Vector2Int.down,
		Vector2Int.down + Vector2Int.left
	};

	private string goodJobArt = "\n    -----------------------------------------\n    ------BBBB----BBB---BBBBB--BB-BB-BB------\n    -----BBBBBB--BBBBB--BBBBBB-BB-BB-BB------\n    -----BB--BB-BBB-BBB-BB--BB---------------\n    -----BB--BB-BB---BB-BB--BB---------------\n    ---------BB-BB---BB-BBBBB--BB-BB-BB------\n    ---------BB-BB---BB-BBBBB--BB-BB-BB------\n    ---------BB-BB---BB-BB--BB-BB-BB-BB------\n    ---------BB-BBB-BBB-BB--BB-BB-BB-BB------\n    ---------BB--BBBBB--BBBBBB-BB-BB-BB------\n    ---------BB---BBB---BBBBB--BB-BB-BB------\n    -----------------------------------------\n    -----------------------------------------\n    -----------------------------------------\n    -------RRRR----RRR-----RRR---RRRRR-------\n    ------RRRRRR--RRRRR---RRRRR--RRRRRR------\n    -----RRR--RR-RRR-RRR-RRR-RRR-RR--RR------\n    -----RR---RR-RR---RR-RR---RR-RR--RR------\n    -----RR-RRRR-RR---RR-RR---RR-RR--RR------\n    -----RR-RRRR-RR---RR-RR---RR-RR--RR------\n    -----RR------RR---RR-RR---RR-RR--RR------\n    -----RRR--RR-RRR-RRR-RRR-RRR-RR--RR------\n    ------RRRRRR--RRRRR---RRRRR--RRRRRR------\n    -------RRRR----RRR-----RRR---RRRRR-------\n    -----------------------------------------\n    ";

	private string goodJobArtCN = "\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        ------RR----RR-----B----B---------R------\n        --------R--R-------B-BBBBBBB------R------\n        -----RRRRRRRRRR----B----B----------------\n        -------R----R----B-B-B-BBB-B------R------\n        -------R-RRRR----B-B--B-B-B-------R------\n        -------RRRR-R-----BB-BBBBBBB------R------\n        -------R----R-----BBB--B---------RRR-----\n        -------RRRRRR------B--BBBBB------RRR-----\n        ---------R-------BBBBB--B--------RRR-----\n        -----RRRRRRRRRR----B--BBBBBB-----RRR-----\n        ---------R---------B----B---------R------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n        -----------------------------------------\n    ";

	private Vector2Int gridSize => stageData[stage].gridSize;

	private int bombCount => stageData[stage].bombCount;

	private float zoom => stageData[stage].zoom;

	private int lastStage => stageData.Length - 1;

	private bool isLastStage => stage == lastStage;

	private int Index(Vector2Int xy)
	{
		return Index(xy.x, xy.y);
	}

	private int Index(int x, int y)
	{
		return y * gridSize.x + x;
	}

	private Vector2Int Pos(int index)
	{
		return new Vector2Int(index % gridSize.x, index / gridSize.x);
	}

	private bool IsOnBoard(Vector2Int pos)
	{
		if (pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0)
		{
			return pos.y < gridSize.y;
		}
		return false;
	}

	public static void EnterScene()
	{
		if (NeoCosmosManager.instance.installed)
		{
			stage = 0;
			scrController instance = scrController.instance;
			sceneToReturnTo = instance.levelName;
			GCS.sceneToLoad = "scnMinesweeper";
			GCS.speedTrialMode = false;
			scrPlayerManager.SetPlayerCount(1);
			instance.StartLoadingScene();
		}
	}

	private void GenerateBoard()
	{
		int num = gridSize.x * gridSize.y;
		tileValue = new int[num];
		hasBomb = new bool[num];
		floors = new scrFloor[num];
		int num2 = 1000;
		for (int i = 0; i < bombCount; i++)
		{
			int num3 = Random.Range(0, num);
			if (hasBomb[num3] && num2 > 0)
			{
				num2--;
				i--;
			}
			else
			{
				hasBomb[num3] = true;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector2Int xy = Pos(j);
			GameObject gameObject = Object.Instantiate(floorPrefab, new Vector2((float)xy.x - (float)gridSize.x / 2f, (float)xy.y - (float)gridSize.y / 2f) + Vector2.one * 0.5f, Quaternion.identity, floorParent);
			gameObject.name = $"Floor ({xy.x}, {xy.y}; Index {j} aka {Index(xy)})";
			floors[j] = gameObject.GetComponent<scrFloor>();
		}
		selectedFloor = num / 2;
		UpdateZoom();
	}

	private void Awake()
	{
		ADOBase.controller.camy.followMode = false;
		scrUIController.instance.txtCountdown.enabled = false;
		GenerateBoard();
		bool flag = stage == 0;
		ADOBase.conductor.song.clip = (flag ? musicTutorial : music);
		dokuroCredits.SetActive(flag);
		readySetGo = true;
		timerStartPosition = -1;
	}

	private void Start()
	{
		ADOBase.controller.chosenPlanet.currfloor = floors[selectedFloor];
		lastPlanetPos = ADOBase.controller.chosenPlanet.transform.position;
		ADOBase.controller.responsive = false;
	}

	private void ProcessTile(int index, Vector2Int direction = default(Vector2Int))
	{
		Vector2Int vector2Int = Pos(index) + direction;
		if (!IsOnBoard(vector2Int))
		{
			return;
		}
		index = Index(vector2Int);
		if (tileValue[index] != 0 || hasBomb[index])
		{
			return;
		}
		int num = 0;
		Vector2Int[] array = neighbourOffsets;
		foreach (Vector2Int vector2Int2 in array)
		{
			Vector2Int vector2Int3 = vector2Int + vector2Int2;
			if (IsOnBoard(vector2Int3))
			{
				int num2 = Index(vector2Int3);
				if (hasBomb[num2])
				{
					num++;
				}
			}
		}
		tileValue[index] = num + 1;
		scrFloor scrFloor2 = floors[index];
		if (num > 0)
		{
			Text componentInChildren = scrFloor2.GetComponentInChildren<Text>();
			componentInChildren.text = num.ToString();
			componentInChildren.color = colors[num].HexToColor();
		}
		else
		{
			array = neighbourOffsets;
			foreach (Vector2Int direction2 in array)
			{
				ProcessTile(index, direction2);
			}
		}
		scrFloor2.SetColor("ABABC2".HexToColor());
		scrFloor2.transform.localScale = Vector2.one * 0.8f;
		scrFloor2.transform.DOScale(1f, 0.5f).SetEase(Ease.OutSine);
		revealedFloors++;
	}

	private void Update()
	{
		Vector2 vector = ADOBase.controller.chosenPlanet.transform.position;
		int num = (int)(ADOBase.conductor.songposition_minusi / ADOBase.conductor.crotchetAtStart) + 1;
		bool flag = isLastStage && !readySetGo;
		int num2 = (readySetGo ? 8 : (flag ? 10 : 3));
		bool flag2 = !vector.Approximately(lastPlanetPos);
		if (flag2)
		{
			Vector2Int vector2Int = Vector2Int.RoundToInt(vector - lastPlanetPos);
			selectedFloor = Index(Pos(selectedFloor) + vector2Int);
			timerStartPosition = num;
			StopCountdownAudio();
			scrUIController.instance.txtCountdown.text = "";
			madeFirstMove = true;
		}
		bool flag3 = tileValue[selectedFloor] == 0;
		if ((readySetGo || (flag3 && madeFirstMove)) && (flag2 || num != lastBeat) && !won && !dead)
		{
			int num3 = num2 + 1 - (num - timerStartPosition - 1);
			Text txtCountdown = scrUIController.instance.txtCountdown;
			if (num3 >= 1 && num3 <= num2)
			{
				if (!readySetGo)
				{
					txtCountdown.gameObject.SetActive(value: true);
					txtCountdown.GetComponent<scrCountdown>().enabled = false;
					txtCountdown.text = num3.ToString();
				}
			}
			else if (num3 == 0)
			{
				if (readySetGo)
				{
					ADOBase.controller.responsive = true;
					readySetGo = false;
				}
				else
				{
					TryRevealTile(selectedFloor);
					txtCountdown.text = "";
				}
			}
			if (flag2)
			{
				ScheduleCountdownAudio(num + 1, num2, !readySetGo);
			}
		}
		if (doingEnding)
		{
			EndingUpdate();
		}
		if (won)
		{
			WaitForEndLevel();
		}
		lastPlanetPos = vector;
		lastBeat = num;
		if (ADOBase.gc.debug && Input.GetKeyDown(KeyCode.F5))
		{
			Win();
		}
		if (readySetGo)
		{
			ADOBase.controller.responsive = false;
		}
	}

	private void UpdateZoom()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = Mathf.Max(1.7777778f / num, 1f);
		ADOBase.controller.camy.zoomSize = zoom * num2;
	}

	private void StopCountdownAudio()
	{
		AudioSource[] array = countdownTicks;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource != null && audioSource.time <= 0f)
			{
				audioSource.Stop();
			}
		}
	}

	private void ScheduleCountdownAudio(int beat, int duration, bool decliningPitch = false)
	{
		scrConductor scrConductor2 = ADOBase.conductor;
		float num = -0.01f;
		for (int i = 0; i < duration; i++)
		{
			double time = scrConductor2.dspTimeSongPosZero + (double)(beat + i) * scrConductor2.crotchetAtStart / (double)scrConductor2.song.pitch + (double)num;
			AudioSource audioSource = AudioManager.Play("sndHat", time, scrConductor2.hitSoundGroup);
			audioSource.pitch = Mathf.Lerp(1f, 0.1f, (float)i / (float)duration);
			countdownTicks[i] = audioSource;
		}
	}

	private void TryRevealTile(int index)
	{
		if (isLastStage)
		{
			PlayEnding();
			return;
		}
		if (revealedFloors == 0)
		{
			MoveBombsAt(index);
			ProcessTile(index);
			return;
		}
		if (hasBomb[index])
		{
			Fail();
			return;
		}
		ProcessTile(index);
		if (revealedFloors == gridSize.x * gridSize.y - bombCount)
		{
			Win();
		}
	}

	private void MoveBombsAt(int index)
	{
		Vector2Int vector2Int = Pos(index);
		List<int> list = new List<int>();
		Vector2Int[] array = neighbourOffsets;
		foreach (Vector2Int vector2Int2 in array)
		{
			Vector2Int vector2Int3 = vector2Int + vector2Int2;
			if (IsOnBoard(vector2Int3))
			{
				list.Add(Index(vector2Int3));
			}
		}
		list.Add(index);
		foreach (int item in list)
		{
			if (!hasBomb[item])
			{
				continue;
			}
			int num = item;
			bool flag;
			do
			{
				num = Random.Range(0, gridSize.x * gridSize.y);
				flag = hasBomb[num];
				if (flag)
				{
					continue;
				}
				foreach (int item2 in list)
				{
					if (num == item2)
					{
						flag = true;
						break;
					}
				}
			}
			while (flag);
			hasBomb[item] = false;
			hasBomb[num] = true;
		}
	}

	private void Fail()
	{
		dead = true;
		ADOBase.controller.playerOne.Die();
		StopCountdownAudio();
		Persistence.kaboomDeaths++;
		for (int i = 0; i < floors.Length; i++)
		{
			if (hasBomb[i])
			{
				bool num = i == selectedFloor;
				float delay = (num ? 0f : Random.Range(0f, 0.5f));
				float duration = 0.33f;
				floors[i].TweenColor(Color.red, duration, Ease.Linear, delay);
				if (!num)
				{
					scrSpike component = Object.Instantiate(minePrefab, floors[i].transform.position, default(Quaternion)).GetComponent<scrSpike>();
					component.transform.localScale = Vector2.zero;
					component.transform.DOScale(Vector2.one, duration).SetEase(Ease.OutBack).SetDelay(delay);
					component.ballSprite.curFrame = Random.Range(0, component.ballSprite.frames.Count);
				}
			}
		}
	}

	private void Win()
	{
		if (won)
		{
			return;
		}
		won = true;
		ADOBase.controller.responsive = false;
		if (!isLastStage)
		{
			scrUIController.instance.txtCountdown.text = RDString.Get("status.congratulations");
		}
		if (isLastStage)
		{
			Persistence.kaboomClears++;
		}
		scrFlash.Flash(Color.white.WithAlpha(0.3f));
		scrSfx.instance.PlaySfx(SfxSound.Applause, MixerGroup.SfxParent);
		for (int i = 0; i < floors.Length; i++)
		{
			if (hasBomb[i])
			{
				scrFloor scrFloor2 = floors[i];
				scrFloor2.ToggleCollider(collEn: false);
				scrFloor2.MoveToBack();
				scrFloor f = scrFloor2;
				f.TweenOpacity(0f, 3f, Ease.InCubic);
				if ((bool)f.legacyFloorSpriteRenderer)
				{
					f.legacyFloorSpriteRenderer.DOColor(new Color(1f, 1f, 1f, 0f), 3f).SetEase(Ease.InCubic);
				}
				scrFloor2.transform.DOScale(0.5f, 3f).SetEase(Ease.InCubic).OnComplete(delegate
				{
					f.enabled = false;
					f.transform.position += Vector3.up * 9999f;
				});
				scrFloor2.transform.DOMoveY(-2f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
				scrFloor2.transform.DORotate(Vector3.forward * 45f, 3f).SetRelative(isRelative: true).SetEase(Ease.InCubic);
			}
		}
	}

	private void WaitForEndLevel()
	{
		if (ADOBase.controller.playerOne.CountValidKeysPressed() > 0)
		{
			EndLevel();
		}
	}

	private void EndLevel()
	{
		if (!transitioningToNextLevel)
		{
			transitioningToNextLevel = true;
			if (isLastStage)
			{
				GCS.sceneToLoad = sceneToReturnTo;
			}
			else
			{
				stage++;
				GCS.sceneToLoad = ADOBase.controller.levelName;
			}
			ADOBase.controller.StartLoadingScene();
		}
	}

	private void PlayEnding()
	{
		doingEnding = true;
		ADOBase.controller.responsive = false;
		int num = gridSize.x * gridSize.y;
		endingDistances = new float[num];
		for (int i = 0; i < num; i++)
		{
			endingDistances[i] = Vector2.Distance(Pos(selectedFloor), Pos(i));
		}
		DOVirtual.DelayedCall(3.5f, delegate
		{
			Win();
		});
	}

	private void EndingUpdate()
	{
		endingTimer += Time.deltaTime;
		int num = gridSize.x * gridSize.y;
		int artOffset = 0;
		string art = (RDString.isChinese ? goodJobArtCN : goodJobArt);
		bool isRainbow;
		Color color = GetTextColor(red: false, out isRainbow);
		bool isRainbow2;
		Color color2 = GetTextColor(red: true, out isRainbow2);
		Color a = "C7C7E2".HexToColor();
		int i;
		for (i = 0; i < num; i++)
		{
			while (getPixel() != '-' && getPixel() != 'R' && getPixel() != 'B')
			{
				artOffset++;
			}
			float value = Pos(i).x;
			Color color3 = Color.HSVToRGB(Mathf.Repeat(Mathf.InverseLerp(5f, gridSize.x - 5, value), 1f), 1f, 1f);
			if (isRainbow2)
			{
				color2 = color3;
			}
			if (isRainbow)
			{
				color = color3;
			}
			Color a2 = ((getPixel() == '-') ? Color.white : ((getPixel() == 'R') ? color2 : color));
			a2 = Color.Lerp(a2, Color.white, 0.25f);
			float num2 = endingDistances[i];
			int num3 = 8;
			float t = endingTimer * (float)num3 - num2;
			int num4 = i;
			floors[num4].floorRenderer.color = Color.Lerp(a, a2, t);
			floors[num4].transform.localScale = Vector2.Lerp(Vector2.one, Vector2.one * 1.055f, t);
		}
		static Color GetTextColor(bool red, out bool reference)
		{
			PlanetColor playerColor = Persistence.GetPlayerColor(red);
			Color result = playerColor.ToRealColor();
			float num5 = (result.r + result.g + result.b) / 3f;
			if ((double)(1f - num5) <= 0.25)
			{
				result = Color.gray;
			}
			reference = playerColor.preset == PlanetColorPreset.Rainbow;
			return result;
		}
		char getPixel()
		{
			return art[i + artOffset];
		}
	}
}
