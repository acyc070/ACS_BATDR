using System;
using DG.Tweening;
using InControl;
using UnityEngine;

public class InsaneController : JMonoBehaviour
{
	private const float TORQUE = 30f;

	[Header("Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[Header("Options")]
	[SerializeField]
	private Animator m_Animator;

	[Header("Left Leg")]
	[SerializeField]
	private Rigidbody m_LeftLeg;

	[SerializeField]
	private Rigidbody m_LeftShin;

	[SerializeField]
	private Rigidbody m_LeftThigh;

	[Header("Right Leg")]
	[SerializeField]
	private Rigidbody m_RightLeg;

	[SerializeField]
	private Rigidbody m_RightShin;

	[SerializeField]
	private Rigidbody m_RightThigh;

	[Header("Other")]
	[SerializeField]
	private Rigidbody m_Neck;

	[SerializeField]
	private Rigidbody m_Head;

	[SerializeField]
	private Rigidbody m_Pelvis;

	private float m_StartTimerMax = 4f;

	private float m_StartTimer;

	private float m_TimerMax = 0.75f;

	private float m_LegTimerL;

	private float m_LegTimerR;

	private float m_HeadTimer;

	private float m_PelvisTimer;

	private float m_LookTimer;

	private bool m_IsActive;

	private Rigidbody[] m_RagdollColliders;

	private InsaneFoot[] m_Feet;

	public event EventHandler OnPlay;

	public event EventHandler OnExit;

	public override void Start()
	{
		GameManager.Instance.LockPause();
		UIManager.SetCursor(active: false);
		GameManager.Instance.ClearUI();
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.Dispose();
		}
		this.m_Feet = base.GetComponentsInChildren<InsaneFoot>();
		unchecked
		{
			for (int i = 0; i < this.m_Feet.Length; i++)
			{
				this.m_Feet[i].OnInteract += this.HandleFootOnInteract;
			}
			this.m_RagdollColliders = base.GetComponentsInChildren<Rigidbody>();
			for (int j = 0; j < this.m_RagdollColliders.Length; j++)
			{
				this.m_RagdollColliders[j].isKinematic = true;
			}
			this.m_LegTimerL = 0f;
			this.m_LegTimerR = 0f;
			this.m_HeadTimer = 0f;
			this.m_PelvisTimer = 0f;
			this.m_LookTimer = 0f;
			GameManager.Instance.HideScreenBlocker(3f, 1f);
			this.OnPlay.Send(this);
		}
	}

	private void Update()
	{
		if (this.m_StartTimer >= this.m_StartTimerMax)
		{
			this.m_LegTimerL += Time.deltaTime;
			this.m_LegTimerR += Time.deltaTime;
			this.m_HeadTimer += Time.deltaTime;
			this.m_PelvisTimer += Time.deltaTime;
			this.m_LookTimer += Time.deltaTime;
		}
		else
		{
			this.m_StartTimer += Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		if (this.m_StartTimer < this.m_StartTimerMax)
		{
			return;
		}
		float num;
		num = PlayerInput.LookX();
		float num2;
		num2 = PlayerInput.LookY();
		float num3;
		num3 = 0f;
		float num4;
		num4 = 0f;
		if (!GameManager.Instance.HasController)
		{
			num3 = PlayerInput.MoveXRaw();
			num4 = PlayerInput.MoveYRaw();
		}
		else
		{
			if (GamepadInput.GetButtonDown(InputControlType.LeftTrigger))
			{
				num3 = -1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.RightTrigger))
			{
				num3 = 1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.LeftBumper))
			{
				num4 = 1f;
			}
			if (GamepadInput.GetButtonDown(InputControlType.RightBumper))
			{
				num4 = -1f;
			}
		}
		if (!this.m_IsActive)
		{
			if (num3 == 0f && num4 == 0f && num == 0f && num2 == 0f)
			{
				return;
			}
			for (int i = 0; i < this.m_RagdollColliders.Length; i = unchecked(i + 1))
			{
				if (this.m_RagdollColliders[i].isKinematic)
				{
					this.m_RagdollColliders[i].isKinematic = false;
				}
			}
			this.m_IsActive = true;
			this.m_LegTimerL = 0f;
			this.m_LegTimerR = 0f;
			this.m_HeadTimer = 0f;
			this.m_PelvisTimer = 0f;
			this.m_LookTimer = 0f;
			DOTween.Sequence().InsertCallback(10f, () =>
			{
				UICredits uICredits;
				uICredits = GameManager.Instance.UIManager.Show<UICredits>("UI/Views/UICredits", "VIEW", true);
				uICredits.OnPlayOutComplete -= this.HandleCreditsOnPlayOutComplete;
				uICredits.OnPlayOutComplete += this.HandleCreditsOnPlayOutComplete;
			});
			return;
		}
		this.m_Animator.enabled = false;
		if (this.m_LookTimer >= this.m_TimerMax)
		{
			float num5;
			num5 = num * 30f;
			float num6;
			num6 = (0f - num2) * 30f;
			this.m_Neck.AddRelativeTorque(num5, 0f, num6, ForceMode.Impulse);
			this.m_Head.AddRelativeTorque(num5 * 2f, 0f, num6 * 2f, ForceMode.Acceleration);
		}
		if (num3 > 0f && this.m_LegTimerL >= this.m_TimerMax)
		{
			this.m_LeftShin.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			this.m_LeftLeg.AddForce(Vector3.up * 30f / 4f, ForceMode.Impulse);
			this.m_LegTimerL = 0f;
		}
		else if (num3 < 0f && this.m_LegTimerR >= this.m_TimerMax)
		{
			this.m_RightShin.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			this.m_RightLeg.AddForce(Vector3.up * 30f / 4f, ForceMode.Impulse);
			this.m_LegTimerR = 0f;
		}
		if (num4 > 0f && this.m_HeadTimer >= this.m_TimerMax)
		{
			this.m_Head.AddForce(Vector3.up * 30f, ForceMode.Impulse);
			this.m_HeadTimer = 0f;
		}
		else if (num4 < 0f && this.m_PelvisTimer >= this.m_TimerMax)
		{
			this.m_Pelvis.AddForce(Vector3.up * 30f * 3f, ForceMode.Impulse);
			this.m_PelvisTimer = 0f;
		}
	}

	private void HandleFootOnInteract(object sender, EventArgs e)
	{
		Vector3 vector;
		vector = (sender as InsaneFoot).ToPosition - this.m_Pelvis.transform.position;
		this.m_Pelvis.AddForce(vector.normalized * 30f * 2f, ForceMode.Impulse);
	}

	private void HandleCreditsOnPlayOutComplete(object sender, EventArgs e)
	{
		((UICredits)sender).OnPlayOutComplete -= this.HandleCreditsOnPlayOutComplete;
		this.OnExit.Send(this);
	}

	protected override void OnDisposed()
	{
		this.OnPlay = null;
		this.OnExit = null;
		this.m_RagdollColliders = null;
		this.m_Feet = null;
		base.OnDisposed();
	}
}
