using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace ByteSheep.Events;

[Serializable]
public abstract class AdvancedEventBase : ISerializationCallbackReceiver
{
	[SerializeField]
	[FormerlySerializedAs("m_inspectorListHeight")]
	private float inspectorListHeight = 40f;

	[FormerlySerializedAs("m_persistentCalls")]
	public AdvancedPersistentCallGroup persistentCalls = new AdvancedPersistentCallGroup();

	[FormerlySerializedAs("m_dynamicArguments")]
	private DynamicArguments dynamicArguments = new DynamicArguments();

	protected void InvokePersistent()
	{
		for (int i = 0; i < persistentCalls.calls.Count; i++)
		{
			persistentCalls.calls[i].Invoke();
		}
	}

	protected void InvokePersistent(object arg0, object arg1 = null, object arg2 = null, object arg3 = null)
	{
		for (int i = 0; i < persistentCalls.calls.Count; i++)
		{
			persistentCalls.calls[i].SetDynamicArguments(dynamicArguments.UpdateDynamicArguments(arg0, arg1, arg2, arg3));
			persistentCalls.calls[i].Invoke();
		}
	}

	public int GetPersistentEventCount()
	{
		return persistentCalls.calls.Count;
	}

	public string GetPersistentMemberName(int index)
	{
		if (GetPersistentEventCount() <= 0)
		{
			return null;
		}
		return persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].memberName;
	}

	public UnityEngine.Object GetPersistentTarget(int index)
	{
		if (GetPersistentEventCount() <= 0)
		{
			return null;
		}
		return persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].target;
	}

	public void SetPersistentListenerState(int index, bool enabled)
	{
		if (GetPersistentEventCount() > 0)
		{
			persistentCalls.calls[Mathf.Clamp(index, 0, Mathf.Max(0, GetPersistentEventCount() - 1))].isCallEnabled = enabled;
			if (enabled)
			{
				OnAfterDeserialize();
			}
		}
	}

	public virtual void OnBeforeSerialize()
	{
	}

	public virtual void OnAfterDeserialize()
	{
		for (int i = 0; i < persistentCalls.calls.Count; i++)
		{
			AdvancedPersistentCall advancedPersistentCall = persistentCalls.calls[i];
			if (!advancedPersistentCall.isCallEnabled || (object)advancedPersistentCall.target == null || advancedPersistentCall.memberName == "")
			{
				continue;
			}
			advancedPersistentCall.argumentValues = AdvancedArgumentCache.CombineArguments(advancedPersistentCall.arguments);
			advancedPersistentCall.argumentTypes = AdvancedArgumentCache.CombineArgumentTypes(advancedPersistentCall.arguments);
			if (advancedPersistentCall.memberType == MemberTypes.Method)
			{
				advancedPersistentCall.methodInfo = advancedPersistentCall.target.GetType().GetMethod(advancedPersistentCall.memberName, advancedPersistentCall.argumentTypes);
				if (advancedPersistentCall.methodInfo != null && advancedPersistentCall.argumentTypes.Length == 0)
				{
					advancedPersistentCall.ZeroParamMethod = (QuickAction)Delegate.CreateDelegate(typeof(QuickAction), advancedPersistentCall.target, advancedPersistentCall.methodInfo, throwOnBindFailure: true);
				}
			}
			else if (advancedPersistentCall.memberType == MemberTypes.Property)
			{
				PropertyInfo property = advancedPersistentCall.target.GetType().GetProperty(advancedPersistentCall.memberName);
				if (property != null)
				{
					advancedPersistentCall.methodInfo = property.GetSetMethod();
				}
			}
			else if (advancedPersistentCall.memberType == MemberTypes.Field)
			{
				advancedPersistentCall.fieldInfo = advancedPersistentCall.target.GetType().GetField(advancedPersistentCall.memberName);
			}
		}
	}
}
