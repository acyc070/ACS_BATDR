using S13Audio.BATDR;

public class CharacterStateDeath : CharacterState
{
	public CharacterStateDeath(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.RemoveEnemy(base.Actor);
		}
		base.Actor.CancelSlide();
		base.Actor.AudioController?.React(BATDRNpcAudioController.NPCReaction.Dead);
	}
}
