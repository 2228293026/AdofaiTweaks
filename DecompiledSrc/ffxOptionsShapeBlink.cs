using DG.Tweening;

public class ffxOptionsShapeBlink : ffxPlusBase
{
	private scrOptionsWindows opWinRef;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
	}

	public void Start()
	{
		if (ADOBase.controller.visualQuality != VisualQuality.Low)
		{
			opWinRef = scrOptionsWindows.opWinRef;
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (ADOBase.controller.visualQuality == VisualQuality.Low)
		{
			return;
		}
		if (opWinRef == null)
		{
			opWinRef = scrOptionsWindows.opWinRef;
		}
		if (ADOBase.controller.playerManager.GetActivePlayers().Count <= 1)
		{
			DOTween.Sequence().AppendCallback(delegate
			{
				opWinRef.SetIcons(OptionsShape.Cross, staticTime: true);
			}).AppendInterval(0.5f)
				.SetLoops(-1);
		}
	}

	public override void ScrubToTime(float t)
	{
	}
}
