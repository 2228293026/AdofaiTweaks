using System;
using System.Collections.Generic;
using ADOFAI.Editor.Components;
using ADOFAI.Editor.Components.Gradients;
using ADOFAI.Editor.Models;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_MinMaxGradient : PropertyControl
{
	public ColorField color1;

	public ColorField color2;

	public GradientField gradient1;

	public GradientField gradient2;

	public TweakableDropdown modeField;

	private SerializedMinMaxGradient current;

	public override List<Selectable> selectables => current.mode switch
	{
		ParticleSystemGradientMode.TwoColors => new List<Selectable> { color1.field, color2.field, modeField.dropdownButton }, 
		ParticleSystemGradientMode.TwoGradients => new List<Selectable> { gradient1.popupButton, gradient2.popupButton, modeField.inputField }, 
		ParticleSystemGradientMode.RandomColor => new List<Selectable> { color1.field, color2.field, modeField.dropdownButton }, 
		ParticleSystemGradientMode.Color => new List<Selectable> { color1.field, modeField.dropdownButton }, 
		ParticleSystemGradientMode.Gradient => new List<Selectable> { gradient1.popupButton, modeField.dropdownButton }, 
		_ => throw new ArgumentOutOfRangeException("mode"), 
	};

	private static SerializedGradient DefaultGradient()
	{
		return new SerializedGradient
		{
			mode = GradientMode.Blend,
			alphaKeys = new SerializedGradient.AlphaKey[2]
			{
				new SerializedGradient.AlphaKey
				{
					time = 0m,
					alpha = 1m
				},
				new SerializedGradient.AlphaKey
				{
					time = 1m,
					alpha = 1m
				}
			},
			colorKeys = new SerializedGradient.ColorKey[2]
			{
				new SerializedGradient.ColorKey
				{
					time = 0m,
					color = Color.white.ToHex(useAlpha: false, hash: false)
				},
				new SerializedGradient.ColorKey
				{
					time = 1m,
					color = Color.white.ToHex(useAlpha: false, hash: false)
				}
			}
		};
	}

	public override void Setup(bool addListener)
	{
		color1.colorPickerPopup = ADOBase.editor.colorPickerPopup;
		color2.colorPickerPopup = ADOBase.editor.colorPickerPopup;
		gradient1.gradientEditor = ADOBase.editor.gradientEditorPopup;
		gradient2.gradientEditor = ADOBase.editor.gradientEditorPopup;
		modeField.Setup();
		modeField.enumTypeString = "ParticleSystemGradientMode";
		modeField.localizeEnumStrings = true;
		modeField.itemValues.Clear();
		modeField.itemValues.AddRange(Enum.GetNames(typeof(ParticleSystemGradientMode)));
		modeField.gameObject.SetActive(value: true);
		modeField.ReloadList();
		if (!addListener)
		{
			return;
		}
		color1.onChange.AddListener(delegate(string color)
		{
			SerializedMinMaxGradient value = current;
			value.color1 = color;
			Save(value);
		});
		color2.onChange.AddListener(delegate(string color)
		{
			SerializedMinMaxGradient value = current;
			value.color2 = color;
			Save(value);
		});
		gradient1.valueChanged.AddListener(delegate(Gradient gradient)
		{
			SerializedMinMaxGradient value = current;
			value.gradient1 = SerializedGradient.FromGradient(gradient);
			Save(value);
		});
		gradient2.valueChanged.AddListener(delegate(Gradient gradient)
		{
			SerializedMinMaxGradient value = current;
			value.gradient2 = SerializedGradient.FromGradient(gradient);
			Save(value);
		});
		TweakableDropdown tweakableDropdown = modeField;
		tweakableDropdown.onValueChanged = (Action<TweakableDropdownItem>)Delegate.Combine(tweakableDropdown.onValueChanged, (Action<TweakableDropdownItem>)delegate(TweakableDropdownItem item)
		{
			ParticleSystemGradientMode particleSystemGradientMode = Enum.Parse<ParticleSystemGradientMode>(item.value);
			SerializedMinMaxGradient value = current;
			value.mode = particleSystemGradientMode;
			switch (particleSystemGradientMode)
			{
			case ParticleSystemGradientMode.Color:
			{
				ref SerializedMinMaxGradient reference = ref value;
				if (reference.color1 == null)
				{
					string text = (reference.color1 = Color.white.ToHex(useAlpha: true, hash: false));
				}
				break;
			}
			case ParticleSystemGradientMode.TwoColors:
			{
				ref SerializedMinMaxGradient reference = ref value;
				if (reference.color1 == null)
				{
					string text = (reference.color1 = Color.white.ToHex(useAlpha: true, hash: false));
				}
				reference = ref value;
				if (reference.color2 == null)
				{
					string text = (reference.color2 = Color.white.ToHex(useAlpha: true, hash: false));
				}
				break;
			}
			case ParticleSystemGradientMode.Gradient:
			case ParticleSystemGradientMode.RandomColor:
			{
				ref SerializedMinMaxGradient reference = ref value;
				SerializedGradient? serializedGradient = reference.gradient1;
				SerializedGradient valueOrDefault = serializedGradient.GetValueOrDefault();
				if (!serializedGradient.HasValue)
				{
					valueOrDefault = DefaultGradient();
					ref SerializedMinMaxGradient reference4 = ref reference;
					SerializedGradient? serializedGradient2 = valueOrDefault;
					reference4.gradient1 = serializedGradient2;
				}
				break;
			}
			case ParticleSystemGradientMode.TwoGradients:
			{
				ref SerializedMinMaxGradient reference = ref value;
				SerializedGradient? serializedGradient = reference.gradient1;
				SerializedGradient valueOrDefault = serializedGradient.GetValueOrDefault();
				if (!serializedGradient.HasValue)
				{
					valueOrDefault = DefaultGradient();
					ref SerializedMinMaxGradient reference2 = ref reference;
					SerializedGradient? serializedGradient2 = valueOrDefault;
					reference2.gradient1 = serializedGradient2;
				}
				reference = ref value;
				serializedGradient = reference.gradient2;
				valueOrDefault = serializedGradient.GetValueOrDefault();
				if (!serializedGradient.HasValue)
				{
					valueOrDefault = DefaultGradient();
					ref SerializedMinMaxGradient reference3 = ref reference;
					SerializedGradient? serializedGradient2 = valueOrDefault;
					reference3.gradient2 = serializedGradient2;
				}
				break;
			}
			}
			SetUIMode(particleSystemGradientMode);
			Refresh(value);
			Save(value);
		});
	}

	private void SetUIMode(ParticleSystemGradientMode mode)
	{
		color1.gameObject.SetActive(mode == ParticleSystemGradientMode.Color || mode == ParticleSystemGradientMode.TwoColors);
		color2.gameObject.SetActive(mode == ParticleSystemGradientMode.TwoColors);
		gradient1.gameObject.SetActive(mode == ParticleSystemGradientMode.Gradient || mode == ParticleSystemGradientMode.TwoGradients || mode == ParticleSystemGradientMode.RandomColor);
		gradient2.gameObject.SetActive(mode == ParticleSystemGradientMode.TwoGradients);
	}

	private void Refresh(SerializedMinMaxGradient value)
	{
		if (!string.IsNullOrEmpty(value.color1))
		{
			color1.value = value.color1;
		}
		if (!string.IsNullOrEmpty(value.color2))
		{
			color2.value = value.color2;
		}
		if (value.gradient1.HasValue)
		{
			gradient1.value = value.gradient1.Value.ToGradient();
			gradient1.Apply();
		}
		if (value.gradient2.HasValue)
		{
			gradient2.value = value.gradient2.Value.ToGradient();
			gradient2.Apply();
		}
	}

	public void SetValue(SerializedMinMaxGradient value)
	{
		current = value;
		modeField.SelectItem(modeField.items[(int)value.mode]);
		Refresh(value);
	}

	private void Save(SerializedMinMaxGradient value)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			current = value;
			propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.name] = value;
			ToggleOthersEnabled();
			OnValueChange();
		}
	}
}
