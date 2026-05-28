using System;

namespace ByteSheep.Events;

[Serializable]
public class AdvancedEvent : AdvancedEventBase
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
public class AdvancedEvent<T> : AdvancedEventBase
{
	protected QuickAction<T> DynamicMethodCalls;

	public void Invoke(T arg0)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0);
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
}
[Serializable]
public class AdvancedEvent<T, U> : AdvancedEventBase
{
	protected QuickAction<T, U> DynamicMethodCalls;

	public void Invoke(T arg0, U arg1)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1);
		}
		InvokePersistent(arg0, arg1);
	}

	protected Type GetActionType()
	{
		return typeof(QuickAction<T, U>);
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
}
[Serializable]
public class AdvancedEvent<T, U, V> : AdvancedEventBase
{
	protected QuickAction<T, U, V> DynamicMethodCalls;

	public void Invoke(T arg0, U arg1, V arg2)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1, arg2);
		}
		InvokePersistent(arg0, arg1, arg2);
	}

	protected Type GetActionType()
	{
		return typeof(QuickAction<T, U, V>);
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
}
[Serializable]
public class AdvancedEvent<T, U, V, W> : AdvancedEventBase
{
	protected QuickAction<T, U, V, W> DynamicMethodCalls;

	public void Invoke(T arg0, U arg1, V arg2, W arg3)
	{
		if (DynamicMethodCalls != null)
		{
			DynamicMethodCalls(arg0, arg1, arg2, arg3);
		}
		InvokePersistent(arg0, arg1, arg2, arg3);
	}

	protected Type GetActionType()
	{
		return typeof(QuickAction<T, U, V, W>);
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
}
