using System.Collections.Generic;
using UnityEngine.UI;

namespace ADOFAI.Editor.Panels;

public class FindCommentPanel : ADOBase
{
	public Text findCommentPanelTitle;

	public Text matchText;

	public Image findArrow;

	public Property findValue;

	public Button findButton;

	public Button prevButton;

	public Button nextButton;

	public Text findButtonText;

	private int[] targetFloors;

	private int selectedIndex = -1;

	private string lastSearchTerm;

	private void Awake()
	{
		findCommentPanelTitle.text = RDString.Get("editor.findComment.title");
		findValue.label.text = RDString.Get("editor.findComment.findValue");
		findButtonText.text = RDString.Get("editor.findComment.findButton");
		matchText.text = RDString.Get("editor.findComment.noMatches");
		findButton.onClick.AddListener(Search);
		prevButton.onClick.AddListener(Prev);
		nextButton.onClick.AddListener(Next);
	}

	private void RefreshText()
	{
		if (targetFloors.Length == 0)
		{
			matchText.text = RDString.Get("editor.findComment.noMatches");
			return;
		}
		matchText.text = RDString.Get("editor.findComment.matches", new Dictionary<string, object>
		{
			{
				"current",
				(selectedIndex + 1).ToString()
			},
			{
				"total",
				targetFloors.Length.ToString()
			}
		});
	}

	private void Refresh()
	{
		lastSearchTerm = findValue.control.text;
		selectedIndex = -1;
		targetFloors = ADOBase.editor.SearchByComment(lastSearchTerm);
	}

	public void Search()
	{
		Refresh();
		if (targetFloors.Length != 0)
		{
			selectedIndex = 0;
			ADOBase.editor.SelectFloor(ADOBase.editor.floors[targetFloors[0]]);
		}
		else
		{
			selectedIndex = -1;
			ADOBase.editor.SelectFloor(ADOBase.editor.floors[0]);
		}
		RefreshText();
	}

	public void Prev()
	{
		if (lastSearchTerm != findValue.control.text || targetFloors.Length == 0)
		{
			Refresh();
		}
		if (0 < selectedIndex)
		{
			selectedIndex--;
			int num = targetFloors[selectedIndex];
			if (0 <= num)
			{
				scrFloor floorToSelect = ADOBase.editor.floors[num];
				ADOBase.editor.SelectFloor(floorToSelect);
				RefreshText();
			}
		}
	}

	public void Next()
	{
		if (lastSearchTerm != findValue.control.text || targetFloors.Length == 0)
		{
			Refresh();
		}
		if (targetFloors.Length > selectedIndex + 1)
		{
			selectedIndex++;
			int num = targetFloors[selectedIndex];
			if (ADOBase.editor.floors.Count >= num + 1)
			{
				scrFloor floorToSelect = ADOBase.editor.floors[num];
				ADOBase.editor.SelectFloor(floorToSelect);
				RefreshText();
			}
		}
	}
}
