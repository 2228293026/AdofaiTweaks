using System.Collections;
using UnityEngine;

namespace CrazyMinnow.AmplitudeWebGL;

[AddComponentMenu("Crazy Minnow Studio/Amplitude/Amplitude")]
public class Amplitude : MonoBehaviour
{
	[HideInInspector]
	public enum DataType
	{
		Amplitude,
		Frequency
	}

	public AudioSource audioSource;

	public int sampleSize = 32;

	public bool absoluteValues = true;

	[Range(0f, 1f)]
	public float boost;

	public float average;

	public float[] sample;

	[HideInInspector]
	public DataType dataType;

	[HideInInspector]
	public string[] sampleSizeNames = new string[8] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };

	[HideInInspector]
	public int[] sampleSizeVals = new int[8] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };

	[HideInInspector]
	private float total;

	[HideInInspector]
	private const float boostBase = 1f;

	[HideInInspector]
	private bool prevPlayState;

	[HideInInspector]
	private float prevTime;

	[HideInInspector]
	private bool restart;

	[HideInInspector]
	private bool run;

	[HideInInspector]
	private float playOnAwakeDelay = 0.1f;

	[HideInInspector]
	private float sign;

	[HideInInspector]
	private float tempVal;

	public float freqBoost;

	[HideInInspector]
	private const float freqBoostMin = 4f;

	[HideInInspector]
	private const float freqBoostMax = 40f;

	private void Awake()
	{
		if ((bool)audioSource && audioSource.playOnAwake)
		{
			audioSource.Stop();
			run = false;
		}
	}

	private void Start()
	{
		sample = new float[sampleSize];
		if ((bool)audioSource)
		{
			if (audioSource.playOnAwake)
			{
				StartCoroutine(PlayOnAwake(playOnAwakeDelay));
			}
			else
			{
				run = true;
			}
		}
	}

	private IEnumerator PlayOnAwake(float delay)
	{
		audioSource.Play();
		yield return new WaitForSeconds(delay);
		run = true;
	}

	private void LateUpdate()
	{
		if (!audioSource || !run)
		{
			return;
		}
		if (audioSource.isPlaying && audioSource.time < prevTime)
		{
			restart = true;
		}
		if (sample.Length != sampleSize)
		{
			sample = new float[sampleSize];
		}
		if (audioSource.isPlaying && (audioSource.isPlaying != prevPlayState || restart))
		{
			prevPlayState = audioSource.isPlaying;
			restart = false;
		}
		if (audioSource.isPlaying)
		{
			if (dataType == DataType.Amplitude)
			{
				audioSource.GetOutputData(sample, 0);
			}
			else
			{
				if (sample.Length == 32)
				{
					sample = new float[64];
				}
				AudioListener.GetSpectrumData(sample, 0, FFTWindow.BlackmanHarris);
			}
			if (sample != null)
			{
				total = 0f;
				for (int i = 0; i < sampleSize; i++)
				{
					if (dataType == DataType.Frequency)
					{
						absoluteValues = true;
					}
					tempVal = sample[i];
					sign = Mathf.Sign(tempVal);
					if (absoluteValues)
					{
						if (dataType == DataType.Amplitude)
						{
							tempVal = Mathf.Abs(tempVal);
							tempVal = Mathf.Pow(tempVal, 1f - boost);
							sample[i] = tempVal;
						}
						else
						{
							tempVal = Mathf.Pow(tempVal, 1f - boost);
							sample[i] = tempVal;
						}
					}
					else
					{
						tempVal = Mathf.Pow(tempVal, 1f - boost);
						sample[i] = tempVal * sign;
					}
					total += sample[i];
				}
				average = total / (float)sampleSize;
			}
			else
			{
				Debug.Log("sample is null");
			}
			prevTime = audioSource.time;
		}
		if (!audioSource.isPlaying && audioSource.isPlaying != prevPlayState)
		{
			for (int j = 0; j < sampleSize; j++)
			{
				sample[j] = 0f;
			}
			average = 0f;
			prevPlayState = audioSource.isPlaying;
			prevTime = 0f;
		}
	}

	public static float ScaleRange(float val, float inMin, float inMax, float outMin, float outMax)
	{
		val = Mathf.Clamp(val, inMin, inMax);
		return Mathf.Clamp(Mathf.Abs((val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin), outMin, outMax);
	}
}
