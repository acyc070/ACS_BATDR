using System;
using DG.Tweening;
using UnityEngine;

public class BreakableHatch : Breakable
{
	private const float SWITCH_HIT_ROTATION = 25f;

	private const float SWITCH_HIT_ROTATION_SPEED = 0.1f;

	private const float SWITCH_OPEN_ROTATION = 40f;

	private const float SWITCH_OPEN_ROTATION_SPEED = 0.5f;

	private const int HIT_LIMIT = 3;

	[Header("Switch")]
	[SerializeField]
	private Transform m_Switch;

	[SerializeField]
	private Transform m_SwitchEndLocation;

	[SerializeField]
	private ParticleSystem m_Sparks;

	[SerializeField]
	private Renderer[] m_SwitchRenderers;

	[Header("Chain")]
	[SerializeField]
	private Transform m_Chain;

	[SerializeField]
	private Transform m_ChainEndLocation;

	[Header("Hatch")]
	[SerializeField]
	private Transform m_Hatch;

	[SerializeField]
	private Transform m_HatchEndLocation;

	[Header("Sparkles")]
	[SerializeField]
	private GameObject m_Sparkles;

	private Sequence m_Sequence;

	private int m_HitCount;

	public bool IsOpen => this.m_HitCount >= 3;

	public event EventHandler OnSwitchHit;

	public event EventHandler OnSwitchBroken;

	public event EventHandler OnSwitchOpenStart;

	public event EventHandler OnSwitchOpen;

	public event EventHandler OnSwitchOpenEnd;

	public override void Start()
	{
		if (GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Easy && GameManager.Instance.GameData.CurrentSave.Difficulty.Difficulty != DifficultyLevel.Normal)
		{
			this.m_Sparkles.SetActive(value: false);
		}
	}

	protected override void InternalHit(RaycastHit hit)
	{
		if ((bool)this.m_Switch || (bool)this.m_SwitchEndLocation || (bool)this.m_Chain || (bool)this.m_ChainEndLocation || (bool)this.m_Hatch || (bool)this.m_HatchEndLocation)
		{
			if (!this.HitSwitchEffects())
			{
				if (this.IsOpen)
				{
					this.OpenHatch();
				}
				else
				{
					this.HitSwitch();
				}
			}
		}
		else
		{
			base.HitOnComplete();
		}
	}

	private bool HitSwitchEffects()
	{
		if (this.IsOpen)
		{
			this.m_Sparks.Emit(1);
			CameraEffects.ShakeRotation(0.4f, 1f);
			this.OnSwitchBroken.Send(this);
			return true;
		}
		this.m_Sparks.Emit(2);
		CameraEffects.ShakeRotation(0.3f, 1.75f);
		unchecked
		{
			this.m_HitCount++;
			this.OnSwitchHit.Send(this);
			return false;
		}
	}

	private void HitSwitch()
	{
		this.ResetSequence();
		this.m_Sequence.Insert(0f, this.m_Switch.DOLocalRotate(new Vector3(25f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		ShaderEffects.Fade("_PostHitGlow", 1f, 0f, 0.4f, this.m_SwitchRenderers);
	}

	private void OpenHatch()
	{
		this.OnSwitchOpenStart.Send(this);
		this.m_Sparkles.SetActive(value: false);
		this.ResetSequence();
		float num;
		num = 0f;
		this.m_Sequence.Insert(num, this.m_Switch.DOLocalRotate(new Vector3(40f, 0f, 0f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce));
		num += 0.15f;
		Vector3 endValue;
		endValue = (this.m_Chain.localPosition + this.m_ChainEndLocation.localPosition) / 2f;
		this.m_Sequence.Insert(num, this.m_Chain.DOLocalMove(endValue, 1.5f).SetEase(Ease.InSine));
		num += 0.5f;
		this.m_Sequence.InsertCallback(num, base.HitOnComplete);
		num++;
		this.m_Sequence.Insert(num, this.m_Chain.DOLocalMove(this.m_ChainEndLocation.localPosition, 1.5f).SetEase(Ease.OutBounce));
		this.m_Sequence.Insert(num, this.m_Hatch.DOLocalRotate(this.m_HatchEndLocation.localEulerAngles, 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBounce));
		this.m_Sequence.InsertCallback(num, () =>
		{
			this.OnSwitchOpen.Send(this);
		});
		this.m_Sequence.InsertCallback(num + 1f, () =>
		{
			this.OnSwitchOpenEnd.Send(this);
		});
		base.SendOnBroken();
	}

	protected override void InternalForceComplete()
	{
		this.m_Sparkles.SetActive(value: false);
		this.m_Switch.localEulerAngles = new Vector3(90f, 0f, 0f);
		this.m_Chain.localPosition = (this.m_Chain.localPosition + this.m_ChainEndLocation.localPosition) / 2f;
		this.m_Chain.localPosition = this.m_ChainEndLocation.localPosition;
		this.m_Hatch.localEulerAngles = this.m_HatchEndLocation.localEulerAngles;
		this.m_HitCount = 3;
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence();
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.OnSwitchHit = null;
		this.OnSwitchBroken = null;
		this.OnSwitchOpenStart = null;
		this.OnSwitchOpen = null;
		this.OnSwitchOpenEnd = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
