using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ADOFAI.Editor.Components.Gradients;

public class GradientField : MonoBehaviour
{
	public GradientGenerator gradientGenerator;

	public Button popupButton;

	public Gradient value;

	public UnityEvent<Gradient> valueChanged;

	public GradientEditor gradientEditor;

	private void Awake()
	{
		popupButton.onClick.AddListener(delegate
		{
			gradientEditor.value = value;
			gradientEditor.onChange = delegate
			{
				gradientGenerator.UpdateGradient();
				valueChanged.Invoke(value);
			};
			gradientEditor.Refresh();
			gradientEditor.Show((RectTransform)base.transform);
		});
		Apply();
	}

	public void Apply()
	{
		gradientGenerator.gradient = value;
		gradientGenerator.UpdateGradient();
	}
}
