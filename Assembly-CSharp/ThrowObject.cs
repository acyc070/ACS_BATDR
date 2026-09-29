using System;
using DG.Tweening;
using UnityEngine;

public class ThrowObject : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Content;

	[SerializeField]
	private GameObject m_Particles;

	[SerializeField]
	private Collider m_Collider;

	[SerializeField]
	private bool m_DestroyOnImpact;

	private bool m_IsHit;

	private ParticleSystem[] m_ParticleSystems;

	private Sequence m_Sequence;

	public GameObject Content => this.m_Content;

	public GameObject Particles => this.m_Particles;

	public Collider Collider => this.m_Collider;

	public event EventHandler OnDrag;

	public event EventHandler OnLand;

	public event EventHandler OnThrow;

	public event EventHandler OnDragLong;

	public override void Start()
	{
		this.InitializeParticles();
	}

	public void Initialize()
	{
		this.m_IsHit = false;
		if (this.m_Content != null)
		{
			this.m_Content.SetActive(value: true);
		}
		if (this.m_Collider != null)
		{
			this.m_Collider.enabled = true;
		}
		this.InitializeParticles();
	}

	private void InitializeParticles()
	{
		if (this.m_Particles != null && this.m_ParticleSystems == null)
		{
			this.m_ParticleSystems = this.m_Particles.GetComponentsInChildren<ParticleSystem>();
		}
		this.DisableParticles();
	}

	public void Throw(Vector3 startPosition, Vector3 toPosition)
	{
		this.ResetSequence();
		base.transform.position = startPosition;
		base.transform.eulerAngles = global::UnityEngine.Random.insideUnitSphere * global::UnityEngine.Random.Range(20f, 360f);
		Vector3 endValue;
		endValue = new Vector3(360f, 720f, 0f);
		this.m_Sequence.SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
		this.m_Sequence.Insert(0f, base.transform.DOLocalRotate(endValue, 0.85f, RotateMode.LocalAxisAdd));
		this.m_Sequence.Insert(0f, base.transform.DOMove(toPosition, 0.85f));
		this.m_Sequence.OnComplete(this.ThrowOnComplete);
	}

	private void ThrowOnComplete()
	{
		if (this.m_Content != null)
		{
			this.m_Content.SetActive(value: false);
		}
		this.EnableParticles();
		this.Land();
	}

	public void EnableParticles()
	{
		this.SetParticles(play: true);
	}

	public void DisableParticles()
	{
		this.SetParticles(play: false);
	}

	private void SetParticles(bool play)
	{
		if (this.m_ParticleSystems == null)
		{
			return;
		}
		for (int i = 0; i < this.m_ParticleSystems.Length; i = unchecked(i + 1))
		{
			ParticleSystem particleSystem;
			particleSystem = this.m_ParticleSystems[i];
			if (play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void Drag()
	{
		CameraEffects.ShakeRotation(0.5f, 1f, 10, 90f, fadeOut: false);
		this.OnDrag.Send(this);
	}

	public void Land()
	{
		if (this.m_Collider != null)
		{
			this.m_Collider.enabled = false;
		}
		this.m_IsHit = true;
		this.EnableParticles();
		CameraEffects.ShakeRotation(0.5f, 2f);
		this.OnLand.Send(this);
	}

	public void Throw()
	{
		this.OnThrow.Send(this);
	}

	public void DragLong()
	{
		CameraEffects.ShakeRotation(3.5f, 1f);
		this.OnDragLong.Send(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.m_IsHit && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			this.m_IsHit = true;
			RaycastHit hit;
			hit = new RaycastHit
			{
				point = other.transform.position
			};
			Vector3 point;
			point = hit.point;
			point.y = GameManager.Instance.Player.transform.position.y - 0.5f;
			GameManager.Instance.Player.AddForce((point - base.transform.position).normalized * 20f);
			GameManager.Instance.Player.Hit(hit, null, DamageCheck.ThrowableGear());
			if (this.m_DestroyOnImpact)
			{
				this.KillSequence();
				this.ThrowOnComplete();
			}
		}
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
		this.OnDrag = null;
		this.OnLand = null;
		this.OnThrow = null;
		this.OnDragLong = null;
		this.KillSequence();
		base.OnDisposed();
	}
}
