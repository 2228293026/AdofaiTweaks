using UnityEngine;

public class scrDisableIfWorldNotComplete : MonoBehaviour
{
	public int world;

	private void Start()
	{
		if (world >= 0)
		{
			if (!Persistence.IsWorldComplete(world))
			{
				base.gameObject.SetActive(value: false);
			}
		}
		else if (Persistence.GetOverallProgressStage() < -world)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
