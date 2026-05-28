using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class QuickPersistentCall
{
	[FormerlySerializedAs("m_genericMenuData")]
	public GenericMenuData genericMenuData;

	[FormerlySerializedAs("m_target")]
	public UnityEngine.Object target;

	[FormerlySerializedAs("m_memberName")]
	public string memberName;

	[FormerlySerializedAs("m_memberType")]
	public MemberTypes memberType;

	[FormerlySerializedAs("m_fieldInfo")]
	public FieldInfo fieldInfo;

	[FormerlySerializedAs("m_argument")]
	public QuickArgumentCache argument;

	[FormerlySerializedAs("m_argumentValue")]
	public object argumentValue;

	[FormerlySerializedAs("m_isDynamic")]
	public bool isDynamic;

	[FormerlySerializedAs("m_isCallEnabled")]
	public bool isCallEnabled;

	[FormerlySerializedAs("m_actionGroup")]
	[HideInInspector]
	public QuickActionGroup actionGroup;

	public void Invoke()
	{
		if (isCallEnabled && !(target == null))
		{
			if (memberType == MemberTypes.Field)
			{
				fieldInfo.SetValue(target, argumentValue);
			}
			else if (!isDynamic)
			{
				actionGroup.Invoke(argumentValue, argument.supportedType);
			}
		}
	}

	public void SetDynamicArgument(object dynamicArgument)
	{
		if (isCallEnabled && isDynamic)
		{
			argumentValue = dynamicArgument;
		}
	}
}
