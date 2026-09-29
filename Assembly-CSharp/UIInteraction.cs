using InControl;
using S13Audio.BATDR;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInteraction : UIController
{
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

	[Header("General")]
	[SerializeField]
	private TextMeshProUGUI m_ActionLabel;

	private InteractableDataVO m_DataVO;

	protected override void OnInitialized(object _data)
	{
		this.Show(_data);
	}

	public void Show(object _data)
	{
		this.m_DataVO = (InteractableDataVO)_data;
		this.DisableAllContainers();
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
		this.m_ActionLabel.text = this.m_DataVO.ActionLabel;
		BATDRUIAudioManager.UIInteractPromptBegin();
		base.m_Visuals.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		if (!(base.m_Visuals == null))
		{
			base.m_Visuals.gameObject.SetActive(value: false);
			BATDRUIAudioManager.UIInteractPromptEnd();
		}
	}

	public void Clear()
	{
		this.m_ActionLabel.text = "";
		this.Hide();
	}

	private void DisableAllContainers()
	{
		if (this.m_KeyboardContainer != null)
		{
			this.m_KeyboardContainer.SetActive(value: false);
		}
		if (this.m_ControllerContainer != null)
		{
			this.m_ControllerContainer.SetActive(value: false);
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
