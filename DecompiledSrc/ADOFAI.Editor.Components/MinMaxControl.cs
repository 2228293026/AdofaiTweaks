using System;
using System.Data;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace ADOFAI.Editor.Components;

public class MinMaxControl : MonoBehaviour
{
	public float start;

	public float end;

	public float min;

	public float max;

	public bool clamp;

	public bool isRange;

	public int maxFloatingPoints;

	public UnityEvent<Tuple<float, float>> onChange;

	public DraggableNumberInputField startInput;

	public DraggableNumberInputField endInput;

	public void RefreshInput()
	{
		RefreshInput(startInput, start);
		RefreshInput(endInput, end);
	}

	private void RefreshInput(DraggableNumberInputField input, float value)
	{
		input.clamp = clamp;
		input.min = min;
		input.max = max;
		input.field.text = value.ToString(CultureInfo.InvariantCulture);
		input.maxFloatingPoints = maxFloatingPoints;
	}

	private void Awake()
	{
		startInput.onDrag.AddListener(delegate
		{
			OnChange(isStart: true);
		});
		endInput.onDrag.AddListener(delegate
		{
			OnChange(isStart: false);
		});
		startInput.onEndEdit.AddListener(delegate
		{
			OnChange(isStart: true);
		});
		endInput.onEndEdit.AddListener(delegate
		{
			OnChange(isStart: false);
		});
		RefreshInput();
	}

	private float? GetValue(string value)
	{
		DataTable dataTable = new DataTable();
		float? result;
		try
		{
			float num = RDEditorUtils.DecodeFloat(dataTable.Compute(value, ""));
			return clamp ? Mathf.Clamp(num, min, max) : num;
		}
		catch
		{
			result = null;
		}
		return result;
	}

	private void OnChange(bool isStart)
	{
		DraggableNumberInputField draggableNumberInputField = (isStart ? startInput : endInput);
		DraggableNumberInputField draggableNumberInputField2 = (isStart ? endInput : startInput);
		float? value = GetValue(draggableNumberInputField.field.text);
		if (!value.HasValue)
		{
			return;
		}
		float valueOrDefault = value.GetValueOrDefault();
		draggableNumberInputField.field.text = valueOrDefault.ToString(CultureInfo.InvariantCulture);
		value = GetValue(draggableNumberInputField2.field.text);
		if (value.HasValue)
		{
			float valueOrDefault2 = value.GetValueOrDefault();
			draggableNumberInputField.field.text = valueOrDefault.ToString(CultureInfo.InvariantCulture);
			if (isRange)
			{
				draggableNumberInputField2.field.text = (isStart ? Mathf.Max(valueOrDefault, valueOrDefault2) : Mathf.Min(valueOrDefault, valueOrDefault2)).ToString(CultureInfo.InvariantCulture);
			}
			start = GetValue(startInput.field.text) ?? start;
			end = GetValue(endInput.field.text) ?? end;
			NotifyChange();
		}
	}

	private void NotifyChange()
	{
		onChange.Invoke(new Tuple<float, float>(start, end));
	}
}
