public class CharacterStateCutscene : CharacterState
{
	public CharacterStateCutscene(Character character, State.Character state)
		: base(character, state)
	{
	}

	public override void InternalOnStateEnter()
	{
		base.Actor.CancelPath();
	}
}
