using System.Collections;
using System.Linq;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class scnSplash : ADOBase
{
	[Header("Fade")]
	public Image fade;

	public float fadeDuration = 0.3f;

	public Ease fadeEase = Ease.Linear;

	[Header("Warnings")]
	public Text healthGameAdvice;

	public float healthGameAdviceDuration;

	public Text alphaWarning;

	public Text translationWarning;

	public float alphaWarningDuration;

	public GameObject pressToBegin;

	private void Start()
	{
		StartCoroutine(ShowAlphaWarningCoroutine());
	}

	private void Update()
	{
	}

	private IEnumerator ShowAlphaWarningCoroutine()
	{
		yield return null;
		yield return null;
		Persistence.language.ToString();
		string text = default(string);
		if (SteamIntegration.initialized && SteamApps.GetCurrentBetaName(ref text, 20) && !GCNS.publicBranches.Contains(text))
		{
			alphaWarning.enabled = true;
			alphaWarning.text = alphaWarning.text.Replace("[branch]", text);
			alphaWarning.color = Color.clear;
			alphaWarning.DOColor(Color.white, 0.5f);
			yield return new WaitForSeconds(0.5f);
			float startTime = Time.unscaledTime;
			while (Time.unscaledTime < startTime + alphaWarningDuration && !Input.anyKeyDown)
			{
				yield return null;
			}
			alphaWarning.DOColor(Color.clear, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
		GoToMenu();
	}

	private void GoToMenu()
	{
		Debug.Log("Go to Menu");
		if (ADOBase.isMobile || ADOBase.isSwitch)
		{
			if (scrConductor.currentPreset.confident)
			{
				Debug.Log("Go to Level Select");
				ADOBase.GoToLevelSelect();
			}
			else
			{
				Debug.Log("Go to Calibration");
				scrConductor.currentPreset.confident = true;
				ADOBase.GoToCalibration();
			}
		}
		else
		{
			fade.DOFade(1f, fadeDuration).SetUpdate(isIndependentUpdate: true).SetEase(fadeEase)
				.OnComplete(delegate
				{
					ADOBase.GoToLevelSelect();
				});
		}
	}
}
