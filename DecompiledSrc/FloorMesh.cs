using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FloorMesh : MonoBehaviour
{
	public class MeshCache
	{
		public Mesh mesh;

		public Vector2[] polygon;
	}

	private const float angle5 = 0.08726646f;

	private const float angle30 = (float)Math.PI / 6f;

	private const float angle45 = (float)Math.PI / 4f;

	private const float angle90 = (float)Math.PI / 2f;

	private const float angle120 = (float)Math.PI * 2f / 3f;

	private const float angle180 = (float)Math.PI;

	private const float angle250 = 4.363323f;

	private const float angle270 = 4.712389f;

	private const float angle360 = (float)Math.PI * 2f;

	private const float leftTurn = (float)Math.PI / 2f;

	private const float rightTurn = -(float)Math.PI / 2f;

	private const float angleEpsilon = 0.0001f;

	private const float rayEpsilon = 0.01f;

	private const int knittingLevel = int.MaxValue;

	private const float shadowWidth = 0.11f;

	private const float uTurnOffset = 2f / 3f;

	private const float defaultCurvaturePoints = 40f;

	private static readonly Color darkMagenta = new Color(0.8f, 0f, 0.8f, 1f);

	private static readonly Color darkCyan = new Color(0f, 0.6f, 0.6f, 1f);

	private static readonly Color darkGreen = new Color(0f, 0.7f, 0f, 1f);

	private static readonly Color darkGray = new Color(0.2f, 0.2f, 0.2f, 1f);

	private static readonly Color orange = new Color(1f, 0.5f, 0f, 1f);

	private static readonly Color lightBlue = new Color(0.3f, 0.6f, 1f, 1f);

	public static HashSet<FloorMesh> floorMeshesThatNeedUpdate = new HashSet<FloorMesh>();

	public static Dictionary<string, MeshCache> cache = new Dictionary<string, MeshCache>();

	private static List<List<Vector2>> polygons = new List<List<Vector2>>();

	private static List<List<int>> connections = new List<List<int>>();

	private static List<Vector2> mainPolygon0 = new List<Vector2>();

	private static List<int> mainPolygon0Conn = new List<int>();

	private static List<Vector2> mainPolygon1 = new List<Vector2>();

	private static List<int> mainPolygon1Conn = new List<int>();

	private static List<Vector2> mainPolygon2 = new List<Vector2>();

	private static List<int> mainPolygon2Conn = new List<int>();

	private static List<Vector2> mainPolygon3 = new List<Vector2>();

	private static List<Vector2> cwShadowPolygon0 = new List<Vector2>();

	private static List<int> cwShadowPolygonConn = new List<int>();

	private static List<Vector2> cwShadowPolygon1 = new List<Vector2>();

	private static List<Vector2> ccwShadowPolygon0 = new List<Vector2>();

	private static List<int> ccwShadowPolygonConn = new List<int>();

	private static List<Vector2> ccwShadowPolygon1 = new List<Vector2>();

	private static float diamondShadowOuterVertexDistance = 0f;

	private static List<Vector2> uvs = new List<Vector2>();

	private static List<Vector2> uvs2 = new List<Vector2>();

	private static List<int> meshIndices = new List<int>();

	[SerializeField]
	private float angle0;

	[SerializeField]
	private float angle1;

	[SerializeField]
	private float width;

	[SerializeField]
	private float length;

	[SerializeField]
	private int curvaturePoints;

	[SerializeField]
	private bool isSprite;

	[SerializeField]
	private bool isHexagon;

	[SerializeField]
	private bool useFInset2;

	[NonSerialized]
	public string cacheKey;

	public PolygonCollider2D polygonCollider;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private float shortAngle;

	private float cornerCenterRadius;

	private float cornerCenterApothem;

	private float curvatureSliceAngle;

	private float angleDifference;

	private float insetDistance0;

	private float insetDistance1;

	private float insetDistance2;

	private int cornerPointIndex;

	private bool pathOverlaps;

	private bool cornerRaysIntersectBeforeSpikes;

	private Vector2 origin;

	private Vector2 bisectorIntersection;

	private Vector2 cornerCircleCenter;

	public float _angle0
	{
		get
		{
			return angle0;
		}
		set
		{
			if (!Mathf.Approximately(value, angle0))
			{
				angle0 = value;
				meshChanged = true;
			}
		}
	}

	public float _angle1
	{
		get
		{
			return angle1;
		}
		set
		{
			if (!Mathf.Approximately(value, angle1))
			{
				angle1 = value;
				meshChanged = true;
			}
		}
	}

	public float _width
	{
		get
		{
			return width;
		}
		set
		{
			if (!Mathf.Approximately(value, width))
			{
				width = value;
				meshChanged = true;
			}
		}
	}

	public float _length
	{
		get
		{
			return length;
		}
		set
		{
			if (!Mathf.Approximately(value, length))
			{
				length = value;
				meshChanged = true;
			}
		}
	}

	public int _curvaturePoints
	{
		get
		{
			return curvaturePoints;
		}
		set
		{
			if (curvaturePoints != value)
			{
				curvaturePoints = value;
				meshChanged = true;
			}
		}
	}

	public bool _isSprite
	{
		get
		{
			return isSprite;
		}
		set
		{
			if (isSprite != value)
			{
				isSprite = value;
				meshChanged = true;
			}
		}
	}

	public bool _isHexagon
	{
		get
		{
			return isHexagon;
		}
		set
		{
			if (isHexagon != value)
			{
				isHexagon = value;
				meshChanged = true;
			}
		}
	}

	public bool _useFInset2
	{
		get
		{
			return useFInset2;
		}
		set
		{
			useFInset2 = value;
			meshChanged = true;
		}
	}

	public bool meshChanged
	{
		set
		{
			floorMeshesThatNeedUpdate.Add(this);
		}
	}

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshChanged = true;
	}

	public static void UpdateAllRequired()
	{
		foreach (FloorMesh item in floorMeshesThatNeedUpdate)
		{
			if ((bool)item)
			{
				item.UpdateMesh();
			}
		}
		floorMeshesThatNeedUpdate.Clear();
	}

	public void UpdateMesh()
	{
		cacheKey = (isSprite ? "sprite" : $"{angle0},{angle1},{width},{length},{curvaturePoints}");
		if (!cache.ContainsKey(cacheKey))
		{
			if (!_isSprite && !_isHexagon)
			{
				GetPositions(angle0 * ((float)Math.PI / 180f), angle1 * ((float)Math.PI / 180f), width, length, curvaturePoints);
			}
			GenerateMesh();
		}
		MeshCache meshCache = cache[cacheKey];
		meshFilter.mesh = meshCache.mesh;
	}

	public void GenerateCollider()
	{
		if (cacheKey != null && cache.TryGetValue(cacheKey, out var value))
		{
			polygonCollider.points = value.polygon;
		}
	}

	private void KnitTriangles(List<Vector2> polygon, List<int> connections, int insetPolygonCount, int offset, bool sp = false)
	{
		int count = polygon.Count;
		for (int i = 0; i < polygon.Count && i < connections.Count; i++)
		{
			int num = connections[i];
			if (num <= 10000)
			{
				meshIndices.Add(offset + i);
				meshIndices.Add(offset + count + num);
				meshIndices.Add(offset + (i + 1) % polygon.Count);
				int num2 = connections[(i + 1) % connections.Count];
				if (num2 != num && num2 >= 0)
				{
					meshIndices.Add(offset + (i + 1) % polygon.Count);
					meshIndices.Add(offset + count + num);
					meshIndices.Add(offset + count + (num + 1) % insetPolygonCount);
				}
			}
		}
	}

	public void GenerateMesh()
	{
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>();
		List<Color> list2 = new List<Color>();
		MeshCache meshCache = new MeshCache();
		uvs = new List<Vector2>();
		uvs2 = new List<Vector2>();
		polygons.Clear();
		List<List<Vector2>> list3 = new List<List<Vector2>>();
		if (isHexagon)
		{
			meshIndices.Clear();
			Vector2[] array = new Vector2[6];
			Vector2[] array2 = new Vector2[6];
			Vector2[] array3 = new Vector2[6];
			Vector2[] array4 = new Vector2[6];
			int[] array5 = new int[6];
			int[] array6 = new int[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = GenerateNthVertex(i, 0f);
				array2[i] = GenerateNthVertex(i, length);
				array5[i] = i;
				array3[i] = GenerateNthVertex(i, length);
				array4[i] = GenerateNthVertex(i, length + 0.11f);
				array6[i] = i;
			}
			KnitTriangles(new List<Vector2>(array2), new List<int>(array5), 6, 0);
			KnitTriangles(new List<Vector2>(array4), new List<int>(array6), 6, 12);
			meshCache.polygon = (Vector2[])array2.Clone();
			List<Vector2> list4 = new List<Vector2>();
			list4.AddRange(array2);
			list4.AddRange(array);
			list4.AddRange(array3);
			list4.AddRange(array4);
			Vector3[] array7 = new Vector3[24];
			for (int j = 0; j < list4.Count; j++)
			{
				array7[j] = new Vector3(list4[j].x, list4[j].y, 0f);
			}
			mesh.vertices = array7;
			MonoBehaviour.print("All Vertices:");
			for (int k = 0; k < list4.Count; k++)
			{
				MonoBehaviour.print(list4[k]);
			}
			Color[] array8 = new Color[24];
			Vector2[] array9 = new Vector2[24];
			Vector2[] array10 = new Vector2[24];
			mesh.SetIndices(meshIndices.ToArray(), MeshTopology.Triangles, 0);
			MonoBehaviour.print("Indices:");
			for (int l = 0; l < meshIndices.Count; l++)
			{
				MonoBehaviour.print(meshIndices[l]);
			}
			for (int m = 0; m < mesh.vertices.Length; m++)
			{
				array8[m] = Color.white;
				if (m < 6)
				{
					array9[m] = new Vector2(0.5f, 1f);
				}
				else if (m < 12)
				{
					array9[m] = new Vector2(0.5f, 0f);
				}
				else if (m < 18)
				{
					array9[m] = new Vector2(0.5f, 1f);
				}
				else if (m < 24)
				{
					array9[m] = new Vector2(0.5f, 2f);
				}
				array10[m] = mesh.vertices[m].xy();
			}
			mesh.uv = array9;
			mesh.uv2 = array10;
			mesh.colors = array8;
			MonoBehaviour.print("Vertices:");
			for (int n = 0; n < mesh.vertices.Length; n++)
			{
				MonoBehaviour.print(mesh.vertices[n]);
			}
			meshCache.mesh = mesh;
			cache[cacheKey] = meshCache;
			return;
		}
		if (isSprite)
		{
			meshCache.polygon = new Vector2[4]
			{
				new Vector2(0.5f, 0.5f),
				new Vector2(-0.5f, 0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2(0.5f, -0.5f)
			};
			mesh.vertices = new Vector3[4]
			{
				new Vector3(0.5f, 0.5f, 0f),
				new Vector3(-0.5f, 0.5f, 0f),
				new Vector3(-0.5f, -0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f)
			};
			mesh.SetIndices(new int[6] { 0, 2, 1, 0, 3, 2 }, MeshTopology.Triangles, 0);
			mesh.uv = new Vector2[4]
			{
				new Vector2(0.5f, 0.5f),
				new Vector2(-0.5f, 0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2(0.5f, -0.5f)
			};
			mesh.uv2 = new Vector2[4]
			{
				new Vector2(0.5f, 0.5f),
				new Vector2(-0.5f, 0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2(0.5f, -0.5f)
			};
			mesh.colors = new Color[4]
			{
				Color.white,
				Color.white,
				Color.white,
				Color.white
			};
			meshCache.mesh = mesh;
			cache[cacheKey] = meshCache;
			return;
		}
		polygons.Add(mainPolygon0, mainPolygon1, mainPolygon2, mainPolygon3);
		list3.Add(cwShadowPolygon0, cwShadowPolygon1, ccwShadowPolygon0, ccwShadowPolygon1);
		foreach (List<Vector2> polygon in polygons)
		{
			for (int num = 0; num < polygon.Count; num++)
			{
				list.Add(polygon[num]);
				list2.Add(Color.white);
			}
		}
		foreach (List<Vector2> item2 in list3)
		{
			for (int num2 = 0; num2 < item2.Count; num2++)
			{
				list.Add(item2[num2]);
				list2.Add(Color.white);
			}
		}
		meshIndices.Clear();
		if (mainPolygon0Conn.Count > 0)
		{
			KnitTriangles(mainPolygon0, mainPolygon0Conn, mainPolygon1.Count, 0);
		}
		if (mainPolygon1Conn.Count > 0)
		{
			KnitTriangles(mainPolygon1, mainPolygon1Conn, mainPolygon2.Count, mainPolygon0.Count);
			if (pathOverlaps && !cornerRaysIntersectBeforeSpikes)
			{
				int num3 = mainPolygon0.Count + mainPolygon1.Count;
				int num4 = num3 - 1;
				int num5 = num3 + mainPolygon2.Count - 1;
				meshIndices.Add(num4, num5, num3);
				meshIndices.Add(num4, num3, mainPolygon0.Count);
				meshIndices.Add(num4, num5 - 1, num5);
				meshIndices.Add(num4, num5 - 2, num5 - 1);
				meshIndices.Add(num4, num5 - 3, num5 - 2);
			}
		}
		if (mainPolygon2Conn.Count > 0)
		{
			KnitTriangles(mainPolygon2, mainPolygon2Conn, mainPolygon3.Count, mainPolygon0.Count + mainPolygon1.Count);
		}
		KnitTriangles(cwShadowPolygon0, cwShadowPolygonConn, cwShadowPolygon1.Count, mainPolygon0.Count + mainPolygon1.Count + mainPolygon2.Count + mainPolygon3.Count);
		KnitTriangles(ccwShadowPolygon0, ccwShadowPolygonConn, ccwShadowPolygon1.Count, mainPolygon0.Count + mainPolygon1.Count + mainPolygon2.Count + mainPolygon3.Count + cwShadowPolygon0.Count + cwShadowPolygon1.Count);
		mesh.vertices = list.ToArray();
		mesh.SetIndices(meshIndices.ToArray(), MeshTopology.Triangles, 0);
		mesh.colors = list2.ToArray();
		List<float> list5 = new List<float> { 0f, insetDistance0, insetDistance1, insetDistance2, 0f };
		float num6 = 1f;
		int num7 = 0;
		foreach (List<Vector2> polygon2 in polygons)
		{
			num6 -= list5[num7] / width;
			for (int num8 = 0; num8 < polygon2.Count; num8++)
			{
				uvs.Add(new Vector2(0.5f, num6));
				Vector2 vector = polygon2[num8];
				uvs2.Add(new Vector2(vector.x, vector.y));
			}
			num7++;
		}
		List<float> list6 = new List<float> { 2f, 1f, 2f, 1f };
		if (diamondShadowOuterVertexDistance > 0f)
		{
			list6[2] = 2f - diamondShadowOuterVertexDistance;
		}
		int num9 = 0;
		foreach (List<Vector2> item3 in list3)
		{
			num6 = list6[num9];
			for (int num10 = 0; num10 < item3.Count; num10++)
			{
				Vector2 item = new Vector2(0.5f, num6);
				if (diamondShadowOuterVertexDistance > 0f && num9 == 2 && (num10 == 0 || num10 == 2))
				{
					item = new Vector2(0.5f, 1f);
				}
				uvs.Add(item);
				Vector2 vector2 = item3[num10];
				uvs2.Add(new Vector2(vector2.x, vector2.y));
			}
			num9++;
		}
		mesh.uv = uvs.ToArray();
		mesh.uv2 = uvs2.ToArray();
		meshCache.mesh = mesh;
		meshCache.polygon = mainPolygon0.ToArray();
		cache[cacheKey] = meshCache;
		static Vector2 GenerateNthVertex(int num11, float length)
		{
			float f = (float)Math.PI / 2f + (float)Math.PI / 3f * (float)num11;
			return new Vector2(Mathf.Cos(f) * length, Mathf.Sin(f) * length);
		}
	}

	private void GetPositions(float angle0, float angle1, float width, float length, int curvaturePoints)
	{
		angle0 = ModAngle360(angle0);
		angle1 = ModAngle360(angle1);
		if (!(ModAngle360(angle1 - angle0) < (float)Math.PI))
		{
			float num = angle0;
			angle0 = angle1;
			angle1 = num;
		}
		insetDistance0 = 0f;
		insetDistance1 = 0f;
		insetDistance2 = 0f;
		cornerCenterApothem = 0f;
		width = Mathf.Max(0f, width);
		length = Mathf.Max(length, width);
		curvaturePoints = Math.Max(curvaturePoints, 3);
		shortAngle = SmallestAngleBetweenTwoAngles(angle1, angle0);
		bool zeroAngle = shortAngle < 0.0001f;
		bool piAngle = Mathf.Abs(shortAngle - (float)Math.PI) < 0.0001f;
		origin = Vector2.zero;
		if (Mathf.Abs(shortAngle - 0f) < 0.0001f)
		{
			origin = origin.Add(angle1, (0f - length * 0.3333333f) / 4f);
			length *= 2f / 3f;
		}
		Vector2 vector = origin.Add(angle0, length);
		float num2 = angle0 + (float)Math.PI / 2f;
		Vector2 startMore = vector.Add(num2, width);
		float angle2 = num2 + (float)Math.PI / 2f;
		Vector2 vector2 = startMore.Add(angle2, width * 0.01f);
		float num3 = angle0 + -(float)Math.PI / 2f;
		Vector2 startLess = vector.Add(num3, width);
		float angle3 = num3 + -(float)Math.PI / 2f;
		Vector2 end = startLess.Add(angle3, width * 0.01f);
		Vector2 vector3 = origin.Add(angle1, length);
		float num4 = angle1 + (float)Math.PI / 2f;
		Vector2 endMore = vector3.Add(num4, width);
		float angle4 = num4 + (float)Math.PI / 2f;
		Vector2 end2 = endMore.Add(angle4, width * 0.01f);
		float num5 = angle1 + -(float)Math.PI / 2f;
		Vector2 endLess = vector3.Add(num5, width);
		float angle5 = num5 + -(float)Math.PI / 2f;
		Vector2 vector4 = endLess.Add(angle5, width * 0.01f);
		Vector2 ccwIntersection = ((zeroAngle || piAngle) ? origin : GetLineIntersection(new Segment(startMore, vector2), new Segment(endLess, vector4)));
		Vector2 cwIntersection = ((zeroAngle || piAngle) ? origin : GetLineIntersection(new Segment(startLess, end), new Segment(endMore, end2)));
		float num6 = 0f;
		if (shortAngle < 0.08726646f)
		{
			num6 = 1f;
		}
		else if (shortAngle < (float)Math.PI / 6f)
		{
			float num7 = (float)Math.PI * 5f / 36f;
			num6 = Mathf.Lerp(1f, 0.83f, Mathf.Pow((shortAngle - 0.08726646f) / num7, 0.5f));
		}
		else if (shortAngle < (float)Math.PI / 4f)
		{
			float num8 = (float)Math.PI / 12f;
			num6 = Mathf.Lerp(0.83f, 0.77f, Mathf.Pow((shortAngle - (float)Math.PI / 6f) / num8, 1f));
		}
		else if (shortAngle < (float)Math.PI / 2f)
		{
			float num9 = (float)Math.PI / 4f;
			num6 = Mathf.Lerp(0.77f, 0.15f, Mathf.Pow((shortAngle - (float)Math.PI / 4f) / num9, 0.7f));
		}
		else if (shortAngle < (float)Math.PI * 2f / 3f)
		{
			float num10 = (float)Math.PI / 6f;
			num6 = Mathf.Lerp(0.15f, 0f, Mathf.Pow((shortAngle - (float)Math.PI / 2f) / num10, 0.5f));
		}
		if (num6 < 0.001f)
		{
			num6 = 0f;
		}
		if ((double)num6 < 0.001)
		{
			num6 = 0f;
		}
		angleDifference = ModAngle360(angle1 - angle0);
		Vector2 center = Vector2.Lerp(cwIntersection, origin, num6);
		Vector2 center2 = Vector2.Lerp(ccwIntersection, origin, num6);
		cornerCenterRadius = Mathf.Lerp(0f, width, num6);
		List<Vector2> cwCornerPoints = new List<Vector2>();
		List<Vector2> ccwCornerPoints = new List<Vector2>();
		Vector2[] array;
		if (angleDifference < 2.0942953f)
		{
			cornerCircleCenter = center;
			array = CreateCircleArc(center, ModAngle360(num4), ModAngle360(num3), cornerCenterRadius, curvaturePoints, out curvatureSliceAngle);
			foreach (Vector2 item in array)
			{
				cwCornerPoints.Add(item);
			}
		}
		else
		{
			cwCornerPoints.Add(cwIntersection);
		}
		if (angleDifference > 4.3634233f)
		{
			cornerCircleCenter = center2;
			array = CreateCircleArc(center2, ModAngle360(num2), ModAngle360(num5), cornerCenterRadius, curvaturePoints, out curvatureSliceAngle);
			foreach (Vector2 item2 in array)
			{
				ccwCornerPoints.Add(item2);
			}
		}
		else
		{
			ccwCornerPoints.Add(ccwIntersection);
		}
		Vector2 b = ((angleDifference < (float)Math.PI) ? ccwIntersection : cwIntersection);
		bool isPathOverlapping = Vector2.Distance(origin, startMore) < Vector2.Distance(origin, b);
		Vector2[] overlappingPoints = null;
		if (isPathOverlapping)
		{
			overlappingPoints = new Vector2[3]
			{
				GetLineIntersection(new Segment(startLess, startMore), new Segment(endLess, vector4)),
				GetLineIntersection(new Segment(startLess, startMore), new Segment(endLess, endMore)),
				GetLineIntersection(new Segment(endLess, endMore), new Segment(startMore, vector2))
			};
		}
		mainPolygon0.Clear();
		mainPolygon0Conn.Clear();
		mainPolygon1.Clear();
		mainPolygon1Conn.Clear();
		mainPolygon2.Clear();
		mainPolygon2Conn.Clear();
		mainPolygon3.Clear();
		cwShadowPolygon0.Clear();
		cwShadowPolygonConn.Clear();
		ccwShadowPolygon0.Clear();
		ccwShadowPolygonConn.Clear();
		cwShadowPolygon1.Clear();
		ccwShadowPolygon1.Clear();
		diamondShadowOuterVertexDistance = 0f;
		cornerPointIndex = GeneratePolygon(mainPolygon0, roundCorners: true);
		List<Vector2> list = mainPolygon0;
		pathOverlaps = isPathOverlapping && !zeroAngle && !piAngle;
		if (pathOverlaps)
		{
			Segment bisector = GetBisector(list[list.Count - 1], list[0], list[1]);
			Segment bisector2 = GetBisector(list[0], list[1], list[2]);
			cornerCenterApothem = cornerCenterRadius * Mathf.Cos(curvatureSliceAngle / 2f);
			bisectorIntersection = GetLineIntersection(bisector, bisector2);
			float num11 = Projection(new Segment(list[1], list[2]), bisectorIntersection);
			cornerRaysIntersectBeforeSpikes = cornerCenterApothem < num11;
			if (cornerRaysIntersectBeforeSpikes)
			{
				insetDistance0 = cornerCenterApothem;
				insetDistance1 = num11 - insetDistance0;
			}
			else
			{
				insetDistance0 = num11;
			}
		}
		else if (num6 > 0f)
		{
			insetDistance0 = cornerCenterRadius;
		}
		else
		{
			insetDistance0 = width;
		}
		bool flag = !zeroAngle && !pathOverlaps;
		if (flag)
		{
			list = new List<Vector2>();
			GeneratePolygon(list, roundCorners: false);
		}
		if (!zeroAngle)
		{
			mainPolygon1 = CreateInsetFromPolygon(list, insetDistance0);
		}
		else
		{
			Vector2 item3 = origin.Add(angle0, length - width);
			mainPolygon1 = new List<Vector2> { item3, origin };
			mainPolygon0Conn.Add(0);
			for (int j = 0; j < curvaturePoints; j++)
			{
				mainPolygon0Conn.Add(1);
			}
			mainPolygon0Conn.Add(0);
		}
		if (!zeroAngle)
		{
			if (flag && mainPolygon1.Count != mainPolygon0.Count)
			{
				_ = mainPolygon0.Count;
				_ = mainPolygon1.Count;
				for (int k = 0; k < mainPolygon0.Count; k++)
				{
					if (k == cornerPointIndex)
					{
						for (int l = 0; l < curvaturePoints; l++)
						{
							mainPolygon0Conn.Add(k);
						}
					}
					else
					{
						mainPolygon0Conn.Add(k);
					}
				}
				insetDistance1 = width - insetDistance0;
				if (insetDistance1 > 0f)
				{
					mainPolygon2 = CreateInsetFromPolygon(mainPolygon1, insetDistance1);
					for (int m = 0; m < mainPolygon2.Count; m++)
					{
						mainPolygon1Conn.Add(m);
					}
				}
			}
			else
			{
				for (int n = 0; n < mainPolygon0.Count; n++)
				{
					mainPolygon0Conn.Add(n);
				}
			}
			if (pathOverlaps)
			{
				if (cornerRaysIntersectBeforeSpikes)
				{
					int startIndex = 4;
					int count = curvaturePoints - 1;
					WeldConnectionsForVertices(mainPolygon0, mainPolygon0Conn, mainPolygon1, startIndex, count);
					mainPolygon1[3] = cornerCircleCenter;
					mainPolygon2 = CreateInsetFromPolygon(mainPolygon1, insetDistance1);
					for (int num12 = 0; num12 < mainPolygon2.Count; num12++)
					{
						mainPolygon1Conn.Add(num12);
					}
					startIndex = mainPolygon1.Count - 2;
					count = 4;
					WeldConnectionsForVertices(mainPolygon1, mainPolygon1Conn, mainPolygon2, startIndex, count);
					insetDistance2 = width - insetDistance1 - insetDistance0;
					mainPolygon2Conn.Add(default(int), default(int), default(int), default(int));
					mainPolygon3.Add(origin);
				}
				else
				{
					int startIndex2 = mainPolygon0.Count - 2;
					WeldConnectionsForVertices(mainPolygon0, mainPolygon0Conn, mainPolygon1, startIndex2, 4);
					Vector2 start = mainPolygon1[mainPolygon0Conn[cornerPointIndex]];
					Vector2 end3 = mainPolygon1[mainPolygon0Conn[cornerPointIndex + 1]];
					float num13 = Projection(new Segment(start, end3), cornerCircleCenter);
					insetDistance1 = cornerCenterApothem - insetDistance0;
					if (useFInset2)
					{
						insetDistance1 = num13;
					}
					mainPolygon2 = CreateInsetFromPolygon(mainPolygon1, insetDistance1);
					float num14 = length - insetDistance0 - insetDistance1;
					float num15 = width - insetDistance0 - insetDistance1;
					Vector2 vector5 = vector.normalized * num14;
					Vector2 vector6 = vector3.normalized * num14;
					Vector2 vector7 = vector5 + (startMore - vector).normalized * num15;
					Vector2 vector8 = vector6 + (endLess - vector3).normalized * num15;
					Vector2 end4 = vector7 + (vector2 - startMore).normalized * 0.01f;
					Vector2 end5 = vector8 + (vector4 - endLess).normalized * 0.01f;
					Vector2 lineIntersection = GetLineIntersection(new Segment(vector7, end4), new Segment(vector8, end5));
					mainPolygon2.RemoveAt(mainPolygon2.Count - 1);
					mainPolygon2.Add(vector7, lineIntersection, vector8);
					for (int num16 = 0; num16 < mainPolygon2.Count; num16++)
					{
						mainPolygon1Conn.Add(num16);
					}
					WeldConnectionsForVertices(mainPolygon1, mainPolygon1Conn, mainPolygon2, 3, curvaturePoints - 3);
					mainPolygon1Conn[mainPolygon1Conn.Count - 1] = -1;
					mainPolygon1Conn[mainPolygon1Conn.Count - 2] = -1;
					mainPolygon1Conn[mainPolygon1Conn.Count - 3] = -1;
					num14 = length - width;
					vector5 = vector.normalized * num14;
					vector6 = vector3.normalized * num14;
					Vector2 vector9 = Vector2.Lerp(mainPolygon2[2], mainPolygon2[mainPolygon2.Count - 2], 0.5f);
					mainPolygon3.Add(vector6, vector9, vector5, vector9);
					mainPolygon2Conn.Add(0, 1, 1, 1, 2, 2, 3, 0);
					insetDistance2 = width - insetDistance0 - insetDistance1;
				}
			}
		}
		if (piAngle)
		{
			Vector2 vector10 = (startMore - vector).normalized * 0.11f + startMore;
			Vector2 vector11 = (endLess - vector3).normalized * 0.11f + endLess;
			ccwShadowPolygon0.Add(vector10, vector11);
			ccwShadowPolygonConn.Add(0, 10001);
			ccwShadowPolygon1.Add(startMore, endLess);
			Vector2 vector12 = (endMore - vector3).normalized * 0.11f + endMore;
			Vector2 vector13 = (startLess - vector).normalized * 0.11f + startLess;
			cwShadowPolygon0.Add(vector12, vector13);
			cwShadowPolygonConn.Add(0, 10001);
			cwShadowPolygon1.Add(endMore, startLess);
			return;
		}
		if (!pathOverlaps && !zeroAngle)
		{
			Vector2 lineIntersection2 = GetLineIntersection(new Segment(vector3, endLess), new Segment(vector, startMore));
			if (Vector2.Distance(lineIntersection2, endLess) < 0.11f)
			{
				ccwShadowPolygon0.Add(startMore, lineIntersection2, endLess);
				ccwShadowPolygonConn.Add(default(int), default(int));
				ccwShadowPolygon1.Add(ccwIntersection);
				diamondShadowOuterVertexDistance = 1f - Vector2.Distance(ccwIntersection, lineIntersection2) / 0.11f;
			}
			else
			{
				Vector2 vector14 = (startMore - vector).normalized * 0.11f + startMore;
				Vector2 vector15 = (endLess - vector3).normalized * 0.11f + endLess;
				Vector2 lineIntersection3 = GetLineIntersection(new Segment(vector14, vector14 + (vector2 - startMore)), new Segment(vector15, vector15 + (vector4 - endLess)));
				ccwShadowPolygon0.Add(vector14, lineIntersection3, vector15);
				ccwShadowPolygonConn.Add(0, 1);
				ccwShadowPolygon1.Add(startMore, ccwIntersection, endLess);
			}
		}
		Vector2 item4 = (endMore - vector3).normalized * 0.11f + endMore;
		Vector2 item5 = (startLess - vector).normalized * 0.11f + startLess;
		cwShadowPolygon0.Add(item4);
		cwShadowPolygonConn.Add(0);
		bool flag2 = cwCornerPoints.Count > 1;
		float radius = (flag2 ? (0.11f + cornerCenterRadius) : 0.11f);
		Vector2[] array2 = CreateCircleArc(center, ModAngle360(num4), ModAngle360(num3), radius, curvaturePoints, out curvatureSliceAngle);
		int num17 = 1;
		array = array2;
		foreach (Vector2 item6 in array)
		{
			cwShadowPolygon0.Add(item6);
			cwShadowPolygonConn.Add(num17);
			if (flag2)
			{
				num17++;
			}
		}
		cwShadowPolygon0.Add(item5);
		cwShadowPolygon1.Add(endMore);
		for (int num18 = 0; num18 < cwCornerPoints.Count; num18++)
		{
			cwShadowPolygon1.Add(cwCornerPoints[num18]);
		}
		cwShadowPolygon1.Add(startLess);
		List<Vector2> CreateInsetFromPolygon(List<Vector2> polygon, float distance)
		{
			List<Vector2> list2 = new List<Vector2>();
			Segment[] array3 = new Segment[polygon.Count];
			for (int num19 = 0; num19 < polygon.Count; num19++)
			{
				Vector2 vector16 = polygon[num19];
				int index = (num19 + 1) % polygon.Count;
				Vector2 vector17 = polygon[index];
				Segment s = new Segment(vector16, vector17);
				Vector2 vector18 = GenerateNormal(s, distance);
				Vector2 vector19 = new Vector2((vector16.x + vector17.x) / 2f, (vector16.y + vector17.y) / 2f);
				new Segment(vector19, vector19 + vector18, Color.gray);
				Segment segment = new Segment(vector16 + vector18, vector17 + vector18, darkGray);
				array3[num19] = segment;
			}
			for (int num20 = 0; num20 < array3.Length; num20++)
			{
				int num21 = Mod(num20 - 1, array3.Length);
				int num22 = Mod(num20, array3.Length);
				Segment a = array3[num21];
				Segment b2 = array3[num22];
				Vector2 lineIntersection4 = GetLineIntersection(a, b2);
				list2.Add(lineIntersection4);
			}
			return list2;
		}
		int GeneratePolygon(List<Vector2> v, bool roundCorners)
		{
			List<Vector2> list2 = (roundCorners ? ccwCornerPoints : new List<Vector2> { ccwIntersection });
			List<Vector2> list3 = (roundCorners ? cwCornerPoints : new List<Vector2> { cwIntersection });
			int result = 0;
			if (zeroAngle)
			{
				v.Add(startMore);
				v.AddRange(list3);
				v.Add(endLess);
			}
			else if (piAngle)
			{
				v.Add(startMore, endLess, endMore, startLess);
			}
			else if (!isPathOverlapping)
			{
				v.Add(startMore);
				bool flag3 = list2.Count > list3.Count;
				if (flag3)
				{
					result = v.Count;
				}
				v.AddRange(list2);
				v.Add(endLess, endMore);
				if (!flag3)
				{
					result = v.Count;
				}
				v.AddRange(list3);
				v.Add(startLess);
			}
			else
			{
				v.Add(startMore, overlappingPoints[2], endMore);
				result = v.Count;
				v.AddRange(list3);
				v.Add(startLess, overlappingPoints[0], endLess, overlappingPoints[1]);
			}
			return result;
		}
	}

	private void PrintState()
	{
		int num = 0;
		string text = $"conns (mainPolygon: {mainPolygon0.Count}, conn: {mainPolygon0Conn.Count}):\n";
		foreach (Vector2 item in mainPolygon0)
		{
			_ = item;
			text += $"p {num} -> {mainPolygon0Conn[num]}\n";
			num++;
		}
		MonoBehaviour.print(text);
	}

	private void WeldConnectionsForVertices(List<Vector2> polygon, List<int> connections, List<Vector2> inset, int startIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			int polygonVertexIndex = (startIndex + i) % polygon.Count;
			WeldWithPreviousVertexConnection(polygon, connections, inset, polygonVertexIndex);
		}
	}

	private void WeldWithPreviousVertexConnection(List<Vector2> polygon, List<int> connections, List<Vector2> inset, int polygonVertexIndex)
	{
		int index = ((polygonVertexIndex == 0) ? (polygon.Count - 1) : (polygonVertexIndex - 1));
		int num = connections[polygonVertexIndex];
		int value = connections[index];
		connections[polygonVertexIndex] = value;
		inset.RemoveAt(num);
		for (int i = 0; i < connections.Count; i++)
		{
			if (connections[i] >= num)
			{
				connections[i]--;
			}
		}
	}

	private bool AnglesAreEqual(float a, float b)
	{
		return Mathf.Abs(a - b) < 0.0001f;
	}

	private float Float(int x)
	{
		return x;
	}

	private List<T> MakeListByRepeatingMember<T>(T member, int count)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < count; i++)
		{
			list.Add(member);
		}
		return list;
	}

	private Vector2[] CreateCircleArc(Vector2 center, float angleA, float angleB, float radius, int pointCount, out float sliceAngle)
	{
		if (angleA > angleB)
		{
			angleA -= (float)Math.PI * 2f;
		}
		Vector2[] array = new Vector2[pointCount];
		for (int i = 0; i < pointCount; i++)
		{
			float t = Float(i) / (float)(pointCount - 1);
			float angle = Mathf.Lerp(angleA, angleB, t);
			array[i] = center.Add(angle, radius);
		}
		sliceAngle = (angleB - angleA) / (float)(pointCount - 1);
		return array;
	}

	private float SmallestAngleBetweenTwoAngles(float angleA, float angleB)
	{
		float val = Mod(angleB - angleA, (float)Math.PI * 2f);
		float val2 = Mod(angleA - angleB, (float)Math.PI * 2f);
		return Math.Min(val, val2);
	}

	private float Mod(float a, float b)
	{
		return (a % b + b) % b;
	}

	private int Mod(int a, int b)
	{
		return (a % b + b) % b;
	}

	private float ModAngle360(float a)
	{
		return Mod(a, (float)Math.PI * 2f);
	}

	private static bool LinesIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		Vector2 vector = new Vector2(c.x - a.x, c.y - a.y);
		Vector2 vector2 = new Vector2(b.x - a.x, b.y - a.y);
		Vector2 vector3 = new Vector2(d.x - c.x, d.y - c.y);
		float num = vector.x * vector2.y - vector.y * vector2.x;
		float num2 = vector.x * vector3.y - vector.y * vector3.x;
		float num3 = vector2.x * vector3.y - vector2.y * vector3.x;
		if (num == 0f)
		{
			if (c.x - a.x < 0f == c.x - b.x < 0f)
			{
				return c.y - a.y < 0f != c.y - b.y < 0f;
			}
			return true;
		}
		if (num3 == 0f)
		{
			return false;
		}
		float num4 = 1f / num3;
		float num5 = num2 * num4;
		float num6 = num * num4;
		if (num5 >= 0f && num5 <= 1f && num6 >= 0f)
		{
			return num6 <= 1f;
		}
		return false;
	}

	private Vector2 GetLineIntersection(Segment a, Segment b)
	{
		Vector2 vector = new Vector2(a.start.x - a.end.x, b.start.x - b.end.x);
		Vector2 b2 = new Vector2(a.start.y - a.end.y, b.start.y - b.end.y);
		float num = Determinant(vector, b2);
		Vector2 a2 = new Vector2(Determinant(a.start, a.end), Determinant(b.start, b.end));
		return new Vector2(Determinant(a2, vector) / num, Determinant(a2, b2) / num);
	}

	private bool ccw(Vector2 A, Vector2 B, Vector2 C)
	{
		return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
	}

	private bool GetSegmentIntersect(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
	{
		if (ccw(A, C, D) != ccw(B, C, D))
		{
			return ccw(A, B, C) != ccw(A, B, D);
		}
		return false;
	}

	private Vector2[] QuadraticBezierCurve(Vector2 a, Vector2 b, Vector2 c, int pointCount)
	{
		Vector2[] array = new Vector2[pointCount];
		int num = pointCount - 1;
		for (int i = 0; i < pointCount; i++)
		{
			float t = Float(i) / (float)num;
			array[i] = QuadraticBezierPoint(a, b, c, t);
		}
		return array;
	}

	private Vector2 QuadraticBezierPoint(Vector2 a, Vector2 b, Vector2 c, float t)
	{
		float x = (1f - t) * (1f - t) * a.x + 2f * (1f - t) * t * b.x + t * t * c.x;
		float y = (1f - t) * (1f - t) * a.y + 2f * (1f - t) * t * b.y + t * t * c.y;
		return new Vector2(x, y);
	}

	private float Determinant(Vector2 a, Vector2 b)
	{
		return a.x * b.y - a.y * b.x;
	}

	private Vector2 GenerateNormal(Segment s, float distance)
	{
		Vector2 result = s.end - s.start;
		float x = result.x;
		float y = result.y;
		result.x = 0f - y;
		result.y = x;
		result.Normalize();
		result.x *= distance;
		result.y *= distance;
		return result;
	}

	private float Projection(Segment s, Vector2 p)
	{
		Vector2 lhs = p - s.start;
		Vector2 normalized = (s.end - s.start).normalized;
		float num = Vector2.Dot(lhs, normalized);
		return Mathf.Sqrt(lhs.magnitude * lhs.magnitude - num * num);
	}

	private Segment GetBisector(Vector2 a, Vector2 b, Vector2 c)
	{
		Vector2 vector = a - b;
		Vector2 vector2 = c - b;
		vector2.Normalize();
		vector.Normalize();
		float num = Mathf.Atan2(vector2.y, vector2.x);
		if (num < 0f)
		{
			num += (float)Math.PI * 2f;
		}
		float num2 = Mathf.Atan2(vector.y, vector.x);
		if (num2 < 0f)
		{
			num2 += (float)Math.PI * 2f;
		}
		float num3 = num - num2;
		float num4 = (((num3 > 0f && num3 < (float)Math.PI) || num3 < -(float)Math.PI) ? 1f : (-1f));
		Vector2 vector3 = ((vector + vector2) / 2f).normalized * num4;
		Color color = ((num4 > 0f) ? Color.green : Color.green);
		return new Segment(b, b + vector3 * 0.1f, color);
	}

	private float Atan2ToAngle(float y, float x)
	{
		float num = Mathf.Atan2(y, x);
		num = ((num > 0f) ? num : ((float)Math.PI * 2f + num));
		num /= (float)Math.PI * 2f;
		return 1f - Mathf.Abs(2f * num - 1f);
	}
}
