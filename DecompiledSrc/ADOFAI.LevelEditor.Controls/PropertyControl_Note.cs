using TMPro;

namespace ADOFAI.LevelEditor.Controls;

public class PropertyControl_Note : PropertyControl
{
	public TMP_Text noteText;

	public override void Setup(bool addListener)
	{
		noteText.text = RDString.Get(propertyInfo.noteKey);
	}
}
