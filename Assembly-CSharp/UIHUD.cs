using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : UIController
{
	[Header("Sprint Options")]
	[SerializeField]
	private CanvasGroup m_SprintCanvasGroup;

	[SerializeField]
	private Image m_SprintBarFiller;

	[SerializeField]
	private Image m_SprintBar;

	[Header("Health Options")]
	[SerializeField]
	private CanvasGroup m_HealthCanvasGroup;

	[SerializeField]
	private Image m_HealthBarFiller;

	[SerializeField]
	private Image m_HealthBar;

	[Header("Stealth... TEMP...")]
	[SerializeField]
	private CanvasGroup m_StealthIndicator;

	[SerializeField]
	private CanvasGroup m_StealthBreakIndicator;

	protected override void OnInitialized(object _data)
	{
		this.m_StealthBreakIndicator.alpha = 0f;
		this.m_StealthIndicator.alpha = 0f;
		this.m_SprintCanvasGroup.alpha = 0f;
		this.m_HealthCanvasGroup.alpha = 0f;
	}

	public void SetSprintBar(float value, float max)
	{
		float num;
		num = 1f - value / max;
		this.m_SprintBarFiller.fillAmount = num;
		if (num < 1f)
		{
			if (this.m_SprintCanvasGroup.alpha <= 0f)
			{
				this.m_SprintCanvasGroup.DOKill();
				this.m_SprintCanvasGroup.DOFade(1f, 0.25f);
			}
		}
		else
		{
			this.m_SprintCanvasGroup.DOKill();
			this.m_SprintCanvasGroup.DOFade(0f, 0.25f);
		}
	}

	public void SetRefill(bool isFilling)
	{
		Color color;
		color = this.m_SprintBar.color;
		if (isFilling)
		{
			color.a = 0.1f;
		}
		else
		{
			color.a = 1f;
		}
		this.m_SprintBar.color = color;
	}

	public void HideSprintBar()
	{
		this.m_SprintCanvasGroup.DOKill();
		this.m_SprintCanvasGroup.DOFade(0f, 0.25f);
	}

	public void HideHealthBar()
	{
		this.m_HealthBarFiller.DOKill();
		this.m_HealthCanvasGroup.DOKill();
		this.m_HealthCanvasGroup.alpha = 0f;
	}

	public void SetHealthBar(float value, float max)
	{
		float fillAmount;
		fillAmount = value / max;
		if (fillAmount < 1f && this.m_HealthCanvasGroup.alpha <= 0f)
		{
			this.m_HealthCanvasGroup.DOKill();
			this.m_HealthCanvasGroup.DOFade(1f, 0.25f);
		}
		this.m_HealthBarFiller.DOKill();
		this.m_HealthBarFiller.DOFillAmount(fillAmount, 0.25f).SetEase(Ease.OutSine).OnComplete(() =>
		{
			if (fillAmount >= 1f)
			{
				this.m_HealthCanvasGroup.DOKill();
				this.m_HealthCanvasGroup.DOFade(0f, 0.5f);
			}
		});
	}

	public void ShowSteath()
	{
		this.KillStealthBreakIndicator();
		this.m_StealthIndicator.DOKill();
		this.m_StealthIndicator.DOFade(0.25f, 0.25f).SetEase(Ease.OutSine);
	}

	public void HideStealth()
	{
		this.KillStealthBreakIndicator();
		this.m_StealthIndicator.DOKill();
		this.m_StealthIndicator.DOFade(0f, 0.25f).SetEase(Ease.OutSine);
	}

	public void BreakStealth()
	{
		this.m_StealthIndicator.DOKill();
		this.m_StealthIndicator.alpha = 1f;
		this.KillStealthBreakIndicator();
		this.m_StealthBreakIndicator.alpha = 1f;
		this.m_StealthBreakIndicator.DOFade(0f, 0.5f).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Restart);
		this.m_StealthBreakIndicator.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Restart);
	}

	private void KillStealthBreakIndicator()
	{
		this.m_StealthBreakIndicator.DOKill();
		this.m_StealthBreakIndicator.alpha = 0f;
		this.m_StealthBreakIndicator.transform.DOKill();
		this.m_StealthBreakIndicator.transform.localScale = Vector3.one;
	}

	protected override void OnDisposed()
	{
		if ((bool)this.m_HealthCanvasGroup)
		{
			this.m_HealthCanvasGroup.DOKill();
		}
		if ((bool)this.m_SprintCanvasGroup)
		{
			this.m_SprintCanvasGroup.DOKill();
		}
		if ((bool)this.m_StealthIndicator)
		{
			this.m_StealthIndicator.DOKill();
		}
		if ((bool)this.m_StealthBreakIndicator)
		{
			this.m_StealthBreakIndicator.DOKill();
		}
		base.OnDisposed();
	}
}
