using UnityEngine;
using UnityEngine.UI;

namespace CrazyMinnow.AmplitudeWebGL;

public class AmplitudeTester : MonoBehaviour
{
	public Amplitude amplitude;

	public Slider uiSlider;

	private void Update()
	{
		if (amplitude.audioSource.isPlaying)
		{
			uiSlider.value = amplitude.average;
		}
	}

	public void Play()
	{
		amplitude.audioSource.Play();
	}

	public void Stop()
	{
		amplitude.audioSource.Stop();
	}
}
