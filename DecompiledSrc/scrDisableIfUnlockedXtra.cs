using UnityEngine;

public class scrDisableIfUnlockedXtra : MonoBehaviour
{
	private void Start()
	{
		if (Persistence.unlockedXF)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
