using UnityEngine;

public class ObjectiveCharacterDirector : Objective
{
	[SerializeField]
	private CharacterDirector m_CharacterDirector;

	protected override void InternalInitialize()
	{
		this.m_CharacterDirector.Initialize();
	}

	protected override void InternalDisable()
	{
		this.m_CharacterDirector.UnloadData();
	}

	protected override void InternalForceComplete()
	{
		this.m_CharacterDirector.LoadData();
	}
}
