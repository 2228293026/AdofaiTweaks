using System;
using System.Collections.Generic;
using System.Linq;
using GDMiniJSON;
using UnityEngine;

namespace ADOFAI;

public class LevelDataCLS : GenericDataCLS
{
	public LevelEvent songSettings;

	public LevelEvent levelSettings;

	public string[] workshopTags = new string[0];

	public override string artist => (string)levelSettings["artist"];

	public override string title => (string)levelSettings["song"];

	public override string author => (string)levelSettings["author"];

	public override string previewImage => (string)levelSettings["previewImage"];

	public override string previewIcon => (string)levelSettings["previewIcon"];

	public override Color previewIconColor => levelSettings.GetColor("previewIconColor");

	public int previewSongStart => (int)levelSettings["previewSongStart"];

	public int previewSongDuration => (int)levelSettings["previewSongDuration"];

	public bool seizureWarning => (bool)levelSettings["seizureWarning"];

	public override string description => (string)levelSettings["levelDesc"];

	public string artistLinks => (string)levelSettings["artistLinks"];

	public float speedTrialAim => (float)levelSettings["speedTrialAim"];

	public override int difficulty => (int)levelSettings["difficulty"];

	public override string[] tags => ((string)levelSettings["levelTags"]).Replace(", ", ",").Split(',', StringSplitOptions.None);

	public DLCManager[] requiredDLCs => DLCManager.DLCManagers.Where((DLCManager x) => !string.IsNullOrEmpty(x.steamWorkshopTag) && (tags.Contains(x.steamWorkshopTag) || workshopTags.Contains(x.steamWorkshopTag))).ToArray();

	public string songFilename
	{
		get
		{
			return (string)songSettings["songFilename"];
		}
		set
		{
			songSettings["songFilename"] = value;
		}
	}

	public int volume => (int)songSettings["volume"];

	public LoadResult loadResult { get; private set; }

	public LevelDataCLS()
	{
		Dictionary<string, LevelEventInfo> settingsInfo = GCS.settingsInfo;
		songSettings = new LevelEvent(0, LevelEventType.SongSettings, settingsInfo["SongSettings"]);
		levelSettings = new LevelEvent(0, LevelEventType.LevelSettings, settingsInfo["LevelSettings"]);
	}

	public bool LoadLevel(string levelPath)
	{
		if (Json.Deserialize(RDFile.ReadAllText(levelPath)) is Dictionary<string, object> rootDict)
		{
			return Decode(rootDict);
		}
		return false;
	}

	public bool Decode(Dictionary<string, object> rootDict)
	{
		loadResult = LoadResult.Error;
		Dictionary<string, object> dictionary = rootDict["settings"] as Dictionary<string, object>;
		if ((int)dictionary["version"] > 18)
		{
			loadResult = LoadResult.FutureVersion;
		}
		if (dictionary.TryGetValue("requiredMods", out var value) && RDEditorUtils.CheckModsDependency(value as object[]))
		{
			loadResult = LoadResult.ModRequired;
			return false;
		}
		levelSettings.Decode(dictionary, "LevelSettings", isGlobal: true);
		songSettings.Decode(dictionary, "SongSettings", isGlobal: true);
		return true;
	}

	public bool IsMissingRequiredMetadata()
	{
		if (!(artist.Trim() == "") && !(title == "") && !(author == ""))
		{
			return previewImage == "";
		}
		return true;
	}
}
