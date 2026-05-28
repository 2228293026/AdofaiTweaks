using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_FilterProperties : PropertyControl
{
	[Header("Runtime")]
	public List<PropertyControl> inputFields = new List<PropertyControl>();

	public bool enableProperties;

	public override void OnSelectedEventChanged(LevelEvent levelEvent)
	{
		if (!(ADOBase.editor.draggedEvIndicator != null))
		{
			ReloadFilterProperties(levelEvent);
		}
	}

	public void ReloadFilterProperties(LevelEvent levelEvent)
	{
		object arg = levelEvent["filter"];
		Type type = Type.GetType($"{arg}, Assembly-CSharp-firstpass");
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
		object obj = Activator.CreateInstance(type);
		base.gameObject.SetActive(value: false);
		PropertyInfo propertyInfo = GCS.levelEventsInfo["SetFilterAdvanced"].propertiesInfo["propertiesTemplate"];
		Dictionary<string, object> dict = propertyInfo.dict;
		List<PropertyControl> list = new List<PropertyControl>(inputFields);
		inputFields.Clear();
		GameObject gameObject = base.transform.parent.parent.gameObject;
		List<string> list2 = new List<string>();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			Type fieldType = fieldInfo.FieldType;
			bool flag = fieldType == typeof(int);
			bool flag2 = fieldType == typeof(float);
			bool flag3 = flag || flag2;
			bool flag4 = fieldType == typeof(Color);
			bool flag5 = fieldType == typeof(Vector2);
			if (!flag3 && !flag4 && !flag5)
			{
				continue;
			}
			string key = "filter_" + fieldInfo.Name;
			string text = fieldInfo.Name.UnCamelCase(RDUtils.UnCamelCaseOptions.SpaceBeforeNumbers | RDUtils.UnCamelCaseOptions.UnderbarToSpace);
			bool exists;
			string withCheck = RDString.GetWithCheck("editor.CameraFilterPackFields." + fieldInfo.Name, out exists);
			list2.Add(key);
			if (text.StartsWith("_"))
			{
				text = text.Substring(1);
			}
			PropertyType type2 = RDEditorUtils.TypeToPropertyType(fieldType);
			string unit = RDEditorUtils.FilterFieldToUnit(type2, fieldInfo.Name);
			float num = RDEditorUtils.UnitMultiplier(unit);
			PropertyInfo propertyInfo2 = new PropertyInfo(dict, propertyInfo.levelEventInfo)
			{
				name = key,
				type = type2,
				customLabel = (exists ? withCheck : text),
				int_min = int.MinValue,
				int_max = int.MaxValue,
				float_min = float.NegativeInfinity,
				float_max = float.PositiveInfinity,
				minVec = new Vector2(float.NegativeInfinity, float.NegativeInfinity),
				maxVec = new Vector2(float.PositiveInfinity, float.PositiveInfinity),
				unit = unit,
				invisible = false,
				encode = true
			};
			bool flag6 = false;
			RangeAttribute customAttribute = fieldInfo.GetCustomAttribute<RangeAttribute>();
			if (flag2)
			{
				if (customAttribute != null)
				{
					propertyInfo2.float_min = customAttribute.min * num;
					propertyInfo2.float_max = customAttribute.max * num;
					flag6 = true;
				}
				propertyInfo2.value_default = (float)fieldInfo.GetValue(obj) * num;
			}
			else if (flag)
			{
				if (customAttribute != null)
				{
					propertyInfo2.int_min = (int)(customAttribute.min * num);
					propertyInfo2.int_max = (int)(customAttribute.max * num);
					flag6 = true;
				}
				propertyInfo2.value_default = (int)((float)(int)fieldInfo.GetValue(obj) * num);
			}
			else if (flag4)
			{
				propertyInfo2.value_default = ((Color)fieldInfo.GetValue(obj)).ToHex(useAlpha: true, hash: false);
			}
			else if (flag5)
			{
				propertyInfo2.value_default = (Vector2)fieldInfo.GetValue(obj);
			}
			propertyInfo2.slider = flag6;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(ADOBase.gc.prefab_property, propertiesPanel.content, worldPositionStays: false);
			int siblingIndex = gameObject.transform.GetSiblingIndex();
			gameObject2.transform.SetSiblingIndex(siblingIndex + 1);
			gameObject = gameObject2;
			Property property = gameObject2.GetComponent<Property>();
			property.gameObject.name = propertyInfo2.name;
			property.key = propertyInfo2.name;
			property.info = propertyInfo2;
			GameObject original = (flag3 ? (flag6 ? ADOBase.gc.prefab_controlSlider : ADOBase.gc.prefab_controlText) : (flag4 ? ADOBase.gc.prefab_controlColor : ((!flag5) ? ADOBase.gc.prefab_controlText : ADOBase.gc.prefab_controlVector2)));
			GameObject gameObject3 = UnityEngine.Object.Instantiate(original);
			gameObject3.GetComponent<RectTransform>().SetParent(property.controlContainer, worldPositionStays: false);
			property.control = gameObject3.GetComponent<PropertyControl>();
			property.control.propertyInfo = propertyInfo2;
			property.control.propertiesPanel = propertiesPanel;
			property.control.propertyTransform = property.GetComponent<RectTransform>();
			property.control.Setup(addListener: true);
			PropertyControl control = property.control;
			PropertyControl_Text propertyControl_Text = control as PropertyControl_Text;
			PropertyControl_Slider propertyControl_Slider = control as PropertyControl_Slider;
			PropertyControl_Color propertyControl_Color = control as PropertyControl_Color;
			PropertyControl_Vector2 propertyControl_Vector = control as PropertyControl_Vector2;
			object obj2 = levelEvent[key];
			if (obj2 != null || flag3 || flag4)
			{
				control.text = obj2?.ToString();
			}
			levelEvent.disabled.TryAdd(key, !enableProperties && obj2 == null);
			List<TMP_InputField> list3 = new List<TMP_InputField>();
			if (flag6)
			{
				list3.Add(propertyControl_Slider.inputField.inputField);
				propertyControl_Slider.onEndEdit.Invoke(control.text);
			}
			else if (flag3)
			{
				list3.Add(propertyControl_Text.inputField);
				propertyControl_Text.onEndEdit.Invoke(control.text);
			}
			else if (flag4)
			{
				TMP_InputField field = propertyControl_Color.colorField.field;
				list3.Add(field);
				field.onEndEdit.Invoke(propertyControl_Color.text);
			}
			else if (flag5)
			{
				list3.Add(propertyControl_Vector.inputX);
				list3.Add(propertyControl_Vector.inputY);
				propertyControl_Vector.SetVectorVals();
			}
			propertiesPanel.SetupCheckmark(property, property.control);
			property.enabledButton.onClick.AddListener(delegate
			{
				using (new SaveStateScope(ADOBase.editor))
				{
					bool value;
					bool flag8 = levelEvent.disabled.TryGetValue(key, out value) && !value;
					levelEvent.disabled[key] = flag8;
					property.offText.SetActive(flag8);
					property.enabledCheckmark.SetActive(!flag8);
					property.control.gameObject.SetActive(!flag8);
					property.enabledButton.GetComponent<RectTransform>().offsetMin = new Vector2(0f, flag8 ? 0f : property.controlContainer.rect.height);
				}
			});
			inputFields.Add(control);
			bool flag7 = control.propertyInfo.CheckIfEnabled(levelEvent, propertiesPanel.selectedTab);
			control.SetEnabled(flag7);
			int num2 = propertiesPanel.propertySelectables.FindLastIndex((PropertiesPanel.PropertySelectable p) => p.control.propertyInfo.name.StartsWith("filter_"));
			if (num2 == -1)
			{
				List<string> list4 = propertiesPanel.properties.Keys.ToList();
				int num3 = list4.FindIndex((string p) => p == base.propertyInfo.name);
				string prevPropertyName = list4[num3 - 1];
				num2 = propertiesPanel.propertySelectables.FindIndex((PropertiesPanel.PropertySelectable p) => p.control.propertyInfo.name == prevPropertyName);
			}
			for (int num4 = 0; num4 < list3.Count; num4++)
			{
				TMP_InputField sel = list3[num4];
				PropertiesPanel.PropertySelectable item = new PropertiesPanel.PropertySelectable(sel, control, property);
				propertiesPanel.propertySelectables.Insert(num2 + 1 + num4, item);
			}
		}
		UnityEngine.Object.DestroyImmediate(obj as UnityEngine.Object);
		foreach (PropertyControl input in list)
		{
			string text2 = input.propertyInfo.name;
			if (!list2.Contains(text2))
			{
				levelEvent.GetData().Remove(text2);
				levelEvent.disabled.Remove(text2);
			}
			UnityEngine.Object.Destroy(input.propertyTransform.gameObject);
			propertiesPanel.propertySelectables.RemoveAll((PropertiesPanel.PropertySelectable ps) => ps.control == input);
		}
	}
}
