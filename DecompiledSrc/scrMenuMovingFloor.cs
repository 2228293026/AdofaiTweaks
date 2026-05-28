using DG.Tweening;
using UnityEngine;

public class scrMenuMovingFloor : MonoBehaviour
{
	public bool moving;

	private int xtraState;

	private bool xtraHidden;

	private Vector3 originalPos;

	private Collider2D coll;

	private Vector2 xtraIslandEntrance = new Vector2(-9f, 10f);

	private Vector2 entranceToXtraIsland = new Vector2(0f, 3f);

	private bool gemIsInTaroWorld = true;

	private void Awake()
	{
		originalPos = base.transform.position;
		if (base.name == "MovingGem_Bottom")
		{
			base.gameObject.SetActive(value: false);
		}
		else if (base.name == "MovingGem_Hop")
		{
			((Behaviour)(object)coll).enabled = false;
			GetComponent<SpriteRenderer>().color = Color.clear;
		}
	}

	private void Start()
	{
		coll = GetComponent<Collider2D>();
	}

	public void ResetPos(bool resetOriginalPosition = true)
	{
		if (resetOriginalPosition)
		{
			base.transform.position = originalPos;
		}
		moving = false;
		if ((Object)(object)coll != null)
		{
			((Behaviour)(object)coll).enabled = true;
		}
	}

	public void RunAnimationGlobal(bool down)
	{
		if (base.gameObject.activeSelf)
		{
			DOTween.Restart(down ? "MoveGemDown" : "MoveGemUp");
			moving = true;
			scrCamera.instance.isMoveTweening = true;
			if ((Object)(object)coll != null)
			{
				((Behaviour)(object)coll).enabled = false;
			}
		}
	}

	public void RunAnimation(bool down)
	{
		if (base.gameObject.activeSelf)
		{
			GetComponent<scrGem>().LocalMove();
			moving = true;
			scrCamera.instance.isMoveTweening = true;
			scrController.instance.responsive = false;
			if ((Object)(object)coll != null)
			{
				((Behaviour)(object)coll).enabled = false;
			}
		}
	}

	public void XtraPop(bool popIn)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (popIn)
		{
			if (component.color != Color.white)
			{
				component.color = Color.white;
				SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].color = Color.white;
				}
			}
			xtraHidden = false;
			((Tween)base.transform.DOLocalMove(originalPos + Vector3.up, 0.5f).SetEase(Ease.OutSine)).OnComplete((TweenCallback)delegate
			{
				if (!xtraHidden && (Object)(object)coll != null)
				{
					((Behaviour)(object)coll).enabled = true;
				}
			});
		}
		else
		{
			base.transform.DOLocalMove(originalPos, 0.5f).SetEase(Ease.OutSine);
			xtraHidden = true;
		}
	}

	public void Hop()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (Persistence.unlockedXF)
		{
			Hop2Xtra();
			return;
		}
		Sequence s = DOTween.Sequence();
		s.AppendCallback(delegate
		{
			moving = true;
			if ((Object)(object)coll != null)
			{
				((Behaviour)(object)coll).enabled = false;
			}
		});
		s.Append(base.transform.DOLocalMove(originalPos + Vector3.up * 4f, 0.35f).SetEase(Ease.OutQuad));
		s.AppendInterval(0.2f);
		s.Append(base.transform.DOLocalMove(originalPos + Vector3.up, 0.35f).SetEase(Ease.InQuad));
	}

	public void Hop2Xtra()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		int num = xtraState;
		if (num != 0 && num == 1)
		{
			Xtra2Front();
			return;
		}
		MonoBehaviour.print("hop2xtra");
		scrCamera cam = scrCamera.instance;
		Sequence sequence = DOTween.Sequence();
		sequence.AppendCallback(delegate
		{
			moving = true;
			if ((Object)(object)coll != null)
			{
				((Behaviour)(object)coll).enabled = false;
			}
			cam.isMoveTweening = true;
			cam.positionState = PositionState.GemToXtra;
		});
		sequence.Append(base.transform.DOMove(xtraIslandEntrance, 1f).SetEase(Ease.InOutSine));
		sequence.AppendInterval(0.1f);
		sequence.AppendCallback(delegate
		{
			originalPos = base.transform.position;
			ResetPos();
			cam.positionState = PositionState.Xtra;
			cam.frompos = cam.transform.position;
			cam.timer = 0f;
			xtraState = 1;
		});
		sequence.OnUpdate(delegate
		{
			cam.topos = new Vector2(base.transform.position.x, base.transform.position.y);
		});
		if (!Persistence.unlockedXF)
		{
			Persistence.unlockedXF = true;
		}
	}

	private void Xtra2Front()
	{
		MonoBehaviour.print("xtra2front");
		scrCamera cam = scrCamera.instance;
		Sequence s = DOTween.Sequence();
		s.AppendCallback(delegate
		{
			moving = true;
			((Behaviour)(object)coll).enabled = false;
			cam.isMoveTweening = true;
			cam.positionState = PositionState.None;
			cam.topos = new Vector2(0f, 0f);
		});
		s.Append(base.transform.DOLocalMove(entranceToXtraIsland, 0.8f).SetEase(Ease.OutQuad));
		s.AppendInterval(0.1f);
		s.AppendCallback(delegate
		{
			originalPos = base.transform.position;
			ResetPos();
			xtraState = 0;
		});
	}

	public void ExitTaroWorld()
	{
		scrController instance = scrController.instance;
		instance.LockInput(1f);
		instance.chosenPlanet.next.transform.DOMove(Vector3.up * 7f, 1f).SetEase(Ease.InOutQuad).SetRelative(isRelative: true);
		instance.chosenPlanet.transform.DOMove(Vector3.up * 7f, 1f).SetEase(Ease.InOutQuad).SetRelative(isRelative: true);
		base.transform.DOMove(Vector3.up * 7f, 1f).SetEase(Ease.InOutQuad).SetRelative(isRelative: true);
		gemIsInTaroWorld = false;
	}

	public void ReturnToTaroWorld()
	{
		if (!gemIsInTaroWorld)
		{
			((Behaviour)(object)coll).enabled = false;
			base.transform.DOMove(Vector3.down * 7f, 1f).SetEase(Ease.InOutQuad).SetRelative(isRelative: true);
			gemIsInTaroWorld = true;
			DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(1f)
				.OnComplete(delegate
				{
					((Behaviour)(object)coll).enabled = true;
				});
		}
	}

	public void MoveCamera(bool down)
	{
		scrCamera.instance.positionState = (down ? PositionState.DLC : PositionState.Levels);
	}

	public void MoveCameraX(bool down)
	{
		scrCamera.instance.positionState = ((!down) ? PositionState.Origin : PositionState.Xtra);
	}
}
