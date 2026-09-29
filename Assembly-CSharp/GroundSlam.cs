using UnityEngine;

public class GroundSlam : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Content;

	public override void Awake()
	{
		this.Disable();
	}

	public void Enable()
	{
		this.m_Content?.SetActive(value: true);
	}

	public void Disable()
	{
		this.m_Content?.SetActive(value: false);
	}
}
