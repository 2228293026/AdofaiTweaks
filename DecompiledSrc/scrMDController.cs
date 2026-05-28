using DG.Tweening;
using UnityEngine;

public class scrMDController : ADOBase
{
	private static scrMDController _instance;

	public GameObject prefab_bearBlue;

	public GameObject prefab_bearPink;

	public GameObject prefab_candy;

	public GameObject prefab_samurai;

	public GameObject prefab_tv;

	public GameObject prefab_hammer;

	public SpriteRenderer bossSpriteRenderer;

	public Sprite idleSprite;

	public Sprite shootingSprite;

	public Sprite lolipopSprite;

	public Sprite lolipopSprite_final;

	public Transform candySpawner;

	public Transform finalBossPosition;

	private Tween shootTween;

	public static scrMDController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindAnyObjectByType<scrMDController>();
			}
			return _instance;
		}
	}

	public void BossShoot()
	{
		shootTween.Kill();
		bossSpriteRenderer.sprite = shootingSprite;
		shootTween = DOVirtual.DelayedCall(0.15f, delegate
		{
			bossSpriteRenderer.sprite = idleSprite;
		});
	}

	public void ChangeToCandy()
	{
		bossSpriteRenderer.transform.ScaleX(1f);
		bossSpriteRenderer.transform.parent.position = finalBossPosition.position;
		bossSpriteRenderer.enabled = true;
	}

	public void BossAppear()
	{
		bossSpriteRenderer.transform.parent.DOMoveY(bossSpriteRenderer.transform.parent.position.y - 10f, 1f).SetEase(Ease.OutExpo);
	}

	public void BossDisappear()
	{
		bossSpriteRenderer.transform.parent.DOMoveY(bossSpriteRenderer.transform.parent.position.y + 10f, 2f).SetEase(Ease.InBack).OnComplete(delegate
		{
			bossSpriteRenderer.enabled = false;
		});
	}

	public void ChangeToFinal()
	{
		bossSpriteRenderer.sprite = lolipopSprite_final;
	}
}
