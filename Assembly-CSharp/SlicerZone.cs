using System;
using DG.Tweening;
using UnityEngine;

public class SlicerZone : ButcherGangSpawner
{
	[SerializeField]
	private Transform m_Test;

	[SerializeField]
	private Animator m_Animator;

	[SerializeField]
	private EventTrigger m_EventTrigger;

	[SerializeField]
	private EventTrigger m_EventTriggerHit;

	[SerializeField]
	private AudioSource m_test_audiosource;

	[SerializeField]
	private ParticleSystem m_TrailParticles;

	[SerializeField]
	private AudioClip[] m_Test_AudioClips;

	[SerializeField]
	protected ControllerRumble.RUMBLE_PRESETS m_presetRumble = ControllerRumble.RUMBLE_PRESETS.NONE;

	[SerializeField]
	protected Vector2 m_CustomRumble;

	private int m_Test_AudioClipIndex;

	public ControllerRumble.RUMBLE_PRESETS PresetRumble => this.m_presetRumble;

	public Vector2 CustomRumble => this.m_CustomRumble;

	public bool IsTriggered { get; private set; }

	protected override void Enable()
	{
		this.m_TrailParticles.Stop();
		this.m_Test.gameObject.SetActive(value: false);
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.ResetAction();
		this.m_EventTrigger.SetActive(active: true);
	}

	protected override void Disable()
	{
		if (!this.IsTriggered)
		{
			this.m_TrailParticles.Stop();
			this.m_Test.gameObject.SetActive(value: false);
			this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
			this.m_EventTrigger.SetActive(active: false);
		}
	}

	private void HandleEventTriggerOnEnter(object sender, EventArgs e)
	{
		this.m_EventTrigger.OnEnter -= this.HandleEventTriggerOnEnter;
		this.IsTriggered = true;
		if (base.IsActive)
		{
			Sequence s;
			s = DOTween.Sequence();
			s.InsertCallback(0f, this.SetLocation);
			s.InsertCallback(0.75f, this.Attack);
			s.InsertCallback(1.75f, this.AttackOnComplete);
		}
		else
		{
			this.AttackOnComplete();
		}
	}

	private void SetLocation()
	{
		Vector3 forward;
		forward = GameManager.Instance.Player.transform.forward;
		this.m_Test.position = base.transform.position + forward * 7.5f;
		this.m_Test.LookAt(base.transform.position, Vector3.up);
		this.m_Test.gameObject.SetActive(value: true);
		this.m_test_audiosource.PlayOneShot(this.m_Test_AudioClips[this.m_Test_AudioClipIndex]);
		unchecked
		{
			this.m_Test_AudioClipIndex++;
			if (this.m_Test_AudioClipIndex >= this.m_Test_AudioClips.Length)
			{
				this.m_Test_AudioClipIndex = 0;
			}
		}
	}

	private void Attack()
	{
		this.m_EventTriggerHit.OnEnter -= this.HandleEventTriggerHitOnEnter;
		this.m_EventTriggerHit.OnEnter += this.HandleEventTriggerHitOnEnter;
		this.m_EventTriggerHit.ResetAction();
		this.m_EventTriggerHit.SetActive(active: true);
		Vector3 forward;
		forward = this.m_Test.forward;
		this.m_Animator.SetTrigger("Interact");
		this.m_TrailParticles.Play();
		ShortcutExtensions.DOMove(endValue: this.m_Test.position + forward * 15f, target: this.m_Test, duration: 1f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed)
			.OnComplete(() =>
			{
				this.m_EventTriggerHit.OnEnter -= this.HandleEventTriggerHitOnEnter;
				this.m_EventTriggerHit.SetActive(active: false);
				this.m_TrailParticles.Stop();
				this.m_Test.gameObject.SetActive(value: false);
			});
		if (this.m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[unchecked((int)this.m_presetRumble)]);
		}
		else
		{
			GameManager.Instance.TriggerRumble(this.m_CustomRumble);
		}
	}

	private void HandleEventTriggerHitOnEnter(object sender, EventArgs e)
	{
		this.m_EventTriggerHit.OnEnter -= this.HandleEventTriggerHitOnEnter;
		this.m_EventTriggerHit.SetActive(active: false);
		RaycastHit hit;
		hit = new RaycastHit
		{
			point = this.m_Test.position
		};
		Vector3 force;
		force = (GameManager.Instance.Player.transform.position - hit.point).normalized * 30f;
		GameManager.Instance.Player.AddForce(force);
		int damage;
		damage = 1;
		if (GameManager.Instance.Player.Health <= 1f)
		{
			damage = 0;
		}
		GameManager.Instance.Player.Hit(hit, null, damage);
	}

	private void AttackOnComplete()
	{
		this.m_Test.gameObject.SetActive(value: false);
		this.m_EventTrigger.OnEnter += this.HandleEventTriggerOnEnter;
		this.m_EventTrigger.ResetAction();
		this.m_EventTrigger.SetActive(active: true);
		this.IsTriggered = false;
		base.SendOnReturned();
	}
}
