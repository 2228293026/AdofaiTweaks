using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;

public static class Analytics
{
	public static float OfficialLevelsTime;

	public static float editorMakingTime;

	public static float editorPlayingTime;

	public static float customLevelsTime;

	public static void LimitAnalyticsIfModded()
	{
		if (RDUtils.GameIsModded())
		{
			Debug.Log("Mods detected! Disabling exception capturing");
			CrashReportHandler.enableCaptureExceptions = false;
		}
	}

	public static void UploadStatsToSteam()
	{
		if (SteamIntegration.initialized)
		{
			SteamUserStats.SetStat("hoursOfficialLevels", OfficialLevelsTime);
			SteamUserStats.SetStat("hoursEditorMaking", editorMakingTime);
			SteamUserStats.SetStat("hoursEditorPlaying", editorPlayingTime);
			SteamUserStats.SetStat("hoursCustomLevelsPlaying", customLevelsTime);
			SteamUserStats.StoreStats();
			OfficialLevelsTime = 0f;
			editorMakingTime = 0f;
			editorPlayingTime = 0f;
			customLevelsTime = 0f;
		}
	}

	public static void UploadStatsToSteam(float _hoursOff = 0f, float _hoursEdMaking = 0f, float _hoursEdPlaying = 0f, float _hoursCustomPlaying = 0f)
	{
		if (SteamIntegration.initialized)
		{
			SteamUserStats.SetStat("hoursOfficialLevels", _hoursOff);
			SteamUserStats.SetStat("hoursEditorMaking", _hoursEdMaking);
			SteamUserStats.SetStat("hoursEditorPlaying", _hoursEdPlaying);
			SteamUserStats.SetStat("hoursCustomLevelsPlaying", _hoursCustomPlaying);
			SteamUserStats.StoreStats();
		}
	}

	public static void UploadBranchToUnity()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		int dayOfYear = DateTime.Now.DayOfYear;
		if (Persistence.lastAnalyticsUpdate != dayOfYear)
		{
			Persistence.lastAnalyticsUpdate = dayOfYear;
			string text = "unknown";
			if (SteamIntegration.initialized)
			{
				SteamApps.GetCurrentBetaName(ref text, 20);
			}
			else if (Application.dataPath.Contains(Path.Combine("steamapps", "common", "A Dance of Fire and Ice")))
			{
				text = "steam-offline";
			}
			else if (ADOBase.platform == Platform.Android)
			{
				text = "android";
			}
			else if (ADOBase.platform == Platform.iOS)
			{
				text = "ios";
			}
			if (!text.IsNullOrEmpty())
			{
				Analytics.CustomEvent("VersionInfo", (IDictionary<string, object>)new Dictionary<string, object> { { "branch", text } });
			}
		}
	}
}
