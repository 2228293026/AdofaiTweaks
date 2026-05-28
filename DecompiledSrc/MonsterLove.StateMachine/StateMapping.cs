using System;
using System.Collections;

namespace MonsterLove.StateMachine;

public class StateMapping
{
	public Enum state;

	public Func<IEnumerator> Enter = StateEngine.DoNothingCoroutine;

	public Func<IEnumerator> Exit = StateEngine.DoNothingCoroutine;

	public Action Finally = StateEngine.DoNothing;

	public Action Update = StateEngine.DoNothing;

	public Action LateUpdate = StateEngine.DoNothing;

	public Action FixedUpdate = StateEngine.DoNothing;

	public StateMapping(Enum state)
	{
		this.state = state;
	}
}
