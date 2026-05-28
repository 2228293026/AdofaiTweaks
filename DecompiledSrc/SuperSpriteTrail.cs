using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Effects/Super Sprite Trail", 0)]
public class SuperSpriteTrail : MonoBehaviour
{
	public enum OnTopDirection
	{
		None,
		Left,
		Right,
		Up,
		Down,
		Forwards,
		Back
	}

	public enum UpdateCycle
	{
		Update,
		FixedUpdate,
		LateUpdate
	}

	[Tooltip("Should new trails be generated, or not? On-screen trails will still linger if set to false.")]
	public bool isActive = true;

	public SpriteRenderer spriteRenderer;

	[Tooltip("How many frames a trail will last for.")]
	public int trailLength = 6;

	[Tooltip("This gradient determines the color of your trail!")]
	public Gradient colorOverTime;

	private Transform t;

	private Transform trailParent;

	private Vector3 lastPos;

	private Vector3 lastRot;

	private Vector3 lastScale;

	private Sprite lastSprite;

	private Color32 lastColor;

	private bool lastFlipX;

	private bool lastFlipY;

	private SpriteDrawMode lastDrawMode;

	private List<GameObject> trailObjects = new List<GameObject>();

	[Tooltip("If true, sprite trail will use the z axis to animate instead of Order In Layer. You probably only want to do this with an orthographic camera in a straight-on perpective.")]
	public bool trailOnZAxis;

	[Tooltip("When trailing on the Z axis, how far sprites will be pushed per frame.")]
	public float zAxisRate = 0.0001f;

	public Material trailMaterial;

	[Tooltip("If true, if the renderer's sprite changes or flips, a trail will happen even when it doesn't move.")]
	public bool animateOnSpriteChange;

	[Tooltip("If true, the sprite renderer will ONLY make new trails when the sprite changes.")]
	public bool animateOnlyOnSpriteChange;

	[Tooltip("If greater than 0, the Sprite Renderer must move this far in order for a trail to render.")]
	public float distanceToActivate;

	[Tooltip("If true, 'Distance To Activate' becomes 'Velocity To Activate'.")]
	public bool requireThisVelocity;

	[Tooltip("What sorting layer the trail will be on. If left blank, it will be the same as the Sprite Renderer's. If the name that's not defined is used, it'll put the trail on the default layer.")]
	public string sortingLayerName = "";

	[Tooltip("If player moves in this direction, sprites will draw on top of them instead of behind.")]
	public OnTopDirection awayFromCamera = OnTopDirection.Back;

	[Tooltip("Decide what cycle the trail will update on. Physics-based movement should use FixedUpdate")]
	public UpdateCycle updateCycle;

	[Tooltip("Additional offset for the trail. Will force an update every frame.")]
	public Vector3 velocity;

	[Tooltip("Trail acceleration, to be added for each segment of the trail.")]
	public Vector3 acceleration;

	[Tooltip("If true, the character will still have to meet other circumstances for a trail to be drawn.")]
	public bool requireChange;

	[Tooltip("If true, you probably want to use LateUpdate as your update cycle, or make sure that this has a later script excecution order than your movement script.")]
	public bool stayLockedToSprite;

	[Tooltip("For different script excecution orders. This should be true if this script gets run AFTER your movement script, and false if otherwise/undefined.")]
	public bool useLastPosition = true;

	[Tooltip("Should the gradient color blend with the sprite's color?")]
	public bool blendWithSpriteColor;

	public int TrailLength
	{
		get
		{
			return trailLength;
		}
		set
		{
			trailLength = value;
			PoolObjects();
		}
	}

	private void Awake()
	{
		t = base.transform;
		if (spriteRenderer == null && GetComponent<SpriteRenderer>() != null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		if (trailMaterial == null && spriteRenderer != null)
		{
			trailMaterial = spriteRenderer.sharedMaterial;
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			PoolObjects();
		}
		RememberTransform();
	}

	private void OnValidate()
	{
		if (trailLength < 0)
		{
			trailLength = 0;
		}
		if (distanceToActivate < 0f)
		{
			distanceToActivate = 0f;
		}
		if (Application.isPlaying && t != null)
		{
			PoolObjects();
		}
	}

	public void Clear()
	{
		int i = 0;
		for (int count = trailObjects.Count; i < count; i++)
		{
			trailObjects[i].SetActive(value: false);
		}
	}

	public void PoolObjects()
	{
		if (trailParent == null)
		{
			trailParent = new GameObject().transform;
			trailParent.name = t.name + " Sprite Trail";
		}
		for (int i = trailLength; i < trailObjects.Count; i++)
		{
			GameObject gameObject = trailObjects[i];
			trailObjects.Remove(gameObject);
			Object.Destroy(gameObject);
		}
		int j = 0;
		for (int num = trailLength; j < num; j++)
		{
			SpriteRenderer spriteRenderer;
			if (j >= trailObjects.Count)
			{
				GameObject gameObject2 = new GameObject();
				spriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
				gameObject2.transform.name = "Sprite Trail";
				gameObject2.transform.parent = trailParent;
				gameObject2.transform.position = t.position;
				gameObject2.transform.rotation = Quaternion.Euler(t.eulerAngles);
				gameObject2.transform.localScale = t.localScale;
				spriteRenderer.sprite = this.spriteRenderer.sprite;
				spriteRenderer.flipX = this.spriteRenderer.flipX;
				spriteRenderer.flipY = this.spriteRenderer.flipY;
				spriteRenderer.drawMode = this.spriteRenderer.drawMode;
				spriteRenderer.sortingOrder = this.spriteRenderer.sortingOrder;
				gameObject2.SetActive(value: false);
				trailObjects.Add(gameObject2);
			}
			else
			{
				GameObject gameObject2 = trailObjects[j];
				spriteRenderer = gameObject2.GetComponent<SpriteRenderer>();
			}
			spriteRenderer.color = colorOverTime.Evaluate((float)j / (float)num);
			if (blendWithSpriteColor)
			{
				spriteRenderer.color *= this.spriteRenderer.color;
			}
			spriteRenderer.sharedMaterial = trailMaterial;
			if (sortingLayerName == "")
			{
				spriteRenderer.sortingLayerName = this.spriteRenderer.sortingLayerName;
			}
			else
			{
				spriteRenderer.sortingLayerName = sortingLayerName;
			}
		}
	}

	private void RememberTransform()
	{
		lastPos = t.position;
		lastRot = t.eulerAngles;
		lastScale = t.localScale;
		lastSprite = spriteRenderer.sprite;
		lastColor = spriteRenderer.color;
		lastFlipX = spriteRenderer.flipX;
		lastFlipY = spriteRenderer.flipY;
		lastDrawMode = spriteRenderer.drawMode;
	}

	private bool ShouldMakeNew()
	{
		if (!requireChange && (velocity != Vector3.zero || acceleration != Vector3.zero))
		{
			return true;
		}
		if (animateOnSpriteChange && animateOnlyOnSpriteChange)
		{
			if (spriteRenderer.sprite != lastSprite || spriteRenderer.flipX != lastFlipX || spriteRenderer.flipY != lastFlipY)
			{
				return true;
			}
			return false;
		}
		if ((animateOnSpriteChange && (spriteRenderer.sprite != lastSprite || spriteRenderer.flipX != lastFlipX || spriteRenderer.flipY != lastFlipY)) || (lastPos != t.position && Vector3.Distance(t.position, lastPos) >= distanceToActivate) || lastRot != t.eulerAngles || lastScale != t.localScale || lastColor != spriteRenderer.color)
		{
			return true;
		}
		return false;
	}

	private bool MovedAwayFromCamera()
	{
		return awayFromCamera switch
		{
			OnTopDirection.Right => t.position.x - lastPos.x > 0f, 
			OnTopDirection.Left => t.position.x - lastPos.x < 0f, 
			OnTopDirection.Up => t.position.y - lastPos.y > 0f, 
			OnTopDirection.Down => t.position.y - lastPos.y < 0f, 
			OnTopDirection.Back => t.position.z - lastPos.z > 0f, 
			OnTopDirection.Forwards => t.position.z - lastPos.z < 0f, 
			_ => false, 
		};
	}

	private void Update()
	{
		if (updateCycle == UpdateCycle.Update)
		{
			MyUpdate();
		}
	}

	private void LateUpdate()
	{
		if (updateCycle == UpdateCycle.LateUpdate)
		{
			MyUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (updateCycle == UpdateCycle.FixedUpdate)
		{
			MyUpdate();
		}
	}

	private void MyUpdate()
	{
		if (!Application.isPlaying || trailObjects.Count == 0)
		{
			return;
		}
		int count = trailObjects.Count;
		for (int num = count - 1; num >= 1; num--)
		{
			if (!stayLockedToSprite)
			{
				trailObjects[num].transform.position = trailObjects[num - 1].transform.position + velocity + acceleration * (num + 1);
			}
			else
			{
				Vector3 vector = (useLastPosition ? lastPos : t.position);
				trailObjects[num].transform.position = vector + velocity * (num + 1);
			}
			trailObjects[num].transform.rotation = trailObjects[num - 1].transform.rotation;
			trailObjects[num].transform.localScale = trailObjects[num - 1].transform.localScale;
			SpriteRenderer component = trailObjects[num].GetComponent<SpriteRenderer>();
			SpriteRenderer component2 = trailObjects[num - 1].GetComponent<SpriteRenderer>();
			if (trailOnZAxis)
			{
				if (trailObjects[0].transform.position.z > t.position.z)
				{
					trailObjects[num].transform.position = trailObjects[num - 1].transform.position + new Vector3(0f, 0f, zAxisRate);
				}
				else
				{
					trailObjects[num].transform.position = trailObjects[num - 1].transform.position - new Vector3(0f, 0f, zAxisRate);
				}
			}
			else if (trailObjects[0].GetComponent<SpriteRenderer>().sortingOrder > spriteRenderer.sortingOrder)
			{
				component.sortingOrder = component2.sortingOrder + 1;
			}
			else
			{
				component.sortingOrder = component2.sortingOrder - 1;
			}
			component.sprite = component2.sprite;
			component.flipX = component2.flipX;
			component.flipY = component2.flipY;
			component.drawMode = component2.drawMode;
			component.color = colorOverTime.Evaluate((float)num / (float)count);
			if (blendWithSpriteColor)
			{
				component.color *= component2.color;
			}
			trailObjects[num].SetActive(trailObjects[num - 1].activeSelf);
		}
		if (isActive && trailLength > 0 && ShouldMakeNew())
		{
			Vector3 vector2 = (useLastPosition ? lastPos : t.position);
			Vector3 euler = (useLastPosition ? lastRot : t.eulerAngles);
			Vector3 localScale = (useLastPosition ? lastScale : t.localScale);
			trailObjects[0].transform.position = vector2 + velocity + acceleration;
			trailObjects[0].transform.rotation = Quaternion.Euler(euler);
			trailObjects[0].transform.localScale = localScale;
			SpriteRenderer component3 = trailObjects[0].GetComponent<SpriteRenderer>();
			component3.sortingOrder = spriteRenderer.sortingOrder;
			if (MovedAwayFromCamera())
			{
				if (trailOnZAxis)
				{
					trailObjects[0].transform.position -= new Vector3(0f, 0f, zAxisRate);
				}
				else
				{
					component3.sortingOrder++;
				}
			}
			else if (trailOnZAxis)
			{
				trailObjects[0].transform.position += new Vector3(0f, 0f, zAxisRate);
			}
			else
			{
				component3.sortingOrder--;
			}
			component3.sprite = spriteRenderer.sprite;
			component3.flipX = spriteRenderer.flipX;
			component3.flipY = spriteRenderer.flipY;
			component3.drawMode = spriteRenderer.drawMode;
			component3.color = colorOverTime.Evaluate(0f);
			if (blendWithSpriteColor)
			{
				component3.color *= spriteRenderer.color;
			}
			trailObjects[0].SetActive(value: true);
			RememberTransform();
		}
		else
		{
			trailObjects[0].SetActive(value: false);
		}
		if ((animateOnSpriteChange && animateOnlyOnSpriteChange) || requireThisVelocity)
		{
			RememberTransform();
		}
	}
}
