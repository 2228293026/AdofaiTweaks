using UnityEngine;

public class scrDisableIfWorldNotCompleteString : MonoBehaviour
{
	public string world;

	private void Start()
	{
		if (!Persistence.IsWorldComplete(world))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
