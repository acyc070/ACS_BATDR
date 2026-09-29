using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIElementSlider : UIElement
{
	[Header("UI Type")]
	[SerializeField]
	private bool m_IsTitle;

	[Header("Slider")]
	[SerializeField]
	protected UISlider m_Slider;

	[Header("Label")]
	[SerializeField]
	protected UIElementLabel m_Label;

	[Header("Slider Background")]
	[SerializeField]
	private Image m_SliderBG;

	[SerializeField]
	private Color m_SliderBGColor;

	[SerializeField]
	private Color m_SliderBGColorReal;

	[Header("Slider Counter")]
	[SerializeField]
	private RectTransform m_SliderCountContainer;

	[Header("Select Settings")]
	[SerializeField]
	protected Color m_ActiveColor;

	[SerializeField]
	protected Color m_InactiveColor;

	[SerializeField]
	protected Color m_ActiveColorReal;

	[SerializeField]
	protected Color m_InactiveColorReal;

	private Sequencer m_Sequencer;

	private UIElementSliderDataVO m_DataVO;

	private Image[] m_SliderCounters;

	public UISlider Slider => this.m_Slider;

	public UIElementLabel Label => this.m_Label;

	protected Color ActiveColor
	{
		get
		{
			if (this.m_IsTitle)
			{
				return this.m_ActiveColor;
			}
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_ActiveColor;
			}
			return this.m_ActiveColorReal;
		}
	}

	protected Color InactiveColor
	{
		get
		{
			if (this.m_IsTitle)
			{
				return this.m_InactiveColor;
			}
			if (!GameManager.Instance.IsRealWorld)
			{
				return this.m_InactiveColor;
			}
			return this.m_InactiveColorReal;
		}
	}

	public Action Callback { get; protected set; }

	public string InvokeCallback { get; protected set; }

	public bool IsSelected { get; private set; }

	public event EventHandler OnEnter;

	public event EventHandler OnExit;

	public event EventHandler OnClick;

	public override void Initialize(object _data)
	{
		base.Initialize(_data);
		this.m_Sequencer = new Sequencer(isIndependent: true);
		this.m_DataVO = (UIElementSliderDataVO)_data;
		if (!this.m_IsTitle && this.m_SliderBG != null)
		{
			this.m_SliderBG.color = (GameManager.Instance.IsRealWorld ? this.m_SliderBGColorReal : this.m_SliderBGColor);
		}
		this.m_SliderCounters = this.m_SliderCountContainer.gameObject.GetComponentsInChildren<Image>(includeInactive: true);
		Image[] sliderCounters;
		sliderCounters = this.m_SliderCounters;
		for (int i = 0; i < sliderCounters.Length; i = unchecked(i + 1))
		{
			sliderCounters[i].color = this.InactiveColor;
		}
		this.m_Label.Label.text = this.m_DataVO.Label;
		this.m_Label.Label.color = this.InactiveColor;
		float value;
		value = GameManager.Instance.PlayerSettings.GetFloat(this.m_DataVO.PlayerPrefs);
		this.m_Slider.value = value;
		this.SliderOnChangedValue(value);
		this.RemoveListeners();
		this.AddListeners();
	}

	public virtual void ForceOnEnter()
	{
		this.IsSelected = true;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.ActiveColor, 0.1f));
		for (int i = 0; i < this.m_SliderCounters.Length; i = unchecked(i + 1))
		{
			Image target;
			target = this.m_SliderCounters[i];
			this.m_Sequencer?.Insert(0f, target.DOColor(this.ActiveColor, 0.1f));
		}
		GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[0]);
	}

	public virtual void ForceOnExit()
	{
		this.IsSelected = false;
		this.m_Sequencer?.CreateSequence(this.m_Label.Label.DOColor(this.InactiveColor, 0.1f));
		for (int i = 0; i < this.m_SliderCounters.Length; i = unchecked(i + 1))
		{
			Image target;
			target = this.m_SliderCounters[i];
			this.m_Sequencer?.Insert(0f, target.DOColor(this.InactiveColor, 0.1f));
		}
	}

	protected virtual void HandleButtonOnEnter(object sender, EventArgs e)
	{
		this.ForceOnEnter();
		this.OnEnter.Send(this);
	}

	protected virtual void HandleButtonOnExit(object sender, EventArgs e)
	{
		this.ForceOnExit();
		this.OnExit.Send(this);
	}

	protected virtual void HandleButtonOnClick(object sender, EventArgs e)
	{
		this.OnClick.Send(this);
	}

	private void SliderOnChangedValue(float value)
	{
		unchecked
		{
			int num;
			num = (int)(value * 10f) + 1;
			if (value == 0f)
			{
				num = 1;
			}
			else if (num <= 1)
			{
				num = 1;
			}
			else if (num >= 10)
			{
				num = 10;
			}
			for (int i = 0; i < this.m_SliderCounters.Length; i++)
			{
				Image image;
				image = this.m_SliderCounters[i];
				if (i <= 0)
				{
					image.enabled = true;
				}
				else if (num <= i)
				{
					image.enabled = false;
				}
				else
				{
					image.enabled = true;
				}
			}
			float value2;
			value2 = value;
			if (value > 0.45f && value < 0.55f)
			{
				value2 = 0.5f;
			}
			GameManager.Instance.PlayerSettings.SetFloat(this.m_DataVO.PlayerPrefs, value2);
		}
	}

	public void Increase()
	{
		if (!(this.m_Slider.value >= 1f))
		{
			float num;
			num = this.m_Slider.value + 0.1f;
			if (num >= 1f)
			{
				num = 1f;
			}
			this.m_Slider.value = num;
		}
	}

	public void Reduce()
	{
		if (!(this.m_Slider.value <= 0f))
		{
			float num;
			num = this.m_Slider.value - 0.1f;
			if (num <= 0f)
			{
				num = 0f;
			}
			this.m_Slider.value = num;
		}
	}

	public virtual void AddListeners()
	{
		this.m_Slider.OnEnter += this.HandleButtonOnEnter;
		this.m_Slider.OnExit += this.HandleButtonOnExit;
		this.m_Slider.onValueChanged.AddListener(this.SliderOnChangedValue);
	}

	public virtual void RemoveListeners()
	{
		this.m_Slider.OnEnter -= this.HandleButtonOnEnter;
		this.m_Slider.OnExit -= this.HandleButtonOnExit;
		this.m_Slider.onValueChanged.RemoveListener(this.SliderOnChangedValue);
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnEnter = null;
		this.OnExit = null;
		this.OnClick = null;
		this.Callback = null;
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
