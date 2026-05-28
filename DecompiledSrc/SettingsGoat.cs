using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsGoat : MonoBehaviour
{
	public RectTransform container;

	public RectTransform head;

	public Image eyes;

	public Image beam;

	public Text description;

	public CanvasGroup descriptionCanvasGroup;

	public Color redEyesColor;

	public Color cyanEyesColor;

	public float frequency;

	public float amplitude;

	public Vector2 goatActivePosition;

	private bool hasText;

	private bool isProjecting;

	private Tween goatPatrol;

	private void Awake()
	{
		description.SetLocalizedFont();
	}

	private void Update()
	{
		head.AnchorPosY(Mathf.Sin(Time.unscaledTime * frequency) * amplitude);
	}

	public void SetDescription(string text)
	{
		hasText = !string.IsNullOrEmpty(text);
		if (hasText)
		{
			description.text = text;
		}
		Color endValue = (hasText ? cyanEyesColor : redEyesColor);
		float endValue2 = (hasText ? 0.5f : 0f);
		float endValue3 = (hasText ? 1f : 0f);
		float num = (hasText ? (-21f) : 0f);
		eyes.DOColor(endValue, 0.15f).SetUpdate(isIndependentUpdate: true);
		beam.DOFade(endValue2, 0.15f).SetUpdate(isIndependentUpdate: true);
		descriptionCanvasGroup.DOFade(endValue3, 0.15f).SetUpdate(isIndependentUpdate: true);
		head.DOLocalRotate(Vector3.forward * num, 0.375f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
		if (hasText)
		{
			goatPatrol?.Kill();
			container.DOKill();
			container.DOAnchorPos(goatActivePosition, 0.5f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuad);
			container.DOScaleX(-1f, 0.5f).SetUpdate(isIndependentUpdate: true);
			if (!isProjecting)
			{
				scrController.instance.pauseMenu.PlayMenuSfx(SfxSound.MenuGoatProjector, 0.5f);
			}
			isProjecting = true;
		}
		else
		{
			isProjecting = false;
			DOVirtual.DelayedCall(2f, delegate
			{
				PatrolGoat();
			});
		}
	}

	private void PatrolGoat()
	{
		if (!hasText)
		{
			float num = 0f;
			int num2 = 0;
			Vector2 vector;
			do
			{
				vector = new Vector2(Random.Range(-100f, -22f), Random.Range(55f, -55f));
				num = (container.anchoredPosition - vector).magnitude * 0.03f;
				num2++;
			}
			while (num < 1.5f && num2 < 100);
			float delay = Random.Range(0.05f, 0.15f);
			goatPatrol?.Kill();
			goatPatrol = container.DOAnchorPos(vector, num).SetUpdate(isIndependentUpdate: true).SetDelay(delay)
				.SetEase(Ease.InOutBack, 1.15f)
				.OnComplete(delegate
				{
					PatrolGoat();
				});
			float endValue = ((vector.x < container.anchoredPosition.x) ? (-1f) : 1f);
			container.DOScaleX(endValue, 0.7f).SetDelay(delay).SetUpdate(isIndependentUpdate: true);
		}
	}
}
