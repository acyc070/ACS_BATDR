using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(800)]
public class SectionLootController : SectionController
{
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeField]
	private LootGroup[] m_Group;

	private LootDrawerConnector[] m_LootDrawerConnectors;

	private LootConnector[] m_LootConnectors;

	public SectionID SectionID => this.m_SectionID;

	public LootGroup[] Group => this.m_Group;

	protected override IEnumerator InternalInitialize()
	{
		this.m_LootDrawerConnectors = base.transform.GetComponentsInChildren<LootDrawerConnector>(includeInactive: true);
		unchecked
		{
			for (int i = 0; i < this.m_LootDrawerConnectors.Length; i++)
			{
				this.m_LootDrawerConnectors[i].Initialize();
			}
			yield return null;
			this.m_LootConnectors = base.transform.GetComponentsInChildren<LootConnector>(includeInactive: true);
			for (int j = 0; j < this.m_LootConnectors.Length; j++)
			{
				this.m_LootConnectors[j].Initialize();
			}
			yield return null;
			for (int k = 0; k < this.m_Group.Length; k++)
			{
				LootGroup lootGroup;
				lootGroup = this.m_Group[k];
				if (lootGroup != null)
				{
					lootGroup.Controller.OnActivate -= this.HandleControllerOnActivate;
					if ((LootDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LootDataObject>(this.m_SectionID, lootGroup.ID) != null)
					{
						lootGroup.IsComplete = true;
						lootGroup.Controller.Content.ForceActivateComplete();
					}
					else
					{
						lootGroup.Controller.OnActivate += this.HandleControllerOnActivate;
					}
				}
			}
		}
	}

	private void HandleControllerOnActivate(object sender, EventArgs e)
	{
		Loot loot;
		loot = sender as Loot;
		loot.OnActivate -= this.HandleControllerOnActivate;
		for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
		{
			LootGroup lootGroup;
			lootGroup = this.m_Group[i];
			if (lootGroup.Controller == loot)
			{
				if ((LootDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, LootDataObject>(this.m_SectionID, lootGroup.ID) == null)
				{
					LootDataObject value;
					value = DataObject<int, LootDataObject>.Create(lootGroup.ID);
					GameManager.Instance.GameData.CurrentSave.AddData(this.m_SectionID, value);
				}
				lootGroup.IsComplete = true;
				break;
			}
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
		if (this.m_Group != null)
		{
			for (int i = 0; i < this.m_Group.Length; i = unchecked(i + 1))
			{
				this.m_Group[i].Controller.OnActivate -= this.HandleControllerOnActivate;
			}
		}
	}
}
