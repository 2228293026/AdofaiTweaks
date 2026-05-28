using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Tile : PropertyControl
{
	public PropertyControl_Text inputField;

	public PropertyControl_Toggle buttonsToggle;

	public Button buttonThisTile;

	public Button buttonFirstTile;

	public Button buttonLastTile;

	public override List<Selectable> selectables => inputField.selectables.Concat(buttonsToggle.selectables).ToList();

	public Tuple<int, TileRelativeTo> tileValue
	{
		get
		{
			int item = Convert.ToInt32(inputField.text);
			TileRelativeTo item2 = RDUtils.ParseEnum(buttonsToggle.text, TileRelativeTo.ThisTile);
			return new Tuple<int, TileRelativeTo>(item, item2);
		}
		set
		{
			inputField.text = value.Item1.ToString();
			buttonsToggle.text = value.Item2.ToString();
		}
	}

	public override void Setup(bool addListener)
	{
		inputField.propertyInfo = propertyInfo;
		inputField.propertiesPanel = propertiesPanel;
		buttonsToggle.propertyInfo = propertyInfo;
		buttonsToggle.propertiesPanel = propertiesPanel;
		inputField.Setup(addListener);
		List<string> list = new List<string>();
		foreach (TileRelativeTo value in Enum.GetValues(typeof(TileRelativeTo)))
		{
			list.Add(value.ToString());
		}
		List<Button> list2 = new List<Button>();
		list2.Add(buttonThisTile);
		list2.Add(buttonFirstTile);
		list2.Add(buttonLastTile);
		Dictionary<string, Button> dictionary = new Dictionary<string, Button>();
		for (int i = 0; i < 3; i++)
		{
			string enumVal = list[i];
			Button button = list2[i];
			dictionary.Add(enumVal, button);
			buttonsToggle.selectablesList.Add(button);
			button.GetComponentInChildren<TMP_Text>().text = RDString.Get("enum.TileRelativeTo." + list[i]);
			button.GetComponent<Button>().onClick.AddListener(delegate
			{
				buttonsToggle.SelectVar(enumVal);
			});
		}
		buttonsToggle.buttonsDict = dictionary;
	}
}
