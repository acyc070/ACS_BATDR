using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootableEnemyDataObject : DataObject<int, LootableEnemyDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private List<GameObjectData> m_GameObjectDatas;

	public override int ID => this.m_DataID;

	public EnemyType EnemyType => this.m_EnemyType;

	public LostOneType LostOneType => this.m_LostOneType;

	public List<GameObjectData> GameObjectDatas => this.m_GameObjectDatas;

	public void SetEnemyType(EnemyType enemyType)
	{
		this.m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		this.m_LostOneType = lostOneType;
	}

	public void SetGameObjectData(GameObject gameObject)
	{
		this.m_GameObjectDatas = GameObjectData.GetChildrenData(gameObject);
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
