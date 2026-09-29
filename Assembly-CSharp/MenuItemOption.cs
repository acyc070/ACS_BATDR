using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class MenuItemOption : JMonoBehaviour
{
	private const float PADDING = 10f;

	private const float LABEL_WIDTH_MAX = 281f;

	private const float STATUS_WIDTH_MAX = 147.9f;

	[SerializeField]
	private TextMeshProUGUI m_Label;

	[SerializeField]
	private TextMeshProUGUI m_Status;

	[SerializeField]
	private RectTransform m_LeftArrow;

	[SerializeField]
	private RectTransform m_RightArrow;

	[SerializeField]
	private UIButton m_LeftBtn;

	[SerializeField]
	private UIButton m_RightBtn;

	private UIButton m_Button;

	private float m_ArrowWidth;

	private bool m_HasTranslationStatus;

	public event EventHandler OnLeft;

	public event EventHandler OnRight;

	public void Init(Transform parent, string label, string status, bool hasTranslationStatus)
	{
		base.transform.SetParent(parent);
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		this.m_ArrowWidth = this.m_LeftArrow.sizeDelta.x;
		this.m_LeftArrow.anchoredPosition = Vector2.zero;
		this.m_RightArrow.anchoredPosition = Vector2.zero;
		this.m_Label.rectTransform.anchoredPosition = Vector2.zero;
		this.m_Status.rectTransform.anchoredPosition = Vector2.zero;
		string Translation;
		Translation = label;
		if (LocalizationManager.TryGetTranslation(label, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
		{
			label = Translation;
		}
		this.m_Label.text = label.ToUpper() + (label.Contains(":") ? "" : ":");
		this.m_HasTranslationStatus = hasTranslationStatus;
		if (this.m_HasTranslationStatus)
		{
			string Translation2;
			Translation2 = status;
			if (LocalizationManager.TryGetTranslation(status, out Translation2, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
			{
				status = Translation2;
			}
		}
		this.m_Status.text = status.ToUpper();
		this.SetOptionAnchor(ref this.m_Label, ref this.m_Status);
		this.m_LeftBtn.OnClick += this.HandleLeftBtnOnClick;
		this.m_RightBtn.OnClick += this.HandleRightBtnOnClick;
		this.m_LeftArrow.gameObject.SetActive(value: false);
		this.m_RightArrow.gameObject.SetActive(value: false);
		this.m_Button = base.gameObject.GetComponent<UIButton>();
		this.m_Button.OnEnter += this.HandleButtonOnEnter;
		this.m_Button.OnExit += this.HandleButtonOnExit;
	}

	public void UpdateValue(string value, bool isLanguage = false)
	{
		string text;
		text = value;
		if (this.m_HasTranslationStatus)
		{
			string Translation;
			Translation = text;
			if (LocalizationManager.TryGetTranslation(text, out Translation, FixForRTL: true, 0, ignoreRTLnumbers: true, applyParameters: true))
			{
				text = Translation;
			}
			text = text.ToUpper();
			if (isLanguage)
			{
				LocalizationManager.UpdateSources();
				LocalizationManager.LocalizeAll(Force: true);
				LocalizationManager.InitializeIfNeeded();
			}
		}
		this.m_Status.text = text;
	}

	public void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.m_LeftArrow.gameObject.SetActive(value: true);
		this.m_RightArrow.gameObject.SetActive(value: true);
	}

	public void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.m_LeftArrow.gameObject.SetActive(value: false);
		this.m_RightArrow.gameObject.SetActive(value: false);
	}

	private void HandleLeftBtnOnClick(object sender, EventArgs e)
	{
		this.PressLeftArrow();
	}

	public void PressLeftArrow()
	{
		this.OnLeft.Send(this);
	}

	private void HandleRightBtnOnClick(object sender, EventArgs e)
	{
		this.PressRightArrow();
	}

	public void PressRightArrow()
	{
		this.OnRight.Send(this);
	}

	public void SetOptionAnchor(ref TextMeshProUGUI label, ref TextMeshProUGUI status)
	{
		label.ForceMeshUpdate();
		Vector2 sizeDelta;
		sizeDelta = label.rectTransform.sizeDelta;
		sizeDelta.x = label.renderedWidth;
		label.rectTransform.sizeDelta = sizeDelta;
		label.ForceMeshUpdate();
		status.ForceMeshUpdate();
		Vector2 sizeDelta2;
		sizeDelta2 = status.rectTransform.sizeDelta;
		float num;
		num = (sizeDelta2.x = Mathf.Clamp(status.renderedWidth + 20f, status.renderedWidth + 20f, 147.9f));
		status.rectTransform.sizeDelta = sizeDelta2;
		status.ForceMeshUpdate();
		float num2;
		num2 = label.renderedWidth / 2f;
		float num3;
		num3 = num / 2f;
		label.rectTransform.anchoredPosition = new Vector2(0f - num3 - 10f, 0f);
		status.rectTransform.anchoredPosition = new Vector3(num2, 0f);
		float num4;
		num4 = num2 + num3 + this.m_ArrowWidth + 10f;
		this.m_LeftArrow.anchoredPosition = new Vector2(0f - num4, 0f);
		this.m_RightArrow.anchoredPosition = new Vector2(num4 - 10f, 0f);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
