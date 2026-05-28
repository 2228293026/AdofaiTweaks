using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class scrMetronome : MonoBehaviour
{
	public double bpm = 140.0;

	public float gain = 0.5f;

	public int signatureHi = 4;

	public int signatureLo = 4;

	private double nextTick;

	private float amp;

	private float phase;

	private double sampleRate;

	private int accent;

	private bool running;

	private void Start()
	{
		accent = signatureHi;
		double dspTime = scrConductor.instance.dspTime;
		sampleRate = AudioSettings.outputSampleRate;
		nextTick = dspTime * sampleRate;
		running = true;
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		if (!running)
		{
			return;
		}
		double num = sampleRate * 60.0 / bpm * 4.0 / (double)signatureLo;
		double num2 = scrConductor.instance.dspTime * sampleRate;
		int num3 = data.Length / channels;
		for (int i = 0; i < num3; i++)
		{
			float num4 = gain * amp * Mathf.Sin(phase);
			for (int j = 0; j < channels; j++)
			{
				data[i * channels + j] += num4;
			}
			while (num2 + (double)i >= nextTick)
			{
				nextTick += num;
				amp = 1f;
				if (++accent > signatureHi)
				{
					accent = 1;
					amp *= 2f;
				}
				Debug.Log("Tick: " + accent + "/" + signatureHi);
			}
			phase += amp * 0.3f;
			amp *= 0.993f;
		}
	}
}
