using DG.Tweening;
using JDS.PostProcessing;

public class UIHitBorder : UIController
{
	private Sequence m_Sequence;

	private DamageEffect m_DamagePostProcessEffect;

	protected override void OnInitialized(object _data)
	{
		this.m_DamagePostProcessEffect = GameManager.Instance.GameCamera.ColorPostProcess.profile.GetSetting<DamageEffect>();
		this.m_DamagePostProcessEffect._Power.value = 0f;
		this.ShowBorder();
	}

	public void ShowBorder()
	{
		this.ResetSequence();
		this.m_Sequence.Insert(0f, DOTween.To(() => this.m_DamagePostProcessEffect._Power.value, (float x) =>
		{
			this.m_DamagePostProcessEffect._Power.value = x;
		}, 0.9f, 0.3f).SetEase(Ease.OutSine));
		this.m_Sequence.Insert(0.3f, DOTween.To(() => this.m_DamagePostProcessEffect._Power.value, (float x) =>
		{
			this.m_DamagePostProcessEffect._Power.value = x;
		}, 0f, 2.3f).SetEase(Ease.OutSine));
		this.m_Sequence.OnComplete(this.PlayOut);
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		this.m_DamagePostProcessEffect = null;
		base.OnDisposed();
	}
}
