using DG.Tweening;
using UnityEngine;

public class ffxSpriteMove : ffxPlusBase
{
	public GameObject targetSprite;

	public Vector2 targetLocalPosition;

	public float offsetBeats;

	public float durationBeats;

	public override bool runOnHit => true;

	public override void StartEffect(scrPlanet planet)
	{
		float crotchet = 60f / (cond.bpm * GetComponent<scrFloor>().speed) / cond.song.pitch;
		DOVirtual.DelayedCall(crotchet * offsetBeats, delegate
		{
			targetSprite.transform.DOLocalMove(targetLocalPosition, durationBeats * crotchet).SetEase(ease);
		});
	}
}
