using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class QuickArgumentCache
{
	[FormerlySerializedAs("m_supportedType")]
	public QuickSupportedTypes supportedType;

	[FormerlySerializedAs("m_stringArgument")]
	public string stringArgument;

	[FormerlySerializedAs("m_intArgument")]
	public int intArgument;

	[FormerlySerializedAs("m_floatArgument")]
	public float floatArgument;

	[FormerlySerializedAs("m_boolArgument")]
	public bool boolArgument;

	[FormerlySerializedAs("m_colorArgument")]
	public Color colorArgument;

	[FormerlySerializedAs("m_vector2Argument")]
	public Vector2 vector2Argument;

	[FormerlySerializedAs("m_vector3Argument")]
	public Vector3 vector3Argument;

	[FormerlySerializedAs("m_objectArgument")]
	public UnityEngine.Object objectArgument;

	[FormerlySerializedAs("m_gameObjectArgument")]
	public GameObject gameObjectArgument;

	[FormerlySerializedAs("m_transformArgument")]
	public Transform transformArgument;

	public object GetArgumentValue()
	{
		return supportedType switch
		{
			QuickSupportedTypes.String => stringArgument, 
			QuickSupportedTypes.Int => intArgument, 
			QuickSupportedTypes.Float => floatArgument, 
			QuickSupportedTypes.Bool => boolArgument, 
			QuickSupportedTypes.Color => colorArgument, 
			QuickSupportedTypes.Vector2 => vector2Argument, 
			QuickSupportedTypes.Vector3 => vector3Argument, 
			QuickSupportedTypes.Object => objectArgument, 
			QuickSupportedTypes.GameObject => gameObjectArgument, 
			QuickSupportedTypes.Transform => transformArgument, 
			_ => null, 
		};
	}

	public Type GetArgumentType()
	{
		return supportedType switch
		{
			QuickSupportedTypes.String => typeof(string), 
			QuickSupportedTypes.Int => typeof(int), 
			QuickSupportedTypes.Float => typeof(float), 
			QuickSupportedTypes.Bool => typeof(bool), 
			QuickSupportedTypes.Color => typeof(Color), 
			QuickSupportedTypes.Vector2 => typeof(Vector2), 
			QuickSupportedTypes.Vector3 => typeof(Vector3), 
			QuickSupportedTypes.Object => typeof(UnityEngine.Object), 
			QuickSupportedTypes.GameObject => typeof(GameObject), 
			QuickSupportedTypes.Transform => typeof(Transform), 
			_ => null, 
		};
	}
}
