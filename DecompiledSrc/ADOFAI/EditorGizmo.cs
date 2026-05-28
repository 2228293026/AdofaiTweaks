using UnityEngine;

namespace ADOFAI;

public class EditorGizmo : ADOBase
{
	public Transform gizmoTransform;

	protected float startScale;

	protected virtual void Awake()
	{
		if (!(gizmoTransform == null))
		{
			startScale = gizmoTransform.transform.localScale.x;
		}
	}

	private void LateUpdate()
	{
		SetScale();
	}

	private void SetScale()
	{
		float num = ADOBase.editor.camera.orthographicSize / 5f;
		if (gizmoTransform != null)
		{
			gizmoTransform.localScale = Vector3.one * startScale * num;
		}
	}
}
