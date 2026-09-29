using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CompanionDirectorDataObject : DataObject<int, CompanionDirectorDataObject>, IDataObject<int>, IDataObject
{
	[SerializeField]
	private int m_DataID;

	[SerializeField]
	private int m_DirectorState;

	[SerializeField]
	private TransformData m_Transform;

	[SerializeField]
	private int m_NodeID;

	[SerializeField]
	private int m_NodeState;

	[SerializeField]
	private List<int> m_RemovedNodes = new List<int>();

	public override int ID => this.m_DataID;

	public int DirectorState => this.m_DirectorState;

	public TransformData Transform => this.m_Transform;

	public int NodeID => this.m_NodeID;

	public int NodeState => this.m_NodeState;

	public List<int> RemovedNodes => this.m_RemovedNodes;

	public void SetDirectorState(bool active)
	{
		this.m_DirectorState = (active ? 1 : 0);
	}

	public void SetTransform(Transform transform)
	{
		this.m_Transform = TransformData.Get(transform);
	}

	public void SetNodeID(int id)
	{
		this.m_NodeID = id;
	}

	public void SetNodeState(bool active)
	{
		this.m_NodeState = (active ? 1 : 0);
	}

	public void RemoveNode(int nodeID)
	{
		if (!this.m_RemovedNodes.Contains(nodeID))
		{
			this.m_RemovedNodes.Add(nodeID);
		}
	}

	protected override void Deserialize()
	{
		this.m_DataID = base.m_ID;
	}
}
