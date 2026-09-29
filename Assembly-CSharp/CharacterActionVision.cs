using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Character/New Character Action Vision")]
public class CharacterActionVision : ScriptableObject
{
	[Header("Vision Settings")]
	[SerializeField]
	[Range(0f, 180f)]
	private float m_Angle;

	[SerializeField]
	[Range(0f, 180f)]
	private float m_StealthAngle;

	[SerializeField]
	private float m_VerticalDistance = 10f;

	[SerializeField]
	private float m_TooCloseRange = 3.5f;

	[SerializeField]
	public float m_CloseRange;

	[SerializeField]
	public float m_AwareRange;

	[SerializeField]
	public float m_SightRange;

	public float Angle => this.m_Angle;

	public float StealthAngle => this.m_StealthAngle;

	public float VerticalDistance => this.m_VerticalDistance;

	public float TooCloseRange => this.m_TooCloseRange;

	public float CloseRange => this.m_CloseRange;

	public float AwareRange => this.m_AwareRange;

	public float SightRange => this.m_SightRange;

	public bool Validated(Character character)
	{
		bool result;
		result = false;
		if (VisionUtility.CheckFOV(angle: (GameManager.Instance.Player.CombatStatus == CombatStatus.Stealth) ? this.m_StealthAngle : this.m_Angle, forward: character.transform.forward, from: character.VisionPosition, to: character.Target, radius: this.m_AwareRange, layerMask: ~(1 << LayerMask.NameToLayer("AI"))))
		{
			result = true;
		}
		return result;
	}
}
