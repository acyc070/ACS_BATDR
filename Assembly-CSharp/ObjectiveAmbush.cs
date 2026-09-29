using UnityEngine;

public class ObjectiveAmbush : Objective
{
	[Header("Enemy Controller")]
	[SerializeField]
	private SectionEnemyController m_SectionEnemyController;

	[Header("Enemey")]
	[SerializeField]
	private EnemySelector m_EnemySelector;

	[SerializeField]
	private CharacterNode m_CharacterNode;

	[Header("Options")]
	[SerializeField]
	private bool m_IsAmbush = true;

	protected override void InternalInitialize()
	{
		Enemy enemy;
		enemy = this.m_SectionEnemyController.Spawn(this.m_EnemySelector, this.m_CharacterNode, getNewNode: true, this.m_IsAmbush);
		enemy.SetSection(this.m_SectionEnemyController.Section.SectionID);
		enemy.Initialize();
		enemy.AddData();
		base.SendOnComplete();
	}
}
