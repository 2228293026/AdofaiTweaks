using DG.Tweening;
using UnityEngine;

public class ffxActivateGoat : ffxPlusBase
{
	public SpriteRenderer goatEyes;

	public SpriteRenderer goatLight;

	public Transform goatTrans;

	public float targetAngle;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		float num = 60f / (cond.bpm * cond.song.pitch * floor.speed);
		Sequence s = DOTween.Sequence();
		s.Append(goatEyes.DOFade(1f, num / 2f).SetEase(Ease.Flash, 3f));
		s.Append(goatTrans.DORotate(new Vector3(0f, 0f, targetAngle), num).SetEase(Ease.OutCubic));
		goatLight.DOFade(0.3f, num / 2f).SetEase(Ease.Flash, 3f);
	}
}
