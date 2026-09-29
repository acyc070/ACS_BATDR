using UnityEngine;

public class ImpactMaterial : JComponent
{
	[SerializeField]
	private ImpactType m_ImpactType = ImpactType.NONE;

	public ImpactType ImpactType => this.m_ImpactType;
}
