using System.Collections.Generic;
using UnityEngine;

public class SetActiveAtAwake : MonoBehaviour
{
	public List<GameObject> objectsToSetActive;

	private void Awake()
	{
		foreach (GameObject item in objectsToSetActive)
		{
			item.SetActive(value: true);
		}
	}
}
