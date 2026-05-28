using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
	public string sortingLayer;

	public int orderInLayer;

	private void Awake()
	{
		Renderer component = GetComponent<Renderer>();
		component.sortingLayerName = sortingLayer;
		component.sortingOrder = orderInLayer;
	}
}
