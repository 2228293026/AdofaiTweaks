using System;
using DG.Tweening;
using UnityEngine;

public class scrHoldRenderer : ADOBase
{
	private const int holdSegments = 32;

	private const int segmentsPerLoop = 100;

	public scrFloor startFloor;

	private scrFloor headFloor;

	private scrFloor tailFloor;

	public Mesh m_mesh;

	public MeshRenderer m_meshRenderer;

	public Bounds meshBounds;

	public Texture tex;

	public bool autoGenerate;

	public bool hit;

	public bool unfilling;

	public float unfillStartTime;

	public float unfillStartVal;

	public float unfillStartValNoEase;

	public float unfillVal;

	public Color touchColor = Color.clear;

	private LineRenderer endCircle;

	private float earlyAngle = (float)Math.PI * 2f;

	private float totalAngleLength;

	private float perc;

	private float perc2;

	private float tweenLength = 0.8f;

	private float rad;

	private float alp;

	private float extendAlp = 1f;

	private int addedLoops;

	private float isCCWMult = 1f;

	private float ang;

	public void Start()
	{
		if (startFloor.showHoldTiming && endCircle == null)
		{
			CreateEndCircle();
		}
		m_meshRenderer.sortingLayerName = "FloorCover";
		m_meshRenderer.sortingOrder = startFloor.floorRenderer.renderer.sortingOrder + 200;
		if (autoGenerate)
		{
			m_mesh = base.gameObject.GetComponent<MeshFilter>().mesh;
			m_meshRenderer.material.SetTexture("_MainTex", tex);
			m_meshRenderer.material.renderQueue = 2900;
			CreateMesh();
		}
		if (RDC.hideTaroGimmicks)
		{
			m_meshRenderer.enabled = false;
		}
		headFloor = startFloor.prevfloor;
		tailFloor = startFloor.nextfloor;
	}

	public void Unfill(bool withHoldTiming = true)
	{
		unfilling = true;
		unfillStartTime = Time.time;
		unfillStartVal = startFloor.holdCompletionEased;
		if (!startFloor.showHoldTiming)
		{
			return;
		}
		if (withHoldTiming)
		{
			DOTween.To(() => rad, delegate(float x)
			{
				rad = x;
			}, 1.8f, 0.4f).SetEase(Ease.OutCubic);
			DOTween.To(() => alp, delegate(float x)
			{
				alp = x;
			}, 0f, 0.4f).SetEase(Ease.Linear);
		}
		else
		{
			rad = 1f;
			alp = 0f;
		}
	}

	public void Hit()
	{
		if (startFloor.showHoldTiming && !hit)
		{
			hit = true;
			unfillStartTime = Time.time;
			unfillStartVal = startFloor.holdCompletionEased;
			DOTween.To(() => rad, delegate(float x)
			{
				rad = x;
			}, 1.8f, 0.4f).SetEase(Ease.OutCubic);
			DOTween.To(() => alp, delegate(float x)
			{
				alp = x;
			}, 0f, 0.4f).SetEase(Ease.Linear);
		}
	}

	public void Update()
	{
		if (startFloor.extendAnim == 0f)
		{
			tailFloor = startFloor;
			extendAlp = 0f;
		}
		else if (startFloor.nextfloor != null && startFloor.nextfloor.extendAnim > 0f)
		{
			tailFloor = startFloor.nextfloor;
			extendAlp = tailFloor.extendAnim;
		}
		if (!headFloor || !tailFloor)
		{
			return;
		}
		UpdateMesh();
		if (unfilling)
		{
			perc = Mathf.Clamp01((Time.time - unfillStartTime) / tweenLength);
			unfillVal = DOVirtual.EasedValue(unfillStartVal, 0f, perc, Ease.OutCubic);
			startFloor.holdCompletionEased = 0f;
			UpdateColor(touchColor, unfillVal);
			if (startFloor.showHoldTiming)
			{
				DrawEndCircle(new Color(1f, 1f, 1f, alp), rad);
			}
			if (Time.time > unfillStartTime + tweenLength + 0.1f)
			{
				unfilling = false;
			}
		}
		else
		{
			UpdateColor(startFloor.holdCompletionEased);
		}
		if (hit)
		{
			DrawEndCircle(new Color(1f, 1f, 1f, alp), rad);
			if (Time.time > unfillStartTime + tweenLength + 0.1f)
			{
				hit = false;
			}
		}
	}

	public void CreateMesh()
	{
		if (startFloor.midSpin)
		{
			return;
		}
		headFloor = startFloor.prevfloor;
		tailFloor = startFloor.nextfloor;
		if (!scrController.instance.gameworld)
		{
			totalAngleLength = (float)Math.PI * (float)(startFloor.holdLength * 2 + 1);
		}
		else
		{
			totalAngleLength = (float)scrMisc.GetAngleMoved(startFloor.entryangle, startFloor.exitangle, GCS.FOOL_SWIRL ? startFloor.isCCW : (!startFloor.isCCW));
			totalAngleLength -= (float)scrMisc.GetInverseAnglePerBeatMultiplanet(startFloor.numPlanets);
			if ((double)totalAngleLength < 0.001 && startFloor.numPlanets > 2)
			{
				totalAngleLength += (float)Math.PI * 2f;
				addedLoops++;
			}
			totalAngleLength += (float)Math.PI * (float)(startFloor.holdLength * 2);
		}
		if (earlyAngle > totalAngleLength)
		{
			earlyAngle = totalAngleLength;
		}
		if (!(m_mesh == null) && !(m_meshRenderer == null) && !(tailFloor == null))
		{
			isCCWMult = (startFloor.isCCW ? (-1f) : 1f);
			if (GCS.FOOL_SWIRL)
			{
				isCCWMult *= -1f;
			}
			int num = startFloor.holdLength + addedLoops;
			double num2 = scrMisc.GetAngleMoved(startFloor.entryangle, startFloor.exitangle, GCS.FOOL_SWIRL ? startFloor.isCCW : (!startFloor.isCCW));
			double num3 = Math.Abs(num2);
			if (num3 <= (double)Mathf.Pow(10f, -6f) || num3 >= (double)((float)Math.PI * 2f - Mathf.Pow(10f, -6f)))
			{
				num2 = (startFloor.midSpin ? 0f : ((float)Math.PI * 2f));
			}
			num2 += (double)((float)num * (float)Math.PI * 2f);
			float value = 0.4f;
			int num4 = (int)Mathf.Floor(100f * ((float)num2 / ((float)Math.PI * 2f)));
			int num5 = 2 + num4 * 2;
			int num6 = num4 * 2;
			Vector3[] vertices = new Vector3[num5];
			Vector2[] array = new Vector2[num5];
			Vector2[] array2 = new Vector2[num5];
			int[] array3 = new int[num6 * 3];
			for (int i = 0; i < num4 + 1; i++)
			{
				float y = (float)i / 100f;
				int num7 = 2 * i;
				array[num7].x = 0f;
				array[num7].y = y;
				num7 = 1 + 2 * i;
				array[num7].x = 1f;
				array[num7].y = y;
			}
			for (int j = 0; j < num5; j++)
			{
				array2[j].x = j;
			}
			for (int k = 0; k < num4; k++)
			{
				array3[k * 6] = k * 2;
				array3[k * 6 + 1] = k * 2 + 1;
				array3[k * 6 + 2] = k * 2 + 2;
				array3[k * 6 + 3] = k * 2 + 2;
				array3[k * 6 + 4] = k * 2 + 1;
				array3[k * 6 + 5] = k * 2 + 3;
			}
			m_mesh.SetVertices(vertices);
			m_mesh.SetUVs(0, array);
			m_mesh.SetUVs(1, array2);
			m_mesh.SetTriangles(array3, 0);
			m_meshRenderer.material.SetInt("_NumVerts", m_mesh.vertices.Length);
			m_meshRenderer.material.SetFloat("_CCW", isCCWMult);
			m_meshRenderer.material.SetFloat("_StartTileSize", 1f);
			m_meshRenderer.material.SetFloat("_EndTileSize", 1f);
			m_meshRenderer.material.SetFloat("_EntryAngle", (float)startFloor.entryangle);
			m_meshRenderer.material.SetFloat("_AngleLength", (float)num2);
			m_meshRenderer.material.SetFloat("_HoldWidth", value);
			m_meshRenderer.material.SetFloat("_StartAlpha", 1f);
			m_meshRenderer.material.SetFloat("_EndAlpha", 1f);
			m_meshRenderer.material.SetVector("_StartPosition", Vector3.zero);
			m_meshRenderer.material.SetVector("_EndPosition", Vector3.zero);
			m_meshRenderer.material.SetFloat("_StartWidth", 1f);
			m_meshRenderer.material.SetFloat("_EndWidth", 1f);
			UpdateMesh();
		}
	}

	public void UpdateFoolDir()
	{
		isCCWMult = (startFloor.isCCW ? (-1f) : 1f);
		m_meshRenderer.material.SetFloat("_CCW", isCCWMult);
		UpdateMesh();
	}

	public void UpdateMesh()
	{
		if (!(m_mesh == null) && !(m_meshRenderer == null) && !(tailFloor == null))
		{
			float tileSize = scrController.instance.tileSize;
			Vector3 position = startFloor.transform.position;
			Vector3 vector = tileSize * tailFloor.radiusScale * new Vector3(Mathf.Sin((float)startFloor.exitangle), Mathf.Cos((float)startFloor.exitangle), 0f);
			Vector3 vector2 = tailFloor.transform.position + vector * -1f;
			float x = headFloor.floorRenderer.transform.localScale.x;
			float x2 = tailFloor.floorRenderer.transform.localScale.x;
			m_meshRenderer.material.SetFloat("_StartTileSize", scrController.instance.tileSize * startFloor.radiusScale);
			m_meshRenderer.material.SetFloat("_EndTileSize", scrController.instance.tileSize * tailFloor.radiusScale);
			m_meshRenderer.material.SetVector("_StartPosition", position);
			m_meshRenderer.material.SetVector("_EndPosition", vector2);
			m_meshRenderer.material.SetFloat("_StartWidth", x);
			m_meshRenderer.material.SetFloat("_EndWidth", x2);
			meshBounds = new Bounds((headFloor.transform.position + tailFloor.transform.position) / 2f, Vector3.one);
			meshBounds.Encapsulate(headFloor.transform.position);
			meshBounds.Encapsulate(startFloor.transform.position);
			meshBounds.Encapsulate(tailFloor.transform.position);
			meshBounds.size += Vector3.one * 5f;
			m_mesh.bounds = meshBounds;
		}
	}

	public void UpdateColor(float percentageToColor)
	{
		UpdateColor(touchColor, percentageToColor);
	}

	public void UpdateColor(Color holdColor, float percentageToColor)
	{
		if (m_mesh == null || m_meshRenderer == null || tailFloor == null)
		{
			return;
		}
		float x = headFloor.floorRenderer.transform.localScale.x;
		float x2 = tailFloor.floorRenderer.transform.localScale.x;
		if ((headFloor.opacity <= 0f || headFloor.opacity == 0f || x <= 0f) && (tailFloor.opacity <= 0f || tailFloor.opacity == 0f || x2 <= 0f))
		{
			m_meshRenderer.enabled = false;
			return;
		}
		m_meshRenderer.enabled = true;
		if (holdColor != Color.clear)
		{
			m_meshRenderer.material.SetColor("_FillColor", holdColor);
			touchColor = holdColor;
		}
		m_meshRenderer.material.SetFloat("_Completion", percentageToColor);
		m_meshRenderer.material.SetFloat("_StartAlpha", headFloor.opacity * startFloor.holdOpacity * extendAlp);
		if (!tailFloor.freeroam)
		{
			m_meshRenderer.material.SetFloat("_EndAlpha", tailFloor.opacity * startFloor.holdOpacity * extendAlp);
		}
		else
		{
			m_meshRenderer.material.SetFloat("_EndAlpha", tailFloor.freeroamFloors[0].opacity * startFloor.holdOpacity * extendAlp);
		}
		if (startFloor.showHoldTiming)
		{
			if (endCircle == null)
			{
				CreateEndCircle();
			}
			if (startFloor.holdCompletion >= 1f || startFloor.holdCompletion <= 0f)
			{
				endCircle.enabled = false;
				return;
			}
			perc = Mathf.Clamp01(startFloor.holdCompletion / (earlyAngle / totalAngleLength) - (totalAngleLength / earlyAngle - 1f));
			rad = DOVirtual.EasedValue(1.8f, 0.3f, perc, Ease.InQuad);
			alp = DOVirtual.EasedValue(0f, 1f, perc, Ease.OutQuad);
			DrawEndCircle(new Color(1f, 1f, 1f, alp), rad);
		}
	}

	private void CreateEndCircle()
	{
		endCircle = base.gameObject.AddComponent<LineRenderer>();
		endCircle.material = new Material(Shader.Find("Sprites/Default"));
		endCircle.positionCount = 33;
		endCircle.sortingLayerName = "Floor";
		endCircle.sortingOrder = startFloor.floorRenderer.renderer.sortingOrder + 5;
		endCircle.startWidth = 0.06f;
		endCircle.endWidth = 0.06f;
	}

	private void DrawEndCircle(Color col, float radius)
	{
		if (!RDC.hideTaroGimmicks)
		{
			endCircle.enabled = true;
			endCircle.startColor = col;
			endCircle.endColor = col;
			for (int i = 0; i < 33; i++)
			{
				ang = (float)i / 32f * (float)Math.PI * 2f;
				endCircle.SetPosition(i, tailFloor.transform.position + radius * Vector3.up * Mathf.Sin(ang) + radius * Vector3.right * Mathf.Cos(ang));
			}
		}
	}
}
