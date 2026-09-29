using UnityEngine;

public class ParentSetter : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Parent;

	public override void Start()
	{
		base.transform.SetParent(this.m_Parent);
	}
}
