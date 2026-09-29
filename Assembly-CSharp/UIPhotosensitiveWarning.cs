using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPhotosensitiveWarning : UIController
{
	[SerializeField]
	private RectTransform m_Container;

	[SerializeField]
	private Image m_BlackOverlay;

	protected override void OnInitialized(object _data)
	{
		this.m_BlackOverlay.gameObject.SetActive(value: true);
		for (int i = 0; i < 10; i = unchecked(i + 1))
		{
			UIElementPhotosensitiveWarningDataVO uIElementPhotosensitiveWarningDataVO;
			uIElementPhotosensitiveWarningDataVO = new UIElementPhotosensitiveWarningDataVO(TextUtility.GetKey("MENU_PHOTOSENSITIVE_WARNING_HEADER", i), TextUtility.GetKey("MENU_PHOTOSENSITIVE_WARNING_BODY", i), "UI/Elements/UIElementPhotosensitiveWarning");
			UIManager.CreateElement(this.m_Container, uIElementPhotosensitiveWarningDataVO.PrefabKey, uIElementPhotosensitiveWarningDataVO);
		}
	}

	public override void PlayIn()
	{
		this.m_BlackOverlay.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(this.PlayInComplete);
	}

	public override void PlayInComplete()
	{
		base.PlayInComplete();
		this.m_BlackOverlay.DOFade(1f, 1f).SetEase(Ease.Linear).SetDelay(3f)
			.OnComplete(this.PlayOut);
	}

	private void Update()
	{
		if (!base.IsDisposed && (Input.GetMouseButtonDown(2) || PlayerInput.ControllerRightBumper()))
		{
			this.m_BlackOverlay.DOKill();
			this.PlayOut();
		}
	}
}
