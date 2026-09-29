using UnityEngine;

public class Mantle : MonoBehaviour
{
	[SerializeField]
	private FlowMantle m_FlowMantle;

	public FlowMantle FlowMantle => this.m_FlowMantle;
}
