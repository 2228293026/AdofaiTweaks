using DG.Tweening;

public static class Timer
{
	public static Tween Add(TweenCallback action, float delay, bool ignoreTimeScale = true)
	{
		return DOVirtual.DelayedCall(delay, action, ignoreTimeScale);
	}
}
