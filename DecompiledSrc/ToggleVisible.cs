using UnityEngine;

public class ToggleVisible : MonoBehaviour
{
	public MonoBehaviour scriptToToggle;

	public bool ignoreIfDisabled;

	private void OnBecameVisible()
	{
		scriptToToggle.enabled = true;
	}

	private void OnBecameInvisible()
	{
		if (!ignoreIfDisabled || base.gameObject.activeSelf)
		{
			scriptToToggle.enabled = false;
		}
	}
}
