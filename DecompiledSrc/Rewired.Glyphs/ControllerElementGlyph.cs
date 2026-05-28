using System;

namespace Rewired.Glyphs;

public abstract class ControllerElementGlyph : ControllerElementGlyphBase
{
	[NonSerialized]
	private ActionElementMap _actionElementMap;

	[NonSerialized]
	private ControllerElementIdentifier _controllerElementIdentifier;

	[NonSerialized]
	private AxisRange _axisRange;

	public ActionElementMap actionElementMap
	{
		get
		{
			return _actionElementMap;
		}
		set
		{
			_actionElementMap = value;
		}
	}

	public ControllerElementIdentifier controllerElementIdentifier
	{
		get
		{
			return _controllerElementIdentifier;
		}
		set
		{
			_controllerElementIdentifier = value;
		}
	}

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
			_axisRange = value;
		}
	}

	protected override void Update()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!ReInput.isReady)
		{
			return;
		}
		if (_actionElementMap == null && controllerElementIdentifier == null)
		{
			Hide();
			return;
		}
		if (actionElementMap != null)
		{
			ShowGlyphsOrText(_actionElementMap);
		}
		else if (controllerElementIdentifier != null)
		{
			ShowGlyphsOrText(_controllerElementIdentifier, axisRange);
		}
		EvaluateObjectVisibility();
	}
}
