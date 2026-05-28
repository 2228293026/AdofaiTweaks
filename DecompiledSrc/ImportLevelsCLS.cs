using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ADOFAI;
using DG.Tweening;
using GDMiniJSON;
using UnityEngine;
using UnityEngine.UI;
using UnityFileDialog;

public class ImportLevelsCLS : ADOBase
{
	public enum ContentType
	{
		BrowseFiles,
		URLView,
		InstallContent,
		NoLevels
	}

	private const string ImportAdozip = "editor.dialog.importAdofaizip";

	private const string ImportAdozipDescription = "editor.dialog.adofaizipDescription";

	private const string StopInstallToken = "packageInstaller.stop";

	private const string ClearToken = "packageInstaller.clear";

	private const string InstallToken = "packageInstaller.install";

	private const string InstallLevelsToken = "cls.installLevels";

	private const string InstallingLevelsToken = "cls.installingLevels";

	private const string LevelsInstalledToken = "cls.installedLevels";

	private const string InstallCompleteToken = "cls.installComplete";

	private const string DoneToken = "editor.dialog.done";

	private const string ErrorsToken = "packageInstaller.errors";

	private const string CheckMissingParamsToken = "packageInstaller.checkMissingParams";

	private const string UnzipErrorToken = "packageInstaller.fakeZipError";

	private const string MainEmptyError = "packageInstaller.mainFileEmptyError";

	private const string MainCorruptError = "packageInstaller.mainFileCorruptError";

	private const string FileNotFoundToken = "packageInstaller.adofaiLevelNotFound";

	private const string MovingToCLSPathError = "packageInstaller.movingToCLSPathError";

	private const string HashErrorToken = "packageInstaller.sameHashError";

	private const string PressRToRefreshToken = "cls.shortcut.refresh";

	private const string NoLevelsFoundRefreshToken = "cls.message.noLevelsFoundRefresh";

	private const string NoLevelsFoundBodyText = "cls.message.noLevelsFoundBody";

	private const string NoLevelsFoundBodySteamDeckToken = "cls.message.noLevelsFoundBodySteamDeck";

	private const string NoLevelsFoundBodyNoSteamworksToken = "cls.message.noLevelsFoundBodyNoSteamworks";

	private const string LoadingToken = "status.loading";

	public GameObject importLevelPrefab;

	public RectTransform importPanel;

	public Image occluderImage;

	public Button closeButton;

	public GameObject dragAndDrop;

	[Header("Browse Panel")]
	public GameObject browsePanel;

	public Button browseLocalButton;

	public Button addFromURLButton;

	public GameObject draggableInstructionsText;

	public GameObject notDraggableInstructionsText;

	[Header("Insert URL Panel")]
	public GameObject urlImportPanel;

	public InputField urlInput;

	public Button addLevelsButton;

	public Button urlGoBackButton;

	[Header("Install Levels Panel")]
	public Text installPanelTitle;

	public Text installDetailsText;

	public GameObject installPanel;

	public Button installButton;

	public Text installButtonText;

	public RectTransform miniImportSection;

	public Button miniBrowseButton;

	public Button miniURLButton;

	public Button clearOrStopLevelsButton;

	[Header("Info Scroll View Content")]
	public RectTransform infoScrollViewContent;

	public Scrollbar infoContentVerticalScrollBar;

	public LevelImporterInfoSection errorsIS;

	public LevelImporterInfoSection toInstallIS;

	public LevelImporterInfoSection installedIS;

	[Header("No Levels Panel")]
	public RectTransform noLevelsPanel;

	public Button workshopButtonNoLevels;

	public Button importButtonNoLevels;

	public Text noLevelsFoundBodyText;

	public Text pressRToRefreshText;

	private bool stoppedInstallCoroutine;

	private bool wasRefreshing;

	public void Initialize()
	{
		dragAndDrop.GetComponent<FileDragAndDrop>().OnFilesDropped = delegate(string[] filesPath)
		{
			foreach (string zipPath in filesPath)
			{
				StartCoroutine(AddContent(zipPath));
			}
			ShowContent(ContentType.InstallContent);
		};
		workshopButtonNoLevels.onClick.AddListener(delegate
		{
			SteamWorkshop.OpenWorkshop();
		});
		importButtonNoLevels.onClick.AddListener(delegate
		{
			ShowContent(ContentType.BrowseFiles);
		});
		browseLocalButton.onClick.AddListener(delegate
		{
			BrowseFilesLocalButton();
		});
		miniBrowseButton.onClick.AddListener(delegate
		{
			BrowseFilesLocalButton();
		});
		addFromURLButton.onClick.AddListener(delegate
		{
			BrowseURLButton();
		});
		miniURLButton.onClick.AddListener(delegate
		{
			BrowseURLButton();
		});
		addLevelsButton.onClick.AddListener(delegate
		{
			AddLevelsFromInputField();
			ShowContent(ContentType.InstallContent);
		});
		closeButton.onClick.AddListener(delegate
		{
			CloseButton();
		});
		installButton.onClick.AddListener(delegate
		{
			InstallContent();
		});
		urlGoBackButton.onClick.AddListener(delegate
		{
			ShowContent(ContentType.BrowseFiles);
		});
		pressRToRefreshText.text = RDString.Get("cls.shortcut.refresh");
		ConfigureSourceButtons();
		Color color = occluderImage.color;
		color.a = 0f;
		occluderImage.color = color;
		occluderImage.raycastTarget = false;
		base.gameObject.SetActive(value: false);
		CheckForDragAndDropSupport();
		errorsIS.levels = new List<ImportLevel>();
		installedIS.levels = new List<ImportLevel>();
		toInstallIS.levels = new List<ImportLevel>();
	}

	private void Start()
	{
		clearOrStopLevelsButton.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (noLevelsPanel.gameObject.activeSelf)
		{
			if (wasRefreshing != ADOBase.cls.refreshing)
			{
				wasRefreshing = ADOBase.cls.refreshing;
				pressRToRefreshText.text = RDString.Get(ADOBase.cls.refreshing ? "status.loading" : "cls.message.noLevelsFoundRefresh");
				workshopButtonNoLevels.interactable = !ADOBase.cls.refreshing;
				importButtonNoLevels.interactable = !ADOBase.cls.refreshing;
			}
			if (!ADOBase.cls.refreshing && Input.GetKeyDown(KeyCode.R))
			{
				ADOBase.cls.Refresh();
			}
		}
	}

	public void ShowContent(ContentType contentType)
	{
		browsePanel.gameObject.SetActive(value: false);
		urlImportPanel.SetActive(value: false);
		installPanel.SetActive(value: false);
		noLevelsPanel.gameObject.SetActive(value: false);
		switch (contentType)
		{
		case ContentType.BrowseFiles:
			browsePanel.gameObject.SetActive(value: true);
			break;
		case ContentType.URLView:
			urlImportPanel.SetActive(value: true);
			break;
		case ContentType.InstallContent:
			installPanel.SetActive(value: true);
			installDetailsText.gameObject.SetActive(value: false);
			installPanelTitle.text = RDString.Get("cls.installLevels");
			installButtonText.text = RDString.Get("packageInstaller.install");
			installButton.onClick.RemoveAllListeners();
			installButton.onClick.AddListener(delegate
			{
				InstallContent();
			});
			miniImportSection.gameObject.SetActive(value: true);
			break;
		case ContentType.NoLevels:
			noLevelsPanel.gameObject.SetActive(value: true);
			break;
		}
	}

	private void BrowseFilesLocalButton()
	{
		BrowseZip();
	}

	private void BrowseURLButton()
	{
		ShowContent(ContentType.URLView);
	}

	public void OnOpenImportPanel()
	{
		if (ADOBase.isSwitch || ADOBase.isMobile)
		{
			return;
		}
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(value: true);
		importPanel.gameObject.SetActive(value: true);
		importPanel.DOKill();
		occluderImage.DOKill();
		scnCLS.instance.optionsPanels.HideAnyPanel();
		ConfigureSourceButtons();
		bool flag = scnCLS.instance.levelCount == 0;
		ShowContent(flag ? ContentType.NoLevels : ContentType.BrowseFiles);
		occluderImage.raycastTarget = true;
		ADOBase.controller.responsive = false;
		if (!activeSelf)
		{
			occluderImage.DOFade(0.8f, 0.3f);
			float yPos = 0f;
			float num = 40f;
			importPanel.anchoredPosition = new Vector2(importPanel.anchoredPosition.x, -1150f);
			importPanel.DOAnchorPosY(yPos + num, 0.22f).SetEase(Ease.OutQuad).OnComplete(delegate
			{
				importPanel.DOAnchorPosY(yPos, 0.08f).SetEase(Ease.InQuad);
			})
				.SetUpdate(isIndependentUpdate: true);
		}
	}

	private void CloseButton()
	{
		if (noLevelsPanel.gameObject.activeSelf)
		{
			OnCloseImportPanel();
		}
		else if (browsePanel.gameObject.activeSelf)
		{
			if ((installedIS.levels == null || installedIS.levels.Count <= 0) && scnCLS.instance.levelCount == 0)
			{
				ShowContent(ContentType.NoLevels);
			}
			else
			{
				OnCloseImportPanel();
			}
		}
		else
		{
			ShowContent(ContentType.BrowseFiles);
		}
	}

	private void ConfigureSourceButtons()
	{
		bool flag = false;
		bool flag2 = false;
		flag = ADOBase.isSteamworks && SteamManager.Initialized;
		flag2 = RDC.runningOnSteamDeck;
		bool active = !flag2;
		bool active2 = flag;
		importButtonNoLevels.gameObject.SetActive(active);
		workshopButtonNoLevels.gameObject.SetActive(active2);
		bool flag3 = !flag2 && flag;
		noLevelsFoundBodyText.text = RDString.Get(flag3 ? "cls.message.noLevelsFoundBody" : (flag2 ? "cls.message.noLevelsFoundBodySteamDeck" : "cls.message.noLevelsFoundBodyNoSteamworks"));
		pressRToRefreshText.gameObject.SetActive(!flag2);
	}

	public void BrowseZip()
	{
		string text = RDString.Get("editor.dialog.adofaizipDescription");
		string[] array = FileBrowser.PickFiles(Persistence.GetLastUsedFolder(), text, GCS.levelZipExtensions, RDString.Get("editor.dialog.importAdofaizip"));
		if (array != null && array.Length != 0)
		{
			Persistence.UpdateLastUsedFolder(array[0]);
			int num = 0;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				_ = array2[i];
				StartCoroutine(AddContent(array[num]));
				num++;
			}
			ShowContent(ContentType.InstallContent);
		}
	}

	public void OnCloseImportPanel()
	{
		bool flag = installedIS.levels != null && installedIS.levels.Count > 0;
		bool flag2 = scnCLS.instance.levelCount == 0;
		if (!flag && flag2)
		{
			ADOBase.cls.QuitPortal();
		}
		else if (flag)
		{
			ADOBase.cls.Refresh();
		}
		HideImportPanel();
	}

	public void HideImportPanel()
	{
		ClearInfoSections();
		ADOBase.controller.responsive = true;
		occluderImage.raycastTarget = false;
		occluderImage.DOFade(0f, 0.3f);
		importPanel.DOAnchorPosY(-1150f, 0.3f).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	private void AddLevelsFromInputField()
	{
		foreach (var (zipPath, filename) in ValidateURL(urlInput.text))
		{
			StartCoroutine(AddContent(zipPath, filename, isUrl: true));
		}
		urlInput.text = string.Empty;
		ShowContent(ContentType.InstallContent);
	}

	private List<(string url, string filename)> ValidateURL(string _input)
	{
		string[] array = _input.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		List<(string, string)> list = new List<(string, string)>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!Uri.TryCreate(text.Trim(), UriKind.Absolute, out var result))
			{
				continue;
			}
			string text2 = text;
			if (result.Host.EndsWith("youtube.com") && result.AbsolutePath.StartsWith("/redirect"))
			{
				text2 = RDEditorUtils.ParseQueryString(result.Query)["q"];
				Uri.TryCreate(text2.Trim(), UriKind.Absolute, out result);
			}
			if (result.Host.Contains("drive.google.com"))
			{
				Match match = Regex.Match(result.AbsoluteUri, "(?:/d/|id=)([a-zA-Z0-9_-]+)");
				if (match.Success)
				{
					string value = match.Groups[1].Value;
					string item = "https://drive.google.com/uc?export=download&id=" + value;
					string item2 = value + ".zip";
					list.Add((item, item2));
					continue;
				}
			}
			string text3 = Path.GetFileName(result.LocalPath);
			string value2 = Path.GetExtension(text3).Replace(".", string.Empty).ToLowerInvariant();
			if (string.IsNullOrEmpty(value2))
			{
				text3 = text2;
			}
			else if (!GCS.levelZipExtensions.Contains(value2))
			{
				continue;
			}
			list.Add((text2, text3));
		}
		return list;
	}

	private void InstallContent()
	{
		StartCoroutine(InstallRoutine());
	}

	private IEnumerator InstallRoutine()
	{
		List<ImportLevel> _levelsToInstall = new List<ImportLevel>(toInstallIS.levels);
		miniImportSection.gameObject.SetActive(value: false);
		ShowStopInstallButton();
		installButton.interactable = false;
		installPanelTitle.text = RDString.Get("cls.installingLevels") ?? "";
		yield return null;
		foreach (ImportLevel level in _levelsToInstall)
		{
			level.BeginInstallProgress();
			level.transform.SetSiblingIndex(0);
			yield return null;
			infoContentVerticalScrollBar.value = 1f;
			if (level.isUrl)
			{
				yield return HandleUrlDownload(level);
				if (stoppedInstallCoroutine)
				{
					HandleCoroutineStop(level);
					break;
				}
			}
			if (IsLevelFolderMissing(level))
			{
				AddLevelToErrorSection(level, RDString.Get("packageInstaller.adofaiLevelNotFound"));
				FinalizeLevelProcessing(level);
				continue;
			}
			PackageInstallerResult<bool> packageInstallerResult = AdoPackageInstaller.CheckFileIsZip(level.folderPath);
			if (!packageInstallerResult.IsSuccess)
			{
				AddLevelToErrorSection(level, packageInstallerResult.Error);
				FinalizeLevelProcessing(level);
				continue;
			}
			bool isAdoZip = Path.GetExtension(level.folderPath).Replace(".", string.Empty).ToLowerInvariant() == "adofai";
			string levelUnzippedDirectory = scnCLS.tempLevelsFolder;
			bool unzipFailed = false;
			yield return StartCoroutine(UnzipLevelFile(level, levelUnzippedDirectory, isAdoZip, delegate(PackageInstallerResult<string> unzipResult)
			{
				if (!unzipResult.IsSuccess)
				{
					AddLevelToErrorSection(level, unzipResult.Error);
					FinalizeLevelProcessing(level);
					unzipFailed = true;
				}
				else
				{
					levelUnzippedDirectory = unzipResult.Value;
				}
			}));
			if (unzipFailed)
			{
				continue;
			}
			PackageInstallerResult<string> packageInstallerResult2 = AdoPackageInstaller.FindLevelFile(levelUnzippedDirectory);
			if (!packageInstallerResult2.IsSuccess)
			{
				AddLevelToErrorSection(level, packageInstallerResult2.Error);
				FinalizeLevelProcessing(level);
				continue;
			}
			string value = packageInstallerResult2.Value;
			bool zipProcessErrors = false;
			yield return ProcessAdoZip(level, value, delegate(PackageInstallerResult<string> processResult)
			{
				if (!processResult.IsSuccess)
				{
					AddLevelToErrorSection(level, processResult.Error);
					FinalizeLevelProcessing(level);
					zipProcessErrors = true;
				}
			});
			if (!zipProcessErrors)
			{
				yield return null;
				FinalizeLevelProcessing(level);
				if (stoppedInstallCoroutine)
				{
					HandleCoroutineStop(level);
					break;
				}
			}
		}
		if (stoppedInstallCoroutine)
		{
			yield break;
		}
		clearOrStopLevelsButton.gameObject.SetActive(value: false);
		installPanelTitle.text = RDString.Get("cls.installComplete");
		int count = installedIS.levels.Count;
		int count2 = errorsIS.levels.Count;
		if (count > 0)
		{
			installDetailsText.gameObject.SetActive(value: true);
			_ = string.Empty;
			string key = ((count != 1) ? "packageInstaller.installDetailsMultiple" : "packageInstaller.installDetailsSingle");
			installDetailsText.text = RDString.Get(key, new Dictionary<string, object> { { "levels", count } });
			if (count2 > 0)
			{
				string key2 = "packageInstaller.installDetailsErrors";
				installDetailsText.text += RDString.Get(key2, new Dictionary<string, object> { { "errors", count2 } });
			}
		}
		else
		{
			string key3 = "packageInstaller.noLevelsInstalled";
			installDetailsText.gameObject.SetActive(value: true);
			installDetailsText.text = RDString.Get(key3, new Dictionary<string, object> { { "errors", count2 } });
		}
		UpdateInfoContentHeight();
		installButtonText.text = RDString.Get("editor.dialog.done");
		installButton.interactable = true;
		installButton.onClick.RemoveAllListeners();
		installButton.onClick.AddListener(delegate
		{
			if (installedIS.levels != null && installedIS.levels.Count > 0)
			{
				OnCloseImportPanel();
			}
			else
			{
				StartOver();
				ShowContent(ContentType.BrowseFiles);
			}
		});
	}

	private IEnumerator ProcessAdoZip(ImportLevel importLevel, string adoLevelPath, Action<PackageInstallerResult<string>> callback)
	{
		LevelData _levelData = new LevelData();
		new Dictionary<string, object>();
		string text = string.Empty;
		if (RDFile.Exists(adoLevelPath))
		{
			try
			{
				if (Json.Deserialize(RDFile.ReadAllText(adoLevelPath)) is Dictionary<string, object> { Count: >1 } dictionary)
				{
					_levelData.Decode(dictionary, out var _);
				}
				else
				{
					Debug.LogError("2: rootDict null or empty:  " + adoLevelPath);
					text = RDString.Get("packageInstaller.mainFileEmptyError");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("3: Error desearializing json: " + adoLevelPath + "\nException: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
				text = RDString.Get("packageInstaller.mainFileCorruptError") + " " + ex.Message;
			}
		}
		else
		{
			Debug.LogError("4: The .adofai file was not found at path:  " + adoLevelPath);
			text = RDString.Get("packageInstaller.adofaiLevelNotFound");
		}
		if (!string.IsNullOrEmpty(text))
		{
			callback?.Invoke(PackageInstallerResult<string>.Failure(text));
			yield break;
		}
		if (ADOBase.cls.loadedLevels.Values.OfType<LevelDataCLS>().FirstOrDefault((LevelDataCLS levelDataCLS) => levelDataCLS.Hash == _levelData.Hash) != null)
		{
			callback?.Invoke(PackageInstallerResult<string>.Failure(RDString.Get("packageInstaller.sameHashError")));
			yield break;
		}
		List<string> missingParams = _levelData.GetMissingParams();
		if (missingParams.Count > 0)
		{
			string text2 = RDString.Get("packageInstaller.checkMissingParams") + ": ";
			for (int num = 0; num < missingParams.Count; num++)
			{
				text2 += RDString.Get(missingParams[num]);
				if (num < missingParams.Count - 1)
				{
					text2 += ", ";
				}
			}
			callback?.Invoke(PackageInstallerResult<string>.Failure(text2));
			yield break;
		}
		try
		{
			PackageInstallerResult<string> packageInstallerResult = AdoPackageInstaller.MoveAdofaiLevelFolder(importLevel.folderPath, scnCLS.localWorldsPath);
			importLevel.folderPath = packageInstallerResult.Value;
			string infoText = _levelData.fullCaption + "\n(" + importLevel.infoText.text + ")";
			int siblingIndex = installedIS.transform.GetSiblingIndex() + 1;
			installedIS.Add(importLevel, infoText, siblingIndex);
			importLevel.OnInstallSuccess();
			callback?.Invoke(PackageInstallerResult<string>.Success(importLevel.folderPath));
		}
		catch (Exception ex2)
		{
			Debug.LogError("5: from " + importLevel.folderPath + " to " + scnCLS.localWorldsPath + ". " + ex2.Message);
			callback?.Invoke(PackageInstallerResult<string>.Failure(RDString.Get("packageInstaller.movingToCLSPathError") + " " + ex2.Message));
		}
	}

	private void FinalizeLevelProcessing(ImportLevel level)
	{
		toInstallIS.Remove(level);
		UpdateInfoContentHeight();
	}

	private void AddLevelToErrorSection(ImportLevel level, string subtitleToken)
	{
		Text infoText = level.infoText;
		level.OnInstallError();
		if (string.IsNullOrEmpty(subtitleToken))
		{
			subtitleToken = "";
		}
		level.UpdateHeight();
		errorsIS.Add(level, infoText.text, errorsIS.transform.GetSiblingIndex() + 1, subtitleToken);
		try
		{
			if (!string.IsNullOrEmpty(level.folderPath))
			{
				if (Directory.Exists(level.folderPath))
				{
					Directory.Delete(level.folderPath, recursive: true);
					Debug.Log("Deleted " + level.folderPath + " level path successfully");
				}
				else if (File.Exists(level.folderPath))
				{
					File.SetAttributes(level.folderPath, FileAttributes.Normal);
					File.Delete(level.folderPath);
					Debug.Log("Deleted " + level.folderPath + " level file successfully");
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("9: IOException: " + ex.Message);
		}
		UpdateInfoContentHeight();
	}

	private IEnumerator AddContent(string zipPath, string filename = null, bool isUrl = false)
	{
		if (toInstallIS.levels.FindAll((ImportLevel level) => level.folderPath == zipPath).Count == 0)
		{
			ImportLevel component = UnityEngine.Object.Instantiate(importLevelPrefab, infoScrollViewContent).GetComponent<ImportLevel>();
			component.progressImage.gameObject.SetActive(value: false);
			component.progressText.gameObject.SetActive(value: false);
			string path = (component.folderPath = Uri.UnescapeDataString(zipPath.Replace("file:", "")));
			if (string.IsNullOrEmpty(filename))
			{
				filename = Path.GetFileName(path);
			}
			component.isUrl = isUrl;
			toInstallIS.Add(component, filename, toInstallIS.transform.GetSiblingIndex() + 1);
			component.transform.SetAsLastSibling();
			ShowClearButton();
			installButton.interactable = true;
		}
		UpdateInfoContentHeight();
		yield return null;
		infoContentVerticalScrollBar.value = 1f;
	}

	private void UpdateInfoContentHeight()
	{
		float spacing = infoScrollViewContent.GetComponent<VerticalLayoutGroup>().spacing;
		float num = spacing * 2f;
		foreach (ImportLevel level in toInstallIS.levels)
		{
			if (level != null && level.gameObject.activeSelf)
			{
				level.UpdateHeight();
				num += level.rectTransform.sizeDelta.y;
				num += spacing;
			}
		}
		foreach (ImportLevel level2 in errorsIS.levels)
		{
			if (level2 != null && level2.gameObject.activeSelf)
			{
				level2.UpdateHeight();
				num += level2.rectTransform.sizeDelta.y;
				num += spacing;
			}
		}
		foreach (ImportLevel level3 in installedIS.levels)
		{
			if (level3 != null && level3.gameObject.activeSelf)
			{
				level3.UpdateHeight();
				num += level3.rectTransform.sizeDelta.y;
				num += spacing;
			}
		}
		infoScrollViewContent.sizeDelta = new Vector2(infoScrollViewContent.sizeDelta.x, num);
	}

	private IEnumerator HandleUrlDownload(ImportLevel level)
	{
		string filename = string.Empty;
		bool alreadyDownloadedFile = false;
		level.progressText.gameObject.SetActive(value: true);
		yield return AdoPackageInstaller.GetFileNameFromUrl(level.folderPath, delegate(PackageInstallerResult<string> result)
		{
			filename = result.Value;
			if (!string.IsNullOrEmpty(result.Error))
			{
				alreadyDownloadedFile = true;
			}
		});
		if (!alreadyDownloadedFile)
		{
			string resultFilePath = Path.Combine(scnCLS.tempLevelsFolder, filename);
			resultFilePath = RDUtils.GetAvailableDirectoryName(resultFilePath);
			yield return AdoPackageInstaller.DownloadPackage(level.folderPath, resultFilePath, level.progressText, delegate(PackageInstallerResult<string> result)
			{
				if (result.IsSuccess)
				{
					level.folderPath = resultFilePath;
				}
				else
				{
					AddLevelToErrorSection(level, result.Error);
				}
			});
			if (stoppedInstallCoroutine)
			{
				yield break;
			}
		}
		level.progressText.gameObject.SetActive(value: false);
	}

	private bool IsLevelFolderMissing(ImportLevel level)
	{
		if (!RDFile.Exists(level.folderPath) && !level.isUrl)
		{
			Debug.LogWarning("File not found: " + level.folderPath);
			return true;
		}
		return false;
	}

	private IEnumerator UnzipLevelFile(ImportLevel level, string levelUnzipDirectory, bool isAdoZip, Action<PackageInstallerResult<string>> callback)
	{
		try
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(level.folderPath);
			string text = Path.Combine(levelUnzipDirectory, fileNameWithoutExtension);
			if (RDDirectory.Exists(text))
			{
				Directory.Delete(text, recursive: true);
			}
			ZipUtils.Unzip(level.folderPath, text);
			levelUnzipDirectory = text;
			if (level.isUrl)
			{
				File.SetAttributes(level.folderPath, FileAttributes.Normal);
				File.Delete(level.folderPath);
			}
			level.folderPath = levelUnzipDirectory;
			callback?.Invoke(PackageInstallerResult<string>.Success(levelUnzipDirectory));
		}
		catch (Exception ex)
		{
			Debug.LogError("Error unzipping " + level.folderPath + ": " + ex.Message);
			callback?.Invoke(PackageInstallerResult<string>.Failure(RDString.Get("packageInstaller.fakeZipError").Replace("[error]", ex.Message)));
		}
		yield return null;
	}

	private void HandleCoroutineStop(ImportLevel level)
	{
		level.StopInstallProgress();
	}

	public void StopInstall()
	{
		stoppedInstallCoroutine = true;
		AdoPackageInstaller.cancelDownload = true;
		clearOrStopLevelsButton.interactable = false;
		ShowClearButton();
		miniImportSection.gameObject.SetActive(value: true);
		installButton.interactable = true;
	}

	private void ClearInfoSections()
	{
		foreach (ImportLevel level in toInstallIS.levels)
		{
			if (level != null)
			{
				UnityEngine.Object.Destroy(level.gameObject);
			}
		}
		toInstallIS.Clear();
		foreach (ImportLevel level2 in errorsIS.levels)
		{
			if (level2 != null)
			{
				UnityEngine.Object.Destroy(level2.gameObject);
			}
		}
		errorsIS.Clear();
		foreach (ImportLevel level3 in installedIS.levels)
		{
			if (level3 != null)
			{
				UnityEngine.Object.Destroy(level3.gameObject);
			}
		}
		installedIS.Clear();
		UpdateInfoContentHeight();
		clearOrStopLevelsButton.interactable = false;
		installButton.interactable = false;
	}

	private void ShowClearButton()
	{
		clearOrStopLevelsButton.gameObject.SetActive(value: true);
		clearOrStopLevelsButton.onClick.RemoveAllListeners();
		clearOrStopLevelsButton.onClick.AddListener(delegate
		{
			ClearInfoSections();
		});
		clearOrStopLevelsButton.interactable = true;
		clearOrStopLevelsButton.GetComponentInChildren<Text>().text = RDString.Get("packageInstaller.clear");
	}

	private void ShowStopInstallButton()
	{
		clearOrStopLevelsButton.gameObject.SetActive(value: true);
		clearOrStopLevelsButton.onClick.RemoveAllListeners();
		clearOrStopLevelsButton.onClick.AddListener(delegate
		{
			StopInstall();
		});
		clearOrStopLevelsButton.interactable = true;
		clearOrStopLevelsButton.GetComponentInChildren<Text>().text = RDString.Get("packageInstaller.stop");
	}

	private void StartOver()
	{
		bool num = installedIS.levels != null && installedIS.levels.Count > 0;
		ClearInfoSections();
		installPanelTitle.text = RDString.Get("cls.installLevels");
		installButtonText.text = RDString.Get("packageInstaller.install");
		installButton.onClick.RemoveAllListeners();
		installButton.onClick.AddListener(delegate
		{
			InstallContent();
		});
		installButton.interactable = false;
		miniImportSection.gameObject.SetActive(value: true);
		if (urlInput != null)
		{
			urlInput.text = string.Empty;
		}
		if (num)
		{
			ADOBase.cls.Refresh();
		}
	}

	private void CheckForDragAndDropSupport()
	{
		bool flag = false;
		flag = true;
		dragAndDrop.SetActive(flag);
		draggableInstructionsText.SetActive(flag);
		notDraggableInstructionsText.SetActive(!flag);
	}
}
