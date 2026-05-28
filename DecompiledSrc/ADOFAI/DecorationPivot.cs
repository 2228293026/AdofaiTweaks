namespace ADOFAI;

public class DecorationPivot : EditorGizmo
{
	public void UpdatePivotCrossImage(bool enable)
	{
		bool flag = ADOBase.editor.SelectionDecorationIsEmpty() || !ADOBase.editor.SelectionDecorationIsSingle();
		if (!flag)
		{
			scrDecoration decoration = scrDecorationManager.GetDecoration(ADOBase.editor.selectedDecorations[0]);
			if (decoration != null)
			{
				gizmoTransform.transform.position = decoration.pivotPosVec;
			}
		}
		gizmoTransform.gameObject.SetActive(enable && !flag);
	}
}
