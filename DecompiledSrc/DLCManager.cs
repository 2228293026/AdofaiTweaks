using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class DLCManager
{
	public static bool initialized;

	public static HashSet<DLCManager> DLCManagers;

	public uint steamAppId;

	public uint winDepotId;

	public uint win64DepotId;

	public uint macDepotId;

	public uint linuxDepotId;

	public string steamWorkshopTag;

	public string groupName;

	public bool own;

	public bool installed;

	public string buildCommit;

	public bool upToDate;

	public string bundleName => groupName.ToLower().Replace(" ", "");

	public static void Setup()
	{
		if (!initialized)
		{
			initialized = true;
			DLCManagers.Add(new NeoCosmosManager());
			DLCManagers.Add(new VegaDLCManager());
			DLCManagers.Add(new FeaturedDLCManager());
		}
	}

	static DLCManager()
	{
		DLCManagers = new HashSet<DLCManager>();
		Setup();
	}

	public abstract string GetMenuScene();

	public abstract bool IsDLCScene(string name);

	public abstract bool IsDLCLevel(string name);

	public bool IsDLCSceneOrLevel(string name)
	{
		if (!IsDLCScene(name))
		{
			return IsDLCLevel(name);
		}
		return true;
	}

	public virtual void CheckInstalled()
	{
		string path = Path.Combine(GCNS.BundlesLoadPath, bundleName + "_scenes_all.bundle");
		string path2 = Path.Combine(GCNS.BundlesLoadPath, bundleName + "_assets_all.bundle");
		installed = File.Exists(path) && File.Exists(path2);
	}

	public void CheckUpToDate()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!own || !installed)
		{
			return;
		}
		if (SteamIntegration.initialized && !GCNS.stableBranches.Contains(GCS.steamBranchName))
		{
			upToDate = true;
			return;
		}
		AsyncOperationHandle<TextAsset> val = Addressables.LoadAssetAsync<TextAsset>((object)(groupName + "/buildCommit"));
		TextAsset textAsset = val.WaitForCompletion();
		if (!(textAsset == null))
		{
			buildCommit = textAsset.text;
			Addressables.Release<TextAsset>(val);
			upToDate = Application.isEditor || GCNS.buildCommit == buildCommit;
			ADOStartup.StartupLog($"GCNS.buildCommit: {GCNS.buildCommit}, {groupName} buildCommit: {buildCommit}, upToDate: {upToDate}");
		}
	}
}
