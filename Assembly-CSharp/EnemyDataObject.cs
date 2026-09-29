using System;
using UnityEngine;

[Serializable]
public class EnemyDataObject : DataObject<int, EnemyDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private int m_CurrentNodeID;

	[SerializeField]
	private int m_CombatState;

	[SerializeField]
	private GameObjectData m_GameObjectDatas;

	public override int ID => this.m_DataID;

	public EnemyType EnemyType => this.m_EnemyType;

	public LostOneType LostOneType => this.m_LostOneType;

	public int CurrentNodeID => this.m_CurrentNodeID;

	public int CombatState => this.m_CombatState;

	public GameObjectData GameObjectDatas => this.m_GameObjectDatas;

	public void SetEnemyType(EnemyType enemyType)
	{
		this.m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		this.m_LostOneType = lostOneType;
	}

	public void SetCurrentNode(int id)
	{
		this.m_CurrentNodeID = id;
	}

	public void SetCombatState(bool inCombat)
	{
		this.m_CombatState = (inCombat ? 1 : 0);
	}

	public void SetGameObjectData(GameObject gameObject)
	{
		this.m_GameObjectDatas = GameObjectData.GetData(gameObject);
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
