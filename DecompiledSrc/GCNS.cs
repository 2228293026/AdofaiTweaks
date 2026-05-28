using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDMiniJSON;
using UnityEngine;

public class GCNS
{
	public class WorldData
	{
		public enum IslandType
		{
			Main,
			Xtra,
			MuseDash,
			Crown,
			Taro,
			Vega,
			CR2024
		}

		public int index;

		public int levelCount;

		public bool hasCheckpoints;

		public float trialAim;

		public LevelSource levelSource;

		public bool isDLC;

		public bool isTech;

		public PortalCreditData levelCredits;

		public PortalCreditData songCredits;

		public PortalCreditData secondaryLevelCredits;

		public PortalCreditData secondarySongCredits;

		public PortalCreditData tertiaryLevelCredits;

		public PortalCreditData tertiarySongCredits;

		public int medalCount;

		public int requiredMedals;

		public int difficulty;

		public bool doNotBuild;

		public bool notRealWorld;

		public IslandType island;

		public DifficultyUIMode availableDifficulties;

		public bool isXtra => island == IslandType.Xtra;

		public bool isMuseDash => island == IslandType.MuseDash;

		public bool isCrown => island == IslandType.Crown;

		public bool isTaro => island == IslandType.Taro;

		public bool isCR2024 => island == IslandType.CR2024;

		public WorldData(Dictionary<string, object> dict)
		{
			Decode(dict);
		}

		public WorldData Decode(Dictionary<string, object> dict)
		{
			index = (int)dict["index"];
			levelCount = (int)dict["levelCount"];
			trialAim = (float)dict["speedTrial"];
			hasCheckpoints = (bool)dict["hasCheckpoints"];
			dict.TryGetValueAs("requiredMedals", out requiredMedals, 0);
			dict.TryGetValueAs("medalCount", out medalCount, 0);
			if (dict.TryGetValueAs<string, object, string>("levelSource", out var valueAs))
			{
				Enum.TryParse<LevelSource>(valueAs, ignoreCase: true, out levelSource);
			}
			dict.TryGetValueAs("isDLC", out isDLC, _default: false);
			dict.TryGetValueAs("isTech", out isTech, _default: false);
			dict.TryGetValueAs("doNotBuild", out doNotBuild, _default: false);
			dict.TryGetValueAs("notRealWorld", out notRealWorld, _default: false);
			if (ADOBase.isExpo)
			{
				dict.TryGetValueAs("doNotBuildExpo", out var valueAs2, _default: false);
				dict.TryGetValueAs("difficulty", out difficulty, 0);
				doNotBuild |= valueAs2;
			}
			if (dict.TryGetValueAs<string, object, string>("availableDifficulties", out var valueAs3))
			{
				Enum.TryParse<DifficultyUIMode>(valueAs3, ignoreCase: true, out availableDifficulties);
			}
			if (dict.TryGetValueAs<string, object, string>("island", out var valueAs4))
			{
				Enum.TryParse<IslandType>(valueAs4, ignoreCase: true, out island);
			}
			DecodeCredits("levelCredits", out levelCredits);
			DecodeCredits("secondaryLevelCredits", out secondaryLevelCredits);
			DecodeCredits("tertiaryLevelCredits", out tertiaryLevelCredits);
			DecodeCredits("songCredits", out songCredits);
			DecodeCredits("secondarySongCredits", out secondarySongCredits);
			DecodeCredits("tertiarySongCredits", out tertiarySongCredits);
			return this;
			void DecodeCredits(string key, out PortalCreditData credits)
			{
				Dictionary<string, object> valueAs5;
				bool flag = dict.TryGetValueAs<string, object, Dictionary<string, object>>(key, out valueAs5);
				credits = (flag ? new PortalCreditData(valueAs5) : null);
			}
		}
	}

	public record FeaturedFolder(string folderId, string title, string artist, int difficulty, string iconColor, uint[] levelIds, bool isTech);

	public const int releaseNumber = 141;

	public static string buildDate = "not-set";

	public static string buildCommit = "not-set";

	private static Dictionary<string, WorldData> _worldData;

	private static string[] _allWorlds;

	private static string[] _dlcWorlds;

	private static string[] _xtraWorlds;

	private static string[] _museDashWorlds;

	private static string[] _crownWorlds;

	public static readonly string[] taroMenus = new string[7] { "scnTaroMenu0", "scnTaroMenu1", "scnTaroMenu2", "scnTaroMenu3", "TP-1", "TP-2", "TP-X" };

	public const int deathsForNewBestJingle = 5;

	public const float minCompletionForNewBestJingle = 0.01f;

	public const int deathsForNewBestApplause = 15;

	public const float practiceSpeedMultiplier = 0.9f;

	public const int defaultPracticeLength = 20;

	public static readonly uint[] FeaturedLevelsIDs = new uint[62]
	{
		2731825935u, 2731817282u, 2731815380u, 2776211226u, 2779312583u, 2779318434u, 2787254380u, 2801793853u, 2802386409u, 2802387483u,
		2804476936u, 2804919454u, 2833657749u, 2833658365u, 2833658039u, 2833657910u, 2895341132u, 2895342067u, 2895343692u, 2897296058u,
		2967497446u, 2967489597u, 2967490490u, 2967491059u, 2970486324u, 2975797214u, 2980872420u, 3012374627u, 3012378858u, 3029188141u,
		3012388315u, 3012389896u, 3012393341u, 3145834958u, 3145844473u, 3145851453u, 3152850530u, 3152852919u, 3152854549u, 3152888560u,
		3296283989u, 3296277169u, 3296315094u, 3296306648u, 3296286495u, 3371821451u, 3371822363u, 3371823581u, 3371824643u, 3371825727u,
		3371831526u, 3371839690u, 3481211876u, 3481210158u, 3481207644u, 3463697889u, 3463697226u, 3481206756u, 3463697353u, 3651162494u,
		3651156043u, 3530442246u
	};

	public static readonly uint[] TechFeaturedLevelsIDs = new uint[18]
	{
		2802388539u, 2802389102u, 2801799196u, 3386616364u, 3386619254u, 3386617648u, 3386618455u, 3530451707u, 3530453333u, 3540508961u,
		3530458012u, 3530458498u, 3530458988u, 3530461261u, 3530460217u, 3652852034u, 3651169643u, 3651163732u
	};

	public const float MaxTraitValue = 22f;

	public static readonly Dictionary<uint, Vector4> TechFeaturedLevelsTraitValues = new Dictionary<uint, Vector4>
	{
		[2802388539u] = new Vector4(15f, 14f, 13f, 15f),
		[2802389102u] = new Vector4(17f, 21f, 18f, 17f),
		[2801799196u] = new Vector4(9f, 6f, 7f, 4f),
		[3386616364u] = new Vector4(3f, 7f, 3f, 2f),
		[3386619254u] = new Vector4(11f, 8f, 7f, 6f),
		[3386617648u] = new Vector4(12f, 12f, 13f, 15f),
		[3386618455u] = new Vector4(17f, 12f, 12f, 11f),
		[3530451707u] = new Vector4(18f, 17f, 17f, 14f),
		[3530453333u] = new Vector4(14f, 14f, 13f, 15f),
		[3540508961u] = new Vector4(16f, 12f, 14f, 15f),
		[3530458012u] = new Vector4(12f, 8f, 7f, 7f),
		[3530458498u] = new Vector4(7f, 8f, 5f, 12f),
		[3530458988u] = new Vector4(20f, 19f, 19f, 17f),
		[3530461261u] = new Vector4(15f, 18f, 14f, 20f),
		[3530460217u] = new Vector4(12f, 9f, 7f, 7f),
		[3652852034u] = new Vector4(13f, 9f, 9f, 9f),
		[3651169643u] = new Vector4(15f, 14f, 14f, 13f),
		[3651163732u] = new Vector4(14f, 14f, 12f, 11f)
	};

	public static readonly uint[] FeralNormalLevelsIds = new uint[2] { 2802386409u, 2802387483u };

	public static readonly FeaturedFolder[] featuredFolders = new FeaturedFolder[5]
	{
		new FeaturedFolder("Feral", "Feral [Easy] & [Medium]", "meganeko", 4, "eeaaff", new uint[2] { 2802386409u, 2802387483u }, isTech: false),
		new FeaturedFolder("Skyscape", "Skyscape", "Plum", 5, "15b7f8", new uint[2] { 2804476936u, 2804919454u }, isTech: false),
		new FeaturedFolder("FogSeesIt", "Fog sees it", "Plant Guy", 5, "dfb6a3", new uint[2] { 3029188141u, 3012388315u }, isTech: false),
		new FeaturedFolder("Avantgarde", "Avantgarde", "Raimukun", 9, "91a1a6", new uint[2] { 3296283989u, 3296277169u }, isTech: false),
		new FeaturedFolder("FeralTech", "Feral [Hard] & [Insane]", "meganeko", 9, "eeaaff", new uint[2] { 2802388539u, 2802389102u }, isTech: true)
	};

	public const string sceneSplash = "scnSplash";

	public const string sceneLevelSelectDesktop = "scnLevelSelect";

	public const string sceneLevelSelectMobile = "scnMobileMenu";

	public const string sceneLoading = "scnLoading";

	public const string sceneCalibration = "scnCalibration";

	public const string sceneEditor = "scnEditor";

	public const string sceneGame = "scnGame";

	public const string sceneCustomLevelSelect = "scnCLS";

	public const string sceneTaroMenu0 = "scnTaroMenu0";

	public const string sceneTaroMenu1 = "scnTaroMenu1";

	public const string sceneTaroMenu2 = "scnTaroMenu2";

	public const string sceneTaroMenu3 = "scnTaroMenu3";

	public const string sceneTPTest = "TP-Test";

	public const string sceneTP1 = "TP-1";

	public const string sceneTP2 = "TP-2";

	public const string sceneTP3 = "TP-X";

	public const string sceneMinesweeper = "scnMinesweeper";

	public const string sceneVegaMenu = "scnVegaMenu";

	public static readonly string[] devBranches = new string[4] { "stardust", "deepspace", "frontline", "steamdeck" };

	public static readonly string[] stableBranches = new string[2] { "public", "older" };

	public static readonly string[] publicBranches = new string[3] { "public", "beta", "older" };

	public static readonly string[] internalBranches = new string[1] { "blackhole" };

	public const string linkNeoCosmosTrailer = "https://7thbe.at/neo-cosmos-trailer";

	public static string[] neoCosmosScenesHashes = null;

	public static string[] neoCosmosShadersHashes = null;

	public static Dictionary<string, WorldData> worldData
	{
		get
		{
			if (_worldData == null)
			{
				_worldData = new Dictionary<string, WorldData>();
				foreach (KeyValuePair<string, object> item in Json.Deserialize(Resources.Load<TextAsset>("LevelMetadata").text) as Dictionary<string, object>)
				{
					string key = item.Key;
					WorldData value = new WorldData(item.Value as Dictionary<string, object>);
					_worldData.Add(key, value);
				}
			}
			return _worldData;
		}
	}

	public static string[] allWorlds
	{
		get
		{
			if (_allWorlds == null)
			{
				_allWorlds = (from w in worldData
					where !w.Value.doNotBuild && !w.Value.notRealWorld
					select w.Key).ToArray();
			}
			return _allWorlds;
		}
	}

	public static string[] dlcWorlds
	{
		get
		{
			if (_dlcWorlds == null)
			{
				_dlcWorlds = (from w in worldData
					where !w.Value.doNotBuild && w.Value.isTaro
					select w.Key).ToArray();
			}
			return _dlcWorlds;
		}
	}

	public static string[] xtraWorlds
	{
		get
		{
			if (_xtraWorlds == null)
			{
				_xtraWorlds = (from w in worldData
					where !w.Value.doNotBuild && w.Value.isXtra
					select w.Key).ToArray();
			}
			return _xtraWorlds;
		}
	}

	public static string[] museDashWorlds
	{
		get
		{
			if (_museDashWorlds == null)
			{
				_museDashWorlds = (from w in worldData
					where !w.Value.doNotBuild && w.Value.isMuseDash
					select w.Key).ToArray();
			}
			return _museDashWorlds;
		}
	}

	public static string[] crownWorlds
	{
		get
		{
			if (_crownWorlds == null)
			{
				_crownWorlds = (from w in worldData
					where !w.Value.doNotBuild && w.Value.isCrown
					select w.Key).ToArray();
			}
			return _crownWorlds;
		}
	}

	public static int numOfWorlds => allWorlds.Length;

	public static string sceneLevelSelect
	{
		get
		{
			if (!ADOBase.isMobileMenu)
			{
				return "scnLevelSelect";
			}
			return "scnMobileMenu";
		}
	}

	public static string FeaturedLevelsBuildPath => Persistence.DataPath + "/Bundles";

	public static string FeaturedLevelsLoadPath => Path.GetFullPath("./") + "Bundles";

	public static string BundlesLoadPath => Path.GetFullPath("./") + "Bundles";

	public static string neoCosmosBundleAssetsPath => BundlesLoadPath + "/neocosmos_assets_all.bundle";

	public static string neoCosmosBundleScenesPath => BundlesLoadPath + "/neocosmos_scenes_all.bundle";

	public static string bundleShadersPath => BundlesLoadPath + "/64b0239cbb1b5026aca97cb135062056_unitybuiltinshaders.bundle";
}
