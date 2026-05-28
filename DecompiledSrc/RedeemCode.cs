using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RedeemCode : MonoBehaviour
{
	public EntitlementsService entitlements;

	public CanvasGroup canvasGroup;

	public TMP_InputField inputField;

	public Button redeemButton;

	public Button cancelButton;

	public GameObject redeemText;

	public GameObject loading;

	private bool isLoading;

	private void Awake()
	{
		canvasGroup.alpha = 0f;
		CanvasGroup obj = canvasGroup;
		bool interactable = (canvasGroup.blocksRaycasts = false);
		obj.interactable = interactable;
		inputField.onSubmit.AddListener(ReedemCode);
		redeemButton.onClick.AddListener(ReedemCode);
		cancelButton.onClick.AddListener(Cancel);
		loading.SetActive(value: false);
	}

	private void Update()
	{
		if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.X))
		{
			if (entitlements.playerIdentifier.IsNullOrEmpty())
			{
				OnRedeemComplete(sucesfull: false, "error.entitlement.playerIdentifier");
			}
			else
			{
				EnablePanel(!canvasGroup.interactable);
			}
		}
	}

	private void EnablePanel(bool enable)
	{
		scrController.instance.paused = enable;
		if (enable)
		{
			canvasGroup.gameObject.SetActive(value: true);
		}
		CanvasGroup obj = canvasGroup;
		bool interactable = (canvasGroup.blocksRaycasts = enable);
		obj.interactable = interactable;
		canvasGroup.DOFade(enable ? 1f : 0f, 0.5f).OnComplete(delegate
		{
			if (enable)
			{
				inputField.Select();
			}
			else
			{
				canvasGroup.gameObject.SetActive(value: false);
			}
		});
	}

	private void SetLoading(bool enable)
	{
		isLoading = enable;
		loading.SetActive(enable);
		redeemText.SetActive(!enable);
	}

	public void ReedemCode()
	{
		ReedemCode(inputField.text);
	}

	public void ReedemCode(string code)
	{
		if (!isLoading)
		{
			SetLoading(enable: true);
			entitlements.RedeemCode(code, OnRedeemComplete);
		}
	}

	public void Cancel()
	{
		EnablePanel(enable: false);
	}

	public void OnRedeemComplete(bool sucesfull, string token)
	{
		SetLoading(enable: false);
		Notification.instance.ShowEntitlementMessage(sucesfull, token);
	}
}
