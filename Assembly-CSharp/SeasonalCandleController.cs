using DG.Tweening;
using UnityEngine;

public class SeasonalCandleController : JMonoBehaviour
{
	private SeasonalCandle[] m_Candles;

	public override void Start()
	{
		this.m_Candles = base.transform.GetComponentsInChildren<SeasonalCandle>(includeInactive: true);
		for (int i = 0; i < this.m_Candles.Length; i = unchecked(i + 1))
		{
			Transform transform;
			transform = this.m_Candles[i].transform;
			if (transform != null)
			{
				float endValue;
				endValue = Random.Range(0.35f, 0.45f);
				float duration;
				duration = Random.Range(1.5f, 2.5f);
				transform.DOKill();
				transform.DOLocalMoveY(endValue, duration).SetRelative().SetEase(Ease.InOutQuad)
					.SetLoops(-1, LoopType.Yoyo);
			}
		}
	}

	protected override void OnDisposed()
	{
		for (int i = 0; i < this.m_Candles.Length; i = unchecked(i + 1))
		{
			this.m_Candles[i].transform.DOKill();
		}
		base.OnDisposed();
	}
}
