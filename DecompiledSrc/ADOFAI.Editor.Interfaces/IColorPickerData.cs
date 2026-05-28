using UnityEngine.UI;

namespace ADOFAI.Editor.Interfaces;

public interface IColorPickerData
{
	string text { get; set; }

	bool usesAlpha { get; }

	Image sample { get; }

	void SetPickerPosition(RDColorPickerPopup popup);

	void OnHide(string value);
}
