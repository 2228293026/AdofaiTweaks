using System;
using ADOFAI.Editor;
using UnityEngine;

namespace ADOFAI;

public static class EditorConstants
{
	public struct EditorKeyShortcut(string key, KeyCode keyCode, KeyCode otherKeyCode, bool usingShift, bool usingCtrl, bool usingAlt, KeyModifier otherKeyModifierMask = KeyModifier.None, bool ctrlIsCmd = true)
	{
		public string key = key;

		public KeyCode keyCode = keyCode;

		public KeyCode otherKeyCode = otherKeyCode;

		public bool usingShift = usingShift;

		public bool usingCtrl = usingCtrl;

		public bool usingAlt = usingAlt;

		public bool ctrlIsCmd = ctrlIsCmd;

		public KeyModifier otherKeyModifierMask = otherKeyModifierMask;
	}

	public const int adofaiFileVersion = 18;

	public const string key_eventType = "eventType";

	public const string key_floor = "floor";

	public const string key_enabled = "enabled";

	public const string key_active = "active";

	public const string key_visible = "visible";

	public const string key_locked = "locked";

	public const string key_pathData = "pathData";

	public const string key_angleData = "angleData";

	public const string key_settings = "settings";

	public const string key_actions = "actions";

	public const string key_decorations = "decorations";

	public const string key_version = "version";

	public const string key_songFilename = "songFilename";

	public const string key_author = "author";

	public const string key_song = "song";

	public const string key_specialArtistType = "specialArtistType";

	public const string key_artist = "artist";

	public const string key_previewImage = "previewImage";

	public const string key_previewIcon = "previewIcon";

	public const string key_previewIconColor = "previewIconColor";

	public const string key_previewSongStart = "previewSongStart";

	public const string key_previewSongDuration = "previewSongDuration";

	public const string key_seizureWarning = "seizureWarning";

	public const string key_levelDesc = "levelDesc";

	public const string key_levelTags = "levelTags";

	public const string key_artistPermission = "artistPermission";

	public const string key_artistLinks = "artistLinks";

	public const string key_speedTrialAim = "speedTrialAim";

	public const string key_difficulty = "difficulty";

	public const string key_bpm = "bpm";

	public const string key_volume = "volume";

	public const string key_hitsound = "hitsound";

	public const string key_hitsoundVolume = "hitsoundVolume";

	public const string key_separateCountdownTime = "separateCountdownTime";

	public const string key_countdownTicks = "countdownTicks";

	public const string key_trackColorType = "trackColorType";

	public const string key_trackColor = "trackColor";

	public const string key_trackShadowColor = "trackShadowColor";

	public const string key_secondaryTrackColor = "secondaryTrackColor";

	public const string key_tileShape = "tileShape";

	public const string key_trackColorAnimDuration = "trackColorAnimDuration";

	public const string key_trackColorPulse = "trackColorPulse";

	public const string key_trackPulseLength = "trackPulseLength";

	public const string key_trackStyle = "trackStyle";

	public const string key_trackTexture = "trackTexture";

	public const string key_trackTextureScale = "trackTextureScale";

	public const string key_trackAnimation = "trackAnimation";

	public const string key_trackDisappearAnimation = "trackDisappearAnimation";

	public const string key_trackBeatsAhead = "beatsAhead";

	public const string key_trackBeatsBehind = "beatsBehind";

	public const string key_trackGlowIntensity = "trackGlowIntensity";

	public const string key_pitch = "pitch";

	public const string key_offset = "offset";

	public const string key_backgroundColor = "backgroundColor";

	public const string key_bgImage = "bgImage";

	public const string key_bgImageColor = "bgImageColor";

	public const string key_parallax = "parallax";

	public const string key_bgDisplayMode = "bgDisplayMode";

	public const string key_loopBG = "loopBG";

	public const string key_bgLockRot = "lockRot";

	public const string key_bgSmoothing = "imageSmoothing";

	public const string key_bgShowDefault = "showDefaultBGIfNoImage";

	public const string key_scalingRatio = "scalingRatio";

	public const string key_camRelativeTo = "relativeTo";

	public const string key_camPosition = "position";

	public const string key_camRotation = "rotation";

	public const string key_camZoom = "zoom";

	public const string key_doStartCamOnLowVFX = "startCamLowVFX";

	public const string key_pulseOnFloor = "pulseOnFloor";

	public const string key_bgVideo = "bgVideo";

	public const string key_floorIconOutlines = "floorIconOutlines";

	public const string key_stickToFloors = "stickToFloors";

	public const string key_planetEase = "planetEase";

	public const string key_planetEaseParts = "planetEaseParts";

	public const string key_planetEasePartBehavior = "planetEasePartBehavior";

	public const string key_legacyFlash = "legacyFlash";

	public const string key_legacySpriteTiles = "legacySpriteTiles";

	public const string key_legacyCamRelativeTo = "legacyCamRelativeTo";

	public const string key_legacyTween = "legacyTween";

	public const string key_disableV15Features = "disableV15Features";

	public const string key_legacyPause = "legacyPause";

	public const string key_requiredMods = "requiredMods";

	public const string key_showDefaultBGTile = "showDefaultBGTile";

	public const string key_defaultBGTileColor = "defaultBGTileColor";

	public const string key_defaultBGShapeType = "defaultBGShapeType";

	public const string key_defaultBGShapeColor = "defaultBGShapeColor";

	public const string key_defaultTextColor = "defaultTextColor";

	public const string key_defaultTextShadowColor = "defaultTextShadowColor";

	public const string key_congratsText = "congratsText";

	public const string key_perfectText = "perfectText";

	public const int ConditionalArraySize = 9;

	public static readonly int InputEventStateSize = Enum.GetValues(typeof(InputEventState)).Length;

	public static readonly int InputEventTargetSize = Enum.GetValues(typeof(InputEventTarget)).Length;

	public static readonly LevelEventType[] soloTypes = new LevelEventType[22]
	{
		LevelEventType.Twirl,
		LevelEventType.Multitap,
		LevelEventType.Checkpoint,
		LevelEventType.SetHitsound,
		LevelEventType.ChangeTrack,
		LevelEventType.ColorTrack,
		LevelEventType.AnimateTrack,
		LevelEventType.SetPlanetRotation,
		LevelEventType.KillPlayer,
		LevelEventType.Hold,
		LevelEventType.SetHoldSound,
		LevelEventType.SetConditionalEvents,
		LevelEventType.MultiPlanet,
		LevelEventType.FreeRoam,
		LevelEventType.Pause,
		LevelEventType.AutoPlayTiles,
		LevelEventType.Hide,
		LevelEventType.ScaleMargin,
		LevelEventType.ScaleRadius,
		LevelEventType.TileDimensions,
		LevelEventType.SetConditionalEvents,
		LevelEventType.Bookmark
	};

	public static readonly LevelEventType[] toggleableTypes = new LevelEventType[4]
	{
		LevelEventType.Twirl,
		LevelEventType.Multitap,
		LevelEventType.Checkpoint,
		LevelEventType.Bookmark
	};

	public static readonly LevelEventType[] settingsTypes = new LevelEventType[8]
	{
		LevelEventType.SongSettings,
		LevelEventType.LevelSettings,
		LevelEventType.TrackSettings,
		LevelEventType.BackgroundSettings,
		LevelEventType.CameraSettings,
		LevelEventType.MiscSettings,
		LevelEventType.EventSettings,
		LevelEventType.DecorationSettings
	};

	public static bool IsSetting(this LevelEventType type)
	{
		if (type != LevelEventType.SongSettings && type != LevelEventType.LevelSettings && type != LevelEventType.TrackSettings && type != LevelEventType.BackgroundSettings && type != LevelEventType.CameraSettings && type != LevelEventType.MiscSettings && type != LevelEventType.EventSettings)
		{
			return type == LevelEventType.DecorationSettings;
		}
		return true;
	}
}
