using System;
using UnityEngine.UI;

public class scrButtonURL : ADOBase
{
	public string link;

	public bool localized;

	[NonSerialized]
	public Button button;

	public void Awake()
	{
		button = base.transform.GetComponent<Button>();
		button.onClick.AddListener(delegate
		{
			OpenURL();
		});
	}

	public virtual void OpenURL()
	{
		if (!link.IsNullOrEmpty() && !ADOBase.isSwitch)
		{
			scrController.instance.TogglePauseGame();
			ADOBase.platformHelper.OpenURL(localized ? RDString.Get(link) : link);
		}
	}
}
