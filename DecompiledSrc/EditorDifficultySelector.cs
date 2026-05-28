using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EditorDifficultySelector : ADOBase
{
	private const float CHANGE_TIME = 0.4f;

	public Text editorDifficultyText;

	public Button buttonChangeDifficulty;

	public Image bullseyeImage;

	public Image upArrowImage;

	public Image downArrowImage;

	public RectTransform selectorRectTransform;

	private void Start()
	{
		buttonChangeDifficulty.onClick.AddListener(delegate
		{
			ToggleDifficulty();
		});
	}

	private void OnEnable()
	{
		UpdateDifficultyDisplay();
	}

	public void SetChangeable(bool changeable)
	{
		selectorRectTransform.DOComplete();
		selectorRectTransform.DOAnchorPosY((!changeable) ? (-120) : 0, 0.4f).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true);
		editorDifficultyText.DOComplete();
		editorDifficultyText.DOFade(changeable ? 1f : 0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true);
		upArrowImage.DOComplete();
		upArrowImage.DOFade(changeable ? 1f : 0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true);
		downArrowImage.DOComplete();
		downArrowImage.DOFade(changeable ? 1f : 0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(isIndependentUpdate: true);
		buttonChangeDifficulty.interactable = changeable;
		UpdateDifficultyDisplay();
	}

	private void ToggleDifficulty()
	{
		int length = Enum.GetValues(typeof(Difficulty)).Length;
		GCS.difficulty++;
		if (GCS.difficulty == (Difficulty)length)
		{
			GCS.difficulty -= length;
		}
		Persistence.SetDefaultDifficulty(GCS.difficulty);
		UpdateDifficultyDisplay();
		editorDifficultyText.rectTransform.DOComplete();
		editorDifficultyText.rectTransform.DOPunchAnchorPos(new Vector2(0f, 5f), 0.2f, 0).SetUpdate(isIndependentUpdate: true);
		scrSfx.instance.PlaySfx(SfxSound.MenuSquelch, MixerGroup.InterfaceParent, 1.5f);
	}

	private void UpdateDifficultyDisplay()
	{
		editorDifficultyText.text = RDString.Get("enum.Difficulty." + GCS.difficulty);
		bullseyeImage.sprite = RDConstants.data.bullseyeSprites[(int)GCS.difficulty];
	}

	public void OnPointerEnter()
	{
		buttonChangeDifficulty.gameObject.SetActive(value: true);
		editorDifficultyText.gameObject.SetActive(value: true);
	}

	public void OnPointerExit()
	{
		buttonChangeDifficulty.gameObject.SetActive(value: false);
		editorDifficultyText.gameObject.SetActive(value: false);
	}
}
