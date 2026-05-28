using TMPro;
using UnityEngine.UI;

public class ArtistUISettings : ADOBase
{
	public TMP_Text artistText;

	public Button button;

	public ApprovalLevelBadge badge;

	public void SetData(string artist, ApprovalLevel approvalLevel)
	{
		base.gameObject.SetActive(value: true);
		artistText.text = artist;
		badge.button.enabled = false;
		badge.UpdateUI(approvalLevel);
	}
}
