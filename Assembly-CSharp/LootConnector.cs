using System;
using System.Collections.Generic;
using UnityEngine;

public class LootConnector : JMonoBehaviour
{
	[SerializeField]
	private Loot m_Loot;

	[SerializeField]
	private LootConnectorGroup[] m_Group;

	[SerializeField]
	private ControllerRumble.RUMBLE_PRESETS m_presetRumble;

	[SerializeField]
	private Vector2 m_CustomRumble;

	private LootDrawer[] m_Drawers;

	private List<ParticleSystem> m_Particles = new List<ParticleSystem>();

	public Loot Loot => this.m_Loot;

	public LootConnectorGroup[] Group => this.m_Group;

	public ControllerRumble.RUMBLE_PRESETS PresetRumble => this.m_presetRumble;

	public Vector2 CustomRumble => this.m_CustomRumble;

	public void Initialize()
	{
		this.m_Drawers = this.m_Loot.GetComponentsInChildren<LootDrawer>(includeInactive: true);
		this.m_Loot.OnActivated -= this.HandleLootOnActivated;
		this.m_Loot.OnActivated += this.HandleLootOnActivated;
		unchecked
		{
			for (int i = 0; i < this.m_Group.Length; i++)
			{
				LootConnectorGroup lootConnectorGroup;
				lootConnectorGroup = this.m_Group[i];
				for (int j = 0; j < this.m_Drawers.Length; j++)
				{
					LootDrawer lootDrawer;
					lootDrawer = this.m_Drawers[j];
					if (lootConnectorGroup.LootDrawerType != lootDrawer.LootDrawerType)
					{
						continue;
					}
					lootConnectorGroup.Content.SetParent(lootDrawer.transform);
					Memo component;
					component = lootConnectorGroup.Content.GetComponent<Memo>();
					if (!(component != null))
					{
						continue;
					}
					ParticleSystem[] componentsInChildren;
					componentsInChildren = component.GetComponentsInChildren<ParticleSystem>();
					if (componentsInChildren != null)
					{
						foreach (ParticleSystem particleSystem in componentsInChildren)
						{
							this.m_Particles.Add(particleSystem);
							particleSystem.Stop();
						}
					}
				}
			}
		}
	}

	private void HandleLootOnActivated(object sender, EventArgs e)
	{
		this.m_Loot.OnActivated -= this.HandleLootOnActivated;
		if (this.m_Particles != null && this.m_Particles.Count > 0)
		{
			for (int i = 0; i < this.m_Particles.Count; i = unchecked(i + 1))
			{
				this.m_Particles[i].Play();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.m_Loot.OnActivated -= this.HandleLootOnActivated;
		base.OnDisposed();
	}
}
