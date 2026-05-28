using System.Collections;
using System.Collections.Generic;
using ADOFAI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopLevelList : MonoBehaviour
{
	public Button btnRefresh;

	public GameObject buttonPrefab;

	public Transform buttonContainer;

	public TMP_Text txtTitleInfo;

	public GameObject worldsLoadingContainer;

	public RectTransform worldsLoadingPlanet;

	public Image thumbnail;

	public Button btnStartUpdate;

	public GameObject noWorldsFoundMessage;

	public int selectedIndex;

	public SteamWorkshop.ResultItem[] foundItems;

	private bool refreshing;

	private bool notFirstTime;

	public void Refresh()
	{
		StartCoroutine(RefreshCo());
	}

	private void Update()
	{
		if (refreshing)
		{
			worldsLoadingPlanet.localRotation = Quaternion.Euler(0f, 0f, worldsLoadingPlanet.localRotation.eulerAngles.z - 7f * Time.unscaledDeltaTime * 60f);
		}
	}

	private void Awake()
	{
		btnRefresh.onClick.AddListener(delegate
		{
			Refresh();
		});
	}

	public IEnumerator RefreshCo()
	{
		refreshing = true;
		if (notFirstTime)
		{
			foreach (Transform item in buttonContainer)
			{
				Object.Destroy(item.gameObject);
			}
		}
		else
		{
			notFirstTime = true;
		}
		worldsLoadingContainer.SetActive(value: true);
		btnStartUpdate.interactable = false;
		btnRefresh.interactable = false;
		txtTitleInfo.text = "";
		thumbnail.gameObject.SetActive(value: false);
		noWorldsFoundMessage.SetActive(value: false);
		int totalPublishedItems = SteamWorkshop.totalPublishedItems;
		int pageCount = totalPublishedItems / 50 + 1;
		List<SteamWorkshop.ResultItem> foundItemsList = new List<SteamWorkshop.ResultItem>();
		for (int page = 0; page < pageCount; page++)
		{
			yield return StartCoroutine(SteamWorkshop.GetPublishedItems(page + 1));
			if (!SteamWorkshop.OperationSuccess)
			{
				break;
			}
			foundItemsList.Add(SteamWorkshop.resultItems.ToArray());
		}
		foundItems = foundItemsList.ToArray();
		MakeButtons();
		refreshing = false;
	}

	private void MakeButtons()
	{
		worldsLoadingContainer.SetActive(value: false);
		btnRefresh.interactable = true;
		if (foundItems.Length == 0)
		{
			noWorldsFoundMessage.SetActive(value: true);
			return;
		}
		btnStartUpdate.interactable = true;
		thumbnail.gameObject.SetActive(value: true);
		new List<int>();
		for (int i = 0; i < foundItems.Length; i++)
		{
			string title = foundItems[i].title;
			Button component = Object.Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
			component.GetComponentInChildren<TMP_Text>().text = title;
			int toSelect = i;
			component.onClick.AddListener(delegate
			{
				SelectLevel(toSelect);
			});
		}
		SelectLevel(0, force: true);
	}

	private void SelectLevel(int index, bool force = false)
	{
		if (force || index != selectedIndex)
		{
			selectedIndex = index;
			txtTitleInfo.text = RDString.Get("editor.dialog.selectedWorld") + "\n" + foundItems[index].title;
			LoadResult status;
			Sprite sprite = TextureManager.LoadNewSprite(foundItems[index].previewImagePath, out status);
			thumbnail.sprite = sprite;
		}
	}
}
