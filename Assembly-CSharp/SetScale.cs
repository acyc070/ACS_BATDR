using UnityEngine;

public class SetScale : JComponent
{
	[SerializeField]
	private Vector3 m_WorldScale = Vector3.one;

	public override void Start()
	{
		Transform parent;
		parent = base.transform.parent;
		base.transform.SetParent(null);
		base.transform.localScale = this.m_WorldScale;
		base.transform.SetParent(parent);
	}
}
