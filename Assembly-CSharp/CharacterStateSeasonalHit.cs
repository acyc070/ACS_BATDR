using S13Audio.BATDR;
using UnityEngine;

public class CharacterStateSeasonalHit(Character character, State.Character state) : CharacterState(character, state)
{
	private float m_HitType;

	private float m_LastHitType;

	private float[] m_HitTypes = new float[2] { 0f, 1f };

	public override void InternalOnStateEnter()
	{
		this.CheckHitType();
		base.Actor.SetPreviousState(State.Character.Seasonal);
		base.Actor.ForceStop(smooth: false);
		base.Actor.Content.Animator.Hit(this.m_HitType);
		if (base.Actor.Target == null && base.Actor is SeasonalEnemy seasonalEnemy)
		{
			seasonalEnemy.GetRandomNode();
		}
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Hit);
	}

	private void CheckHitType()
	{
		while (this.m_HitType == this.m_LastHitType)
		{
			this.m_HitType = this.m_HitTypes[Random.Range(0, this.m_HitTypes.Length)];
		}
		this.m_LastHitType = this.m_HitType;
	}
}
