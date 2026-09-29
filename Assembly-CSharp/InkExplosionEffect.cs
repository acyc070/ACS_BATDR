using System;
using DG.Tweening;
using UnityEngine;

public class InkExplosionEffect : JMonoBehaviour
{
	private const string MELT_PERCENTAGE = "_MeltPercentage";

	[SerializeField]
	private ParticleSystem m_Explosion;

	[SerializeField]
	private ParticleSystem m_Drops;

	private Renderer m_Renderer;

	public event EventHandler OnExplode;

	public override void Awake()
	{
		base.Awake();
		ParticleSystem.CollisionModule collision;
		collision = this.m_Drops.collision;
		collision.quality = ParticleSystemCollisionQuality.High;
		collision.radiusScale = 0.001f;
	}

	public void Birth(Renderer _renderer)
	{
		if (this.m_Renderer == null)
		{
			this.m_Renderer = _renderer;
		}
		this.Explode(10, 10);
	}

	public void ExplodeOnly()
	{
		this.Explode(10, 10);
	}

	public void Activate(Renderer _renderer, float meltStart = 2f, float meltDuration = 2f)
	{
		if (this.m_Renderer == null)
		{
			this.m_Renderer = _renderer;
		}
		Sequence s;
		s = DOTween.Sequence();
		float num;
		num = meltStart;
		if (this.m_Renderer.material.HasProperty("_MeltPercentage"))
		{
			s.Insert(num, this.m_Renderer.material.DOFloat(1f, "_MeltPercentage", meltDuration).SetEase(Ease.Linear));
		}
		num += meltDuration;
		s.InsertCallback(num, this.DeathExplode);
		s.InsertCallback(num + 2f, this.SendOnExplode);
	}

	private void DeathExplode()
	{
		this.Explode(20, 10);
		this.m_Renderer.enabled = false;
	}

	private void Explode(int explosionCount, int dropCount)
	{
		base.transform.SetParent(null);
		ParticleSystem.ShapeModule shape;
		shape = this.m_Explosion.shape;
		shape.skinnedMeshRenderer = this.m_Renderer as SkinnedMeshRenderer;
		this.m_Explosion.Emit(explosionCount);
		ParticleSystem.ShapeModule shape2;
		shape2 = this.m_Drops.shape;
		shape2.skinnedMeshRenderer = this.m_Renderer as SkinnedMeshRenderer;
		this.m_Drops.Emit(dropCount);
	}

	private void SendOnExplode()
	{
		this.OnExplode.Send(this);
		base.Dispose();
	}

	public void Reset()
	{
		this.OnExplode = null;
		this.m_Renderer.material.SetFloat("_MeltPercentage", 0f);
		this.m_Renderer.enabled = true;
	}

	protected override void OnDisposed()
	{
		this.m_Renderer = null;
		this.OnExplode = null;
		base.OnDisposed();
	}
}
