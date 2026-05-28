using UnityEngine;

public class HifiEnabler : ADOBase
{
	public GameObject[] objectsToActivate;

	public MonoBehaviour[] scriptsToActivate;

	private void Awake()
	{
		ADOBase.controller.UpdateVisualSettings();
		bool active = ADOBase.controller.visualQuality == VisualQuality.High;
		if (objectsToActivate != null)
		{
			GameObject[] array = objectsToActivate;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(active);
				}
			}
		}
		if (scriptsToActivate == null)
		{
			return;
		}
		MonoBehaviour[] array2 = scriptsToActivate;
		foreach (MonoBehaviour monoBehaviour in array2)
		{
			if (monoBehaviour != null)
			{
				monoBehaviour.enabled = active;
			}
		}
	}
}
