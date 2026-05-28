using UnityEngine;

public class scrBenchmark : ADOBase
{
	private const float TargetFPS = 45f;

	private const float TargetTime = 1f;

	private const int maxIterations = 5;

	private const int waitFramesBeforeStarting = 30;

	private float[] frameDeltaTime = new float[1000];

	private float target = 1f;

	private float minResolution = 0.3f;

	private float maxResolution = 1f;

	private float t;

	private int framesRecorded;

	private int waitFrames;

	private int iterations;

	private bool running;

	public string text
	{
		set
		{
			scrUIController.instance.txtLevelName.text = value;
		}
	}

	private void Awake()
	{
		target = 1f;
		running = true;
		text = "target: " + target;
	}

	private void Update()
	{
		if (waitFrames < 30)
		{
			waitFrames++;
		}
		else
		{
			if (!running)
			{
				return;
			}
			if (t >= 1f)
			{
				float num = 0f;
				for (int i = 0; i < framesRecorded; i++)
				{
					num += frameDeltaTime[i];
				}
				float num2 = num / (float)framesRecorded;
				float num3 = 1f / num2;
				bool flag = num3 >= 45f;
				text = $"averageDeltaTime: {num2}, averageFPS: {num3}, higherTarget {flag}";
				ChooseNewTarget(flag);
			}
			t += Time.deltaTime;
			frameDeltaTime[framesRecorded] = Time.deltaTime;
			framesRecorded++;
		}
	}

	private void ChooseNewTarget(bool higherTarget)
	{
		if (higherTarget && target == 1f)
		{
			Finish();
		}
		if (higherTarget)
		{
			minResolution = target;
		}
		else
		{
			maxResolution = target;
		}
		target = (minResolution + maxResolution) / 2f;
		t = 0f;
		framesRecorded = 0;
		iterations++;
		int displayWidth = ADOBase.GetDisplayWidth();
		Screen.SetResolution(height: Mathf.RoundToInt((float)ADOBase.GetDisplayHeight() * target), width: Mathf.RoundToInt((float)displayWidth * target), fullscreen: true);
		text = "new target: " + target;
		if (iterations == 5)
		{
			Finish();
		}
	}

	private void Finish()
	{
		running = false;
		text = "Benchmark complete! Your resolution has been set to " + Mathf.RoundToInt(target * 100f) + "%";
	}
}
