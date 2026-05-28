using System.Collections.Generic;
using UnityEngine;

public class ListItemPool : ADOBase
{
	public GameObject itemPrefab;

	[HideInInspector]
	public Transform itemHolder;

	private List<GameObject> pooledItems = new List<GameObject>();

	public bool Initialized { get; private set; }

	public void Initialize()
	{
		itemHolder = new GameObject().transform;
		itemHolder.SetParent(base.transform);
		itemHolder.name = itemPrefab.name + "Pool";
		itemHolder.transform.localScale = Vector3.one;
		Initialized = true;
	}

	public GameObject GetPooledItem(RectTransform parent, Vector3 position)
	{
		if (!Initialized)
		{
			return null;
		}
		GameObject obj = ((itemHolder.childCount > 0) ? itemHolder.GetChild(0).gameObject : CreateItem());
		obj.transform.SetParent(parent);
		RectTransform component = obj.GetComponent<RectTransform>();
		component.anchoredPosition = component.anchoredPosition.WithY(position.y);
		component.offsetMin = component.offsetMin.WithX(0f);
		component.offsetMax = component.offsetMax.WithX(0f);
		obj.SetActive(value: true);
		return obj;
	}

	public void SendItemBackToPool(GameObject item)
	{
		if (Initialized)
		{
			item.transform.SetParent(itemHolder);
			item.SetActive(value: false);
		}
	}

	private GameObject CreateItem()
	{
		GameObject gameObject = Object.Instantiate(itemPrefab, itemHolder);
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(value: false);
		pooledItems.Add(gameObject);
		return gameObject;
	}
}
