using Rewired.Glyphs.UnityUI;
using UnityEngine;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class InputFieldInfo : UIElementInfo
{
	private int _actionElementMapId;

	private AxisRange _axisRange;

	public UnityUIControllerElementGlyph glyphOrText { get; set; }

	public int actionId { get; set; }

	public AxisRange axisRange
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _axisRange;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			_axisRange = value;
			if (glyphOrText != null)
			{
				glyphOrText.axisRange = value;
			}
		}
	}

	public int actionElementMapId
	{
		get
		{
			return _actionElementMapId;
		}
		set
		{
			_actionElementMapId = value;
			if (glyphOrText != null)
			{
				glyphOrText.actionElementMap = ReInput.mapping.GetActionElementMap(value);
			}
		}
	}

	public ControllerType controllerType { get; set; }

	public int controllerId { get; set; }
}
