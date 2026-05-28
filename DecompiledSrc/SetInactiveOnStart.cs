using UnityEngine;

public class SetInactiveOnStart : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}
}
