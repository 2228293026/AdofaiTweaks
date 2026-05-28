using TMPro;
using UnityEngine;

public class EditorSpeedIndicator : ADOBase
{
	public TMP_Text percent;

	public void LessSpeed()
	{
		ShiftSpeed(-1);
	}

	public void MoreSpeed()
	{
		ShiftSpeed(1);
	}

	private void UpdatePercentText(int speedPercent)
	{
		percent.text = speedPercent + "%";
	}

	private void Awake()
	{
		UpdatePercentText(Persistence.shortcutPlaySpeed);
	}

	private void ShiftSpeed(int direction)
	{
		int shortcutPlaySpeed = Persistence.shortcutPlaySpeed;
		int num = ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 1 : 10);
		num *= direction;
		shortcutPlaySpeed = Mathf.Clamp(shortcutPlaySpeed + num, 1, 1000);
		Persistence.shortcutPlaySpeed = shortcutPlaySpeed;
		shortcutPlaySpeed = Persistence.shortcutPlaySpeed;
		UpdatePercentText(shortcutPlaySpeed);
	}
}
