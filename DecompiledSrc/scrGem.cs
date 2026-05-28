using DG.Tweening;
using UnityEngine;

public class scrGem : ADOBase
{
	public Transform startPosition;

	public Transform endPosition;

	public int startIndex;

	public int endIndex;

	[Header("Rotation")]
	public bool rotate = true;

	public float rotateDuration = 4f;

	public float rotateDelay;

	public Ease rotateEase = Ease.Linear;

	[Header("Movement")]
	public bool move = true;

	public float moveDuration = 0.7f;

	public Ease moveEase = Ease.OutQuad;

	public bool moveToStart = true;

	private scrMenuMovingFloor movingFloor;

	private void Awake()
	{
		if (move && GCS.FOOL_JOKER)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			movingFloor = GetComponent<scrMenuMovingFloor>();
		}
	}

	private void Start()
	{
		if (rotate)
		{
			LocalRotate();
		}
	}

	private void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (startPosition == null || endPosition == null)
		{
			return;
		}
		foreach (scrPlayer item in scrPlayerManager.instance)
		{
			Vector3 position = item.planetarySystem.chosenPlanet.transform.position;
			float num = Vector2.Distance(startPosition.position, position);
			float num2 = Vector2.Distance(endPosition.position, position);
			bool flag = num2 > num;
			float num3 = (flag ? num : num2);
			if (moveToStart != flag && num3 < 3f)
			{
				moveToStart = !flag;
				LocalMove(moveCamera: false);
				moveToStart = flag;
				break;
			}
		}
	}

	private void LocalRotate()
	{
		base.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), rotateDuration).SetDelay(rotateDelay).SetRelative(isRelative: true)
			.SetEase(rotateEase)
			.SetLoops(-1, LoopType.Restart);
	}

	public void LocalMove(bool moveCamera = true)
	{
		if (!movingFloor.moving)
		{
			if (moveCamera)
			{
				scrCamera.instance.positionState = (PositionState)(moveToStart ? endIndex : startIndex);
				scrCamera.instance.isMoveTweening = true;
			}
			Vector3 endValue = (moveToStart ? (endPosition.localPosition - startPosition.localPosition) : (startPosition.localPosition - endPosition.localPosition));
			DOTween.Kill("LocalMove", complete: true);
			base.transform.DOLocalMove(endValue, moveDuration).SetId("LocalMove").SetEase(moveEase)
				.SetRelative(isRelative: true)
				.OnComplete(delegate
				{
					movingFloor.ResetPos(resetOriginalPosition: false);
				});
		}
	}

	public void JumpTo(int jumpKey)
	{
		if (!movingFloor.moving)
		{
			movingFloor.moving = true;
			scnLevelSelect.instance.JumpWithKey(jumpKey);
			base.transform.DOLocalMove(new Vector3(0f, moveToStart ? 5 : (-5), 0f), moveDuration).SetEase(moveEase).SetRelative(isRelative: true)
				.OnComplete(delegate
				{
					movingFloor.ResetPos();
				});
		}
	}
}
