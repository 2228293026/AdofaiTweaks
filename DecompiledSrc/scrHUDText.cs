using UnityEngine;
using UnityEngine.UI;

public class scrHUDText : MonoBehaviour
{
	public bool isTitle;

	public bool changeColor = true;

	private Graphic gui;

	private Shadow shadow;

	private void Awake()
	{
		gui = GetComponent<Graphic>();
		if (TryGetComponent<Shadow>(out var component))
		{
			shadow = component;
		}
	}

	private void Update()
	{
		bool flag = RDC.partialNoHud || (!RDC.noHud && (!GCS.d_dontShowTitles || !isTitle));
		if (gui.enabled != flag)
		{
			gui.enabled = flag;
		}
		if (flag && changeColor)
		{
			gui.color = scrVfx.instance.currentColourScheme.colourText;
			if ((bool)shadow && ADOBase.customLevel != null)
			{
				shadow.effectColor = scrVfx.instance.currentColourScheme.colourTextShadow;
			}
		}
	}
}
