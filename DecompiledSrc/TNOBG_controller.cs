using DG.Tweening;
using UnityEngine;

public class TNOBG_controller : ADOBase
{
	public Camera tnoCam;

	private scrCamera mainCam;

	public Texture2D[] palettes;

	private TNOFilter filterComp;

	private Tween paletteTransitionTween;

	private void Start()
	{
		mainCam = scrCamera.instance;
		tnoCam.transform.SetParent(mainCam.transform);
		tnoCam.transform.localPosition = Vector3.zero;
		filterComp = tnoCam.GetComponent<TNOFilter>();
	}

	private void LateUpdate()
	{
		tnoCam.orthographicSize = mainCam.camobj.orthographicSize;
	}

	private void OnDestroy()
	{
		if (tnoCam != null)
		{
			Object.Destroy(tnoCam.gameObject);
		}
	}

	public void TransitionPalette(int palette, float durBeats)
	{
		if (palette < 0 || palette >= palettes.Length)
		{
			Debug.LogWarning("Bad palette index!");
			return;
		}
		if (paletteTransitionTween != null)
		{
			paletteTransitionTween.Kill(complete: true);
		}
		if (durBeats == 0f)
		{
			SwapPalette(palette);
			return;
		}
		float num = 60f / (ADOBase.conductor.bpm * ADOBase.conductor.song.pitch * ADOBase.controller.currFloor.speed);
		float duration = durBeats * num;
		filterComp.palette2 = palettes[palette];
		paletteTransitionTween = DOTween.To(() => filterComp.interp, delegate(float t)
		{
			filterComp.interp = t;
		}, 1f, duration).OnComplete(delegate
		{
			SwapPalette(palette);
		});
	}

	private void SwapPalette(int palette)
	{
		filterComp.palette = palettes[palette];
		filterComp.interp = 0f;
	}
}
