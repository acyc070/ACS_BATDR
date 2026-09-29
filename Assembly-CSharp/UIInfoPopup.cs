using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPopup : UIController
{
	[Header("Images")]
	[SerializeField]
	private Image m_ScreenShot;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_TitleLabel;

	[SerializeField]
	private TextMeshProUGUI m_HeaderLabel;

	[SerializeField]
	private TextMeshProUGUI m_MessageLabel;

	[SerializeField]
	private TextMeshProUGUI m_PressLabel;

	[SerializeField]
	private TextMeshProUGUI m_DismissLabel;

	[Header("Keyboard")]
	[SerializeField]
	private GameObject m_KeyboardContainer;

	[SerializeField]
	private TextMeshProUGUI m_KeyboardInputLabel;

	[Header("Controller")]
	[SerializeField]
	private GameObject m_ControllerContainer;

	[SerializeField]
	private Image m_ControllerInputImage;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	private UIInfoPopupDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIInfoPopupDataVO)_data;
		this.m_TitleLabel.text = TextUtility.GetKey(this.m_DataVO.TitleLabel).ConvertAll().ToUpper();
		this.m_HeaderLabel.text = TextUtility.GetKey(this.m_DataVO.HeaderLabel).ConvertAll().ToUpper();
		string input;
		input = this.m_DataVO.Input;
		if (GameManager.Instance.HasController)
		{
			input = GameManager.Instance.ControllerInput.GetString(this.m_DataVO.Input);
		}
		this.m_MessageLabel.text = TextUtility.GetKey(this.m_DataVO.MessageLabel).ConvertAll(input).ToUpper();
		this.m_ScreenShot.sprite = GameManager.Instance.AssetManager.GetAsset<InfoPopupData>(this.m_DataVO.Image).Image;
		this.m_ScreenShot.SetNativeSize();
		this.m_PressLabel.text = TextUtility.GetKey("UI_INFO_POPUP_DISMISS_PRESS").ToUpper();
		if (!GameManager.Instance.HasController)
		{
			if (this.m_ControllerContainer != null)
			{
				this.m_ControllerContainer.SetActive(value: false);
			}
			if (this.m_KeyboardContainer != null)
			{
				this.m_KeyboardContainer.SetActive(value: true);
			}
			this.m_KeyboardInputLabel.text = PlayerInputConstants.INTERACT;
		}
		else
		{
			if (this.m_KeyboardContainer != null)
			{
				this.m_KeyboardContainer.SetActive(value: false);
			}
			if (this.m_ControllerContainer != null)
			{
				this.m_ControllerContainer.SetActive(value: true);
			}
			this.m_ControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(InputControlType.Action3);
		}
		this.m_DismissLabel.text = TextUtility.GetKey("UI_INFO_POPUP_DISMISS").ToUpper();
	}

	private void Update()
	{
		if (!base.IsDisposed)
		{
			if (this.m_InputTimer >= this.m_InputDelay && PlayerInput.InteractOnPressed())
			{
				GameManager.Instance.ShowCrosshair();
				GameManager.Instance.HideInfoPopup();
			}
			else
			{
				this.m_InputTimer += Time.unscaledDeltaTime;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
