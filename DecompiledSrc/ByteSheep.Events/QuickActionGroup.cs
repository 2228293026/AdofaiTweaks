using System;
using UnityEngine;

namespace ByteSheep.Events;

[Serializable]
public class QuickActionGroup
{
	public QuickAction QuickDelegate;

	public QuickAction<string> QuickStringDelegate;

	public QuickAction<int> QuickIntDelegate;

	public QuickAction<float> QuickFloatDelegate;

	public QuickAction<bool> QuickBoolDelegate;

	public QuickAction<Color> QuickColorDelegate;

	public QuickAction<Vector2> QuickVector2Delegate;

	public QuickAction<Vector3> QuickVector3Delegate;

	public QuickAction<UnityEngine.Object> QuickObjectDelegate;

	public QuickAction<GameObject> QuickGameObjectDelegate;

	public QuickAction<Transform> QuickTransformDelegate;

	public void SetDelegate(Delegate listener, QuickSupportedTypes type)
	{
		switch (type)
		{
		case QuickSupportedTypes.String:
			QuickStringDelegate = (QuickAction<string>)listener;
			break;
		case QuickSupportedTypes.Int:
			QuickIntDelegate = (QuickAction<int>)listener;
			break;
		case QuickSupportedTypes.Float:
			QuickFloatDelegate = (QuickAction<float>)listener;
			break;
		case QuickSupportedTypes.Bool:
			QuickBoolDelegate = (QuickAction<bool>)listener;
			break;
		case QuickSupportedTypes.Color:
			QuickColorDelegate = (QuickAction<Color>)listener;
			break;
		case QuickSupportedTypes.Vector2:
			QuickVector2Delegate = (QuickAction<Vector2>)listener;
			break;
		case QuickSupportedTypes.Vector3:
			QuickVector3Delegate = (QuickAction<Vector3>)listener;
			break;
		case QuickSupportedTypes.Object:
			QuickObjectDelegate = (QuickAction<UnityEngine.Object>)listener;
			break;
		case QuickSupportedTypes.GameObject:
			QuickGameObjectDelegate = (QuickAction<GameObject>)listener;
			break;
		case QuickSupportedTypes.Transform:
			QuickTransformDelegate = (QuickAction<Transform>)listener;
			break;
		default:
			QuickDelegate = (QuickAction)listener;
			break;
		}
	}

	public void Invoke(object argument, QuickSupportedTypes type)
	{
		switch (type)
		{
		case QuickSupportedTypes.String:
			if (QuickStringDelegate != null)
			{
				QuickStringDelegate(argument as string);
			}
			break;
		case QuickSupportedTypes.Int:
			if (QuickIntDelegate != null)
			{
				QuickIntDelegate((int)argument);
			}
			break;
		case QuickSupportedTypes.Float:
			if (QuickFloatDelegate != null)
			{
				QuickFloatDelegate((float)argument);
			}
			break;
		case QuickSupportedTypes.Bool:
			if (QuickBoolDelegate != null)
			{
				QuickBoolDelegate((bool)argument);
			}
			break;
		case QuickSupportedTypes.Color:
			if (QuickColorDelegate != null)
			{
				QuickColorDelegate((Color)argument);
			}
			break;
		case QuickSupportedTypes.Vector2:
			if (QuickVector2Delegate != null)
			{
				QuickVector2Delegate((Vector2)argument);
			}
			break;
		case QuickSupportedTypes.Vector3:
			if (QuickVector3Delegate != null)
			{
				QuickVector3Delegate((Vector3)argument);
			}
			break;
		case QuickSupportedTypes.Object:
			if (QuickObjectDelegate != null)
			{
				QuickObjectDelegate(argument as UnityEngine.Object);
			}
			break;
		case QuickSupportedTypes.GameObject:
			if (QuickGameObjectDelegate != null)
			{
				QuickGameObjectDelegate(argument as GameObject);
			}
			break;
		case QuickSupportedTypes.Transform:
			if (QuickTransformDelegate != null)
			{
				QuickTransformDelegate(argument as Transform);
			}
			break;
		default:
			if (QuickDelegate != null)
			{
				QuickDelegate();
			}
			break;
		}
	}
}
