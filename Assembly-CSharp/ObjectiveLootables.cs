using UnityEngine;

public class ObjectiveLootables : Objective
{
	[SerializeField]
	private Loot[] m_Lootables;

	[SerializeField]
	private bool m_DisableOnComplete;

	protected override void InternalEnable()
	{
		base.Initialize();
	}

	protected override void InternalInitialize()
	{
		for (int i = 0; i < this.m_Lootables.Length; i = unchecked(i + 1))
		{
			Loot loot;
			loot = this.m_Lootables[i];
			if (!loot.Content.IsActivated)
			{
				loot.Content.Enable();
			}
		}
		if (!this.m_DisableOnComplete)
		{
			base.SendOnComplete();
		}
	}

	protected override void InternalDisable()
	{
		for (int i = 0; i < this.m_Lootables.Length; i = unchecked(i + 1))
		{
			this.m_Lootables[i].Content.Disable();
		}
		if (this.m_DisableOnComplete)
		{
			base.SendOnComplete();
		}
	}
}
