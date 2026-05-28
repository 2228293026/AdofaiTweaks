using System;
using UnityEngine;

namespace MonsterLove.StateMachine;

[RequireComponent(typeof(StateEngine))]
public class StateBehaviour : ADOBase
{
	private StateEngine _stateMachine;

	public StateEngine stateMachine
	{
		get
		{
			if (_stateMachine == null)
			{
				_stateMachine = GetComponent<StateEngine>();
			}
			if (_stateMachine == null)
			{
				throw new Exception("Please make sure StateEngine is also present on any StateBehaviour objects");
			}
			return _stateMachine;
		}
	}

	public States state => (States)(object)stateMachine.GetState();

	protected void Initialize<T>()
	{
		stateMachine.Initialize<T>(this);
	}

	public virtual void ChangeState(Enum newState)
	{
		stateMachine.ChangeState(newState);
	}

	protected virtual void ChangeState(Enum newState, StateTransition transition)
	{
		stateMachine.ChangeState(newState, transition);
	}
}
