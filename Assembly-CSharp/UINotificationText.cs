using DG.Tweening;
using InControl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationText : UIController
{
	[Header("Canvas Groups")]
	[SerializeField]
	private CanvasGroup m_CanvasGroup;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_InkDemonabel;

	[SerializeField]
	private TextMeshProUGUI m_InputLabel;

	[Header("Keyboard")]
	[SerializeField]
	private GameObject m_KeyboardContainer;

	[SerializeField]
	private TextMeshProUGUI m_KeyboardInputLabel;

	[Header("Controller")]
	[SerializeField]
	private GameObject m_ControllerContainer;

	[SerializeField]
	private GameObject m_ControllerInputContainer;

	[SerializeField]
	private GameObject m_ControllerJoystickL;

	[SerializeField]
	private GameObject m_ControllerJoystickR;

	[SerializeField]
	private GameObject m_ControllerInput;

	[SerializeField]
	private Image m_ControllerInputImage;

	[Header("Controller Labels")]
	[SerializeField]
	private TextMeshProUGUI m_PressLabel;

	[SerializeField]
	private TextMeshProUGUI m_ActionLabel;

	[Header("Controller Button Type")]
	[SerializeField]
	private Image m_ControllerButtonImage;

	[SerializeField]
	private Sprite m_ControllerButtonLB;

	[SerializeField]
	private Sprite m_ControllerButtonLT;

	[SerializeField]
	private Sprite m_ControllerButtonRB;

	[SerializeField]
	private Sprite m_ControllerButtonRT;

	private UINotificationDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UINotificationDataVO)_data;
		this.m_CanvasGroup.alpha = 0f;
		if (this.m_DataVO.IsInkDemon)
		{
			this.m_KeyboardContainer.SetActive(value: false);
			this.m_ControllerContainer.SetActive(value: false);
			this.m_InkDemonabel.text = this.m_DataVO.Message;
			return;
		}
		this.m_InkDemonabel.gameObject.SetActive(value: false);
		if (this.m_DataVO.HasInput)
		{
			this.m_KeyboardContainer.SetActive(value: false);
			this.m_ControllerContainer.SetActive(value: true);
			this.m_PressLabel.text = this.m_DataVO.PressLabel;
			this.m_ActionLabel.text = this.m_DataVO.ActionLabel;
			if (GameManager.Instance.HasController)
			{
				this.m_InputLabel.gameObject.SetActive(value: false);
				this.m_ControllerInputContainer.SetActive(value: true);
				if (this.m_DataVO.InputControlType == InputControlType.LeftStickButton)
				{
					this.m_ControllerJoystickL.SetActive(value: true);
					this.m_ControllerJoystickR.SetActive(value: false);
					this.m_ControllerInput.SetActive(value: false);
					return;
				}
				if (this.m_DataVO.InputControlType == InputControlType.RightStickButton)
				{
					this.m_ControllerJoystickL.SetActive(value: false);
					this.m_ControllerJoystickR.SetActive(value: true);
					this.m_ControllerInput.SetActive(value: false);
					return;
				}
				this.m_ControllerJoystickL.SetActive(value: false);
				this.m_ControllerJoystickR.SetActive(value: false);
				this.m_ControllerInput.SetActive(value: true);
				this.m_ControllerInputImage.sprite = GameManager.Instance.ControllerInput.GetSprite(this.m_DataVO.InputControlType);
				if (this.m_DataVO.InputControlType == InputControlType.LeftBumper)
				{
					this.m_ControllerButtonImage.sprite = this.m_ControllerButtonLB;
				}
				else if (this.m_DataVO.InputControlType == InputControlType.LeftTrigger)
				{
					this.m_ControllerButtonImage.sprite = this.m_ControllerButtonLT;
				}
				else if (this.m_DataVO.InputControlType == InputControlType.RightBumper)
				{
					this.m_ControllerButtonImage.sprite = this.m_ControllerButtonRB;
				}
				else if (this.m_DataVO.InputControlType == InputControlType.RightTrigger)
				{
					this.m_ControllerButtonImage.sprite = this.m_ControllerButtonRT;
				}
			}
			else
			{
				this.m_ControllerInputContainer.SetActive(value: false);
				this.m_InputLabel.gameObject.SetActive(value: true);
				this.m_InputLabel.text = this.m_DataVO.InputLabel;
			}
		}
		else
		{
			this.m_ControllerContainer.SetActive(value: false);
			this.m_KeyboardContainer.SetActive(value: true);
			this.m_Label.text = this.m_DataVO.Message;
		}
	}

	public override void PlayIn()
	{
		this.m_CanvasGroup.DOFade(1f, 0.25f).SetEase(Ease.Linear).OnComplete(this.PlayOut);
	}

	public override void PlayOut()
	{
		this.m_CanvasGroup.DOFade(0f, 0.25f).SetEase(Ease.Linear).SetDelay(3f)
			.OnComplete(this.PlayOutComplete);
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		this.m_CanvasGroup.DOKill();
		base.OnDisposed();
	}
}
