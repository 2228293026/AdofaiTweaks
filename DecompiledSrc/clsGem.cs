using DG.Tweening;

public class clsGem : ffxPlusBase
{
	public bool down;

	private bool moving;

	public override void StartEffect(scrPlanet planet)
	{
		scnCLS instance = scnCLS.instance;
		int num = (down ? instance.gemBottomY : instance.gemTopY);
		Move(move: true);
		base.transform.DOMoveY(num, 1f).SetEase(Ease.InOutSine).OnComplete(delegate
		{
			Move(move: false);
		});
	}

	private void Move(bool move)
	{
		scnCLS instance = scnCLS.instance;
		GetComponent<scrMenuMovingFloor>().moving = move;
		moving = move;
		(down ? instance.gemBottom.gameObject : instance.gemTop.gameObject).SetActive(!move);
		if (!move)
		{
			base.transform.MoveY(down ? instance.gemTopY : instance.gemBottomY);
		}
		int targetIndex = (down ? (instance.levelCount - 1) : 0);
		instance.LoadTileIconsNearby(targetIndex);
	}

	private void Update()
	{
		if (moving)
		{
			cam.Refocus(cam.transform.position.WithY(base.transform.position.y));
		}
	}
}
