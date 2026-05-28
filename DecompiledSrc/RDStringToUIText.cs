using UnityEngine;
using UnityEngine.UI;

public class RDStringToUIText : MonoBehaviour
{
	public string key;

	public float cjkScale;

	public bool enlargeForMobile;

	public bool changeFontForLanguage = true;

	private void Start()
	{
		Text component = GetComponent<Text>();
		if (component != null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				component.text = RDString.Get(key);
			}
			Apply(component, changeFontForLanguage, enlargeForMobile, cjkScale);
			return;
		}
		TextMesh component2 = GetComponent<TextMesh>();
		if (component2 != null)
		{
			if (!string.IsNullOrEmpty(key))
			{
				component2.text = RDString.Get(key);
			}
			Apply(component2, changeFontForLanguage, enlargeForMobile, cjkScale);
		}
		else
		{
			Debug.LogError("didn't found any uitext or textmesh to set RDString with key: " + key + " onGameObject: " + base.gameObject.name);
		}
	}

	public static void Apply(Text text, bool changeFontForLanguage, bool enlargeForMobile, float scale = 0f)
	{
		if (changeFontForLanguage)
		{
			FontData fontData = RDString.fontData;
			_ = RDConstants.data;
			text.font = fontData.font;
			text.lineSpacing = fontData.lineSpacing;
			float num = ((scale != 0f) ? scale : 1f);
			text.fontSize = Mathf.RoundToInt((float)text.fontSize * num);
		}
	}

	public static void Apply(TextMesh text, bool changeFontForLanguage, bool enlargeForMobile, float cjkScale = 0f)
	{
		if (changeFontForLanguage)
		{
			FontData fontData = RDString.fontData;
			text.font = fontData.font;
			text.lineSpacing = fontData.lineSpacing;
			text.GetComponent<MeshRenderer>().material = text.font.material;
			if (changeFontForLanguage)
			{
				float num = ((cjkScale != 0f) ? cjkScale : 1f);
				text.fontSize = Mathf.RoundToInt((float)text.fontSize * num);
			}
		}
		else
		{
			MonoBehaviour.print("change font for language is false!: " + text.gameObject.name);
		}
	}
}
