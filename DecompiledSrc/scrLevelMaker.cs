using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrLevelMaker : ADOBase
{
	public const double FloorAngleEpsilon = 1E-06;

	public const string FloorContainerName = "Floors";

	public const char Angle0 = 'R';

	public const char Angle45 = 'E';

	public const char Angle60 = 'T';

	public const char Angle90 = 'U';

	public const char Angle135 = 'Q';

	public const char Angle150 = 'G';

	public const char Angle180 = 'L';

	public const char Angle240 = 'F';

	public const char Angle270 = 'D';

	public const char Angle225 = 'Z';

	public const char Angle330 = 'B';

	public const char Angle315 = 'C';

	public const char Angle30 = 'J';

	public const char Angle120 = 'H';

	public const char Angle210 = 'N';

	public const char Angle300 = 'M';

	public const char Angle15 = 'p';

	public const char Angle75 = 'o';

	public const char Angle105 = 'q';

	public const char Angle165 = 'W';

	public const char Angle195 = 'x';

	public const char Angle255 = 'V';

	public const char Angle285 = 'Y';

	public const char Angle345 = 'A';

	public const char AngleMidspin = '!';

	public const char Angle60Add = 't';

	public const char Angle300Add = 'y';

	public const char Angle120CW = 'h';

	public const char Angle120CCW = 'j';

	public const char Angle108CW = '5';

	public const char Angle108CCW = '6';

	public const char Angle128CW = '7';

	public const char Angle128CCW = '8';

	public const char Angle210CW = '9';

	public const float midSpinAngle = 999f;

	public const float sAngle = -999f;

	public GameObject spriteFloor;

	public GameObject meshFloor;

	public string caption;

	public float addoffset;

	public bool lockChanges;

	public bool isgameworld = true;

	public bool isOldLevel;

	public bool useInitialTrackStyle;

	public bool hideDifficultyUI;

	[Header("Info")]
	public string leveldata;

	public float[] floorAngles;

	public List<scrFloor> listFloors = new List<scrFloor>();

	public List<FreeroamArea> listFreeroam = new List<FreeroamArea>();

	public List<scrFloor> listFreeroamStartTiles = new List<scrFloor>();

	public float bpm_forUnityUseOnly;

	public float pitch_forUnityUseOnly;

	public float volume_forUnityUseOnly;

	public float highestBPM;

	[NonSerialized]
	public GameObject holdContainer;

	[NonSerialized]
	public scrLevelMaker2 lm2;

	private List<GameObject> holdGOs = new List<GameObject>();

	private Material holdMaterial;

	private GameObject floorContainer;

	private static scrLevelMaker _instance;

	private static int sceneInstanceID;

	public static scrLevelMaker instance
	{
		get
		{
			bool flag = true;
			if (Application.isPlaying)
			{
				SceneHandle handle = SceneManager.GetActiveScene().handle;
				flag = ADOBase.isMobileMenu || sceneInstanceID != handle;
				sceneInstanceID = handle;
			}
			if (_instance == null && flag)
			{
				_instance = UnityEngine.Object.FindFirstObjectByType<scrLevelMaker>();
				_instance?.Init();
			}
			return _instance;
		}
	}

	private GameObject GetFloorContainer()
	{
		GameObject gameObject = GameObject.Find("Floors");
		if (gameObject == null)
		{
			gameObject = new GameObject("Floors");
		}
		return gameObject;
	}

	private void Awake()
	{
		_instance = this;
		Init();
	}

	private void Init()
	{
		lm2 = GetComponent<scrLevelMaker2>();
	}

	public void FixListFloors()
	{
		scrFloor[] source = UnityEngine.Object.FindObjectsByType<scrFloor>(FindObjectsSortMode.None);
		listFloors = source.OrderBy((scrFloor o) => o.seqID).ToList();
		Debug.Log(listFloors.Count);
		lm2 = GetComponent<scrLevelMaker2>();
		lm2.listFloorClone = listFloors;
	}

	public void CopyProperties()
	{
		lm2 = GetComponent<scrLevelMaker2>();
		scrConductor obj = scrConductor.instance;
		obj.addoffset = addoffset;
		obj.bpm = bpm_forUnityUseOnly;
		obj.song.pitch = pitch_forUnityUseOnly;
		obj.song.volume = volume_forUnityUseOnly;
		scrController obj2 = scrController.instance;
		obj2.tileShape = (lm2.BigTiles ? TileShape.Long : TileShape.Short);
		obj2.caption = caption;
	}

	public static double OldToNewAngle(double oldAngle)
	{
		return (7.853981852531433 - oldAngle) % 6.2831854820251465;
	}

	public List<scrFloor> MakeLevel()
	{
		if (isOldLevel)
		{
			InstantiateStringFloors();
		}
		else
		{
			InstantiateFloatFloors();
		}
		lm2 = GetComponent<scrLevelMaker2>();
		for (int i = 0; i < listFloors.Count; i++)
		{
			scrFloor scrFloor2 = listFloors[i];
			scrFloor2.styleNum = 0;
			if (isgameworld)
			{
				scrFloor2.UpdateAngle();
			}
			scrFloor2.SetTileColor(lm2.tilecolor);
			int num = 100 + (listFloors.Count - i);
			num *= 5;
			scrFloor2.SetSortingOrder(num);
			scrFloor2.startPos = scrFloor2.transform.position;
			scrFloor2.startRot = scrFloor2.transform.rotation.eulerAngles;
			scrFloor2.tweenRot = scrFloor2.startRot;
			scrFloor2.offsetPos = Vector3.zero;
			if (scrFloor2.isportal && Application.isPlaying)
			{
				scrFloor2.SpawnPortalParticles();
			}
		}
		if (LevelData.shouldTryMigrate && (bool)ADOBase.editor && ADOBase.editor.levelData.version == 9)
		{
			LevelData.shouldTryMigrate = false;
			bool flag = true;
			foreach (LevelEvent levelEvent in ADOBase.editor.levelData.levelEvents)
			{
				if (levelEvent.eventType == LevelEventType.Twirl)
				{
					flag = !flag;
				}
				else if (levelEvent.eventType == LevelEventType.Pause && levelEvent.floor < listFloors.Count)
				{
					scrFloor scrFloor3 = listFloors[levelEvent.floor];
					bool num2 = Math.Abs(scrFloor3.entryangle % (Math.PI / 2.0) - scrFloor3.exitangle % (Math.PI / 2.0)) <= 0.0001;
					bool flag2 = Math.Abs(scrMisc.GetAngleMoved(scrFloor3.entryangle, scrFloor3.exitangle, flag) - 6.2831854820251465) < 0.0001;
					if (num2 && !flag2)
					{
						levelEvent["duration"] = (float)levelEvent["duration"] + 1f;
					}
				}
			}
		}
		return listFloors;
	}

	public void InstantiateStringFloors()
	{
		bool flag = Application.isPlaying;
		GameObject gameObject = GetFloorContainer();
		string text = leveldata;
		int num = listFloors.Count;
		int num2 = text.Length + 1;
		Material floorSpriteDefault = RDConstants.data.floorSpriteDefault;
		if (num > 0 && listFloors[0].GetComponent<FloorMeshRenderer>() != null)
		{
			flag = false;
		}
		if (flag)
		{
			if (num > num2)
			{
				for (int i = num2; i < num; i++)
				{
					scrFloor scrFloor2 = listFloors[i];
					if (scrFloor2 != null)
					{
						UnityEngine.Object.DestroyImmediate(scrFloor2.gameObject);
					}
				}
				listFloors.RemoveRange(num2, num - num2);
			}
		}
		else
		{
			foreach (scrFloor listFloor in listFloors)
			{
				if (listFloor != null)
				{
					UnityEngine.Object.DestroyImmediate(listFloor.gameObject);
				}
			}
			num = 0;
			listFloors.Clear();
		}
		if (listFloors.Count > 0)
		{
			ADOBase.conductor.onBeats.Clear();
		}
		if (listFloors.Count == 0)
		{
			scrFloor component = UnityEngine.Object.Instantiate(spriteFloor, Vector3.zero, Quaternion.identity).GetComponent<scrFloor>();
			component.gameObject.transform.parent = gameObject.transform;
			component.hasLit = true;
			component.entryangle = 4.71238899230957;
			component.name = "0/FloorR";
			listFloors.Add(component);
		}
		else
		{
			scrFloor scrFloor3 = listFloors[0];
			ResetFloor(scrFloor3, Vector3.zero, floorSpriteDefault);
			scrFloor3.hasLit = true;
			scrFloor3.entryangle = 4.71238899230957;
			scrFloor3.name = "0/FloorR";
		}
		int num3 = 1;
		float num4 = Mathf.Sin((float)Math.PI / 4f);
		float num5 = Mathf.Sin((float)Math.PI / 6f);
		Transform parent = gameObject.transform;
		Vector3 zero = Vector3.zero;
		bool flag2 = true;
		float num6 = 1f;
		for (int j = 0; j < text.Length; j++)
		{
			double radius = scrController.instance.tileSize;
			double num7 = 0.0;
			bool isEditor = Application.isEditor;
			scrFloor scrFloor4 = listFloors[j];
			if (text[j] == '[' && isEditor)
			{
				bool flag3 = false;
				int num8 = j + 1;
				bool flag4 = false;
				if (j + 1 <= text.Length && text[j + 1] == '*')
				{
					flag4 = true;
					num8++;
				}
				while (j + 1 <= text.Length && !flag3)
				{
					j++;
					if (text[j] == ']')
					{
						flag3 = true;
					}
				}
				string s = text.Substring(num8, j - num8).Replace(" ", "");
				float result = 0f;
				if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					float num9 = (float)Math.PI / 180f * result;
					num7 = (flag4 ? scrMisc.incrementAngle(scrFloor4.entryangle, num9) : ((double)num9));
				}
			}
			else
			{
				num7 = text[j] switch
				{
					'R' => 1.5707963705062866, 
					'U' => 0.0, 
					'L' => 4.71238899230957, 
					'D' => 3.1415927410125732, 
					'Q' => 5.4977874755859375, 
					'E' => 0.7853981852531433, 
					'Z' => 3.9269909858703613, 
					'C' => 2.356194496154785, 
					't' => scrMisc.incrementAngle(scrFloor4.entryangle, (float)Math.PI / 3f * (float)(flag2 ? 1 : (-1))), 
					'h' => scrMisc.incrementAngle(scrFloor4.entryangle, 2.094395160675049), 
					'y' => scrMisc.incrementAngle(scrFloor4.entryangle, (float)Math.PI / 3f * (float)((!flag2) ? 1 : (-1))), 
					'j' => scrMisc.incrementAngle(scrFloor4.entryangle, -2.094395160675049), 
					'5' => scrMisc.incrementAngle(scrFloor4.entryangle, 1.8849555253982544), 
					'6' => scrMisc.incrementAngle(scrFloor4.entryangle, -1.8849555253982544), 
					'7' => scrMisc.incrementAngle(scrFloor4.entryangle, 2.243994951248169), 
					'8' => scrMisc.incrementAngle(scrFloor4.entryangle, -2.243994951248169), 
					'9' => scrMisc.incrementAngle(scrFloor4.entryangle, 3.665191411972046), 
					'T' => 0.5235987901687622, 
					'B' => 2.6179938316345215, 
					'F' => 3.665191411972046, 
					'G' => 5.759586334228516, 
					'J' => 1.0471975803375244, 
					'M' => 2.094395160675049, 
					'N' => 4.188790321350098, 
					'H' => 5.235987663269043, 
					'p' => 1.3089969158172607, 
					'o' => 0.2617993950843811, 
					'q' => 6.021385669708252, 
					'W' => 4.974188327789307, 
					'x' => 4.450589656829834, 
					'V' => 3.4033920764923096, 
					'Y' => 2.879793167114258, 
					'A' => 1.832595705986023, 
					'!' => (float)scrFloor4.entryangle, 
					_ => 0.0, 
				};
			}
			Vector3 vectorFromAngle = scrMisc.getVectorFromAngle(num7, radius);
			zero += vectorFromAngle;
			if (listFloors.Count > 0)
			{
				listFloors[j].exitangle = num7;
			}
			scrFloor scrFloor5;
			if (j < num - 1)
			{
				scrFloor5 = listFloors[j + 1];
				_ = scrFloor5.gameObject;
				ResetFloor(scrFloor5, zero, floorSpriteDefault);
			}
			else
			{
				scrFloor5 = UnityEngine.Object.Instantiate(spriteFloor, zero, Quaternion.identity, parent).GetComponent<scrFloor>();
				listFloors.Add(scrFloor5);
			}
			scrFloor4.nextfloor = scrFloor5;
			scrFloor5.stringDirection = text[j];
			scrFloor5.seqID = num3;
			scrFloor5.entryangle = (num7 + 3.1415927410125732) % 6.2831854820251465;
			char c = text[j];
			if (c == '!')
			{
				listFloors[num3 - 1].midSpin = true;
			}
			bool flag5 = true;
			bool bigTiles = lm2.BigTiles;
			while (flag5 && j < text.Length - 1)
			{
				flag5 = false;
				if ("UDLRQEZCthTFGByjHJMN!56789[qWVYAxop".Contains(text[j + 1]))
				{
					break;
				}
				j++;
				flag5 = true;
				switch (c)
				{
				case 'S':
					scrFloor5.speed = 0.25f;
					break;
				case 'X':
					scrFloor5.speed = 0.5f;
					break;
				case 'O':
					scrFloor5.speed = 2f;
					break;
				case 'P':
					scrFloor5.speed = 4f;
					break;
				case '/':
					flag2 = !flag2;
					break;
				case '>':
					scrFloor5.floorIcon = FloorIcon.Rabbit;
					num6 *= 2f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				case '*':
					scrFloor5.floorIcon = FloorIcon.DoubleRabbit;
					num6 *= 4f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				case '_':
					scrFloor5.floorIcon = FloorIcon.Rabbit;
					num6 /= 0.75f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				case '<':
					scrFloor5.floorIcon = FloorIcon.Snail;
					num6 /= 2f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				case '%':
					scrFloor5.floorIcon = FloorIcon.DoubleSnail;
					num6 /= 4f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				case '-':
					scrFloor5.floorIcon = FloorIcon.Snail;
					num6 *= 0.75f;
					if (bigTiles)
					{
						scrFloor5.SetIconAngle((float)Math.PI);
					}
					break;
				}
			}
			scrFloor5.isCCW = !flag2;
			scrFloor5.speed = num6;
			scrFloor5.isportal = j == text.Length - 1 && ADOBase.controller.gameworld;
			scrFloor5.levelnumber = Portal.EndOfLevel;
			if (j < num)
			{
				scrFloor5.CheckPortalSprite();
				scrFloor5.UpdateIconSprite();
			}
			num3++;
		}
		listFloors.Last().exitangle = listFloors.Last().entryangle + 3.1415927410125732;
	}

	private void ResetFloor(scrFloor floor, Vector3 position, Material material)
	{
		_ = floor.gameObject;
		GameObject obj = floor.gameObject;
		obj.transform.position = position;
		obj.transform.rotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;
		ffxPlusBase[] components = obj.GetComponents<ffxPlusBase>();
		for (int i = 0; i < components.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(components[i]);
		}
		if (Application.isPlaying)
		{
			floor.floorRenderer.material.CopyPropertiesFromMaterial(material);
		}
		floor.Reset();
	}

	public void InstantiateFloatFloors()
	{
		bool flag = Application.isPlaying;
		GameObject gameObject = GameObject.Find("Floors");
		if (gameObject == null)
		{
			gameObject = new GameObject("Floors");
		}
		int num = listFloors.Count;
		int num2 = floorAngles.Length + 1;
		Material floorMeshDefault = RDConstants.data.floorMeshDefault;
		if (!Application.isPlaying || (num > 0 && listFloors[0].GetComponent<FloorSpriteRenderer>() != null))
		{
			flag = false;
		}
		if (flag)
		{
			if (num > num2)
			{
				for (int i = num2; i < num; i++)
				{
					scrFloor scrFloor2 = listFloors[i];
					if (scrFloor2 != null)
					{
						UnityEngine.Object.DestroyImmediate(scrFloor2.gameObject);
					}
				}
				listFloors.RemoveRange(num2, num - num2);
			}
		}
		else
		{
			foreach (scrFloor listFloor in listFloors)
			{
				if (listFloor != null && listFloor.gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(listFloor.gameObject);
				}
			}
			num = 0;
			listFloors.Clear();
		}
		if (listFloors.Count > 0)
		{
			ADOBase.conductor.onBeats.Clear();
		}
		if (listFloors.Count == 0)
		{
			GameObject gameObject2 = null;
			if (Application.isPlaying)
			{
				gameObject2 = UnityEngine.Object.Instantiate(meshFloor, Vector3.zero, Quaternion.identity);
			}
			scrFloor component = gameObject2.GetComponent<scrFloor>();
			component.gameObject.transform.parent = gameObject.transform;
			component.hasLit = true;
			component.entryangle = 4.71238899230957;
			component.name = "0/Floor 0";
			listFloors.Add(component);
		}
		else
		{
			scrFloor scrFloor3 = listFloors[0];
			ResetFloor(scrFloor3, Vector3.zero, floorMeshDefault);
			scrFloor3.hasLit = true;
			scrFloor3.entryangle = 4.71238899230957;
			scrFloor3.name = "0/Floor 0";
		}
		scrFloor scrFloor4 = listFloors[0];
		bool flag2 = true;
		Vector3 zero = Vector3.zero;
		for (int j = 0; j < floorAngles.Length; j++)
		{
			double radius = scrController.instance.tileSize;
			double num3 = 0.0;
			_ = Application.isEditor;
			float num4 = floorAngles[j];
			num3 = (scrFloor4.exitangle = ((num4 == 999f) ? ((double)(float)scrFloor4.entryangle) : ((double)((0f - num4 + 90f) * ((float)Math.PI / 180f)))));
			Vector3 vectorFromAngle = scrMisc.getVectorFromAngle(num3, radius);
			zero += vectorFromAngle;
			scrFloor scrFloor5 = null;
			GameObject gameObject3 = null;
			if (j < num - 1)
			{
				scrFloor5 = listFloors[j + 1];
				gameObject3 = scrFloor5.gameObject;
				ResetFloor(scrFloor5, zero, floorMeshDefault);
			}
			else
			{
				if (Application.isPlaying)
				{
					gameObject3 = UnityEngine.Object.Instantiate(meshFloor, zero, Quaternion.identity);
				}
				gameObject3.gameObject.transform.parent = gameObject.transform;
				scrFloor5 = gameObject3.GetComponent<scrFloor>();
				listFloors.Add(scrFloor5);
			}
			scrFloor4.nextfloor = scrFloor5;
			scrFloor5.floatDirection = num4;
			scrFloor5.seqID = j + 1;
			scrFloor5.entryangle = (num3 + 3.1415927410125732) % 6.2831854820251465;
			scrFloor5.isCCW = !flag2;
			scrFloor5.speed = 1f;
			if (num4 == 999f)
			{
				scrFloor4.midSpin = true;
			}
			if (j == floorAngles.Length - 1 && ADOBase.controller.gameworld)
			{
				scrFloor5.isportal = true;
				scrFloor5.levelnumber = Portal.EndOfLevel;
			}
			scrFloor4 = scrFloor5;
		}
		scrFloor4.exitangle = scrFloor4.entryangle + 3.1415927410125732;
	}

	public void CalculateFloorAngleLengths()
	{
		scrFloor scrFloor2 = listFloors[0];
		scrFloor2.entryangle = 4.71238898038469;
		float num = ADOBase.conductor.adjustedCountdownTicks - 1f;
		scrFloor2.angleLength = (double)num * Math.PI + scrMisc.GetAngleMoved(scrFloor2.entryangle, scrFloor2.exitangle, !scrFloor2.isCCW);
	}

	public double CalculateSingleFloorAngleLength(scrFloor cf)
	{
		cf.prevfloor = listFloors[Math.Max(0, cf.seqID - 1)];
		double num = scrMisc.GetInverseAnglePerBeatMultiplanet(cf.numPlanets) * (double)((!cf.isCCW) ? 1 : (-1));
		if (cf.midSpin)
		{
			num = 0.0;
		}
		if (cf.prevfloor.midSpin && cf.numPlanets > 2)
		{
			num -= (6.2831854820251465 + scrMisc.GetInverseAnglePerBeatMultiplanet(cf.numPlanets)) * (double)((!cf.isCCW) ? 1 : (-1));
		}
		double num2 = scrMisc.GetAngleMoved(cf.entryangle + num, cf.exitangle + (cf.midSpin ? num : 0.0), !cf.isCCW);
		double num3 = Math.Abs(num2);
		if (num3 <= 1E-06 || num3 >= 6.283184482025146)
		{
			if (cf.midSpin)
			{
				num2 = 0.0;
			}
			else
			{
				num2 = 6.2831854820251465;
				cf.turnaround = true;
			}
		}
		else
		{
			cf.turnaround = false;
		}
		if (cf.holdLength > 0)
		{
			num2 += (double)((float)(cf.holdLength * 2) * (float)Math.PI);
		}
		cf.angleLength = num2;
		cf.hasAppendedExtraBeatsFromAngleLength = false;
		return num2;
	}

	public void CalculateFloorEntryTimes()
	{
		scrConductor scrConductor2 = ADOBase.conductor;
		float pitch = scrConductor2.song.pitch;
		highestBPM = 0f;
		double num = 0.0;
		if (listFloors.Count == 1)
		{
			return;
		}
		scrFloor scrFloor2 = listFloors[0];
		float num2 = scrConductor2.adjustedCountdownTicks - 1f;
		num += scrConductor2.crotchetAtStart * (double)num2 + scrMisc.GetTimeBetweenAngles(scrFloor2.entryangle, scrFloor2.exitangle, scrFloor2.speed, scrConductor2.bpm, !scrFloor2.isCCW);
		listFloors[0].entryTime = 0.0;
		listFloors[1].entryTime = num;
		listFloors[1].entryTimePitchAdj = num / (double)pitch;
		for (int i = 1; i < listFloors.Count - 1; i++)
		{
			scrFloor scrFloor3 = listFloors[i];
			scrFloor3.prevfloor = listFloors[i - 1];
			scrFloor obj = listFloors[i + 1];
			double num3 = scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor3.numPlanets) * (double)((!scrFloor3.isCCW) ? 1 : (-1));
			if (scrFloor3.midSpin)
			{
				num3 = 0.0;
			}
			if (scrFloor3.prevfloor.midSpin && scrFloor3.numPlanets > 2)
			{
				num3 -= (6.2831854820251465 + scrMisc.GetInverseAnglePerBeatMultiplanet(scrFloor3.numPlanets)) * (double)((!scrFloor3.isCCW) ? 1 : (-1));
			}
			double num4 = scrMisc.GetTimeBetweenAngles(scrFloor3.entryangle + num3, scrFloor3.exitangle + (scrFloor3.midSpin ? num3 : 0.0), scrFloor3.speed, scrConductor2.bpm, !scrFloor3.isCCW);
			bool flag = num4 <= 1E-06 || num4 >= (double)(2f * (float)scrConductor2.crotchetAtStart / scrFloor3.speed) - 1E-06;
			if (flag)
			{
				num4 = (scrFloor3.midSpin ? 0.0 : (2.0 * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor3.speed, scrConductor2.bpm, isCW: false)));
			}
			num += num4;
			if (scrFloor3.holdLength > 0)
			{
				num += (double)(scrFloor3.holdLength * 2) * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor3.speed, scrConductor2.bpm, isCW: false);
			}
			float num5 = scrFloor3.extraBeats;
			if (num5 > 0f && flag)
			{
				num5 -= 1f;
			}
			num = (obj.entryTime = num + (double)num5 * scrMisc.GetTimeBetweenAngles(0.0, 3.1415927410125732, scrFloor3.speed, scrConductor2.bpm, isCW: false));
			obj.entryTimePitchAdj = num / (double)pitch;
			_ = scrFloor3.speed;
			_ = scrConductor2.bpm;
			float num6 = scrFloor3.speed * scrConductor2.bpm;
			if (num6 > highestBPM)
			{
				highestBPM = num6;
			}
		}
		scrFloor2.entryBeat = -1.0;
		double num7 = 0.0;
		for (int j = 1; j < listFloors.Count - 1; j++)
		{
			scrFloor scrFloor4 = listFloors[j];
			scrFloor4.entryBeat = num7;
			double num8 = CalculateSingleFloorAngleLength(scrFloor4);
			num7 += num8 / Math.PI + (double)scrFloor4.extraBeats;
		}
	}

	public static float GetAngleFromFloorCharDirectionWithCheck(char direction, out bool exists)
	{
		float? num = direction switch
		{
			'R' => 0f, 
			'E' => 45f, 
			'U' => 90f, 
			'Q' => 135f, 
			'L' => 180f, 
			'Z' => 225f, 
			'D' => 270f, 
			'C' => 315f, 
			'B' => 300f, 
			'T' => 60f, 
			'G' => 120f, 
			'F' => 240f, 
			'J' => 30f, 
			'H' => 150f, 
			'N' => 210f, 
			'M' => 330f, 
			'p' => 15f, 
			'o' => 75f, 
			'q' => 105f, 
			'W' => 165f, 
			'x' => 195f, 
			'V' => 255f, 
			'Y' => 285f, 
			'A' => 345f, 
			'!' => 999f, 
			_ => null, 
		};
		exists = num is float;
		return num.GetValueOrDefault();
	}

	public static float GetAngleFromFloorCharDirection(char direction)
	{
		bool exists;
		return GetAngleFromFloorCharDirectionWithCheck(direction, out exists);
	}

	public char GetHFlippedDirection(char direction)
	{
		return direction switch
		{
			'R' => 'L', 
			'E' => 'Q', 
			'U' => 'U', 
			'Q' => 'E', 
			'L' => 'R', 
			'Z' => 'C', 
			'D' => 'D', 
			'C' => 'Z', 
			'B' => 'F', 
			'T' => 'G', 
			'G' => 'T', 
			'F' => 'B', 
			'J' => 'H', 
			'H' => 'J', 
			'N' => 'M', 
			'M' => 'N', 
			'p' => 'W', 
			'o' => 'q', 
			'q' => 'o', 
			'W' => 'p', 
			'x' => 'A', 
			'V' => 'Y', 
			'Y' => 'V', 
			'A' => 'x', 
			'6' => '5', 
			'5' => '6', 
			'8' => '7', 
			'7' => '8', 
			_ => direction, 
		};
	}

	public float GetHFlippedDirection(float direction)
	{
		if (direction == 999f)
		{
			return direction;
		}
		return (0f - direction + 180f) % 360f;
	}

	public char GetVFlippedDirection(char direction)
	{
		return direction switch
		{
			'R' => 'R', 
			'E' => 'C', 
			'U' => 'D', 
			'Q' => 'Z', 
			'L' => 'L', 
			'Z' => 'Q', 
			'D' => 'U', 
			'C' => 'E', 
			'B' => 'T', 
			'T' => 'B', 
			'G' => 'F', 
			'F' => 'G', 
			'J' => 'M', 
			'H' => 'N', 
			'N' => 'H', 
			'M' => 'J', 
			'p' => 'A', 
			'o' => 'Y', 
			'q' => 'V', 
			'W' => 'x', 
			'x' => 'W', 
			'V' => 'q', 
			'Y' => 'o', 
			'A' => 'p', 
			'6' => '5', 
			'5' => '6', 
			'8' => '7', 
			'7' => '8', 
			_ => direction, 
		};
	}

	public float GetVFlippedDirection(float direction)
	{
		if (direction != 999f)
		{
			return (0f - direction) % 360f;
		}
		return direction;
	}

	public char GetRotDirection(char direction, bool CW)
	{
		return direction switch
		{
			'R' => CW ? 'D' : 'U', 
			'E' => CW ? 'C' : 'Q', 
			'U' => CW ? 'R' : 'L', 
			'Q' => CW ? 'E' : 'Z', 
			'L' => CW ? 'U' : 'D', 
			'Z' => CW ? 'Q' : 'C', 
			'D' => CW ? 'L' : 'R', 
			'C' => CW ? 'Z' : 'E', 
			'B' => CW ? 'N' : 'J', 
			'T' => CW ? 'M' : 'H', 
			'G' => CW ? 'J' : 'N', 
			'F' => CW ? 'H' : 'M', 
			'J' => CW ? 'B' : 'G', 
			'H' => CW ? 'T' : 'F', 
			'N' => CW ? 'G' : 'B', 
			'M' => CW ? 'F' : 'T', 
			'p' => CW ? 'Y' : 'q', 
			'o' => CW ? 'A' : 'W', 
			'q' => CW ? 'p' : 'x', 
			'W' => CW ? 'o' : 'V', 
			'x' => CW ? 'q' : 'Y', 
			'V' => CW ? 'W' : 'A', 
			'Y' => CW ? 'x' : 'p', 
			'A' => CW ? 'V' : 'o', 
			_ => direction, 
		};
	}

	public float GetRotDirection(float direction, bool CW)
	{
		if (direction != 999f)
		{
			return direction + (float)((!CW) ? 1 : (-1)) * 90f;
		}
		return direction;
	}

	public static float[] StringToAngleArray(string levelStr)
	{
		float[] array = new float[levelStr.Length];
		float num = 0f;
		for (int i = 0; i < levelStr.Length; i++)
		{
			char c = levelStr[i];
			bool exists;
			float num2 = GetAngleFromFloorCharDirectionWithCheck(c, out exists);
			if (!exists)
			{
				num2 = num + c switch
				{
					'5' => 72f, 
					'6' => -72f, 
					'7' => 52f, 
					'8' => -52f, 
					'9' => -30f, 
					'h' => 120f, 
					'j' => -120f, 
					't' => 60f, 
					'y' => 300f, 
					_ => 0f, 
				};
			}
			array[i] = num2;
			num = num2;
		}
		return array;
	}

	public void ClearFreeroam()
	{
		if (listFreeroam.Count > 0)
		{
			foreach (FreeroamArea item in listFreeroam)
			{
				if (item.Count > 0)
				{
					foreach (scrFloor item2 in item)
					{
						if (item2 != null)
						{
							UnityEngine.Object.DestroyImmediate(item2.gameObject);
						}
					}
				}
				item.Clear();
			}
		}
		listFreeroam.Clear();
		listFreeroamStartTiles = new List<scrFloor>();
	}

	public void DrawFreeroam()
	{
		ClearFreeroam();
		foreach (scrFloor listFloor in listFloors)
		{
			if (listFloor.freeroam)
			{
				MakeFreeroamGrid(listFloor);
			}
		}
	}

	public void MakeFreeroamGrid(scrFloor floorComp)
	{
		floorContainer = GetFloorContainer();
		float radiusScale = floorComp.radiusScale;
		float x = floorComp.transform.localScale.x;
		floorComp.radiusScale = radiusScale;
		double num = scrController.instance.tileSize * radiusScale;
		int num2 = (int)floorComp.freeroamDimensions.x;
		int num3 = (int)floorComp.freeroamDimensions.y;
		FreeroamArea item = new FreeroamArea(floorComp);
		listFreeroam.Add(item);
		listFreeroamStartTiles.Add(floorComp);
		float num4 = 90f;
		num4 *= (float)Math.PI / 180f;
		Vector3 vector = new Vector3(floorComp.freeroamOffset.x + floorComp.freeroamOffset.y * Mathf.Cos(num4), floorComp.freeroamOffset.y * Mathf.Sin(num4), 0f) * (float)num;
		for (int i = 0; i < num2 * num3; i++)
		{
			int num5 = i % num2;
			int num6 = (int)Mathf.Floor((float)i / (float)num2);
			Vector3 right = Vector3.right;
			Vector3 vector2 = new Vector3(Mathf.Cos(num4), Mathf.Sin(num4), 0f);
			GameObject gameObject = null;
			Vector3 vector3 = right * num5 * (float)num + vector2 * num6 * (float)num;
			Vector3 position = floorComp.gameObject.transform.position + vector + vector3;
			if (Application.isPlaying)
			{
				gameObject = UnityEngine.Object.Instantiate(meshFloor, position, Quaternion.identity);
			}
			gameObject.name = $"{floorComp.seqID}-{i + 1}/Floor freeroam x{num5} y{num6}";
			gameObject.gameObject.transform.parent = floorContainer.transform;
			scrFloor component = gameObject.GetComponent<scrFloor>();
			component.prevfloor = floorComp;
			component.nextfloor = floorComp.nextfloor;
			component.freeroam = true;
			component.freeroamGenerated = true;
			component.freeroamRegion = listFreeroamStartTiles.Count - 1;
			component.freeroamPosition = new Vector2Int(num5, num6);
			FloorMesh floorMesh = component.GetComponent<FloorMeshRenderer>().floorMesh;
			floorComp.freeroamFloors.Add(component);
			component.isCCW = floorComp.isCCW;
			component.numPlanets = floorComp.numPlanets;
			listFreeroam.Last().Add(component);
			component.angleCorrectionType = floorComp.angleCorrectionType;
			component.floatDirection = 0f;
			component.stringDirection = 'R';
			component.styleNum = floorComp.styleNum;
			component.initialTrackStyle = floorComp.initialTrackStyle;
			component.seqID = floorComp.seqID;
			component.entryangle = -1.5707963705062866;
			component.exitangle = 1.5707963705062866;
			component.radiusScale = radiusScale;
			component.speed = floorComp.speed;
			component.transform.localScale = Vector3.up * x + Vector3.right * x;
			if (Application.isPlaying)
			{
				component.SetColor(floorComp.floorRenderer.color);
			}
			component.SetTrackStyle(component.initialTrackStyle, initial: true);
			float width = (floorMesh._length = Mathf.Lerp(ADOBase.controller.baseFloorDimensions.y, ADOBase.controller.baseFloorDimensions.x, 0.4f));
			floorMesh._width = width;
			component.startPos = gameObject.transform.position;
			component.startRot = gameObject.transform.rotation.eulerAngles;
			component.tweenRot = component.startRot;
			component.UpdateAngle();
		}
	}

	public void ColorFreeroam()
	{
		foreach (FreeroamArea item in listFreeroam)
		{
			foreach (scrFloor item2 in item)
			{
				item2.SetColor(item.parentFloor.floorRenderer.material.GetColor("_Color"));
			}
			item.parentFloor.floorRenderer.renderer.enabled = false;
		}
	}

	public void DrawHolds(bool unfillHolds = false)
	{
		GameObject gameObject = GameObject.Find("Hold Container");
		if (gameObject == null)
		{
			gameObject = new GameObject("Hold Container");
		}
		holdContainer = gameObject;
		if (holdMaterial == null)
		{
			holdMaterial = new Material(RDConstants.data.holdShader);
			holdMaterial.SetTexture("_MainTex", lm2.holdTex);
			holdMaterial.renderQueue = 2900;
		}
		foreach (GameObject holdGO in holdGOs)
		{
			UnityEngine.Object.Destroy(holdGO);
		}
		holdGOs.Clear();
		foreach (scrFloor listFloor in listFloors)
		{
			if (listFloor.holdLength >= 0)
			{
				GameObject gameObject2 = new GameObject();
				scrHoldRenderer scrHoldRenderer2 = gameObject2.AddComponent<scrHoldRenderer>();
				gameObject2.AddComponent<MeshFilter>();
				gameObject2.AddComponent<MeshRenderer>();
				gameObject2.name = "Hold";
				gameObject2.transform.parent = holdContainer.transform;
				holdGOs.Add(gameObject2);
				Mesh mesh = gameObject2.GetComponent<MeshFilter>().mesh;
				MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
				mesh.Clear();
				component.material = new Material(holdMaterial);
				scrHoldRenderer2.m_mesh = mesh;
				scrHoldRenderer2.m_meshRenderer = component;
				listFloor.holdRenderer = scrHoldRenderer2;
				scrHoldRenderer2.startFloor = listFloor;
				scrHoldRenderer2.CreateMesh();
				if (unfillHolds)
				{
					scrHoldRenderer2.Unfill();
				}
			}
		}
	}

	public int DrawMultiPlanet(bool forcePlaying = false)
	{
		bool flag = !forcePlaying && scrController.instance.paused;
		foreach (PlanetRenderer dummyPlanet in scrController.instance.dummyPlanets)
		{
			if (dummyPlanet != null)
			{
				UnityEngine.Object.Destroy(dummyPlanet.gameObject);
			}
		}
		foreach (LineRenderer multiPlanetLine in scrController.instance.multiPlanetLines)
		{
			if (multiPlanetLine != null)
			{
				UnityEngine.Object.Destroy(multiPlanetLine.gameObject);
			}
		}
		if (RDC.hideTaroGimmicks)
		{
			return 2;
		}
		scrController.instance.dummyPlanets.Clear();
		scrController.instance.multiPlanetLines.Clear();
		int num = 2;
		int num2 = 2;
		foreach (scrFloor listFloor in listFloors)
		{
			if (!flag && listFloor.seqID < GCS.checkpointNum)
			{
				continue;
			}
			_ = listFloor.numPlanets;
			_ = listFloor.speed;
			bool flag2 = (GCS.FOOL_SWIRL ? (!listFloor.isCCW) : listFloor.isCCW);
			int numPlanets = listFloor.numPlanets;
			num = Math.Max(num, numPlanets);
			if (numPlanets > num2)
			{
				float num3 = (float)listFloor.entryangle + (float)((!flag2) ? 1 : (-1)) * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(numPlanets) / 2f;
				double num4 = (double)scrController.instance.tileSize / (2.0 * (double)Mathf.Sin(3.141592f / (float)numPlanets));
				double num5 = 0.0;
				Vector3 vector = new Vector3(Mathf.Sin(num3) * (float)num4, Mathf.Cos(num3) * (float)num4, 0f);
				for (int i = num2; i < numPlanets; i++)
				{
					PlanetRenderer planetRenderer = UnityEngine.Object.Instantiate(scrController.instance.planetRed.planetRenderer);
					UnityEngine.Object.Destroy(planetRenderer.GetComponent<scrPlanet>());
					planetRenderer.ringComp.enabled = false;
					planetRenderer.ringComp.transform.localScale = Vector3.zero;
					planetRenderer.sprite.visible = true;
					planetRenderer.name = $"PlanetDummy {i} on floor {listFloor.seqID}";
					planetRenderer.EnableCustomColor();
					planetRenderer.SetPlanetColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					planetRenderer.SetTailColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					planetRenderer.SetEmojiMode(enabled: false);
					planetRenderer.attachedDummyFloor = listFloor.seqID;
					planetRenderer.DisableParticles();
					Renderer[] componentsInChildren = planetRenderer.GetComponentsInChildren<Renderer>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						componentsInChildren[j].sortingOrder = -1;
					}
					if (flag)
					{
						planetRenderer.Destroy();
					}
					listFloor.dummyPlanets.Add(planetRenderer);
					num5 = (double)((!flag2) ? 1 : (-1)) * (Math.PI * 2.0 * (((double)(1 - i) - (double)numPlanets / 2.0) / (double)numPlanets)) + (double)num3;
					planetRenderer.transform.localPosition = new Vector3(vector.x + Mathf.Sin((float)num5) * (float)num4, vector.y + Mathf.Cos((float)num5) * (float)num4, vector.z);
					scrController.instance.dummyPlanets.Add(planetRenderer);
					planetRenderer.transform.SetParent(listFloor.transform, worldPositionStays: false);
				}
				GameObject obj = new GameObject();
				LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
				lineRenderer.positionCount = numPlanets + 1;
				obj.transform.parent = listFloor.transform;
				lineRenderer.material = scrController.instance.lineMaterial;
				lineRenderer.textureMode = LineTextureMode.Tile;
				lineRenderer.sortingLayerName = "Floor";
				lineRenderer.sortingOrder = listFloor.floorRenderer.renderer.sortingOrder + 6;
				lineRenderer.startWidth = 0.05f;
				lineRenderer.endWidth = 0.05f;
				Color endColor = (lineRenderer.startColor = scrController.instance.lineColor.WithAlpha(listFloor.opacity * 0.5f));
				lineRenderer.endColor = endColor;
				lineRenderer.name = "Planet polygon indicator (floor " + listFloor.seqID + ")";
				lineRenderer.useWorldSpace = false;
				for (int k = 0; k < numPlanets + 1; k++)
				{
					num5 = (double)((!flag2) ? 1 : (-1)) * (Math.PI * 2.0 * (((double)(1 - k) - (double)numPlanets / 2.0) / (double)numPlanets)) + (double)num3;
					lineRenderer.SetPosition(k, new Vector3(vector.x + Mathf.Sin((float)num5) * (float)num4, vector.y + Mathf.Cos((float)num5) * (float)num4, vector.z));
				}
				listFloor.multiplanetLine = lineRenderer;
				lineRenderer.transform.position = listFloor.transform.position;
				lineRenderer.transform.eulerAngles = listFloor.transform.eulerAngles;
				scrController.instance.multiPlanetLines.Add(lineRenderer);
			}
			else if (numPlanets < num2)
			{
				float num6 = (float)listFloor.entryangle + (float)((!flag2) ? 1 : (-1)) * (float)scrMisc.GetInverseAnglePerBeatMultiplanet(num2) / 2f;
				double num7 = (double)scrController.instance.tileSize / (2.0 * (double)Mathf.Sin(3.141592f / (float)num2));
				double num8 = 0.0;
				Vector3 vector2 = new Vector3(Mathf.Sin(num6) * (float)num7, Mathf.Cos(num6) * (float)num7, 0f);
				for (int l = numPlanets; l < num2; l++)
				{
					PlanetRenderer planetRenderer2 = UnityEngine.Object.Instantiate(scrController.instance.planetRed.planetRenderer);
					UnityEngine.Object.Destroy(planetRenderer2.GetComponent<scrPlanet>());
					planetRenderer2.name = "PlanetDummy " + l + " on floor " + listFloor.seqID;
					planetRenderer2.EnableCustomColor();
					planetRenderer2.SetPlanetColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					planetRenderer2.SetTailColor(new Color(0.9f, 0.9f, 0.9f, 1f));
					planetRenderer2.SetEmojiMode(enabled: false);
					planetRenderer2.DisableParticles();
					planetRenderer2.Destroy(!flag);
					planetRenderer2.attachedDummyFloor = listFloor.seqID;
					listFloor.dummyPlanets.Add(planetRenderer2);
					num8 = (double)((!flag2) ? 1 : (-1)) * (Math.PI * 2.0 * (((double)(1 - l) - (double)num2 / 2.0) / (double)num2)) + (double)num6;
					planetRenderer2.transform.localPosition = new Vector3(vector2.x + Mathf.Sin((float)num8) * (float)num7, vector2.y + Mathf.Cos((float)num8) * (float)num7, vector2.z) + listFloor.transform.position;
					planetRenderer2.ringComp.transform.localScale = Vector3.one * 0.4f;
					planetRenderer2.ringComp.enabled = false;
					scrController.instance.dummyPlanets.Add(planetRenderer2);
					planetRenderer2.transform.SetParent(listFloor.transform);
				}
			}
			num2 = numPlanets;
		}
		scrController.instance.lineMaterial.DOFloat(scrController.instance.lineMaterial.GetFloat("_Time0") + 10f, "_Time0", 10f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental)
			.SetUpdate(isIndependentUpdate: true);
		return num;
	}

	public void RefreshAngles()
	{
		foreach (scrFloor listFloor in listFloors)
		{
			listFloor.UpdateAngle();
		}
	}

	public static double StringToAngle(char path)
	{
		return path switch
		{
			'R' => (float)Math.PI / 2f, 
			'U' => 0f, 
			'L' => 4.712389f, 
			'D' => (float)Math.PI, 
			'Q' => 5.4977875f, 
			'E' => (float)Math.PI / 4f, 
			'Z' => 3.926991f, 
			'C' => (float)Math.PI * 3f / 4f, 
			'T' => (float)Math.PI / 6f, 
			'B' => 2.6179938f, 
			'F' => 3.6651914f, 
			'G' => 5.7595863f, 
			'J' => (float)Math.PI / 3f, 
			'M' => (float)Math.PI * 2f / 3f, 
			'N' => 4.1887903f, 
			'H' => 5.2359877f, 
			'p' => 1.3089969f, 
			'o' => (float)Math.PI / 12f, 
			'q' => 6.0213857f, 
			'W' => 4.9741883f, 
			'x' => 4.4505897f, 
			'V' => 3.403392f, 
			'Y' => 2.8797932f, 
			'A' => 1.8325957f, 
			'!' => 999f, 
			_ => 0f, 
		};
	}
}
