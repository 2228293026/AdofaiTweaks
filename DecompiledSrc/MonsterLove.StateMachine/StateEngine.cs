using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonsterLove.StateMachine;

public class StateEngine : MonoBehaviour
{
	private StateMapping currentState;

	private StateMapping destinationState;

	private Dictionary<Enum, StateMapping> stateLookup;

	private Dictionary<string, Delegate> methodLookup;

	private readonly string[] ignoredNames = new string[4] { "add", "remove", "get", "set" };

	private bool isInTransition;

	private IEnumerator currentTransition;

	private IEnumerator exitRoutine;

	private IEnumerator enterRoutine;

	private IEnumerator queuedChange;

	public bool IsInTransition => isInTransition;

	public event Action<Enum> Changed;

	public void Initialize<T>(StateBehaviour entity)
	{
		Array values = Enum.GetValues(typeof(T));
		stateLookup = new Dictionary<Enum, StateMapping>();
		for (int i = 0; i < values.Length; i++)
		{
			StateMapping stateMapping = new StateMapping((Enum)values.GetValue(i));
			stateLookup.Add(stateMapping.state, stateMapping);
		}
		MethodInfo[] methods = entity.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		char[] separator = "_".ToCharArray();
		for (int j = 0; j < methods.Length; j++)
		{
			string[] array = methods[j].Name.Split(separator);
			if (array.Length <= 1)
			{
				continue;
			}
			Enum key;
			try
			{
				key = (Enum)Enum.Parse(typeof(T), array[0]);
			}
			catch (ArgumentException)
			{
				for (int k = 0; k < ignoredNames.Length && !(array[0] == ignoredNames[k]); k++)
				{
				}
				continue;
			}
			StateMapping stateMapping2 = stateLookup[key];
			switch (array[1])
			{
			case "Enter":
			{
				if (methods[j].ReturnType == typeof(IEnumerator))
				{
					stateMapping2.Enter = CreateDelegate<Func<IEnumerator>>(methods[j], entity);
					break;
				}
				Action action2 = CreateDelegate<Action>(methods[j], entity);
				stateMapping2.Enter = delegate
				{
					action2();
					return (IEnumerator)null;
				};
				break;
			}
			case "Exit":
			{
				if (methods[j].ReturnType == typeof(IEnumerator))
				{
					stateMapping2.Exit = CreateDelegate<Func<IEnumerator>>(methods[j], entity);
					break;
				}
				Action action = CreateDelegate<Action>(methods[j], entity);
				stateMapping2.Exit = delegate
				{
					action();
					return (IEnumerator)null;
				};
				break;
			}
			case "Finally":
				stateMapping2.Finally = CreateDelegate<Action>(methods[j], entity);
				break;
			case "Update":
				stateMapping2.Update = CreateDelegate<Action>(methods[j], entity);
				break;
			case "LateUpdate":
				stateMapping2.LateUpdate = CreateDelegate<Action>(methods[j], entity);
				break;
			case "FixedUpdate":
				stateMapping2.FixedUpdate = CreateDelegate<Action>(methods[j], entity);
				break;
			}
		}
	}

	private V CreateDelegate<V>(MethodInfo method, object target) where V : class
	{
		return (Delegate.CreateDelegate(typeof(V), target, method) as V) ?? throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
	}

	public void ChangeState(Enum newState, StateTransition transition = StateTransition.Safe)
	{
		if (stateLookup == null)
		{
			throw new Exception("States have not been configured, please call initialized before trying to set state");
		}
		if (!stateLookup.ContainsKey(newState))
		{
			throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
		}
		StateMapping stateMapping = stateLookup[newState];
		if (currentState == stateMapping)
		{
			return;
		}
		if (queuedChange != null)
		{
			StopCoroutine(queuedChange);
			queuedChange = null;
		}
		switch (transition)
		{
		case StateTransition.Safe:
			if (isInTransition)
			{
				if (exitRoutine != null)
				{
					destinationState = stateMapping;
					return;
				}
				if (enterRoutine != null)
				{
					queuedChange = WaitForPreviousTransition(stateMapping);
					StartCoroutine(queuedChange);
					return;
				}
			}
			break;
		case StateTransition.Overwrite:
			if (currentTransition != null)
			{
				StopCoroutine(currentTransition);
			}
			if (exitRoutine != null)
			{
				StopCoroutine(exitRoutine);
			}
			if (enterRoutine != null)
			{
				StopCoroutine(enterRoutine);
			}
			if (currentState != null)
			{
				currentState.Finally();
			}
			currentState = null;
			break;
		}
		isInTransition = true;
		currentTransition = ChangeToNewStateRoutine(stateMapping);
		StartCoroutine(currentTransition);
	}

	private IEnumerator ChangeToNewStateRoutine(StateMapping newState)
	{
		destinationState = newState;
		if (currentState != null)
		{
			exitRoutine = currentState.Exit();
			if (exitRoutine != null)
			{
				yield return StartCoroutine(exitRoutine);
			}
			exitRoutine = null;
			currentState.Finally();
		}
		currentState = destinationState;
		if (currentState != null)
		{
			enterRoutine = currentState.Enter();
			if (enterRoutine != null)
			{
				yield return StartCoroutine(enterRoutine);
			}
			enterRoutine = null;
			if (this.Changed != null)
			{
				this.Changed(currentState.state);
			}
		}
		isInTransition = false;
	}

	private IEnumerator WaitForPreviousTransition(StateMapping nextState)
	{
		while (isInTransition)
		{
			yield return null;
		}
		ChangeState(nextState.state);
	}

	private void FixedUpdate()
	{
		if (currentState != null)
		{
			currentState.FixedUpdate();
		}
	}

	private void Update()
	{
		if (currentState != null && !IsInTransition)
		{
			currentState.Update();
		}
	}

	private void LateUpdate()
	{
		if (currentState != null && !IsInTransition)
		{
			currentState.LateUpdate();
		}
	}

	public static void DoNothing()
	{
	}

	public static void DoNothingCollider(Collider other)
	{
	}

	public static void DoNothingCollision(Collision other)
	{
	}

	public static IEnumerator DoNothingCoroutine()
	{
		yield break;
	}

	public Enum GetState()
	{
		if (currentState == null)
		{
			return null;
		}
		return currentState.state;
	}
}
