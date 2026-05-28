using UnityEngine;

public class scrDisableIfNotUnlockedXtra : MonoBehaviour
{
	private void Start()
	{
		if (!Persistence.unlockedXF)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
