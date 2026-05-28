using System.Linq;
using UnityEngine;

public class WorldLightsDisplay_Crown : WorldLightsDisplay
{
	public override void UpdateStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true)
	{
		lights[0].gameObject.SetActive(isWorldComplete);
		lights[1].gameObject.SetActive(isWorldPerfect);
		lights[2].gameObject.SetActive(isWorldSpeedTrial);
		int num = new bool[3] { isWorldComplete, isWorldPerfect, isWorldSpeedTrial }.Count((bool x) => x);
		Vector2[][] array = new Vector2[3][]
		{
			new Vector2[1]
			{
				new Vector2(0f, 1.518f)
			},
			new Vector2[2]
			{
				new Vector2(-1.503f, 1.169f),
				new Vector2(1.509f, 1.167f)
			},
			new Vector2[3]
			{
				new Vector2(-1.503f, 1.169f),
				new Vector2(0f, 1.518f),
				new Vector2(1.509f, 1.167f)
			}
		};
		if (num == 0)
		{
			return;
		}
		Vector2[] array2 = array[num - 1];
		int num2 = 0;
		for (int num3 = 0; num3 < lights.Length; num3++)
		{
			Transform transform = lights[num3];
			if (transform.gameObject.activeSelf)
			{
				Vector2 vector = array2[num2];
				transform.localPosition = new Vector3(vector.x, vector.y, transform.localPosition.z);
				num2++;
			}
		}
	}
}
