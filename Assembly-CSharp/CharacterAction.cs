using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action")]
public class CharacterAction : ScriptableObject
{
	[SerializeField]
	private string m_ActionName = "Action";

	[SerializeField]
	private string m_AnimationTrigger = "";

	public string Name => this.m_ActionName;

	public string AnimationTrigger => this.m_AnimationTrigger;

	protected virtual State.Character m_CharacterState { get; }

	public State.Character CharacterState => this.m_CharacterState;

	public void Action(Character character)
	{
		this.OnAction(character);
	}

	protected virtual void OnAction(Character character)
	{
	}

	public bool Validate(Character character, Transform target)
	{
		int num;
		if (!character)
		{
			num = 0;
		}
		else
		{
			num = (this.OnValidated(character, target) ? 1 : 0);
			if (num != 0)
			{
				character.SetCharacterAction(this);
				character.SetState(this.CharacterState);
			}
		}
		return unchecked((byte)num) != 0;
	}

	protected virtual bool OnValidated(Character character, Transform target)
	{
		return true;
	}
}
