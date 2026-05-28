using System.Linq;
using UnityEngine;

public class WorldLightsDisplay_Lanterns : WorldLightsDisplay
{
	public bool dontUpdatePositions;

	protected override void Awake()
	{
		base.Awake();
		DoHolidayLanterns();
	}

	public override void UpdateStates(bool isWorldComplete, bool isWorldPerfect, bool isWorldSpeedTrial, bool updatePositions = true)
	{
		lights[0].gameObject.SetActive(isWorldComplete);
		lights[1].gameObject.SetActive(isWorldPerfect);
		lights[2].gameObject.SetActive(isWorldSpeedTrial);
		if (dontUpdatePositions)
		{
			return;
		}
		int num = new bool[3] { isWorldComplete, isWorldPerfect, isWorldSpeedTrial }.Count((bool x) => x);
		Vector2[][] array = new Vector2[3][]
		{
			new Vector2[1]
			{
				new Vector2(-0.06042726f, 0.01435363f)
			},
			new Vector2[2]
			{
				new Vector2(-1.175f, 0.034f),
				new Vector2(1.14f, 0.03399968f)
			},
			new Vector2[3]
			{
				new Vector2(-2.090776f, 0.4f),
				new Vector2(-0.06042726f, 0.01435363f),
				new Vector2(2.078689f, 0.4f)
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
				transform.GetChild(0).localPosition = new Vector3(vector.x, vector.y, transform.localPosition.z);
				num2++;
			}
		}
	}

	private void DoHolidayLanterns()
	{
		if (!dontUpdatePositions)
		{
			Sprite[] array = null;
			if (ADOBase.IsHalloweenWeek())
			{
				array = RDC.data.halloweenLanternSprites;
			}
			else if (ADOBase.IsCNY())
			{
				array = RDC.data.CNYLanternSprites;
			}
			if (array != null)
			{
				lights[0].GetComponentInChildren<SpriteRenderer>().sprite = array[0];
				lights[1].GetComponentInChildren<SpriteRenderer>().sprite = array[1];
				lights[2].GetComponentInChildren<SpriteRenderer>().sprite = array[2];
			}
		}
	}
}
