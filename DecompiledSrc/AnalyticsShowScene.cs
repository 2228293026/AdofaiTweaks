using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;

public class AnalyticsShowScene : ADOBase
{
	public TextMeshProUGUI textHolderPersonal;

	public TextMeshProUGUI textHolderGlobal;

	public TMP_InputField field;

	public UIGridRenderer grid;

	public UILineRenderer lineRenderer;

	public GameObject DrawButtonsPanel;

	public GameObject DebugPanel;

	private string steamName;

	private int cls_entered;

	private int editor_entered;

	private float hoursOfficialLevels;

	private float hoursEditorMaking;

	private float hoursEditorPlaying;

	private float hoursCustomLevelsPlaying;

	private int levelselect_entered;

	[SerializeField]
	public List<StatsData> globalStats = new List<StatsData>();

	private int daysPeriodCache;

	private void Awake()
	{
		_ = SteamIntegration.instance;
		SteamUserStats.RequestCurrentStats();
	}

	private void Start()
	{
		GetPersonalStats();
		DrawButtonsPanel.SetActive(value: false);
	}

	public void UploadValues()
	{
		Analytics.UploadStatsToSteam(hoursOfficialLevels, hoursEditorMaking, hoursEditorPlaying, hoursCustomLevelsPlaying);
	}

	public void GetGlobalStats()
	{
		if (SteamIntegration.initialized)
		{
			Debug.Log("getting global values");
			if (int.TryParse(field.text, out var result) && field.text != null)
			{
				RetrieveGlobalStats(result);
				DrawButtonsPanel.SetActive(value: true);
			}
		}
	}

	public void AnalyticsValueUp()
	{
		if (SteamIntegration.initialized)
		{
			hoursOfficialLevels += 1f;
			RefreshPersonalStatsText();
		}
	}

	public void AnalyticsValueDown()
	{
		if (SteamIntegration.initialized)
		{
			hoursOfficialLevels -= 1f;
			RefreshPersonalStatsText();
		}
	}

	private void GetPersonalStats()
	{
		if (SteamIntegration.initialized)
		{
			steamName = SteamFriends.GetPersonaName();
			SteamUserStats.GetStat("cls_entered", ref cls_entered);
			SteamUserStats.GetStat("editor_entered", ref editor_entered);
			SteamUserStats.GetStat(globalStats[0].globalStatField, ref hoursOfficialLevels);
			SteamUserStats.GetStat(globalStats[1].globalStatField, ref hoursEditorMaking);
			SteamUserStats.GetStat(globalStats[2].globalStatField, ref hoursEditorPlaying);
			SteamUserStats.GetStat(globalStats[3].globalStatField, ref hoursCustomLevelsPlaying);
			SteamUserStats.GetStat("levelselect_entered", ref levelselect_entered);
			RefreshPersonalStatsText();
		}
	}

	private void RefreshPersonalStatsText()
	{
		string text = "";
		text = "Personal Stats:\n\n";
		text = text + steamName + "\n";
		text = text + "cls_entered: " + cls_entered + "\n";
		text = text + "editor_entered: " + editor_entered + "\n";
		text = text + "hoursOfficialLevels: " + hoursOfficialLevels + "\n";
		text = text + "hoursEditorMaking: " + hoursEditorMaking + "\n";
		text = text + "hoursEditorPlaying: " + hoursEditorPlaying + "\n";
		text = text + "hoursCustomLevelsPlaying: " + hoursCustomLevelsPlaying + "\n";
		text = text + "levelselect_entered: " + levelselect_entered + "\n";
		textHolderPersonal.text = text;
	}

	private void RetrieveGlobalStats(int _daysPeriod)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log("here " + _daysPeriod);
		textHolderGlobal.text = "";
		daysPeriodCache = _daysPeriod;
		SteamAPICall_t val = SteamUserStats.RequestGlobalStats(_daysPeriod);
		CallResult<GlobalStatsReceived_t>.Create((APIDispatchDelegate<GlobalStatsReceived_t>)null).Set(val, (APIDispatchDelegate<GlobalStatsReceived_t>)delegate(GlobalStatsReceived_t pCallback, bool bIOFailure)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if ((int)pCallback.m_eResult != 1 || bIOFailure)
			{
				Debug.Log("There was an error retrieving global stats.");
			}
			else
			{
				Debug.Log($"game id: {pCallback.m_nGameID}, daysPeriodCache {daysPeriodCache}");
				for (int i = 0; i < globalStats.Count; i++)
				{
					double[] array = new double[daysPeriodCache];
					SteamUserStats.GetGlobalStatHistory(globalStats[i].globalStatField, array, (uint)(daysPeriodCache * 8));
					globalStats[i].value = array;
				}
			}
		});
	}

	public void DrawButton(int _index)
	{
		DrawGraph(daysPeriodCache, _index);
	}

	private void DrawGraph(int _days, int _statIndex)
	{
		double num = 0.0;
		for (int i = 0; i < globalStats[_statIndex].value.Length; i++)
		{
			if (globalStats[_statIndex].value[i] > num)
			{
				num = globalStats[_statIndex].value[i];
			}
		}
		Debug.Log("MAX VALUE: " + num);
		grid.gridSize = new Vector2Int(_days, Mathf.FloorToInt((float)num + 1f));
		grid.SetAllDirty();
		lineRenderer.points.Clear();
		int num2 = 0;
		double[] value = globalStats[_statIndex].value;
		foreach (double num3 in value)
		{
			lineRenderer.points.Add(new Vector2(num2, (float)num3));
			num2++;
		}
		textHolderGlobal.text = globalStats[_statIndex].globalStatField + "\n";
		int num4 = 0;
		value = globalStats[_statIndex].value;
		foreach (double num5 in value)
		{
			textHolderGlobal.text += $"Day {num4 + 1}: {Mathf.Round((float)num5)}\n";
			num4++;
		}
	}

	private void Update()
	{
		if (SteamIntegration.initialized)
		{
			SteamAPI.RunCallbacks();
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				DebugPanel.SetActive(!DebugPanel.activeSelf);
			}
		}
	}

	private void OnGlobalStatsReceived(GlobalStatsReceived_t pCallback, bool bIOFailure)
	{
	}
}
