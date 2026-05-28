using System;
using System.Data;
using ADOFAI.Editor.Components;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_FloatPairBase : PropertyControl
{
	public (string, string) Validate(MinMaxControl control, bool clamp = true)
	{
		Tuple<float, float> tuple = new Tuple<float, float>(control.start, control.end);
		string text = control.startInput.field.text;
		string text2 = control.endInput.field.text;
		if (float.TryParse(text, out var result) && float.TryParse(text2, out var result2))
		{
			tuple = new Tuple<float, float>(result, result2);
			if (clamp)
			{
				tuple = propertyInfo.Validate(tuple);
			}
		}
		else
		{
			DataTable dataTable = new DataTable();
			try
			{
				tuple = new Tuple<float, float>(RDEditorUtils.DecodeFloat(dataTable.Compute(text, "")), tuple.Item2);
			}
			catch
			{
			}
			try
			{
				object dictValue = dataTable.Compute(text2, "");
				tuple = new Tuple<float, float>(tuple.Item1, RDEditorUtils.DecodeFloat(dictValue));
			}
			catch
			{
			}
		}
		string item = tuple.Item1.ToString("0.######");
		string item2 = tuple.Item2.ToString("0.######");
		return (item, item2);
	}
}
