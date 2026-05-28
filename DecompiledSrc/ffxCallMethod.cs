using System;
using System.Collections.Generic;
using System.Reflection;
using ADOFAI;
using UnityEngine;

public class ffxCallMethod : ffxPlusBase
{
	public string methodName;

	private MethodInfo method;

	private bool hasArguments;

	private string[] argString;

	private object instance;

	private FieldInfo field;

	private bool setup;

	public static string[] GetParameters(string text)
	{
		text = text.Trim();
		if (!text.StartsWith("(") || !text.EndsWith(")"))
		{
			return null;
		}
		if (text.Length == 2)
		{
			return null;
		}
		text = text.Trim('(', ')');
		return text.Split(',', StringSplitOptions.None);
	}

	public void Setup()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		methodName = methodName.TrimAllSpaces();
		Type type = null;
		Level level = ADOBase.controller.level;
		Type typeFromHandle = typeof(Level);
		if (methodName.IndexOf("(") == -1 || methodName.IndexOf("=") != -1)
		{
			return;
		}
		string text = methodName;
		string text2 = null;
		bool flag = false;
		int num = methodName.IndexOf("(");
		if (num > 0)
		{
			text = methodName.Substring(0, num);
			text2 = methodName.Substring(num, methodName.Length - num);
			flag = true;
		}
		if (type != null)
		{
			method = type.GetMethod(text);
		}
		if (method == null && level != null)
		{
			method = level.GetType().GetMethod(text);
		}
		if (method == null)
		{
			method = typeof(Level).GetMethod(text);
		}
		if (method != null && instance == null)
		{
			instance = level;
		}
		if (method == null)
		{
			method = typeFromHandle.GetMethod(text);
		}
		if (method != null && instance == null)
		{
			instance = level;
		}
		if (flag)
		{
			argString = GetParameters(text2);
			if (argString != null)
			{
				hasArguments = true;
			}
		}
		if (method == null)
		{
			Debug.LogWarning("CallCustomMethod: Method " + methodName + " doesn't exist");
		}
	}

	public override void StartEffect(scrPlanet planet)
	{
		AdjustDurationForHardbake();
		if (!setup)
		{
			Setup();
		}
		if (!(method != null))
		{
			return;
		}
		List<object> list = new List<object>();
		if (hasArguments)
		{
			string[] array = argString;
			foreach (string text in array)
			{
				if (text.StartsWith("str:"))
				{
					list.Add(RDEditorUtils.DecodeString(text).Remove(0, 4));
				}
				else if (text.Contains("true"))
				{
					list.Add(true);
				}
				else if (text.Contains("false"))
				{
					list.Add(false);
				}
				else if (text.Contains("."))
				{
					list.Add(RDEditorUtils.DecodeFloat(text));
				}
				else
				{
					list.Add(RDEditorUtils.DecodeInt(text));
				}
			}
		}
		method.Invoke(instance, hasArguments ? list.ToArray() : null);
	}

	public override void Decode(LevelEvent evnt)
	{
		methodName = evnt.GetString("method");
		Setup();
	}
}
