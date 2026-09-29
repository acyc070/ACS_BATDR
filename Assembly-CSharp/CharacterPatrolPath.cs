using UnityEngine;

public class CharacterPatrolPath : JMonoBehaviour
{
	[SerializeField]
	private Transform[] m_Path;

	[SerializeField]
	private bool m_IsSinglePath;

	[SerializeField]
	private bool m_IsYoYo;

	public Transform[] Path => this.m_Path;

	public int PathLength => this.m_Path.Length;

	public bool IsSinglePath => this.m_IsSinglePath;

	public bool IsYoYo => this.m_IsYoYo;
}
