using System;
using DG.Tweening;
using UnityEngine;

public class InkDemon : JMonoBehaviour
{
	[Serializable]
	public class DeathAnimations
	{
		public AnimationClip InkDemon;

		public AnimationClip Audrey;
	}

	[Header("Status")]
	public bool IsActive;

	public bool CanUse;

	[Header("Ink Demon")]
	[SerializeField]
	private CharacterContent m_InkDemon;

	[SerializeField]
	private Animator m_Animator;

	[Header("Death Animations")]
	[SerializeField]
	private DeathAnimations[] m_DeathAnimations;

	private Transform m_Parent;

	private LayerMask m_TeleportLayers;

	private RaycastHit m_TargetHit;

	private RaycastHit m_TargetColliderHit;

	private RaycastHit m_TargetColliderToHit;

	private Vector3 m_TargetPosition;

	private Vector3 m_ToTargetPosition;

	private Vector3 m_TargetColliderPosition;

	private Vector3 m_ToTargetColliderPosition;

	private float m_Height = 8f;

	private float m_Radius = 1.25f;

	private float m_Distance = 6f;

	private bool m_IsBehind;

	public event EventHandler OnStart;

	public event EventHandler OnEnd;

	public event EventHandler OnHide;

	public event EventHandler OnBehind;

	public override void Start()
	{
		this.m_Parent = base.transform.parent;
		this.m_InkDemon.gameObject.SetActive(value: false);
		this.m_TeleportLayers = ~(1 << (LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("AI")));
	}

	public void Enable()
	{
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.Active);
		this.OnStart.Send(this);
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
	}

	private void FixedUpdate()
	{
		if (base.IsDisposed || GameManager.Instance.IsPaused || GameManager.Instance.Player == null)
		{
			return;
		}
		bool flag;
		flag = false;
		Vector3 forward;
		forward = GameManager.Instance.Player.transform.forward;
		Quaternion identity;
		identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, global::UnityEngine.Random.Range(-60, 60), 0f);
		Vector3 vector;
		vector = GameManager.Instance.Player.transform.position + Vector3.up * 0.5f;
		Vector3 vector2;
		vector2 = vector + Vector3.up * this.m_Height / 2f + Vector3.up * (0f - (this.m_Height * 0.5f - this.m_Radius));
		Vector3 p;
		p = vector2 + Vector3.up * (this.m_Height - this.m_Radius * 2f);
		this.m_TargetPosition = vector + identity * -forward * this.m_Distance;
		Color color;
		color = Color.green;
		if (!GameManager.Instance.Player.IsGrounded)
		{
			color = Color.red;
		}
		else if (this.CheckDistance(vector2, p, this.m_Radius, identity * -forward, this.m_Distance))
		{
			color = Color.red;
		}
		else
		{
			flag = true;
		}
		DebugUtility.DebugCapsule(this.m_TargetPosition, this.m_TargetPosition + Vector3.up * this.m_Height, color, this.m_Radius);
		if (!this.IsActive || GameManager.Instance.Player == null)
		{
			return;
		}
		if (flag)
		{
			if (GameManager.Instance.Player.CombatStatus != CombatStatus.Hide)
			{
				if (GameManager.Instance.Player.CurrentState == State.Player.Default)
				{
					this.Death();
				}
			}
			else
			{
				this.Hidden();
			}
		}
		else if (GameManager.Instance.Player.CombatStatus == CombatStatus.Hide)
		{
			this.Hidden();
		}
		else if (!this.m_IsBehind)
		{
			this.m_IsBehind = true;
			this.OnBehind.Send(this);
		}
	}

	private void Hidden()
	{
		this.Hide();
		this.IsActive = false;
		this.m_IsBehind = false;
		if (GameManager.Instance.InkDemonManager == null)
		{
			GameManager.Instance.InkDemonManager = InkDemonManager.Create();
		}
		GameManager.Instance.InkDemonManager.ResetTimer();
		GameManager.Instance.InkDemonManager.CheckAvailability();
		GameManager.Instance.Player.SetBattleStatus(BattleStatus.None);
	}

	private bool CheckDistance(Vector3 p1, Vector3 p2, float radius, Vector3 forward, float maxDistance)
	{
		bool result;
		result = false;
		RaycastHit hitInfo;
		if (Physics.CapsuleCast(p1, p2, radius, forward, out this.m_TargetColliderToHit, maxDistance, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			this.m_TargetPosition = new Vector3(this.m_TargetColliderToHit.point.x, this.m_TargetPosition.y, this.m_TargetColliderToHit.point.z);
			result = true;
		}
		else if (!Physics.SphereCast(p2 + forward * maxDistance, 1f, Vector3.down, out hitInfo, this.m_Height - radius, this.m_TeleportLayers, QueryTriggerInteraction.Ignore))
		{
			result = true;
		}
		return result;
	}

	private void Hide()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.Statistics.UpdateILife();
		this.OnHide.Send(this);
	}

	private void Death()
	{
		this.IsActive = false;
		this.OnEnd.Send(this);
		this.m_IsBehind = false;
		GameManager.Instance.Player.SetDisable(disable: true);
		GameManager.Instance.UIManager.Clear();
		CameraEffects.ShakeRotation(2f, 1f);
		base.transform.SetParent(null);
		this.m_TargetPosition.y = GameManager.Instance.Player.transform.position.y;
		base.transform.position = this.m_TargetPosition;
		Vector3 normalized;
		normalized = (GameManager.Instance.Player.transform.position - base.transform.position).normalized;
		base.transform.eulerAngles = Quaternion.LookRotation(normalized).eulerAngles;
		this.m_InkDemon.gameObject.SetActive(value: true);
		DeathAnimations deathAnimations;
		deathAnimations = this.m_DeathAnimations[global::UnityEngine.Random.Range(0, this.m_DeathAnimations.Length)];
		if (deathAnimations != null)
		{
			deathAnimations.InkDemon.name = "Death";
			this.m_InkDemon.UpdateClipOverrides(deathAnimations.InkDemon);
			this.m_Animator.SetTrigger("Death");
			GameManager.Instance.GameCamera.SetFirstPersonArmsActive(active: false);
			deathAnimations.Audrey.name = "Interact";
			GameManager.Instance.Player.UpdatePlayerContentClipOverrides(deathAnimations.Audrey);
			GameManager.Instance.Player.EnterInteraction("Interact");
			GameManager.Instance.Player.transform.DORotate(Quaternion.LookRotation(-normalized).eulerAngles, 0.25f).SetEase(Ease.Linear);
		}
	}

	public void Respawn()
	{
		GameManager.Instance.ShowGameOver(GameOverType.InkDemon);
		this.m_InkDemon.gameObject.SetActive(value: false);
		if (this.m_Parent != null)
		{
			base.transform.SetParent(this.m_Parent);
		}
	}

	protected override void OnDisposed()
	{
		this.OnStart = null;
		this.OnEnd = null;
		this.OnHide = null;
		this.OnBehind = null;
		this.m_Parent = null;
		base.OnDisposed();
	}
}
