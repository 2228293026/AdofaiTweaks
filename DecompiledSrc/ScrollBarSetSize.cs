using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollBarSetSize : MonoBehaviour
{
	public float minimumSize = 0.05f;

	private Scrollbar scrollbar;

	private void Awake()
	{
		scrollbar = GetComponent<Scrollbar>();
	}

	private void LateUpdate()
	{
		SetScrollBarMinimumSize();
	}

	public void SetScrollBarMinimumSize()
	{
		if (scrollbar.size < minimumSize)
		{
			scrollbar.size = minimumSize;
		}
	}
}
