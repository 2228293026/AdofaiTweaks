using DG.Tweening;

namespace MobileMenu;

public class MobileMenuMinesweeperPuzzle : ADOBase
{
	public MobileMenuGrabbableMine[] mines;

	private bool finishPuzzle;

	private void Update()
	{
		if (!finishPuzzle && CheckPuzzleFinished())
		{
			DoFinish();
		}
	}

	private void DoFinish()
	{
		finishPuzzle = true;
		ADOBase.conductor.DuckSongStart(0f);
		DOTween.Sequence().AppendInterval(0.5f).AppendCallback(delegate
		{
			scrSfx.instance.PlaySfx(SfxSound.MobileMenuMineswooce, MixerGroup.SfxParent);
		})
			.AppendInterval(0.8f)
			.AppendCallback(delegate
			{
				scnMinesweeper.EnterScene();
			});
	}

	private bool CheckPuzzleFinished()
	{
		MobileMenuGrabbableMine[] array = mines;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].exploded)
			{
				return false;
			}
		}
		return true;
	}
}
