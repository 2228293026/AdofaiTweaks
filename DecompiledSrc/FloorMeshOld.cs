using UnityEngine;

public class FloorMeshOld : MonoBehaviour
{
	[Header("GameObjects")]
	public Transform circle;

	public Transform start;

	public Transform end;

	public MeshRenderer[] renderers;

	[Header("Variables")]
	public float startAngle;

	public float endAngle;

	public float border;

	public float width;

	public float length;

	public float height = 1f;

	public int index;

	public Material material;

	private void Update()
	{
		UpdateMesh();
	}

	public void UpdateMesh()
	{
		start.localEulerAngles = Vector3.forward * startAngle;
		end.localEulerAngles = Vector3.forward * endAngle;
		ScaleXY(start, length / 2f - border * 0.5f, width);
		ScaleXY(end, length / 2f - border * 0.5f, width);
		ScaleXZ(circle, width, width);
		ScaleXZ(base.transform, 1f, height);
		UpdateMaterial();
	}

	public void UpdateMaterial()
	{
		for (int i = 0; i < 3; i++)
		{
			renderers[i].material = material;
			renderers[i].sortingOrder = 1000 - index * 2 + 1;
		}
	}

	public void ScaleXY(Transform t, float x, float y)
	{
		Vector3 localScale = t.localScale;
		localScale.x = x;
		localScale.y = y;
		t.localScale = localScale;
	}

	public void ScaleXZ(Transform t, float x, float z)
	{
		Vector3 localScale = t.localScale;
		localScale.x = x;
		localScale.z = z;
		t.localScale = localScale;
	}
}
