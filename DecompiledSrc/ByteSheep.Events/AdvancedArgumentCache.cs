using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class AdvancedArgumentCache
{
	public string typeName;

	[FormerlySerializedAs("m_supportedType")]
	public AdvancedSupportedTypes supportedType;

	[FormerlySerializedAs("m_objectArgument")]
	public UnityEngine.Object objectArgument;

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

	public string enumArgument;

	public object GetArgumentValue()
	{
		switch (supportedType)
		{
		case AdvancedSupportedTypes.Object:
			return objectArgument;
		case AdvancedSupportedTypes.String:
			return stringArgument;
		case AdvancedSupportedTypes.Int:
			return intArgument;
		case AdvancedSupportedTypes.Float:
			return floatArgument;
		case AdvancedSupportedTypes.Bool:
			return boolArgument;
		case AdvancedSupportedTypes.Color:
			return colorArgument;
		case AdvancedSupportedTypes.Vector2:
			return vector2Argument;
		case AdvancedSupportedTypes.Vector3:
			return vector3Argument;
		case AdvancedSupportedTypes.Enum:
		{
			Type type = Type.GetType(typeName);
			string[] names = Enum.GetNames(type);
			for (int i = 0; i < names.Length; i++)
			{
				if (enumArgument == names[i])
				{
					return Enum.GetValues(type).GetValue(i);
				}
			}
			return 0;
		}
		default:
			return null;
		}
	}

	public Type GetArgumentType()
	{
		switch (supportedType)
		{
		case AdvancedSupportedTypes.Object:
			return typeof(UnityEngine.Object);
		case AdvancedSupportedTypes.String:
		case AdvancedSupportedTypes.Enum:
			return typeof(string);
		case AdvancedSupportedTypes.Int:
			return typeof(int);
		case AdvancedSupportedTypes.Float:
			return typeof(float);
		case AdvancedSupportedTypes.Bool:
			return typeof(bool);
		case AdvancedSupportedTypes.Color:
			return typeof(Color);
		case AdvancedSupportedTypes.Vector2:
			return typeof(Vector2);
		case AdvancedSupportedTypes.Vector3:
			return typeof(Vector3);
		default:
			return null;
		}
	}

	public static object[] CombineArguments(AdvancedArgumentCache[] arguments)
	{
		object[] array = new object[arguments.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = arguments[i].GetArgumentValue();
		}
		return array;
	}

	public static Type[] CombineArgumentTypes(AdvancedArgumentCache[] arguments)
	{
		Type[] array = new Type[arguments.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = arguments[i].GetArgumentValue().GetType();
		}
		return array;
	}
}
