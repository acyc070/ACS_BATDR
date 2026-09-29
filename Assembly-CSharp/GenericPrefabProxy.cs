using UnityEngine;

public class GenericPrefabProxy : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Prefab;

	[SerializeField]
	private Transform m_Parent;

	public override void Awake()
	{
		if (!(this.m_Prefab == null))
		{
			Object.Instantiate(this.m_Prefab, (this.m_Parent != null) ? this.m_Parent : base.transform).name = this.m_Prefab.name;
			Object.Destroy(this);
		}
	}
}
