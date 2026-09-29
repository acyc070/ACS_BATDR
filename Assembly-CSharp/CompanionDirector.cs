using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-59)]
public class CompanionDirector : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Nodes")]
	[SerializeField]
	private Transform m_CompanionNodeParent;

	[Header("Scared")]
	[SerializeField]
	private CompanionNode m_ScaredNode;

	[Header("Animation Clips")]
	[SerializeField]
	private AnimationClipOverrideGroup[] m_AnimationClipOverrideGroup;

	[Header("Prefabs")]
	[SerializeField]
	private Companion m_CompanionPrefab;

	private Companion m_Companion;

	private List<CompanionNode> m_Locations = new List<CompanionNode>();

	private SectionID m_SectionID;

	private bool m_IsScared;

	private bool m_IsInitialized;

	public bool IsInitialized => this.m_IsInitialized;

	public bool IsActive { get; private set; }

	public void SetSectionID(SectionID sectionID)
	{
		this.m_SectionID = sectionID;
	}

	public void Initialize()
	{
		if (!this.m_IsInitialized)
		{
			base.StartCoroutine(this.InternalInitialize());
		}
	}

	private IEnumerator InternalInitialize()
	{
		CompanionDirectorDataObject companionDirectorData;
		companionDirectorData = this.GetCompanionDirectorData(out var _);
		if (companionDirectorData != null && companionDirectorData.DirectorState == 1)
		{
			yield return this.InternalLoadData();
		}
		yield return null;
	}

	public void LoadData()
	{
		base.StartCoroutine(this.InternalLoadData());
	}

	private IEnumerator InternalLoadData()
	{
		CompanionDirectorDataObject companionDirectorData;
		companionDirectorData = this.GetCompanionDirectorData(out var _);
		unchecked
		{
			if (companionDirectorData != null)
			{
				this.InitializeDirector();
				CompanionNode node;
				node = null;
				for (int num = this.m_Locations.Count - 1; num >= 0; num--)
				{
					CompanionNode companionNode;
					companionNode = this.m_Locations[num];
					if (companionNode.ID != companionDirectorData.NodeID)
					{
						foreach (int removedNode in companionDirectorData.RemovedNodes)
						{
							if (companionNode.ID == removedNode && companionNode.ID != companionDirectorData.NodeID)
							{
								companionNode.OnComplete -= this.HandleCompanionNodeOnComplete;
								this.m_Locations.Remove(companionNode);
								break;
							}
						}
					}
					else
					{
						node = companionNode;
					}
				}
				Transform transform;
				transform = new GameObject().transform;
				transform.position = companionDirectorData.Transform.Position;
				transform.eulerAngles = companionDirectorData.Transform.EulerAngles;
				if (this.m_ScaredNode != null && companionDirectorData.NodeID == this.m_ScaredNode.ID)
				{
					node = this.m_ScaredNode;
				}
				this.Initialize(transform, node, companionDirectorData.NodeState == 1);
			}
			yield return null;
		}
	}

	public void UnloadData(bool disposeCopanion = false)
	{
		CompanionDirectorDataObject companionDirectorData;
		companionDirectorData = this.GetCompanionDirectorData(out var sectionData);
		if (companionDirectorData != null)
		{
			sectionData?.CompanionDirectorData.Remove(companionDirectorData);
		}
		this.IsActive = false;
		if (disposeCopanion && this.m_Companion != null)
		{
			this.m_Companion.Dispose();
			this.m_Companion = null;
		}
		this.ClearNodes();
	}

	public void UpdateData()
	{
		if (this.m_Companion != null)
		{
			CompanionDirectorDataObject companionDirectorData;
			companionDirectorData = this.GetCompanionDirectorData(out var _);
			if (companionDirectorData != null)
			{
				companionDirectorData.SetDirectorState(this.IsActive);
				companionDirectorData.SetTransform(this.m_Companion.transform);
				companionDirectorData.SetNodeID(this.m_Companion.CurrentNode.ID);
				companionDirectorData.SetNodeState(this.m_Companion.CurrentState == State.Character.Cutscene);
			}
		}
	}

	public void RemoveNodeData(int removeNode)
	{
		this.GetCompanionDirectorData(out var _)?.RemoveNode(removeNode);
	}

	private CompanionDirectorDataObject GetCompanionDirectorData(out SectionDataObject sectionData)
	{
		sectionData = null;
		CompanionDirectorDataObject result;
		result = null;
		sectionData = (SectionDataObject)GameManager.Instance.GameData.CurrentSave.DataDirectories.SectionDirectory.GetValue(this.m_SectionID);
		if (sectionData != null)
		{
			result = (CompanionDirectorDataObject)GameManager.Instance.GameData.CurrentSave.GetData<int, CompanionDirectorDataObject>(sectionData.ID, this.m_ID);
		}
		return result;
	}

	public void Initialize(Transform startLocation, CompanionNode node = null, bool isActive = false)
	{
		if (!this.m_IsInitialized)
		{
			this.InitializeDirector();
		}
		if (this.m_Companion == null && GameManager.Instance.Bendy != null)
		{
			this.m_Companion = GameManager.Instance.Bendy;
		}
		if (this.m_Companion == null)
		{
			this.m_Companion = (GameManager.Instance.Bendy = GameManager.Instance.AssetManager.CreateAsset<Companion>(this.m_CompanionPrefab));
			this.m_Companion.SetParent(base.transform);
			this.m_Companion.transform.position = startLocation.position;
			this.m_Companion.transform.eulerAngles = startLocation.eulerAngles;
			this.m_Companion.Initialize();
			this.m_Companion.SetDirector(this);
			if (node != null)
			{
				if (isActive)
				{
					this.m_Companion.Agent.Agent.enabled = false;
					this.m_Companion.SetState(State.Character.Cutscene);
					node.OnNodeReached();
					this.m_Companion.NodeReached(node, isInstant: true);
				}
				else
				{
					this.m_Companion.Agent.Agent.enabled = true;
					this.m_Companion.SetState(State.Character.Companion);
				}
				this.m_Companion.SetNode(node);
			}
			else
			{
				this.m_Companion.SetNode(this.GetClosest(startLocation.position));
			}
		}
		else
		{
			this.m_Companion.ForceExitNode(isInitialize: true);
			this.m_Companion.Director.ClearNodes();
			this.m_Companion.SetNode(null);
			this.m_Companion.SetParent(base.transform);
			this.m_Companion.Director.UnloadData();
			this.m_Companion.SetDirector(this);
			this.UpdateNodes(this.m_CompanionNodeParent);
			this.m_Companion.SetNode(this.GetClosest(GameManager.Instance.Player.transform.position));
		}
		if (node == null && this.GetCompanionDirectorData(out var sectionData) == null)
		{
			CompanionDirectorDataObject companionDirectorDataObject;
			companionDirectorDataObject = DataObject<int, CompanionDirectorDataObject>.Create(this.m_ID);
			companionDirectorDataObject.SetDirectorState(this.IsActive);
			companionDirectorDataObject.SetTransform(this.m_Companion.transform);
			companionDirectorDataObject.SetNodeID(this.m_Companion.CurrentNode.ID);
			companionDirectorDataObject.SetNodeState(this.m_Companion.CurrentState == State.Character.Cutscene);
			sectionData.CompanionDirectorData.Add(this.m_ID, companionDirectorDataObject);
		}
	}

	private void InitializeDirector()
	{
		this.m_IsInitialized = true;
		this.IsActive = true;
		CompanionNode[] componentsInChildren;
		componentsInChildren = this.m_CompanionNodeParent.GetComponentsInChildren<CompanionNode>();
		foreach (CompanionNode companionNode in componentsInChildren)
		{
			this.m_Locations.Add(companionNode);
			companionNode.OnComplete -= this.HandleCompanionNodeOnComplete;
			companionNode.OnComplete += this.HandleCompanionNodeOnComplete;
		}
		for (int j = 0; j < this.m_AnimationClipOverrideGroup.Length; j = unchecked(j + 1))
		{
			this.m_AnimationClipOverrideGroup[j].Initialize();
		}
	}

	private void HandleCompanionNodeOnComplete(object sender, EventArgs e)
	{
		CompanionNode companionNode;
		companionNode = (CompanionNode)sender;
		companionNode.OnComplete -= this.HandleCompanionNodeOnComplete;
		if (this.m_Locations.Contains(companionNode))
		{
			this.m_Locations.Remove(companionNode);
			this.m_Companion.OnAnimationComplete -= this.HandleCompanionOnAnimationComplete;
			this.m_Companion.OnAnimationComplete += this.HandleCompanionOnAnimationComplete;
			this.UpdateAnimationClips(companionNode.EventCompleteReaction);
			this.m_Companion.SetAnimationTrigger("Interact");
			this.RemoveNodeData(companionNode.ID);
		}
	}

	private void HandleCompanionOnAnimationComplete(object sender, EventArgs e)
	{
		this.m_Companion.OnAnimationComplete -= this.HandleCompanionOnAnimationComplete;
		this.m_Companion.SetNode(null);
		this.m_Companion.Agent.Agent.enabled = true;
		this.m_Companion.SetState(State.Character.Companion);
	}

	public void UpdateNodes(Transform nodeParent)
	{
		this.ClearNodes();
		this.m_Locations = new List<CompanionNode>();
		CompanionNode[] componentsInChildren;
		componentsInChildren = nodeParent.GetComponentsInChildren<CompanionNode>();
		foreach (CompanionNode companionNode in componentsInChildren)
		{
			this.m_Locations.Add(companionNode);
			companionNode.OnComplete -= this.HandleCompanionNodeOnComplete;
			companionNode.OnComplete += this.HandleCompanionNodeOnComplete;
		}
	}

	public void ClearNodes()
	{
		if (this.m_Locations != null)
		{
			for (int i = 0; i < this.m_Locations.Count; i = unchecked(i + 1))
			{
				this.m_Locations[i].OnComplete -= this.HandleCompanionNodeOnComplete;
			}
			this.m_Locations.Clear();
			this.m_Locations = null;
		}
	}

	private void Update()
	{
		if (this.m_Companion == null || this.m_Locations == null || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused || this.m_Companion.CurrentState == State.Character.Cutscene)
		{
			return;
		}
		if (GameManager.Instance.Player.CombatStatus != CombatStatus.Combat)
		{
			if (this.m_IsScared)
			{
				this.m_IsScared = false;
				this.UpdateAnimationClips("NotScared");
				this.m_Companion.Movement.SetRunSpeed(10f);
			}
			CompanionNode closest;
			closest = this.GetClosest(GameManager.Instance.Player.transform.position);
			if (this.m_Companion.CurrentNode != closest && closest != null)
			{
				if (this.m_Companion.CurrentNode != null && this.m_Companion.CurrentNode is CompanionNode companionNode)
				{
					companionNode.Disable();
				}
				this.m_Companion.SetNode(closest);
			}
			if (this.m_Companion.CurrentNode != null && Vector3.Distance(this.m_Companion.transform.position, this.m_Companion.CurrentNode.Position) <= this.m_Companion.CharacterVision.CloseRange)
			{
				this.m_Companion.CurrentNode.OnNodeReached();
			}
		}
		else if (this.m_ScaredNode != null && this.m_Companion.CurrentNode != this.m_ScaredNode && !this.m_IsScared)
		{
			this.m_IsScared = true;
			this.UpdateAnimationClips("Scared");
			this.m_Companion.SetNode(this.m_ScaredNode);
			this.m_Companion.Movement.SetRunSpeed(2f);
		}
	}

	public CompanionNode GetClosest(Vector3 _currentPosition)
	{
		CompanionNode result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		foreach (CompanionNode location in this.m_Locations)
		{
			float num2;
			num2 = Vector3.Distance(location.transform.position, _currentPosition);
			if (num2 < num)
			{
				result = location;
				num = num2;
			}
		}
		return result;
	}

	public void UpdateAnimationClips(string name)
	{
		List<AnimationClip> list;
		list = new List<AnimationClip>();
		unchecked
		{
			for (int i = 0; i < this.m_AnimationClipOverrideGroup.Length; i++)
			{
				AnimationClipOverrideGroup animationClipOverrideGroup;
				animationClipOverrideGroup = this.m_AnimationClipOverrideGroup[i];
				if (animationClipOverrideGroup.Name == name)
				{
					for (int j = 0; j < animationClipOverrideGroup.AnimationClipGroups.Length; j++)
					{
						list.Add(animationClipOverrideGroup.AnimationClipGroups[j].AnimationClip);
					}
					break;
				}
			}
			this.m_Companion.UpdateAnimationClipOverrides(list.ToArray());
		}
	}

	public void UpdateNode(CompanionNode node)
	{
		if (this.m_Companion.CurrentNode != node)
		{
			this.m_Companion.SetNode(node);
		}
	}

	protected override void OnDisposed()
	{
		if (this.m_Companion != null)
		{
			this.m_Companion.OnAnimationComplete -= this.HandleCompanionOnAnimationComplete;
			this.m_Companion = null;
		}
		if (this.m_Locations != null)
		{
			this.m_Locations.Clear();
			this.m_Locations = null;
		}
		base.OnDisposed();
	}
}
