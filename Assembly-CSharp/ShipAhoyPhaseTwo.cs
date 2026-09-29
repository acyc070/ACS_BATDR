using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShipAhoyPhaseTwo : BossPhase
{
	[Header("ShipAhoy")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[SerializeField]
	private AnimationClip m_IdleAnimationClip;

	[SerializeField]
	private AnimationClip m_SwingLoopAnimationClip;

	[Header("Gear")]
	[SerializeField]
	private Transform m_ThrowGearLocation;

	[SerializeField]
	private AnimationClip m_ThrowGearsAnimationClip;

	[Header("Anchor")]
	[SerializeField]
	private ThrowObject m_Anchor;

	[SerializeField]
	private AnimationClip m_PickUpAnchorClip;

	[SerializeField]
	private AnimationClip m_PullAnimationClip;

	[Header("Controllers")]
	[SerializeField]
	private GentRechargerInstant m_GentRechargerInstant;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroups;

	public bool IsActive { get; private set; }

	public event EventHandler OnAnimationEvent;

	public event EventHandler OnAnimationEvent2;

	public event EventHandler OnAnimationComplete;

	protected override void InternalInitialize()
	{
		this.m_GentRechargerInstant.SetActive(active: true);
		this.SetIdle();
		for (int i = 0; i < this.m_AnimationClipOverrideGroups.Length; i = unchecked(i + 1))
		{
			this.m_AnimationClipOverrideGroups[i].Initialize();
		}
		this.m_CharacterContent.GenericAnimationEvents.SetReciever(this);
		this.SetActive(active: true);
	}

	protected override void InternalComplete()
	{
		this.SetIdle();
		this.SetActive(active: false);
	}

	private void SetIdle()
	{
		this.m_IdleAnimationClip.name = "Idle";
		this.m_CharacterContent.UpdateClipOverrides(this.m_IdleAnimationClip);
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
		if (this.IsActive)
		{
			this.ThrowObject();
		}
	}

	private void ThrowObject()
	{
		this.RemoveListeners();
		this.AddListeners();
		this.m_ThrowGearsAnimationClip.name = "Interact";
		this.m_CharacterContent.UpdateClipOverrides(this.m_ThrowGearsAnimationClip);
		this.m_CharacterContent.SetAnimationTrigger("Interact");
		this.m_CharacterContent.transform.eulerAngles = Vector3.zero;
	}

	private void HandleThrowObjectOnAnimationEvent(object sender, EventArgs e)
	{
		Vector3 vector;
		vector = GameManager.Instance.GameCamera.transform.position + Vector3.down;
		if (Physics.Raycast(direction: vector - this.m_ThrowGearLocation.position, origin: this.m_ThrowGearLocation.position, hitInfo: out var hitInfo, maxDistance: float.PositiveInfinity, layerMask: LayerMaskUtility.GetInvisibleColliders, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
		{
			vector = hitInfo.point;
		}
		ThrowObject component;
		component = GameManager.Instance.PoolingManager.GetFromPool("Projectiles/Throwable_Gear", 4f).GetComponent<ThrowObject>();
		component.Initialize();
		component.Throw(this.m_ThrowGearLocation.position, vector);
	}

	private void HandleThrowObjectOnAnimationEvent2(object sender, EventArgs e)
	{
		Vector3 forward;
		forward = new Vector3(GameManager.Instance.Player.transform.position.x, 0f, GameManager.Instance.Player.transform.position.z) - new Vector3(this.m_CharacterContent.transform.position.x, 0f, this.m_CharacterContent.transform.position.z);
		this.m_CharacterContent.transform.rotation = Quaternion.LookRotation(forward);
	}

	private void HandleThrowObjectOnAnimationComplete(object sender, EventArgs e)
	{
		this.RemoveListeners();
		if (this.IsActive)
		{
			this.ThrowAnchor();
		}
	}

	public void AnimationEvent()
	{
		this.OnAnimationEvent.Send(this);
	}

	public void AnimationEvent2()
	{
		this.OnAnimationEvent2.Send(this);
	}

	public void AnimationComplete()
	{
		this.OnAnimationComplete.Send(this);
	}

	private void ThrowAnchor()
	{
		this.m_SwingLoopAnimationClip.name = "Interact";
		this.m_CharacterContent.UpdateClipOverride(this.m_SwingLoopAnimationClip);
		this.m_CharacterContent.SetAnimationTrigger("Interact");
	}

	public void Land()
	{
		this.m_Anchor.Land();
	}

	public void Drag()
	{
		this.m_Anchor.Drag();
	}

	public void Throw()
	{
		this.m_Anchor.Throw();
	}

	public void DragLong()
	{
		this.m_Anchor.DragLong();
	}

	public void PickUp()
	{
		this.m_Anchor.DisableParticles();
	}

	public void ThrowComplete()
	{
		this.ThrowObject();
	}

	public void Roar()
	{
		CameraEffects.ShakeRotation(1f, 1f);
	}

	public void RoarComplete()
	{
		this.m_CharacterContent.transform.DOKill();
		Vector3 position;
		position = GameManager.Instance.Player.transform.position;
		Vector3 forward;
		forward = new Vector3(position.x, 0f, position.z) - new Vector3(this.m_CharacterContent.transform.position.x, 0f, this.m_CharacterContent.transform.position.z);
		bool flag;
		flag = true;
		float num;
		num = Vector3.Distance(position, this.m_CharacterContent.transform.position);
		if (num > 40f)
		{
			this.UpdateAnimationClips("Far");
		}
		else if (num > 30f)
		{
			this.UpdateAnimationClips("Medium");
		}
		else if (num > 14f)
		{
			this.UpdateAnimationClips("Close");
		}
		else
		{
			flag = false;
		}
		if (flag)
		{
			this.m_CharacterContent.transform.DORotate(Quaternion.LookRotation(forward).eulerAngles, 0.25f).SetEase(Ease.InSine).SetDelay(1f);
			this.m_CharacterContent.SetAnimationTrigger("Interact");
			this.m_Anchor.Initialize();
		}
		else
		{
			this.m_CharacterContent.transform.DORotate(Quaternion.LookRotation(forward).eulerAngles, 0.25f).SetEase(Ease.InSine);
			this.UpdateAnimationClips("TooClose");
			this.m_CharacterContent.SetAnimationTrigger("Interact");
		}
	}

	public void AttackClose()
	{
		Vector3 vector;
		vector = this.m_CharacterContent.transform.position + Vector3.up * 3.5f;
		float num;
		num = 15f;
		int damage;
		damage = 1;
		if (Physics.OverlapSphereNonAlloc(vector + this.m_CharacterContent.transform.forward * (num / 2f), results: new Collider[1], radius: num / 2f, layerMask: LayerMaskUtility.Player(), queryTriggerInteraction: QueryTriggerInteraction.Ignore) > 0 && GameManager.Instance.Player != null)
		{
			RaycastHit hit;
			hit = new RaycastHit
			{
				point = this.m_CharacterContent.transform.position
			};
			GameManager.Instance.Player.Hit(hit, null, damage);
			Vector3 force;
			force = this.m_CharacterContent.transform.forward * 15f;
			force.y += 2f;
			GameManager.Instance.Player.AddForce(force);
		}
	}

	public void AttackCloseComplete()
	{
		this.ThrowAnchor();
		this.Recenter();
	}

	public void Recenter()
	{
		this.m_CharacterContent.transform.DOKill();
		this.m_CharacterContent.transform.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.InSine);
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list;
		list = new List<AnimationClip>();
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipOverrideGroups.Length; i++)
			{
				AnimationClipOverrideGroup animationClipOverrideGroup;
				animationClipOverrideGroup = this.m_AnimationClipOverrideGroups[i];
				if (animationClipOverrideGroup.Name == name)
				{
					for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
					{
						list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
					}
					break;
				}
			}
			this.m_CharacterContent.UpdateClipOverrides(list.ToArray());
		}
	}

	private void AddListeners()
	{
		this.OnAnimationEvent += this.HandleThrowObjectOnAnimationEvent;
		this.OnAnimationEvent2 += this.HandleThrowObjectOnAnimationEvent2;
		this.OnAnimationComplete += this.HandleThrowObjectOnAnimationComplete;
	}

	private void RemoveListeners()
	{
		this.OnAnimationEvent -= this.HandleThrowObjectOnAnimationEvent;
		this.OnAnimationEvent2 -= this.HandleThrowObjectOnAnimationEvent2;
		this.OnAnimationComplete -= this.HandleThrowObjectOnAnimationComplete;
	}

	protected override void OnDisposed()
	{
		this.OnAnimationEvent = null;
		this.OnAnimationEvent2 = null;
		this.OnAnimationComplete = null;
		this.RemoveListeners();
		if (this.m_CharacterContent != null)
		{
			this.m_CharacterContent.transform.DOKill();
		}
		base.OnDisposed();
	}
}
