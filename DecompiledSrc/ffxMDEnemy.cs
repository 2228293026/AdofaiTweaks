using UnityEngine;

public class ffxMDEnemy : ffxPlusBase
{
	public enum EnemyType
	{
		Candy,
		BearBlue,
		BearPink,
		Samurai,
		TV
	}

	public enum Direction
	{
		Left,
		Right,
		Up,
		Down,
		Boss
	}

	public enum HitDirection
	{
		Left,
		Right
	}

	public Direction direction;

	public HitDirection hitDirection;

	public EnemyType enemyType;

	private float moveDuration = 8f;

	private const float speed = 8f;

	private const float gravity = 100f;

	private Transform enemyTr;

	private bool moving;

	private bool hit;

	private Vector3 movementVector;

	private Vector3 velocity;

	private scrMDController mdc;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		mdc = scrMDController.instance;
		floor.gameObject.AddComponent<ffxMDEnemyHit>().onHit = OnHit;
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		if (direction == Direction.Boss)
		{
			moveDuration *= 0.5f;
		}
		if (enemyType == EnemyType.TV)
		{
			moveDuration *= 0.8f;
		}
		float num = 60f / (bpm * floor.speed);
		startTime = Mathf.Max((float)floor.entryTime - moveDuration * num, 0f);
		duration = num / cond.song.pitch * moveDuration;
		FloorSetup();
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (!(enemyTr == null))
		{
			if (direction == Direction.Boss)
			{
				enemyTr.position = mdc.candySpawner.position;
				movementVector = (base.transform.position - mdc.candySpawner.position) / 8f / duration / cond.song.pitch;
				mdc.BossShoot();
			}
			else
			{
				enemyTr.position = base.transform.position - movementVector * 8f * duration * cond.song.pitch;
			}
			enemyTr.gameObject.SetActive(value: true);
			moving = true;
		}
	}

	private void Update()
	{
		float num = Time.deltaTime * cond.song.pitch;
		if (!moving)
		{
			return;
		}
		if (hit)
		{
			velocity += Vector3.down * 100f * num;
			enemyTr.position += velocity * num;
			enemyTr.eulerAngles += Vector3.forward * -360f * 2f * ((hitDirection == HitDirection.Right) ? 1 : (-1)) * num;
			return;
		}
		enemyTr.position += movementVector * num * 8f;
		if (enemyType == EnemyType.Candy)
		{
			enemyTr.eulerAngles += Vector3.forward * 360f * num;
		}
	}

	public void FloorSetup()
	{
		if (Persistence.visualEffects == VisualEffects.Minimum || Persistence.visualQuality == VisualQuality.Low)
		{
			return;
		}
		Vector3 position = Vector3.zero;
		if (direction != Direction.Boss)
		{
			switch (direction)
			{
			case Direction.Up:
				movementVector = Vector3.up;
				break;
			case Direction.Down:
				movementVector = Vector3.down;
				break;
			case Direction.Left:
				movementVector = Vector3.left;
				break;
			case Direction.Right:
				movementVector = Vector3.right;
				break;
			}
			position = base.transform.position - movementVector * 8f * duration;
		}
		GameObject original = null;
		switch (enemyType)
		{
		case EnemyType.Candy:
			original = mdc.prefab_candy;
			break;
		case EnemyType.BearBlue:
			original = mdc.prefab_bearBlue;
			break;
		case EnemyType.BearPink:
			original = mdc.prefab_bearPink;
			break;
		case EnemyType.Samurai:
			original = mdc.prefab_samurai;
			break;
		case EnemyType.TV:
			original = mdc.prefab_tv;
			break;
		}
		GameObject gameObject = Object.Instantiate(original, position, Quaternion.identity);
		enemyTr = gameObject.GetComponent<Transform>();
		if (direction == Direction.Right)
		{
			gameObject.GetComponentInChildren<SpriteRenderer>().flipX = true;
		}
	}

	public void OnHit()
	{
		hit = true;
		int num = ((hitDirection == HitDirection.Right) ? 1 : (-1));
		velocity = new Vector3(8 * num, 18f, 0f);
	}
}
