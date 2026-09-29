using System;
using UnityEngine;

public class CharacterDeathSplashDamage : JMonoBehaviour
{
	[SerializeField]
	private float m_Radius = 3f;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_Percent = 0.6f;

	private Enemy m_Enemy;

	public override void Start()
	{
		this.m_Enemy = base.GetComponent<Enemy>();
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnCharacterDeath -= this.HandleCharacterOnCharacterDeath;
			this.m_Enemy.OnCharacterDeath += this.HandleCharacterOnCharacterDeath;
		}
	}

	private void HandleCharacterOnCharacterDeath(object sender, EventArgs e)
	{
		this.m_Enemy.OnCharacterDeath -= this.HandleCharacterOnCharacterDeath;
		RaycastHit hit;
		hit = new RaycastHit
		{
			point = base.transform.position + Vector3.up
		};
		Collider[] array;
		array = Physics.OverlapSphere(base.transform.position, this.m_Radius, LayerMaskUtility.Enemy() | LayerMaskUtility.IgnoreEnemy(), QueryTriggerInteraction.Ignore);
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			Enemy component;
			component = array[i].GetComponent<Enemy>();
			if (!(component == null) && !(component.transform == base.transform) && component.EnemyType == EnemyType.InkWidow && global::UnityEngine.Random.value < this.m_Percent)
			{
				component.OnHit(hit);
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Enemy != null)
		{
			this.m_Enemy.OnCharacterDeath -= this.HandleCharacterOnCharacterDeath;
			this.m_Enemy = null;
		}
		base.OnDisposed();
	}
}
