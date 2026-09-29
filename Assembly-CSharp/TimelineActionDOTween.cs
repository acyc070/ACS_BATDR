using System;
using DG.Tweening;

[Serializable]
public class TimelineActionDOTween
{
	public DOTweenAnimation DOTweenAnimation;

	public bool Stop;

	public void Action()
	{
		if (!this.Stop)
		{
			this.DOTweenAnimation?.DOPlay();
		}
		else
		{
			this.DOTweenAnimation?.DOPause();
		}
	}
}
