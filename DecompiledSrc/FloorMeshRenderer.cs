using UnityEngine;

public class FloorMeshRenderer : FloorRenderer
{
	public FloorMesh floorMesh;

	public PolygonCollider2D polygonCollider;

	public override Color color
	{
		get
		{
			if (material == null)
			{
				Debug.Log(base.gameObject.name + ": Floor material is null");
				return Color.magenta;
			}
			return material.GetColor(scrFloor.ShaderProperty_Color);
		}
		set
		{
			material.SetColor(scrFloor.ShaderProperty_Color, value);
			cachedColor = value;
		}
	}

	public override Sprite sprite
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public override void Awake()
	{
		base.Awake();
		floorMesh.polygonCollider = polygonCollider;
		renderer.sortingLayerName = "Floor";
	}

	public override void SetAngle(float entryAngle, float exitAngle)
	{
		float angle = (entryAngle + 0f) * 57.29578f;
		float angle2 = exitAngle * 57.29578f;
		floorMesh._angle0 = angle;
		floorMesh._angle1 = angle2;
	}
}
