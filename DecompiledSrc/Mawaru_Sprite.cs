using System.Collections.Generic;
using UnityEngine;

public class Mawaru_Sprite : MonoBehaviour
{
	public SpriteRenderer render;

	public float miscf;

	public Vector3 startpos;

	public SpriteMask mask;

	public List<Sprite> frames = new List<Sprite>();

	public int curFrame;

	public float frameDelay = 0.1f;

	public float frameTime;

	public List<float> frameTimings = new List<float>();

	public bool animate = true;

	public bool loop = true;

	public int firstFrame;

	public int lastFrame = -1;

	public float loopFrameDelay = -1f;

	public bool enabledOnStartup;

	public bool disableOnInvisible = true;

	public bool hidden;

	private int updates;

	public float speed => 1f / scrConductor.instance.song.pitch;

	private void OnBecameVisible()
	{
		if (disableOnInvisible)
		{
			base.enabled = true;
		}
	}

	private void OnBecameInvisible()
	{
		if (disableOnInvisible && (updates > 0 || (!enabledOnStartup && updates == 0)))
		{
			base.enabled = false;
		}
	}

	public void OnGUI()
	{
		if (render == null)
		{
			render = base.gameObject.GetComponent<SpriteRenderer>();
		}
	}

	public void Awake()
	{
		startpos = base.transform.position;
		render = base.gameObject.GetComponent<SpriteRenderer>();
		if (frameTimings.Count > 0 && frameTimings.Count == frames.Count)
		{
			frameDelay = frameTimings[curFrame];
		}
		base.enabled = enabledOnStartup;
		if (hidden)
		{
			render.enabled = false;
		}
	}

	public void FadeEdge(float left, float top, float right, float bottom)
	{
		float x = left + top;
		float y = bottom + right;
		float z = top + right;
		float w = left + bottom;
		render.material.SetVector("_Fade", new Vector4(x, y, z, w));
	}

	public void Update()
	{
		updates++;
		if (frames.Count <= 1 || !animate)
		{
			return;
		}
		frameTime += Time.deltaTime / speed;
		if (!(frameTime > frameDelay))
		{
			return;
		}
		curFrame++;
		frameTime -= frameDelay;
		if (curFrame >= frames.Count || (lastFrame > -1 && curFrame > lastFrame))
		{
			if (loop)
			{
				curFrame = firstFrame;
			}
			else
			{
				curFrame--;
			}
		}
		if (frameTimings.Count > 0 && frameTimings.Count == frames.Count)
		{
			frameDelay = frameTimings[curFrame];
		}
		render.sprite = frames[curFrame];
	}

	public void SetState(int f)
	{
		if (f >= frames.Count)
		{
			f = firstFrame;
		}
		frameTime = 0f;
		if (lastFrame < f && lastFrame > -1)
		{
			lastFrame = f;
		}
		curFrame = f;
		if (frameTimings.Count > 0 && frameTimings.Count == frames.Count)
		{
			frameDelay = frameTimings[curFrame];
		}
		render.sprite = frames[curFrame];
	}

	public void GotoAndPlay(int f)
	{
		SetState(f);
		animate = true;
	}

	public void Play()
	{
		SetState(curFrame + 1);
		animate = true;
	}

	public void Hide()
	{
		render.enabled = false;
		hidden = true;
	}

	public void Unhide()
	{
		render.enabled = true;
		hidden = false;
	}

	public void LateUpdate()
	{
		if ((Object)(object)mask != null)
		{
			mask.sprite = render.sprite;
		}
	}
}
