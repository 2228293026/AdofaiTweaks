using UnityEngine;

public class ffxMDHammer : ffxPlusBase
{
	public GameObject prefab;

	public bool clockwise;

	private const float moveDuration = 3f;

	private const float initialRotation = -135f;

	private Transform hammerTr;

	private bool moving;

	private bool hit;

	public override void Awake()
	{
		base.Awake();
		hifiEffect = true;
		if (ADOBase.controller.visualQuality == VisualQuality.High)
		{
			floor.gameObject.AddComponent<ffxMDEnemyHit>().onHit = OnHit;
		}
	}

	public override void SetStartTime(float bpm, float degreeOffset = 0f)
	{
		float num = 60f / (bpm * floor.speed);
		startTime = Mathf.Max((float)floor.entryTime - 3f * num, 0f);
		duration = num / cond.song.pitch * 3f;
		FloorSetup();
	}

	public override void StartEffect(scrPlanet planet)
	{
		if (!(hammerTr == null))
		{
			hammerTr.parent.gameObject.SetActive(value: true);
			moving = true;
		}
	}

	private void Update()
	{
		float num = Time.deltaTime * cond.song.pitch;
		if (moving)
		{
			float num2 = (hit ? 450f : (-97.5f));
			hammerTr.localEulerAngles -= Vector3.forward * num2 * num;
			if (hammerTr.localEulerAngles.z <= (float)(hit ? 250 : 190) && hammerTr.localEulerAngles.z >= 90f)
			{
				hammerTr.GetComponent<SpriteRenderer>().color = Color.white.WithAlpha(0f);
			}
		}
	}

	public void FloorSetup()
	{
		Transform component = Object.Instantiate(prefab, base.transform.position, Quaternion.identity).GetComponent<Transform>();
		if (clockwise)
		{
			component.ScaleX(-1f);
		}
		hammerTr = component.GetChild(0);
		hammerTr.localEulerAngles = Vector3.forward * -135f;
	}

	public void OnHit()
	{
		hit = true;
	}
}
