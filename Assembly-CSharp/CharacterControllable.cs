using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CharacterControllable : Character
{
	[Header("Controllable")]
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_CameraLookAt;

	[SerializeField]
	private Transform m_GroundLocation;

	[SerializeField]
	private ParticleSystem m_GroundParticles;

	private Sequence m_SlideSequence;

	private Transform m_Freeroam;

	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	private float m_MoveY;

	private float m_LookRotation;

	private bool m_TestScale;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => PlayerInput.Run();

	public override float MoveXInput => 0f;

	public override float MoveYInput => PlayerInput.MoveY();

	public override float RotateXInput => PlayerInput.LookX();

	public override bool AttackInput => PlayerInput.Attack();

	protected override void InternalInitialize()
	{
		GameManager.Instance.BeastBendy = this;
		base.StartCoroutine(this.InitializeDelay());
	}

	private IEnumerator InitializeDelay()
	{
		yield return new WaitForSeconds(0.5f);
		this.m_CameraPivot.SetParent(null);
		this.m_CameraPivot.position = base.transform.position;
		this.m_CameraPivot.rotation = base.transform.rotation;
		this.Enable();
	}

	protected override void InternalUpdate()
	{
		if (!(this.m_Freeroam == null))
		{
			this.m_LookRotation = this.RotateXInput;
			this.m_MoveY = this.MoveYInput;
		}
	}

	protected override void InternalFixedUpdate()
	{
		if (this.m_Freeroam == null || this.GetInputs())
		{
			return;
		}
		this.m_CameraPivot.position = Vector3.Lerp(this.m_CameraPivot.position, base.transform.position, Time.deltaTime * 5f);
		Vector3 eulerAngles;
		eulerAngles = this.m_CameraPivot.eulerAngles;
		Vector3 normalized;
		normalized = (base.transform.position - this.m_CameraContainer.position).normalized;
		normalized.y *= 0f;
		float num;
		num = Vector3.Angle(base.transform.forward, normalized);
		if (base.CurrentState == State.Character.Controllable)
		{
			if (this.m_MoveY > 0f)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.m_CameraPivot.rotation, Time.deltaTime * 5f);
			}
			else if (this.m_MoveY < 0f)
			{
				if (num > 120f)
				{
					eulerAngles.y += 180f;
				}
				Quaternion b;
				b = Quaternion.Euler(eulerAngles);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, Time.deltaTime * 5f);
			}
		}
		this.m_CameraPivot.eulerAngles += new Vector3(0f, this.m_LookRotation, 0f);
		Vector3 vector;
		vector = base.transform.position + Vector3.up * 5f;
		Vector3 direction;
		direction = this.m_CameraContainer.position - vector;
		Vector3 b2;
		b2 = this.m_CameraContainer.position;
		if (Physics.SphereCast(vector, 4f, direction, out var hitInfo, 15f, ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("AI"))), QueryTriggerInteraction.Ignore))
		{
			Vector3 vector2;
			vector2 = hitInfo.point - vector;
			b2 = hitInfo.point - vector2.normalized * 2f;
		}
		b2.y = this.m_CameraContainer.position.y;
		this.m_Freeroam.position = Vector3.Lerp(this.m_Freeroam.position, b2, Time.deltaTime * 5f);
		Quaternion b3;
		b3 = Quaternion.LookRotation(this.m_CameraLookAt.position - this.m_Freeroam.position);
		this.m_Freeroam.rotation = Quaternion.Slerp(this.m_Freeroam.rotation, b3, Time.deltaTime * 8f);
	}

	private bool GetInputs()
	{
		bool result;
		result = false;
		if (base.CurrentState == State.Character.Controllable)
		{
			if (PlayerInput.Attack())
			{
				bool num;
				num = this.RunInput && this.MoveYInput > 0f;
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				base.SetState(State.Character.Attack);
				base.SetPreviousState(State.Character.Controllable);
				if (num)
				{
					if (this.m_SlideSequence != null)
					{
						this.m_SlideSequence.Kill();
					}
					this.m_SlideSequence = DOTween.Sequence();
					this.m_SlideSequence.SetUpdate(UpdateType.Fixed);
					this.m_SlideSequence.InsertCallback(0f, () =>
					{
						this.m_GroundParticles.Emit(2);
					});
					this.m_SlideSequence.InsertCallback(0.15f, () =>
					{
						this.m_GroundParticles.Emit(2);
					});
					this.m_SlideSequence.InsertCallback(0.25f, () =>
					{
						this.m_GroundParticles.Emit(2);
					});
					this.m_SlideSequence.InsertCallback(0.35f, () =>
					{
						this.m_GroundParticles.Emit(2);
					});
					this.m_SlideSequence.InsertCallback(0.45f, () =>
					{
						this.m_GroundParticles.Emit(2);
					});
					this.m_SlideSequence.OnUpdate(() =>
					{
						base.Controller.Move(base.transform.forward * 0.5f);
					});
				}
				result = true;
			}
			else if (PlayerInput.AttackSecondary())
			{
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				base.SetState(State.Character.Cutscene);
				base.SetPreviousState(State.Character.Controllable);
				base.Content.Animator.SetTrigger("AttackBehind");
				result = true;
			}
			else if (PlayerInput.Jump())
			{
				base.Content.Animator.SetMovementState(0f, smooth: false);
				base.Content.Animator.SetMovementSpeed(0f, smooth: false);
				base.Agent.CancelPath();
				base.SetState(State.Character.Cutscene);
				base.SetNextState(State.Character.Controllable);
				base.Content.Animator.SetTrigger("Alert");
				result = true;
			}
		}
		return result;
	}

	public void Enable()
	{
		base.Agent.Agent.enabled = false;
		this.m_Freeroam = GameManager.Instance.GameCamera.InitializeFreeRoamCam();
		base.SetState(State.Character.Controllable);
		base.Rotation.ResetRotation();
		base.SetActive(active: true);
	}

	public void Disable()
	{
		base.SetActive(active: false);
		base.SetState(State.Character.Cutscene);
	}

	public override void OnAttack()
	{
		RaycastHit hit;
		hit = new RaycastHit
		{
			point = base.transform.position + base.transform.forward * 5f
		};
		Collider[] array;
		array = Physics.OverlapSphere(hit.point, 10f);
		array.Shuffle();
		int num;
		num = 0;
		Collider[] array2;
		array2 = array;
		unchecked
		{
			for (int i = 0; i < array2.Length; i++)
			{
				IHittable componentInParent;
				componentInParent = array2[i].GetComponentInParent<IHittable>();
				if (componentInParent == null || componentInParent.IsBroken)
				{
					continue;
				}
				if (componentInParent is Dummy)
				{
					if (num < 5)
					{
						num++;
						componentInParent.Hit(hit);
					}
				}
				else
				{
					componentInParent.Hit(hit);
				}
			}
			CameraEffects.ShakeRotation(0.5f, 0.35f);
		}
	}

	public void OnAttacks(int amount)
	{
		string assetKey;
		assetKey = ((Random.value < 0.5f) ? "GroundCrack/Decal_Ground_Crack_01" : "GroundCrack/Decal_Ground_Crack_02");
		Transform transform;
		transform = GameManager.Instance.AssetManager.CreateAsset<Transform>(assetKey);
		transform.position = this.m_GroundLocation.position;
		transform.eulerAngles = new Vector3(this.m_GroundLocation.eulerAngles.x, Random.Range(0f, 360f), 0f);
		transform.localScale = Vector3.one * ((float)amount * 1.5f);
		Object.Destroy(transform.gameObject, 5f);
		RaycastHit hit;
		hit = new RaycastHit
		{
			point = this.m_GroundLocation.position
		};
		unchecked
		{
			Collider[] array;
			array = Physics.OverlapSphere(this.m_GroundLocation.position, amount * 4);
			array.Shuffle();
			int num;
			num = 0;
			Collider[] array2;
			array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IHittable componentInParent;
				componentInParent = array2[i].GetComponentInParent<IHittable>();
				if (componentInParent == null || componentInParent.IsBroken)
				{
					continue;
				}
				if (componentInParent is Dummy)
				{
					if (num < amount)
					{
						num++;
						componentInParent.Hit(hit);
					}
				}
				else
				{
					componentInParent.Hit(hit);
				}
			}
			CameraEffects.ShakeRotation(0.5f, amount);
			this.m_GroundParticles.Emit(5);
		}
	}

	public override void OnHit(RaycastHit hit)
	{
		if (base.m_State.ID != State.Character.Hit)
		{
			base.Content.Animator.SetMovementState(0f, smooth: false);
			base.Content.Animator.SetMovementSpeed(0f, smooth: false);
			base.Agent.CancelPath();
			base.SetState(State.Character.Hit);
		}
		base.Content.Animator.Hit(this.m_HitType);
		while (this.m_HitType == this.m_LastHitType)
		{
			this.m_HitType = this.m_HitTypes[Random.Range(0, this.m_HitTypes.Length)];
		}
		this.m_LastHitType = this.m_HitType;
	}
}
