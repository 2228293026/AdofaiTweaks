using System;
using UnityEngine;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Rating : PropertyControl
{
	public Image[] stars;

	public Button[] buttons;

	public int score;

	public override string text
	{
		get
		{
			return GetInt().ToString();
		}
		set
		{
			int result = 1;
			int.TryParse(value, out result);
			UpdateStars(result);
		}
	}

	private void Awake()
	{
		Button[] array = buttons;
		foreach (Button button in array)
		{
			button.onClick.AddListener(delegate
			{
				SetInt(Array.IndexOf(buttons, button) + 1);
			});
		}
	}

	private int GetInt()
	{
		return score;
	}

	private void SetInt(int val)
	{
		using (new SaveStateScope(ADOBase.editor))
		{
			score = val;
			propertiesPanel.inspectorPanel.selectedEvent[propertyInfo.name] = score;
			UpdateStars(val);
			OnValueChange();
		}
	}

	private void UpdateStars(int val)
	{
		Color color = ((val <= 3) ? ADOBase.gc.difficultyEasy : ((val <= 6) ? ADOBase.gc.difficultyMedium : ((val <= 9) ? ADOBase.gc.difficultyHard : ADOBase.gc.difficultyHardest)));
		Image[] array = stars;
		foreach (Image image in array)
		{
			Color color2 = ((Array.IndexOf(stars, image) + 1 <= val) ? color : Color.black);
			image.color = color2;
		}
	}
}
