using TMPro;
using UnityEngine.UI;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Export : PropertyControl
{
	public Button exportButton;

	public TMP_Text buttonText;

	private void Awake()
	{
		exportButton.onClick.AddListener(delegate
		{
			ShowExport();
		});
		buttonText.text = RDString.Get("editor.export");
	}

	private void ShowExport()
	{
		if (EditorWebServices.artists == null)
		{
			buttonText.text = RDString.Get("status.loading");
			ADOBase.editor.webServices.LoadAllArtists(delegate
			{
				buttonText.text = RDString.Get("editor.export");
				ADOBase.editor.ShowExportWindow(0);
			});
		}
		else
		{
			ADOBase.editor.ShowExportWindow(0);
		}
	}
}
