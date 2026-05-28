using System;
using ADOFAI;
using UnityEngine;

public class PanelTransformGizmoHolder : TransformGizmoHolder
{
	[Header("General")]
	public string saveName;

	public int minSize = 340;

	public int maxSize = 750;

	[Header("Snap")]
	public RectTransform snapTarget;

	public int snapRange = 20;

	private RectTransform rect;

	private InspectorPanel panel;

	private RectTransform panelParentRect;

	private bool dragging;

	private Vector2 startPanelSize;

	private string xKey => "panelSize_" + saveName + "_x";

	private string yKey => "panelSize_" + saveName + "_y";

	private float Validate(float v)
	{
		float num = Mathf.Clamp(v, minSize, maxSize);
		if (snapTarget != null)
		{
			float x = snapTarget.sizeDelta.x;
			if (Mathf.Abs(x - num) <= (float)snapRange)
			{
				num = x;
			}
		}
		return num;
	}

	private new void Awake()
	{
		if (string.IsNullOrEmpty(saveName))
		{
			throw new Exception("PanelTransformGizmoHolder saveName is empty! put name for savefile in unity inspector.");
		}
		base.Awake();
		rect = GetComponent<RectTransform>();
		panel = GetComponentInParent<InspectorPanel>();
		panelParentRect = panel.rect.parent.GetComponent<RectTransform>();
		float v = Persistence.generalPrefs.GetFloat(xKey);
		panel.rect.SizeDeltaX(Validate(v));
	}

	private void Update()
	{
		if (ADOBase.isEditingLevel && base.transform.localScale != Vector3.one)
		{
			Debug.LogError("localScale is different than one! " + base.transform.localScale);
			base.transform.localScale = Vector3.one;
		}
	}

	protected override void LateUpdate()
	{
		if (!ADOBase.isEditingLevel)
		{
			if (!dragging)
			{
				return;
			}
			ADOBase.editor.draggingGizmo = null;
			ADOBase.editor.lastHoveredGizmo = null;
			DragEnd();
		}
		UpdateGizmosTransform();
		UpdateGizmosVisibility();
	}

	public override void DragStart(TransformGizmo handle)
	{
		dragging = true;
		startPanelSize = panel.rect.sizeDelta;
	}

	public override void DragEnd()
	{
		dragging = false;
		if (!string.IsNullOrEmpty(saveName))
		{
			Persistence.generalPrefs.SetFloat(xKey, panel.rect.sizeDelta.x);
		}
	}

	public override void Drag(Vector2 mouseTranslation, Vector2 mouseDelta)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(panel.rect, mouseDelta, null, out var localPoint);
		if (Mathf.Approximately(panel.rect.anchorMin.x, 1f))
		{
			localPoint.x = 0f - (localPoint.x + panelParentRect.rect.width);
		}
		panel.rect.SizeDeltaX(Validate(startPanelSize.x + localPoint.x));
	}

	private void UpdateGizmosTransform()
	{
		Handle handle = handles[0];
		Vector3 mousePosition = Input.mousePosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(handle.gizmoRect, mousePosition, null, out var localPoint);
		handle.imageRect.anchoredPosition = handle.imageRect.anchoredPosition.WithY(localPoint.y);
		if (!dragging)
		{
			HandleAnimation(handle);
		}
		if ((UnityEngine.Object)(object)handle.collider != null)
		{
			handle.collider.size = handle.collider.size.WithY(rect.rect.height);
		}
	}
}
