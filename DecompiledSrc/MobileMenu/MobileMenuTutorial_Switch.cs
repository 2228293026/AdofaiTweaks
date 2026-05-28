using UnityEngine;

namespace MobileMenu;

public class MobileMenuTutorial_Switch : ADOBase
{
	public SpriteRenderer backgroundFader;

	public GameObject instructions;

	public scrFloor[] floors;

	private bool tutorialCompleted;

	private void Awake()
	{
		backgroundFader.gameObject.SetActive(value: true);
		instructions.SetActive(value: true);
		ADOBase.controller.camy.transform.MoveXY(floors[0].transform.position.x, floors[0].transform.position.y);
	}

	private void DoCompleteTutorial()
	{
		tutorialCompleted = true;
		ADOBase.controller.EnterLevel("1-2");
	}

	private void Update()
	{
		scrFloor scrFloor2 = floors[0];
		scrFloor[] array = floors;
		scrFloor scrFloor3 = array[array.Length - 1];
		scrCamera camy = ADOBase.controller.camy;
		float x = camy.transform.position.x;
		float x2 = ADOBase.controller.chosenPlanet.transform.position.x;
		float num = Mathf.Lerp(x, x2, 4f * Time.deltaTime);
		camy.transform.position = camy.transform.position.WithX(num);
		float num2 = Mathf.InverseLerp(scrFloor2.transform.position.x, scrFloor2.transform.position.y, num);
		backgroundFader.color = backgroundFader.color.WithAlpha(1f - num2);
		ADOBase.conductor.song2.volume = num2;
		if (!tutorialCompleted && ADOBase.controller.chosenPlanet.currfloor == scrFloor3)
		{
			DoCompleteTutorial();
		}
	}
}
