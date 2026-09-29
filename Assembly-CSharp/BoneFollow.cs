using System;
using UnityEngine;

public class BoneFollow : JMonoBehaviour
{
	[Serializable]
	public class BoneSimulator
	{
		public Transform Simulation;

		public Transform Bone;
	}

	[SerializeField]
	private BoneSimulator[] m_BoneConnectors;

	public bool IsActive { get; private set; }

	public override void Start()
	{
		if (this.m_BoneConnectors.Length != 0)
		{
			this.m_BoneConnectors[0].Simulation.SetParent(null);
		}
		this.IsActive = true;
	}

	private void LateUpdate()
	{
		if (this.IsActive && !(GameManager.Instance.Player == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			for (int i = 0; i < this.m_BoneConnectors.Length; i = unchecked(i + 1))
			{
				BoneSimulator boneSimulator;
				boneSimulator = this.m_BoneConnectors[i];
				boneSimulator.Bone.position = boneSimulator.Simulation.position;
				boneSimulator.Bone.eulerAngles = boneSimulator.Simulation.eulerAngles;
			}
		}
	}

	public void Enable()
	{
		for (int i = 0; i < this.m_BoneConnectors.Length; i = unchecked(i + 1))
		{
			BoneSimulator boneSimulator;
			boneSimulator = this.m_BoneConnectors[i];
			Rigidbody component;
			component = boneSimulator.Bone.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
			}
			Collider component2;
			component2 = boneSimulator.Bone.GetComponent<Collider>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
			boneSimulator.Bone.position = boneSimulator.Simulation.position;
			boneSimulator.Bone.eulerAngles = boneSimulator.Simulation.eulerAngles;
		}
		this.IsActive = true;
	}

	public void Disable()
	{
		this.IsActive = false;
		for (int i = 0; i < this.m_BoneConnectors.Length; i = unchecked(i + 1))
		{
			BoneSimulator obj;
			obj = this.m_BoneConnectors[i];
			Rigidbody component;
			component = obj.Bone.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = true;
			}
			Collider component2;
			component2 = obj.Bone.GetComponent<Collider>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}
	}

	public void ClearSimulations()
	{
		for (int i = 0; i < this.m_BoneConnectors.Length; i = unchecked(i + 1))
		{
			BoneSimulator boneSimulator;
			boneSimulator = this.m_BoneConnectors[i];
			if (boneSimulator != null && boneSimulator.Simulation != null)
			{
				global::UnityEngine.Object.Destroy(boneSimulator.Simulation.gameObject);
			}
		}
	}

	[ContextMenu("Get Bones")]
	private void GetBones()
	{
		Transform[] componentsInChildren;
		componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("04"))
			{
				this.m_BoneConnectors[0].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("04"))
			{
				this.m_BoneConnectors[0].Simulation = transform;
				this.m_BoneConnectors[0].Simulation.localPosition = this.m_BoneConnectors[0].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("05"))
			{
				this.m_BoneConnectors[1].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("05"))
			{
				this.m_BoneConnectors[1].Simulation = transform;
				this.m_BoneConnectors[1].Simulation.localPosition = this.m_BoneConnectors[1].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("06"))
			{
				this.m_BoneConnectors[2].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("06"))
			{
				this.m_BoneConnectors[2].Simulation = transform;
				this.m_BoneConnectors[2].Simulation.localPosition = this.m_BoneConnectors[2].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("07"))
			{
				this.m_BoneConnectors[3].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("07"))
			{
				this.m_BoneConnectors[3].Simulation = transform;
				this.m_BoneConnectors[3].Simulation.localPosition = this.m_BoneConnectors[3].Bone.localPosition;
			}
			else if (transform.name.ToLower().Contains("bn_pipe") && transform.name.ToLower().Contains("08"))
			{
				this.m_BoneConnectors[4].Bone = transform;
			}
			else if (transform.name.ToLower().Contains("08"))
			{
				this.m_BoneConnectors[4].Simulation = transform;
				this.m_BoneConnectors[4].Simulation.localPosition = this.m_BoneConnectors[4].Bone.localPosition;
			}
		}
	}
}
