using System;
using System.Collections.Generic;
using DG.Tweening;
using S13Audio;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class InteractableComboLock : JMonoBehaviour
{
	[Serializable]
	public class NumberWheel
	{
		public Interactable Interactable;

		public Transform Wheel;

		public int MaxIndex = 5;

		private const int DEFAULT_START_INDEX = 3;

		public int CurrentIndex { get; private set; }

		public void Initialize(int code)
		{
			this.CurrentIndex = code;
			while (this.CurrentIndex == code)
			{
				this.CurrentIndex = global::UnityEngine.Random.Range(1, this.MaxIndex);
			}
			int num;
			num = 3;
			while (num != this.CurrentIndex)
			{
				num = unchecked(num + 1);
				if (num > this.MaxIndex)
				{
					num = 1;
				}
				this.Wheel.localEulerAngles += InteractableComboLock.WHEEL_ROTATION;
			}
			this.AddListener();
		}

		public void InitializeCode(int code)
		{
			this.CurrentIndex = code;
			int num;
			num = 3;
			while (num != this.CurrentIndex)
			{
				num = unchecked(num + 1);
				if (num > this.MaxIndex)
				{
					num = 1;
				}
				this.Wheel.localEulerAngles += InteractableComboLock.WHEEL_ROTATION;
			}
			this.AddListener();
		}

		public void ForceComplete(int code)
		{
			this.CurrentIndex = code;
			int num;
			num = 3;
			while (num != this.CurrentIndex)
			{
				num = unchecked(num + 1);
				if (num > this.MaxIndex)
				{
					num = 1;
				}
				this.Wheel.localEulerAngles += InteractableComboLock.WHEEL_ROTATION;
			}
		}

		private void HandleInteractableOnInteract(object sender, EventArgs e)
		{
			this.RemoveListener();
			GameManager.Instance.Player.Interaction.ResetInteraction();
			this.DoRotate().OnComplete(unchecked(() =>
			{
				this.CurrentIndex++;
				if (this.CurrentIndex > this.MaxIndex)
				{
					this.CurrentIndex = 1;
				}
				this.AddListener();
				this.Interactable.ResetAction();
				this.Interactable.SetActive(active: true);
			}));
		}

		public Tweener DoRotate()
		{
			this.ClearTweens();
			return this.Wheel.DOLocalRotate(InteractableComboLock.WHEEL_ROTATION, 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);
		}

		public void ClearTweens()
		{
			this.Wheel.DOKill();
		}

		private void AddListener()
		{
			this.Interactable.OnInteract += this.HandleInteractableOnInteract;
		}

		public void RemoveListener()
		{
			this.Interactable.OnInteract -= this.HandleInteractableOnInteract;
		}
	}

	private static Vector3 WHEEL_ROTATION;

	[Header("Light")]
	[SerializeField]
	private GameObject m_Light;

	[Header("Button")]
	[SerializeField]
	private Interactable m_Button;

	[SerializeField]
	private Transform m_ButtonPushLocation;

	[Header("Number Wheels")]
	[SerializeField]
	private List<NumberWheel> m_NumberWheels = new List<NumberWheel>();

	[Space]
	[Header("S13Content")]
	[SerializeField]
	private S13ActionGroupReference onWrongCodeAction;

	[SerializeField]
	private S13ActionGroupReference onCorrectCodeAction;

	private Sequence m_ButtonSequence;

	private Vector3 m_ButtonDefaultPosition;

	private int m_Code = -1;

	public List<NumberWheel> NumberWheels => this.m_NumberWheels;

	public event EventHandler OnSuccess;

	public void Initialize(int code)
	{
		this.m_ButtonDefaultPosition = this.m_Button.transform.localPosition;
		string text;
		text = code.ToString();
		unchecked
		{
			for (int i = 0; i < this.m_NumberWheels.Count; i++)
			{
				NumberWheel numberWheel;
				numberWheel = this.m_NumberWheels[i];
				numberWheel.Interactable.SetActive(active: false);
				numberWheel.Initialize(text[i].ToString().ParseInt());
			}
			this.m_Button.SetActive(active: false);
			this.m_Code = code;
			this.AddListeners();
			this.m_Button.SetActive(active: true);
			for (int j = 0; j < this.m_NumberWheels.Count; j++)
			{
				this.m_NumberWheels[j].Interactable.SetActive(active: true);
			}
		}
	}

	public void InitializeCode(int code, int finalCode)
	{
		this.m_ButtonDefaultPosition = this.m_Button.transform.localPosition;
		string text;
		text = code.ToString();
		unchecked
		{
			for (int i = 0; i < this.m_NumberWheels.Count; i++)
			{
				NumberWheel numberWheel;
				numberWheel = this.m_NumberWheels[i];
				numberWheel.Interactable.SetActive(active: false);
				numberWheel.InitializeCode(text[i].ToString().ParseInt());
			}
			this.m_Button.SetActive(active: false);
			this.m_Code = finalCode;
			this.AddListeners();
			this.m_Button.SetActive(active: true);
			for (int j = 0; j < this.m_NumberWheels.Count; j++)
			{
				this.m_NumberWheels[j].Interactable.SetActive(active: true);
			}
		}
	}

	public void ForceComplete(int code)
	{
		string text;
		text = code.ToString();
		for (int i = 0; i < this.m_NumberWheels.Count; i = unchecked(i + 1))
		{
			NumberWheel numberWheel;
			numberWheel = this.m_NumberWheels[i];
			numberWheel.Interactable.SetActive(active: false);
			numberWheel.ForceComplete(text[i].ToString().ParseInt());
		}
		this.m_Light.SetActive(value: false);
		this.m_Button.SetActive(active: false);
		this.m_Button.transform.localPosition = this.m_ButtonPushLocation.localPosition;
	}

	private void HandleButtonOnInteract(object sender, EventArgs e)
	{
		this.RemoveListeners();
		this.m_Button.SetActive(active: false);
		string text;
		text = "";
		for (int i = 0; i < this.m_NumberWheels.Count; i = unchecked(i + 1))
		{
			text += this.m_NumberWheels[i].CurrentIndex;
		}
		this.EnterCode(text.ParseInt());
	}

	private void EnterCode(int code)
	{
		if (code == this.m_Code)
		{
			this.Success();
		}
		else
		{
			this.Fail();
		}
	}

	private void Success()
	{
		for (int i = 0; i < this.m_NumberWheels.Count; i = unchecked(i + 1))
		{
			NumberWheel numberWheel;
			numberWheel = this.m_NumberWheels[i];
			numberWheel.ClearTweens();
			numberWheel.RemoveListener();
			numberWheel.Interactable.SetActive(active: false);
		}
		this.m_Light.SetActive(value: false);
		this.ResetButtonSequence();
		this.m_ButtonSequence.Insert(0f, this.m_Button.transform.DOLocalMove(this.m_ButtonPushLocation.localPosition, 0.2f).SetEase(Ease.OutBack));
		this.m_ButtonSequence.OnComplete(this.SuccessOnComplete);
		this.S13OnCorrectCode();
	}

	private void SuccessOnComplete()
	{
		this.OnSuccess.Send(this);
	}

	private void Fail()
	{
		this.ResetButtonSequence();
		this.m_ButtonSequence.Insert(0f, this.m_Button.transform.DOLocalMove(this.m_ButtonPushLocation.localPosition, 0.1f).SetEase(Ease.Linear));
		this.m_ButtonSequence.Insert(0.1f, this.m_Button.transform.DOLocalMove(this.m_ButtonDefaultPosition, 0.1f).SetEase(Ease.OutSine));
		this.m_ButtonSequence.OnComplete(this.FailOnComplete);
		this.S13OnWrongCode();
	}

	private void FailOnComplete()
	{
		GameManager.Instance.Player.Interaction.ResetInteraction();
		this.AddListeners();
		this.m_Button.SetActive(active: true);
	}

	private void S13OnCorrectCode()
	{
		this.onCorrectCodeAction?.Execute();
	}

	private void S13OnWrongCode()
	{
		this.onWrongCodeAction?.Execute();
	}

	private void ResetButtonSequence()
	{
		this.KillButtonSequence();
		this.m_ButtonSequence = DOTween.Sequence();
	}

	private void KillButtonSequence()
	{
		if (this.m_ButtonSequence != null)
		{
			this.m_ButtonSequence.Kill();
			this.m_ButtonSequence = null;
		}
	}

	private void AddListeners()
	{
		this.m_Button.OnInteract += this.HandleButtonOnInteract;
	}

	private void RemoveListeners()
	{
		this.m_Button.OnInteract -= this.HandleButtonOnInteract;
	}

	protected override void OnDisposed()
	{
		this.OnSuccess = null;
		this.RemoveListeners();
		this.KillButtonSequence();
		for (int i = 0; i < this.m_NumberWheels.Count; i = unchecked(i + 1))
		{
			NumberWheel numberWheel;
			numberWheel = this.m_NumberWheels[i];
			numberWheel.ClearTweens();
			numberWheel.RemoveListener();
		}
		base.OnDisposed();
	}

	static InteractableComboLock()
	{
		InteractableComboLock.WHEEL_ROTATION = new Vector3(0f, 0f, -72f);
	}
}
