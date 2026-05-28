using UnityEngine;

public class scrOptionsDot : ADOBase
{
	private struct Dot
	{
		private Transform transform;

		private float lastScale;

		private float nextScale;

		private float curDur;

		private float curTimer;

		public void Setup(Transform transform, Renderer renderer)
		{
			this.transform = transform;
			lastScale = Random.Range(0.125f, 0.325f);
			nextScale = Random.Range(0.125f, 0.325f);
			curDur = Random.Range(0.1f, 0.2f);
			renderer.sortingLayerName = "Bg";
			renderer.sortingOrder = -1;
		}

		public void Update()
		{
			curTimer += Time.deltaTime;
			if (curTimer >= curDur)
			{
				lastScale = nextScale;
				nextScale = Random.Range(0.125f, 0.325f);
				curDur = Random.Range(0.1f, 0.2f);
				curTimer = 0f;
			}
			float num = Mathf.Lerp(lastScale, nextScale, curTimer / curDur);
			transform.localScale = new Vector3(num, num, 1f);
		}
	}

	private const float minDur = 0.1f;

	private const float maxDur = 0.2f;

	private const float minScale = 0.125f;

	private const float maxScale = 0.325f;

	public Transform[] dotTransforms;

	public Renderer[] dotRenderers;

	private Dot[] dots;

	private void Awake()
	{
		if (ADOBase.controller.visualEffects == VisualEffects.Full)
		{
			dots = new Dot[dotTransforms.Length];
			for (int i = 0; i < dotTransforms.Length; i++)
			{
				dots[i].Setup(dotTransforms[i], dotRenderers[i]);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (dots == null)
		{
			base.enabled = false;
			return;
		}
		for (int i = 0; i < dots.Length; i++)
		{
			dots[i].Update();
		}
	}
}
