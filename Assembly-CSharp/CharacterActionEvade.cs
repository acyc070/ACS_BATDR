using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action Evade")]
public class CharacterActionEvade : CharacterAction
{
	[SerializeField]
	private Vector3Direction m_Direction = Vector3Direction.Back;

	[Header("Evade Settings")]
	[SerializeField]
	private float m_Speed = 4f;

	public Vector3Direction Direction => this.m_Direction;

	public float Speed => this.m_Speed;

	protected override State.Character m_CharacterState => State.Character.Evade;

	protected override bool OnValidated(Character character, Transform target)
	{
		bool result;
		result = false;
		if (character != null && character.transform != null && target != null)
		{
			float value;
			value = global::UnityEngine.Random.value;
			if (target.gameObject.layer == LayerMask.NameToLayer("AI"))
			{
				if (Vector3.Angle(target.position - character.transform.position, character.transform.forward) < 45f && value < 0.2f)
				{
					result = true;
				}
			}
			else if (GameManager.Instance.Player.IsAttacking && Vector3.Distance(character.transform.position, target.position) < 8f && Vector3.Angle(target.position - character.transform.position, -this.GetDirection(character)) < 30f && Vector3.Angle(character.transform.position - target.position, target.forward) < 15f && value < 0.5f)
			{
				result = true;
			}
		}
		return result;
	}

	private Vector3 GetDirection(Character character)
	{
		Transform transform;
		transform = character.transform;
		Vector3 result;
		result = transform.forward;
		if (this.m_Direction == Vector3Direction.Back)
		{
			result = -transform.forward;
		}
		else if (this.m_Direction == Vector3Direction.Left)
		{
			result = -transform.right;
		}
		else if (this.m_Direction == Vector3Direction.Right)
		{
			result = transform.right;
		}
		return result;
	}
}
