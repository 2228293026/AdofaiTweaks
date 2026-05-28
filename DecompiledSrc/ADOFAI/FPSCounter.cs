using TMPro;
using UnityEngine;

namespace ADOFAI;

public class FPSCounter : MonoBehaviour
{
	private const int Samples = 120;

	public TMP_Text text;

	private readonly float[] _samples = new float[120];

	private int sampleIndex;

	private int framesElapsed;

	private double lastTimeReference;

	private int fps;

	private RectTransform rt;

	private void Awake()
	{
		rt = base.gameObject.GetComponent<RectTransform>();
	}

	private void Update()
	{
		text.enabled = GCS.showFPS;
		if (GCS.showFPS)
		{
			rt.anchoredPosition = new Vector2(24f, -24f);
			rt.MoveZ(0f);
			sampleIndex = (int)Mathf.Repeat(sampleIndex + 1, 120f);
			float num = 1f / Time.unscaledDeltaTime;
			_samples[sampleIndex] = num;
			float num2 = 0f;
			for (int i = 0; i < 120; i++)
			{
				num2 += _samples[i];
			}
			float f = num2 / 120f;
			double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
			if (realtimeSinceStartupAsDouble - lastTimeReference >= 1.0)
			{
				lastTimeReference = realtimeSinceStartupAsDouble;
				text.text = "FPS: " + Mathf.RoundToInt(f);
			}
		}
	}
}
