using DG.Tweening;
using UnityEngine;

public class AreaCover : MonoBehaviour
{
	public Mawaru_Sprite bg;

	public Mawaru_Sprite fg;

	private Vector3 compressed = new Vector3(0f, 0.1f, 1f);

	private Vector3 uncompressed1 = new Vector3(1f, 0.1f, 1f);

	private Vector3 uncompressed2 = new Vector3(1f, 1f, 1f);

	private Color whiteClear = new Color(1f, 1f, 1f, 0f);

	private void Awake()
	{
		bg.transform.localScale = compressed;
		fg.transform.localScale = compressed;
		bg.render.enabled = false;
		fg.render.enabled = false;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Spawn(float dur)
	{
		bg.transform.localScale = compressed;
		fg.transform.localScale = compressed;
		fg.render.DOColor(Color.white, 0f);
		fg.render.DOColor(Color.white, 0f);
		bg.render.enabled = true;
		fg.render.enabled = true;
		DOTween.Sequence().Append(bg.transform.DOScale(uncompressed1, dur * 0.3f).SetEase(Ease.OutBack));
		DOTween.Sequence().AppendInterval(dur * 0.2f).Append(bg.transform.DOScale(uncompressed2, dur * 0.7f).SetEase(Ease.OutQuart));
		DOTween.Sequence().Append(fg.transform.DOScale(uncompressed1, dur * 0.3f).SetEase(Ease.OutBack));
		DOTween.Sequence().AppendInterval(dur * 0.2f).Append(fg.transform.DOScale(uncompressed2, dur * 0.7f).SetEase(Ease.OutQuart));
	}

	public void Hide()
	{
		DOTween.Shake(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, 0.5f, 0.6f, 60);
		DOTween.Sequence().Append(fg.render.DOColor(whiteClear, 0f)).AppendInterval(0.1f)
			.Append(fg.render.DOColor(Color.white, 0f))
			.AppendInterval(0.1f)
			.Append(fg.render.DOColor(whiteClear, 0f))
			.AppendInterval(0.1f)
			.Append(fg.render.DOColor(Color.white, 0f))
			.AppendInterval(0.1f)
			.Append(fg.render.DOColor(whiteClear, 0f))
			.AppendInterval(0.1f)
			.Append(fg.render.DOColor(Color.white, 0f))
			.Append(fg.render.DOColor(whiteClear, 0.4f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				fg.render.enabled = false;
			}));
		DOTween.Sequence().Append(bg.render.DOColor(whiteClear, 0f)).AppendInterval(0.1f)
			.Append(bg.render.DOColor(Color.white, 0f))
			.AppendInterval(0.1f)
			.Append(bg.render.DOColor(whiteClear, 0f))
			.AppendInterval(0.1f)
			.Append(bg.render.DOColor(Color.white, 0f))
			.AppendInterval(0.1f)
			.Append(bg.render.DOColor(whiteClear, 0f))
			.AppendInterval(0.1f)
			.Append(bg.render.DOColor(Color.white, 0f))
			.Append(bg.render.DOColor(whiteClear, 0.4f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				bg.render.enabled = false;
			}));
	}

	public void HideGentle()
	{
		DOTween.Sequence().Append(fg.render.DOColor(whiteClear, 0.4f).SetEase(Ease.InQuad).OnComplete(delegate
		{
			fg.render.enabled = false;
		}));
		DOTween.Sequence().Append(bg.render.DOColor(whiteClear, 0.4f).SetEase(Ease.InQuad).OnComplete(delegate
		{
			bg.render.enabled = false;
		}));
	}
}
