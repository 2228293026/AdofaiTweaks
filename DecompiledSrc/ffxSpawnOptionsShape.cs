using DG.Tweening;
using UnityEngine;

public class ffxSpawnOptionsShape : ffxPlusBase
{
	public OptionsShape targetShape;

	private GameObject shapePrefab;

	private Sprite circleSprite;

	private Sprite squareSprite;

	private Sprite crossSprite;

	private Sprite plusSprite;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		shapePrefab = ADOBase.gc.prefab_options_shape;
		circleSprite = ADOBase.gc.sprite_options_circle;
		squareSprite = ADOBase.gc.sprite_options_square;
		crossSprite = ADOBase.gc.sprite_options_cross;
		plusSprite = ADOBase.gc.sprite_options_plus;
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			GameObject shape = Object.Instantiate(shapePrefab, planet.transform.position, Quaternion.identity);
			shape.transform.PositionX(shape.transform.position.x + Random.Range(4f, -4f));
			shape.transform.PositionY(shape.transform.position.y + Random.Range(4f, -4f));
			shape.transform.localScale = Vector3.one * Random.Range(2f, 4f);
			SpriteRenderer component = shape.GetComponent<SpriteRenderer>();
			SpriteRenderer spriteRenderer = component;
			spriteRenderer.sprite = targetShape switch
			{
				OptionsShape.Square => squareSprite, 
				OptionsShape.Plus => plusSprite, 
				OptionsShape.Cross => crossSprite, 
				_ => component.sprite, 
			};
			if (scrController.coopMode)
			{
				component.color = planet.planetRenderer.planetColor.ToRealColor();
				int count = ADOBase.controller.playerManager.GetActivePlayers().Count;
				shape.transform.localScale *= 1f - (float)(count - 1) * 0.25f;
			}
			else
			{
				component.color = ((Random.Range(0, 2) == 1) ? Color.red : Color.cyan);
			}
			float num = 0.5f;
			scrFloor currFloor = ADOBase.controller.currFloor;
			if (currFloor != null && currFloor.seqID <= ADOBase.lm.listFloors.Count - 3)
			{
				num = (float)(currFloor.nextfloor.nextfloor.entryTime - currFloor.nextfloor.entryTime) / cond.song.pitch;
			}
			component.DOBlendableColor(component.color.WithAlpha(0f), num * 2f).OnComplete(delegate
			{
				Object.Destroy(shape);
			});
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
