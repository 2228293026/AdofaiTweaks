using System;
using System.Reflection;

namespace ByteSheep.Events;

[Serializable]
public class QuickEvent : QuickEventBase
{
	protected QuickAction DynamicMethodCalls;

	public void Invoke()
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls();
		}
		InvokePersistent();
	}

	public void AddListener(QuickAction listener)
	{
		DynamicMethodCalls = (QuickAction)Delegate.Combine(DynamicMethodCalls, listener);
	}

	public void RemoveListener(QuickAction listener)
	{
		DynamicMethodCalls = (QuickAction)Delegate.Remove(DynamicMethodCalls, listener);
	}

	public void RemoveAllListeners()
	{
		DynamicMethodCalls = null;
	}
}
[Serializable]
public class QuickEvent<T> : QuickEventBase
{
	protected QuickAction<T> DynamicMethodCalls;

	protected QuickAction<T> PersistentDynamicMethodCalls;

	public void Invoke(T arg0)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0);
		}
		if (PersistentDynamicMethodCalls != null)
		{
			PersistentDynamicMethodCalls(arg0);
		}
		InvokePersistent(arg0);
	}

	protected Type GetActionType()
	{
		return typeof(QuickAction<T>);
	}

	public void AddListener(QuickAction<T> listener)
	{
		DynamicMethodCalls = (QuickAction<T>)Delegate.Combine(DynamicMethodCalls, listener);
	}

	public void RemoveListener(QuickAction<T> listener)
	{
		DynamicMethodCalls = (QuickAction<T>)Delegate.Remove(DynamicMethodCalls, listener);
	}

	public void RemoveAllListeners()
	{
		DynamicMethodCalls = null;
	}

	protected void AddPersistentListener(QuickAction<T> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T>)Delegate.Combine(PersistentDynamicMethodCalls, listener);
	}

	protected void RemovePersistentListener(QuickAction<T> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T>)Delegate.Remove(PersistentDynamicMethodCalls, listener);
	}

	public void RemoveAllPersistentListeners()
	{
		PersistentDynamicMethodCalls = null;
	}

	public override void OnAfterDeserialize()
	{
		RemoveAllPersistentListeners();
		base.OnAfterDeserialize();
		for (int i = 0; i < persistentCalls.calls.Count; i++)
		{
			QuickPersistentCall quickPersistentCall = persistentCalls.calls[i];
			if (!quickPersistentCall.isCallEnabled || (object)quickPersistentCall.target == null || quickPersistentCall.memberName == "" || !quickPersistentCall.isDynamic)
			{
				continue;
			}
			if (quickPersistentCall.memberType == MemberTypes.Method)
			{
				MethodInfo method = quickPersistentCall.target.GetType().GetMethod(quickPersistentCall.memberName, new Type[1] { typeof(T) });
				if (method != null)
				{
					AddPersistentListener((QuickAction<T>)Delegate.CreateDelegate(GetActionType(), quickPersistentCall.target, method, throwOnBindFailure: false));
				}
			}
			else if (quickPersistentCall.memberType == MemberTypes.Property)
			{
				PropertyInfo property = quickPersistentCall.target.GetType().GetProperty(quickPersistentCall.memberName);
				if (property != null)
				{
					MethodInfo setMethod = property.GetSetMethod();
					if (setMethod != null)
					{
						AddPersistentListener((QuickAction<T>)Delegate.CreateDelegate(GetActionType(), quickPersistentCall.target, setMethod, throwOnBindFailure: false));
					}
				}
			}
			else if (quickPersistentCall.memberType == MemberTypes.Field)
			{
				quickPersistentCall.fieldInfo = quickPersistentCall.target.GetType().GetField(quickPersistentCall.memberName);
			}
		}
	}
}
[Serializable]
public class QuickEvent<T, U> : QuickEventBase
{
	protected QuickAction<T, U> DynamicMethodCalls;

	protected QuickAction<T, U> PersistentDynamicMethodCalls;

	public void Invoke(T arg0, U arg1)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1);
		}
		if (PersistentDynamicMethodCalls != null)
		{
			PersistentDynamicMethodCalls(arg0, arg1);
		}
		InvokePersistent();
	}

	public void AddListener(QuickAction<T, U> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U>)Delegate.Combine(DynamicMethodCalls, listener);
	}

	public void RemoveListener(QuickAction<T, U> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U>)Delegate.Remove(DynamicMethodCalls, listener);
	}

	public void RemoveAllListeners()
	{
		DynamicMethodCalls = null;
	}

	protected void AddPersistentListener(QuickAction<T, U> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U>)Delegate.Combine(PersistentDynamicMethodCalls, listener);
	}

	protected void RemovePersistentListener(QuickAction<T, U> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U>)Delegate.Remove(PersistentDynamicMethodCalls, listener);
	}

	public void RemoveAllPersistentListeners()
	{
		PersistentDynamicMethodCalls = null;
	}

	public override void OnAfterDeserialize()
	{
		RemoveAllPersistentListeners();
		base.OnAfterDeserialize();
	}
}
[Serializable]
public class QuickEvent<T, U, V> : QuickEventBase
{
	protected QuickAction<T, U, V> DynamicMethodCalls;

	protected QuickAction<T, U, V> PersistentDynamicMethodCalls;

	public void Invoke(T arg0, U arg1, V arg2)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1, arg2);
		}
		if (PersistentDynamicMethodCalls != null)
		{
			PersistentDynamicMethodCalls(arg0, arg1, arg2);
		}
		InvokePersistent();
	}

	public void AddListener(QuickAction<T, U, V> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U, V>)Delegate.Combine(DynamicMethodCalls, listener);
	}

	public void RemoveListener(QuickAction<T, U, V> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U, V>)Delegate.Remove(DynamicMethodCalls, listener);
	}

	public void RemoveAllListeners()
	{
		DynamicMethodCalls = null;
	}

	protected void AddPersistentListener(QuickAction<T, U, V> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U, V>)Delegate.Combine(PersistentDynamicMethodCalls, listener);
	}

	protected void RemovePersistentListener(QuickAction<T, U, V> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U, V>)Delegate.Remove(PersistentDynamicMethodCalls, listener);
	}

	public void RemoveAllPersistentListeners()
	{
		PersistentDynamicMethodCalls = null;
	}

	public override void OnAfterDeserialize()
	{
		RemoveAllPersistentListeners();
		base.OnAfterDeserialize();
	}
}
[Serializable]
public class QuickEvent<T, U, V, W> : QuickEventBase
{
	protected QuickAction<T, U, V, W> DynamicMethodCalls;

	protected QuickAction<T, U, V, W> PersistentDynamicMethodCalls;

	public void Invoke(T arg0, U arg1, V arg2, W arg3)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1, arg2, arg3);
		}
		if (PersistentDynamicMethodCalls != null)
		{
			PersistentDynamicMethodCalls(arg0, arg1, arg2, arg3);
		}
		InvokePersistent();
	}

	public void AddListener(QuickAction<T, U, V, W> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U, V, W>)Delegate.Combine(DynamicMethodCalls, listener);
	}

	public void RemoveListener(QuickAction<T, U, V, W> listener)
	{
		DynamicMethodCalls = (QuickAction<T, U, V, W>)Delegate.Remove(DynamicMethodCalls, listener);
	}

	public void RemoveAllListeners()
	{
		DynamicMethodCalls = null;
	}

	protected void AddPersistentListener(QuickAction<T, U, V, W> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U, V, W>)Delegate.Combine(PersistentDynamicMethodCalls, listener);
	}

	protected void RemovePersistentListener(QuickAction<T, U, V, W> listener)
	{
		PersistentDynamicMethodCalls = (QuickAction<T, U, V, W>)Delegate.Remove(PersistentDynamicMethodCalls, listener);
	}

	public void RemoveAllPersistentListeners()
	{
		PersistentDynamicMethodCalls = null;
	}

	public override void OnAfterDeserialize()
	{
		RemoveAllPersistentListeners();
		base.OnAfterDeserialize();
	}
}
