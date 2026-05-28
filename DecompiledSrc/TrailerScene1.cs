using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrailerScene1 : TaroBGScript
{
	[Header("Assets", order = 0)]
	private GameObject something;

	public List<Mawaru_Sprite> embers;

	private List<Ember> elist;

	private float edelay = 0.15f;

	private float etimer;

	private int curEmber;

	public GameObject bossBody;

	public GameObject bossHand1;

	public GameObject bossHand2;

	public Mawaru_Sprite glare;

	private Vector3 squish = new Vector3(0.6f, 1f, 1f);

	private float beat;

	private new void Awake()
	{
		base.Awake();
		elist = new List<Ember>();
		bpms = new List<Tuple<double, double>>();
		bpms.Add(new Tuple<double, double>(0.0, 150.0));
		mb(1f, FunnyText);
		SortTables();
		mpf(-1f, Drunk, 999f);
		foreach (Mawaru_Sprite ember in embers)
		{
			Ember item = new Ember(ember);
			elist.Add(item);
		}
		mb(21f, Camera);
	}

	private void FunnyText()
	{
		Debug.Log("Yee haw!");
	}

	private void Drunk()
	{
		beat = (float)songBeat;
		for (int i = 0; i < 35; i++)
		{
			scrFloor floor = scrLevelMaker.instance.listFloors[i];
			FloorResetPosition(floor);
			FloorDrunkVibe(floor, 0.3f, 0f, 6f);
		}
		bossBody.transform.localPosition = Vector3.right * 30f + Vector3.up * 0.2f * Mathf.Sin(Time.time * 0.9f);
		bossHand1.transform.localPosition = Vector3.right * -3.5f + Vector3.up * (-0.25f + 0.25f * Mathf.Sin(Time.time * 1f)) + Vector3.forward * 35f;
		bossHand2.transform.localPosition = Vector3.right * 17f + Vector3.up * (-0.4f + -0.25f * Mathf.Sin(Time.time * 1f)) + Vector3.forward * 35f;
		scrController.instance.planetBlue.Destroy();
		etimer -= Time.deltaTime;
		if (etimer < 0f)
		{
			etimer += edelay;
			Ember ember = elist[curEmber];
			ember.spawnTime = Time.time;
			ember.maxLife = 3.5f;
			ember.lifeTime = ember.maxLife;
			ember.randScale = UnityEngine.Random.Range(1f, 1.5f);
			ember.emb.transform.localScale = squish * ember.randScale;
			ember.xspeed = UnityEngine.Random.Range(0.3f, 0.6f);
			ember.yspeed = UnityEngine.Random.Range(1.8f, 2.5f);
			ember.xwaveSize = UnityEngine.Random.Range(0.6f, 0.8f);
			ember.xwavePeriod = UnityEngine.Random.Range(0.4f, 0.8f);
			ember.xwaveOffset = UnityEngine.Random.Range(0f, 1f);
			ember.alive = true;
			ember.emb.render.enabled = true;
			ember.emb.transform.localPosition = Vector3.up * -8f + Vector3.forward * 10f + Vector3.right * UnityEngine.Random.Range(-10f, 10f);
			curEmber = (curEmber + 1) % elist.Count;
		}
		foreach (Ember item in elist)
		{
			if (item.alive)
			{
				item.lifeTime -= Time.deltaTime;
				item.emb.render.color = new Color(1f, 1f, 1f, item.lifeTime / item.maxLife);
				float num = item.yspeed * Time.deltaTime;
				float num2 = item.xspeed * Time.deltaTime + item.xwaveSize * Mathf.Sin(Time.time * (float)Math.PI * 2f * item.xwavePeriod + item.xwaveOffset * (float)Math.PI * 2f) * Time.deltaTime;
				item.emb.transform.localPosition += Vector3.right * num2;
				item.emb.transform.localPosition += Vector3.up * num;
				item.emb.transform.eulerAngles = Vector3.forward * (57.29578f * Mathf.Atan2(num, num2) - 90f);
				item.emb.transform.localScale = squish * item.randScale * (0.5f + 0.5f * item.lifeTime / item.maxLife);
				if (item.lifeTime < 0f)
				{
					item.alive = false;
					item.emb.render.enabled = false;
				}
			}
		}
	}

	private void Camera()
	{
		base.camy.zoomSize = 1f;
		DOTween.To(() => base.camy.zoomSize, delegate(float x)
		{
			base.camy.zoomSize = x;
		}, 1.2f, 8f).SetEase(Ease.OutCubic);
		Debug.Log("YEE HAW");
		DOTween.Sequence().SetUpdate(isIndependentUpdate: true).AppendInterval(0.8f)
			.Append(glare.render.DOColor(Color.white, 0.06f).SetEase(Ease.InQuad))
			.Append(glare.render.DOColor(whiteClear, 4f).SetEase(Ease.OutQuad));
	}
}
