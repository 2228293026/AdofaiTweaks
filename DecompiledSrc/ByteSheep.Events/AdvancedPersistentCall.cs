using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public class AdvancedPersistentCall
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

	[FormerlySerializedAs("m_methodInfo")]
	public MethodInfo methodInfo;

	[FormerlySerializedAs("m_arguments")]
	public AdvancedArgumentCache[] arguments;

	[FormerlySerializedAs("m_argumentValues")]
	public object[] argumentValues;

	[FormerlySerializedAs("m_argumentTypes")]
	public Type[] argumentTypes;

	[FormerlySerializedAs("m_isDynamic")]
	public bool isDynamic;

	[FormerlySerializedAs("m_isCallEnabled")]
	public bool isCallEnabled;

	public QuickAction ZeroParamMethod = delegate
	{
	};

	public void Invoke()
	{
		if (isCallEnabled && !(target == null))
		{
			if (memberType == MemberTypes.Field && argumentValues.Length != 0 && fieldInfo != null)
			{
				fieldInfo.SetValue(target, argumentValues[0]);
			}
			else if (argumentValues.Length == 0)
			{
				ZeroParamMethod();
			}
			else if (methodInfo != null)
			{
				methodInfo.Invoke(target, argumentValues);
			}
		}
	}

	public void SetDynamicArguments(object[] dynamicArguments)
	{
		if (isCallEnabled && isDynamic)
		{
			argumentValues = dynamicArguments;
		}
	}
}
