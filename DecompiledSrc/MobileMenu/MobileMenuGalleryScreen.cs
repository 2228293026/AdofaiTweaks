using UnityEngine;

namespace MobileMenu;

public class MobileMenuGalleryScreen : MobileMenuScreen
{
	public MobileTrailerPlayer trailerPlayer;

	private bool trailerIsPlaying;

	public bool isInTrailer => trailerIsPlaying;

	public override void Instantiate()
	{
		base.Instantiate();
		transform = new GameObject("GalleryScreen").transform;
	}

	public override void Interact(bool fromKeyboard)
	{
		if (fromKeyboard && !trailerPlayer.videoPlayer.isPlaying)
		{
			trailerPlayer.Toggle();
		}
	}

	public void OnToggleTrailerComplete(bool play)
	{
		trailerIsPlaying = play;
	}
}
