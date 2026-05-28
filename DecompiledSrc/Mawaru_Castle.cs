using DG.Tweening;
using TMPro;
using UnityEngine;

public class Mawaru_Castle : MonoBehaviour
{
	public Mawaru_Sprite princess;

	public Mawaru_Sprite worry;

	public Mawaru_Sprite heart;

	public Mawaru_Sprite princess_hat;

	public Mawaru_Sprite castle;

	public Mawaru_Sprite ring;

	public TextMeshPro speech;

	public ParticleSystem fire;

	public float princessGrav;

	public float hatGrav;

	public bool exploded;

	public bool won;

	private float nextTear = 505f;

	private Color halfOpac = new Color(1f, 1f, 1f, 0.5f);

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private float beat => (float)TaroBGScript.instance.songBeat;

	public float speed => TaroBGScript.instance.speed;

	public void Awake()
	{
		fire.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		if (speech != null)
		{
			speech.SetLocalizedFont();
		}
	}

	public void SetText(string txt)
	{
		speech.text = txt;
	}

	public void Explode()
	{
		if (!exploded)
		{
			castle.SetState(1);
			princess.SetState(5);
			princess.animate = false;
			fire.Play();
			exploded = true;
			princess_hat.render.enabled = true;
			princessGrav = 6f;
			hatGrav = 8f;
		}
	}

	public void UpdateWorry()
	{
		if (beat > nextTear && beat < 567f && ADOBase.controller.currentState == States.PlayerControl)
		{
			worry.render.enabled = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(worry.render.DOColor(halfOpac, 0.1f * speed))
				.Append(worry.render.DOColor(whiteClear, 0.6f * speed));
			worry.transform.eulerAngles = Vector3.forward * -30f;
			worry.transform.DORotate(Vector3.forward * -120f, 0.75f * speed);
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(worry.transform.DORotate(Vector3.forward * -30f, 0f))
				.Append(worry.transform.DORotate(Vector3.forward * -120f, 0.75f * speed).SetEase(Ease.Linear));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(worry.transform.DOLocalMoveX(0.6f, 0f))
				.Append(worry.transform.DOMoveX(0.4f, 0.75f * speed).SetRelative(isRelative: true).SetEase(Ease.Linear));
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).Append(worry.transform.DOLocalMoveY(2.1f, 0f))
				.Append(worry.transform.DOMoveY(0.2f, 0.35f * speed).SetRelative(isRelative: true).SetEase(Ease.OutCubic))
				.Append(worry.transform.DOMoveY(-0.25f, 0.4f * speed).SetRelative(isRelative: true).SetEase(Ease.InCubic));
			nextTear += 2f;
		}
	}
}
