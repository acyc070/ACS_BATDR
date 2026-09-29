using System;
using UnityEngine;

[Serializable]
public class CharacterDirectorDataObject : DataObject<int, CharacterDirectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private EnemyType m_EnemyType;

	[SerializeField]
	private LostOneType m_LostOneType;

	[SerializeField]
	private int m_CombatState;

	[SerializeField]
	private TransformData m_Transform;

	[SerializeField]
	private int m_CurrentPathIndex;

	[SerializeField]
	private bool m_HasSpecialPath;

	public override int ID => this.m_DataID;

	public EnemyType EnemyType => this.m_EnemyType;

	public LostOneType LostOneType => this.m_LostOneType;

	public int CombatState => this.m_CombatState;

	public TransformData Transform => this.m_Transform;

	public int CurrentPathIndex => this.m_CurrentPathIndex;

	public bool HasSpecialPath => this.m_HasSpecialPath;

	public void SetEnemyType(EnemyType enemyType)
	{
		this.m_EnemyType = enemyType;
	}

	public void SetLostOneType(LostOneType lostOneType)
	{
		this.m_LostOneType = lostOneType;
	}

	public void SetCombatState(bool inCombat)
	{
		this.m_CombatState = (inCombat ? 1 : 0);
	}

	public void SetTransform(Transform transform)
	{
		this.m_Transform = TransformData.Get(transform);
	}

	public void SetPathIndex(int index)
	{
		this.m_CurrentPathIndex = index;
	}

	public void SetSpecialPath(bool hasSpecialPath)
	{
		this.m_HasSpecialPath = hasSpecialPath;
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
