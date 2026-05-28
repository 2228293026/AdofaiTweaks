using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Steamworks;
using UnityEngine;

public static class SteamWorkshop
{
	public enum WorkshopError
	{
		None = -1,
		TagsConversion = 0,
		LevelNameNullOrEmpty = 1,
		PreviewImageNotFound = 2,
		PreviewImageInvalidSize = 7,
		ItemFolderNotFound = 3,
		CreateItemFailed = 4,
		InvalidUpdateHandle = 5,
		UpdateItemFailed = 6,
		DeleteItemFailed = 8,
		DownloadItemFailed = 9,
		GetItemInstallInfoFailed = 10,
		SubscribedItemPathNotFound = 11,
		UnsubscribeFailed = 12,
		GetSubscribedItemsQuery = 13,
		GetSubscribeItemInfo = 14,
		GetItemPreviewFile = 16,
		MissingIncludedFile = 15
	}

	public struct ResultItem(PublishedFileId_t id, string title, string path, string previewImagePath, string[] tags = null)
	{
		public PublishedFileId_t id = id;

		public string title = title;

		public string path = path;

		public string previewImagePath = previewImagePath;

		public string[] tags = tags ?? Array.Empty<string>();
	}

	public static int totalPublishedItems;

	public static List<ResultItem> resultItems;

	public static List<WorkshopError> errors;

	public static float itemUploadProgress;

	public static bool gettingSubscribedItemsInProgress;

	public static bool mustAcceptWorkshopLegalAgreement;

	private static bool lastCallResultEnded;

	private static ulong lastBytesProcessed;

	private static UGCUpdateHandle_t updateHandle;

	public static PublishedFileId_t lastPublishedFileId;

	public static PublishedFileId_t lastDownloadFileId;

	private static CallResult<CreateItemResult_t> CreateItemResult;

	private static CallResult<DeleteItemResult_t> DeleteItemResult;

	private static CallResult<SubmitItemUpdateResult_t> SubmitItemUpdateResult;

	private static CallResult<SteamUGCQueryCompleted_t> SteamUGCQueryCompleted;

	private static CallResult<SteamUGCRequestUGCDetailsResult_t> SteamUGCRequestUGCDetailsResult;

	private static CallResult<RemoteStorageDownloadUGCResult_t> RemoteStorageDownloadUGCResult;

	private static Callback<DownloadItemResult_t> DownloadItemResult;

	private static Callback<RemoteStoragePublishedFileUnsubscribed_t> RSPublishedFileUnsubscribed;

	private static Callback<GameOverlayActivated_t> GameOverlayActivated;

	private static Callback<FloatingGamepadTextInputDismissed_t> FloatingGamepadTextInputDismissed;

	public static bool overlayActive;

	public static bool OperationSuccess => errors.Count == 0;

	public static string PreviewImageFolder => Application.persistentDataPath;

	public static event Action<PublishedFileId_t, bool> OnItemDownloaded;

	public static void Setup()
	{
		lastBytesProcessed = 0uL;
		DownloadItemResult = Callback<DownloadItemResult_t>.Create((DispatchDelegate<DownloadItemResult_t>)OnDownloadItemResult);
		RSPublishedFileUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create((DispatchDelegate<RemoteStoragePublishedFileUnsubscribed_t>)OnPublishedFileUnsubscribed);
		GameOverlayActivated = Callback<GameOverlayActivated_t>.Create((DispatchDelegate<GameOverlayActivated_t>)OnToggleGameOverlay);
		FloatingGamepadTextInputDismissed = Callback<FloatingGamepadTextInputDismissed_t>.Create((DispatchDelegate<FloatingGamepadTextInputDismissed_t>)OnFloatingGamepadTextInputDismissed);
	}

	public static void OpenWorkshop()
	{
		Debug.Log("Opening Workshop");
		string text = "https://steamcommunity.com//workshop/browse?appid=977950";
		foreach (DLCManager dLCManager in DLCManager.DLCManagers)
		{
			if (!string.IsNullOrEmpty(dLCManager.steamWorkshopTag) && !dLCManager.installed)
			{
				text = text + "&excludedtags[]=" + dLCManager.steamWorkshopTag;
			}
		}
		text = text.Replace(' ', '+');
		SteamFriends.ActivateGameOverlayToWebPage(text, (EActivateGameOverlayToWebPageMode)1);
	}

	public static void OnToggleGameOverlay(GameOverlayActivated_t param)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		overlayActive = param.m_bActive == 1;
	}

	public static void ShowItemOnWorkshop(PublishedFileId_t publishedFileId)
	{
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + publishedFileId.m_PublishedFileId, (EActivateGameOverlayToWebPageMode)0);
	}

	public static bool OverlayEnabled()
	{
		return SteamUtils.IsOverlayEnabled();
	}

	public static bool ShowTextInput()
	{
		return SteamUtils.ShowFloatingGamepadTextInput((EFloatingGamepadTextInputMode)0, 0, 0, 50, 10);
	}

	public static void OnFloatingGamepadTextInputDismissed(FloatingGamepadTextInputDismissed_t pCallback)
	{
		scnCLS instance = scnCLS.instance;
		if (instance != null)
		{
			instance.StartCoroutine(instance.optionsPanels.ToggleSearchMode(search: false));
		}
	}

	public static void Subscribe(PublishedFileId_t publishedFileId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.SubscribeItem(publishedFileId);
	}

	public static void Unsubscribe(PublishedFileId_t publishedFileId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.UnsubscribeItem(publishedFileId);
	}

	public static void OnPublishedFileUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t param)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		_ = param.m_nAppID != ((CGameID)(ref SteamIntegration.instance.gameID)).AppID();
	}

	public static IEnumerator GetPublishedItems(int pageNumber = 1)
	{
		resultItems = new List<ResultItem>();
		errors = new List<WorkshopError>();
		CSteamID steamID = SteamUser.GetSteamID();
		UGCQueryHandle_t queryHandle = SteamUGC.CreateQueryUserUGCRequest(((CSteamID)(ref steamID)).GetAccountID(), (EUserUGCList)0, (EUGCMatchingUGCType)0, (EUserUGCListSortOrder)3, SteamUtils.GetAppID(), SteamUtils.GetAppID(), (uint)pageNumber);
		SteamAPICall_t val = SteamUGC.SendQueryUGCRequest(queryHandle);
		SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create((APIDispatchDelegate<SteamUGCQueryCompleted_t>)null);
		int itemsNumber = 0;
		lastCallResultEnded = false;
		SteamUGCQueryCompleted.Set(val, (APIDispatchDelegate<SteamUGCQueryCompleted_t>)delegate(SteamUGCQueryCompleted_t param, bool bIOFailure)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (bIOFailure || (int)param.m_eResult != 1)
			{
				Debug.Log($"GetPublishedItems query failed: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
				errors.Add(WorkshopError.GetSubscribedItemsQuery);
			}
			else
			{
				itemsNumber = (int)param.m_unNumResultsReturned;
				totalPublishedItems = (int)param.m_unTotalMatchingResults;
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
		if (!OperationSuccess)
		{
			yield break;
		}
		SteamUGCDetails_t itemDetails = default(SteamUGCDetails_t);
		for (uint i = 0u; i < itemsNumber; i++)
		{
			if (SteamUGC.GetQueryUGCResult(queryHandle, i, ref itemDetails))
			{
				lastCallResultEnded = false;
				string previewImagePath = Path.Combine(PreviewImageFolder, $"{itemDetails.m_nPublishedFileId}.png");
				SteamAPICall_t val2 = SteamRemoteStorage.UGCDownloadToLocation(itemDetails.m_hPreviewFile, previewImagePath, 0u);
				RemoteStorageDownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create((APIDispatchDelegate<RemoteStorageDownloadUGCResult_t>)null);
				RemoteStorageDownloadUGCResult.Set(val2, (APIDispatchDelegate<RemoteStorageDownloadUGCResult_t>)delegate(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					//IL_004f: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Invalid comparison between Unknown and I4
					//IL_007c: Unknown result type (might be due to invalid IL or missing references)
					if (param.m_nAppID == SteamUtils.GetAppID() && param.m_ulSteamIDOwner == SteamUser.GetSteamID().m_SteamID)
					{
						if (bIOFailure || (int)param.m_eResult != 1)
						{
							Debug.Log($"UGCDownloadToLocation failed for item {i}: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
							errors.Add(WorkshopError.GetItemPreviewFile);
						}
						else
						{
							resultItems.Add(new ResultItem(itemDetails.m_nPublishedFileId, ((SteamUGCDetails_t)(ref itemDetails)).m_rgchTitle, string.Empty, previewImagePath));
						}
					}
					lastCallResultEnded = true;
				});
				yield return new WaitUntil(() => lastCallResultEnded);
			}
			else
			{
				errors.Add(WorkshopError.GetSubscribeItemInfo);
			}
		}
		errors = errors.Distinct().ToList();
	}

	public static IEnumerator GetSubscribedItems()
	{
		gettingSubscribedItemsInProgress = true;
		try
		{
			errors = new List<WorkshopError>();
			resultItems = new List<ResultItem>();
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] allSubscribedItemsId = (PublishedFileId_t[])(object)new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(allSubscribedItemsId, numSubscribedItems);
			UGCQueryHandle_t query = SteamUGC.CreateQueryUGCDetailsRequest(allSubscribedItemsId, (uint)allSubscribedItemsId.Length);
			SteamUGC.SetReturnOnlyIDs(query, true);
			SteamAPICall_t val = SteamUGC.SendQueryUGCRequest(query);
			SteamUGCRequestUGCDetailsResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create((APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)null);
			bool tagFetched = false;
			Dictionary<PublishedFileId_t, string[]> tags = new Dictionary<PublishedFileId_t, string[]>();
			SteamUGCRequestUGCDetailsResult.Set(val, (APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)delegate
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				string text2 = default(string);
				for (int i = 0; i < allSubscribedItemsId.Length; i++)
				{
					PublishedFileId_t key = allSubscribedItemsId[i];
					uint queryUGCNumTags = SteamUGC.GetQueryUGCNumTags(query, (uint)i);
					string[] array2 = (tags[key] = new string[queryUGCNumTags]);
					string[] array4 = array2;
					for (int j = 0; j < queryUGCNumTags; j++)
					{
						SteamUGC.GetQueryUGCTag(query, (uint)i, (uint)j, ref text2, 255u);
						array4[j] = text2;
					}
				}
				SteamUGC.ReleaseQueryUGCRequest(query);
				tagFetched = true;
			});
			yield return new WaitUntil(() => tagFetched);
			PublishedFileId_t[] array = allSubscribedItemsId;
			ulong num2 = default(ulong);
			string text = default(string);
			uint num3 = default(uint);
			foreach (PublishedFileId_t subscribedItemId in array)
			{
				if (!ItemIsUsable(subscribedItemId) && SteamUGC.DownloadItem(subscribedItemId, true))
				{
					lastCallResultEnded = false;
					yield return new WaitUntil(() => lastCallResultEnded);
					if (!OperationSuccess)
					{
						continue;
					}
				}
				if (SteamUGC.GetItemInstallInfo(subscribedItemId, ref num2, ref text, 1000u, ref num3))
				{
					if (!string.IsNullOrEmpty(text) && Directory.Exists(text))
					{
						resultItems.Add(new ResultItem(subscribedItemId, string.Empty, text, string.Empty, tags[subscribedItemId]));
					}
					else
					{
						errors.Add(WorkshopError.SubscribedItemPathNotFound);
					}
				}
				else
				{
					errors.Add(WorkshopError.GetItemInstallInfoFailed);
				}
			}
			errors = errors.Distinct().ToList();
		}
		finally
		{
			gettingSubscribedItemsInProgress = false;
		}
	}

	public static bool ItemIsUsable(PublishedFileId_t publishedFileId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EItemState val = (EItemState)SteamUGC.GetItemState(publishedFileId);
		if (!((Enum)val).HasFlag((Enum)(object)(EItemState)16) && !((Enum)val).HasFlag((Enum)(object)(EItemState)32) && !((Enum)val).HasFlag((Enum)(object)(EItemState)8))
		{
			return ((Enum)val).HasFlag((Enum)(object)(EItemState)4);
		}
		return false;
	}

	public static float GetItemDownloadProgress(PublishedFileId_t publishedFileId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ulong num = default(ulong);
		ulong num2 = default(ulong);
		if (!SteamUGC.GetItemDownloadInfo(publishedFileId, ref num, ref num2))
		{
			return 0f;
		}
		float num3 = (float)num / (float)num2;
		if (float.IsNaN(num3))
		{
			return 0f;
		}
		return num3;
	}

	private static void OnDownloadItemResult(DownloadItemResult_t param)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!(param.m_unAppID != ((CGameID)(ref SteamIntegration.instance.gameID)).AppID()))
		{
			bool flag = (int)param.m_eResult == 1;
			if (!flag)
			{
				errors.Add(WorkshopError.DownloadItemFailed);
			}
			lastCallResultEnded = true;
			SteamWorkshop.OnItemDownloaded?.Invoke(param.m_nPublishedFileId, flag);
		}
	}

	public static void CheckDownloadInfo()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ulong num = default(ulong);
		ulong num2 = default(ulong);
		SteamUGC.GetItemDownloadInfo(lastDownloadFileId, ref num, ref num2);
		if ((float)num > 0f && !((float)num2 > 0f))
		{
		}
	}

	public static IEnumerator UploadToWorkshop(string title, string description, string previewImagePath, string contentPath, string[] tags, PublishedFileId_t updateId = default(PublishedFileId_t), DLCManager[] requiredDLC = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		errors = new List<WorkshopError>();
		if (string.IsNullOrEmpty(title))
		{
			errors.Add(WorkshopError.LevelNameNullOrEmpty);
		}
		else
		{
			int num = 127;
			if (Encoding.UTF8.GetByteCount(title) > num)
			{
				byte[] bytes = Encoding.Default.GetBytes(title);
				title = Encoding.UTF8.GetString(bytes, 0, num);
			}
		}
		if (string.IsNullOrEmpty(previewImagePath) || !RDFile.Exists(previewImagePath))
		{
			errors.Add(WorkshopError.PreviewImageNotFound);
		}
		else
		{
			long length = new FileInfo(previewImagePath).Length;
			if (length <= 16 || length >= 1000000)
			{
				errors.Add(WorkshopError.PreviewImageInvalidSize);
			}
		}
		if (string.IsNullOrEmpty(contentPath) || !RDDirectory.Exists(contentPath))
		{
			errors.Add(WorkshopError.ItemFolderNotFound);
		}
		if (!OperationSuccess)
		{
			yield break;
		}
		for (int i = 0; i < tags.Length; i++)
		{
			tags[i] = tags[i].Truncate(255);
		}
		if (!string.IsNullOrEmpty(description) && description.Length > 1000)
		{
			description = description.Truncate(1000);
		}
		SteamAPICall_t val;
		if (updateId == default(PublishedFileId_t))
		{
			val = SteamUGC.CreateItem(SteamUtils.GetAppID(), (EWorkshopFileType)0);
			CreateItemResult = CallResult<CreateItemResult_t>.Create((APIDispatchDelegate<CreateItemResult_t>)null);
			lastCallResultEnded = false;
			CreateItemResult.Set(val, (APIDispatchDelegate<CreateItemResult_t>)delegate(CreateItemResult_t param, bool bIOFailure)
			{
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_0004: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Invalid comparison between Unknown and I4
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				if (!bIOFailure && (int)param.m_eResult == 1)
				{
					lastPublishedFileId = param.m_nPublishedFileId;
				}
				else
				{
					errors.Add(WorkshopError.CreateItemFailed);
				}
				lastCallResultEnded = true;
			});
			yield return new WaitUntil(() => lastCallResultEnded);
			if (!OperationSuccess)
			{
				yield break;
			}
		}
		else
		{
			lastPublishedFileId = updateId;
		}
		updateHandle = SteamUGC.StartItemUpdate(((CGameID)(ref SteamIntegration.instance.gameID)).AppID(), lastPublishedFileId);
		ERemoteStoragePublishedFileVisibility val2 = (ERemoteStoragePublishedFileVisibility)(((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.U)) ? 3 : 0);
		if (!SteamUGC.SetItemTitle(updateHandle, title) || (!string.IsNullOrEmpty(description) && !SteamUGC.SetItemDescription(updateHandle, description)) || !SteamUGC.SetItemTags(updateHandle, (IList<string>)tags) || !SteamUGC.SetItemPreview(updateHandle, previewImagePath) || !SteamUGC.SetItemContent(updateHandle, contentPath) || !SteamUGC.SetItemVisibility(updateHandle, val2))
		{
			errors.Add(WorkshopError.InvalidUpdateHandle);
			yield return DeleteItem(lastPublishedFileId);
			yield break;
		}
		foreach (DLCManager dLCManager in requiredDLC)
		{
			SteamUGC.AddAppDependency(lastPublishedFileId, new AppId_t(dLCManager.steamAppId));
		}
		val = SteamUGC.SubmitItemUpdate(updateHandle, "Item Created");
		SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
		lastCallResultEnded = false;
		SubmitItemUpdateResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)delegate(SubmitItemUpdateResult_t param, bool bIOFailure)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			mustAcceptWorkshopLegalAgreement = param.m_bUserNeedsToAcceptWorkshopLegalAgreement;
			if (bIOFailure || (int)param.m_eResult != 1)
			{
				errors.Add(WorkshopError.UpdateItemFailed);
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
		if (!OperationSuccess)
		{
			yield return DeleteItem(lastPublishedFileId);
		}
	}

	private static IEnumerator DeleteItem(PublishedFileId_t publishedFileId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SteamAPICall_t val = SteamUGC.DeleteItem(publishedFileId);
		DeleteItemResult = CallResult<DeleteItemResult_t>.Create((APIDispatchDelegate<DeleteItemResult_t>)null);
		lastCallResultEnded = false;
		DeleteItemResult.Set(val, (APIDispatchDelegate<DeleteItemResult_t>)delegate(DeleteItemResult_t param, bool bIOFailure)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			if (bIOFailure || (int)param.m_eResult != 1)
			{
				errors.Add(WorkshopError.DeleteItemFailed);
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
	}

	public static void CheckUploadInfo()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		ulong num = default(ulong);
		ulong num2 = default(ulong);
		SteamUGC.GetItemUpdateProgress(updateHandle, ref num, ref num2);
		if ((float)num > 0f && (float)num2 > 0f && num2 > num && lastBytesProcessed != num)
		{
			lastBytesProcessed = num;
			itemUploadProgress = (float)num / ((float)num2 * 1f);
		}
	}
}
