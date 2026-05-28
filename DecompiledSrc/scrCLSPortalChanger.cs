using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using Steamworks;
using UnityEngine;

public class scrCLSPortalChanger : ADOBase
{
	public enum PortalType
	{
		Classic,
		Tech,
		Workshop
	}

	public PortalType portalType;

	public int updateInterval = 5;

	public float animationDuration = 1f;

	public float startDelay;

	public PortalQuad portalQuad1;

	public PortalQuad portalQuad2;

	private bool usingFirstQuad = true;

	private PublishedFileId_t[] workshopLevelIds;

	private float lastUpdate = -1000f;

	private void Start()
	{
		if (portalType == PortalType.Workshop && SteamIntegration.initialized)
		{
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] array = (PublishedFileId_t[])(object)new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
			workshopLevelIds = array.Where((PublishedFileId_t x) => !GCNS.FeaturedLevelsIDs.Contains((uint)x.m_PublishedFileId) && !GCNS.TechFeaturedLevelsIDs.Contains((uint)x.m_PublishedFileId)).ToArray();
		}
	}

	private Texture2D GetTexture()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		switch (portalType)
		{
		case PortalType.Classic:
		case PortalType.Tech:
		{
			uint[] array = ((portalType == PortalType.Classic) ? GCNS.FeaturedLevelsIDs : GCNS.TechFeaturedLevelsIDs);
			uint num3 = array[Random.Range(0, array.Length)];
			return Resources.Load<Texture2D>(Path.Combine("FeaturedLevels", num3.ToString(), "portal"));
		}
		case PortalType.Workshop:
		{
			if (workshopLevelIds == null)
			{
				return Resources.Load<Texture2D>("CLS/workshop_portal_icon");
			}
			if (workshopLevelIds.Length == 0)
			{
				return ADOBase.cls.workshopPortalTexture;
			}
			ulong num = default(ulong);
			string path = default(string);
			uint num2 = default(uint);
			bool itemInstallInfo = SteamUGC.GetItemInstallInfo(workshopLevelIds[Random.Range(0, workshopLevelIds.Length)], ref num, ref path, 1024u, ref num2);
			string path2 = (itemInstallInfo ? Path.Combine(path, "main.adofai") : "");
			if (!itemInstallInfo || !RDFile.Exists(path2))
			{
				return ADOBase.cls.workshopPortalTexture;
			}
			Dictionary<string, object> rootDict = Json.DeserializePartially(RDFile.ReadAllText(path2), "actions") as Dictionary<string, object>;
			LevelDataCLS levelDataCLS = new LevelDataCLS();
			levelDataCLS.Decode(rootDict);
			LoadResult status;
			Texture2D texture2D = TextureManager.LoadTexture(Path.Combine(path, levelDataCLS.previewImage), out status);
			if (!texture2D)
			{
				return ADOBase.cls.workshopPortalTexture;
			}
			return texture2D;
		}
		default:
			return null;
		}
	}

	private void Update()
	{
		if (Time.time - lastUpdate < (float)updateInterval)
		{
			return;
		}
		float num = lastUpdate;
		bool flag = usingFirstQuad;
		lastUpdate = Time.time;
		Texture2D texture = GetTexture();
		PortalQuad oldQuad = (flag ? portalQuad2 : portalQuad1);
		PortalQuad newQuad = (flag ? portalQuad1 : portalQuad2);
		Texture oldTexture = oldQuad.GetTexture();
		if (oldTexture == texture)
		{
			return;
		}
		usingFirstQuad = !usingFirstQuad;
		DOVirtual.DelayedCall((num < 0f) ? 0f : startDelay, delegate
		{
			newQuad.SetTexture(texture);
			oldQuad.Fade(0f, animationDuration);
			newQuad.Fade(1f, animationDuration).OnComplete(delegate
			{
				oldQuad.RemoveTexture();
				if (portalType == PortalType.Workshop)
				{
					Object.Destroy(oldTexture);
				}
				else
				{
					Resources.UnloadAsset(oldTexture);
				}
			});
		});
	}
}
