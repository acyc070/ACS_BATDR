using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : UIController
{
	[Header("Crosshairs")]
	[SerializeField]
	private CanvasGroup m_Cavnas;

	[SerializeField]
	private Image m_Default;

	[SerializeField]
	private Image m_Weapon;

	protected override void OnInitialized(object _data)
	{
		this.m_Cavnas.alpha = 0f;
	}

	public void Show()
	{
		this.CheckType(GameManager.Instance.Player.CurrentWeapon != null);
		this.m_Cavnas.gameObject.SetActive(value: true);
		this.m_Cavnas.DOKill();
		this.m_Cavnas.DOFade(0.9f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
	}

	public void Hide()
	{
		this.m_Cavnas.DOKill();
		this.m_Cavnas.DOFade(0f, 0.5f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true).OnComplete(this.ForceHide);
	}

	public void ForceHide()
	{
		this.m_Cavnas.gameObject.SetActive(value: false);
	}

	private void CheckType(bool hasWeapon)
	{
		if (this.m_Weapon != null)
		{
			this.m_Weapon.enabled = hasWeapon;
		}
		if (this.m_Default != null)
		{
			this.m_Default.enabled = !hasWeapon;
		}
	}

	protected override void OnDisposed()
	{
		this.m_Cavnas.DOKill();
		base.OnDisposed();
	}
}
