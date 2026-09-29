using System;
using UnityEngine;

public class RagdollController : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Model;

	private Ragdoll[] m_Ragdolls;

	private bool m_IsActive;

	public override void Awake()
	{
		this.m_Ragdolls = this.m_Model.GetComponentsInChildren<Ragdoll>(includeInactive: true);
		for (int i = 0; i < this.m_Ragdolls.Length; i = unchecked(i + 1))
		{
			this.m_Ragdolls[i].OnHit -= this.HandleRagdollOnHit;
			this.m_Ragdolls[i].OnHit += this.HandleRagdollOnHit;
		}
	}

	private void HandleRagdollOnHit(object sender, EventArgs e)
	{
		for (int i = 0; i < this.m_Ragdolls.Length; i = unchecked(i + 1))
		{
			Ragdoll obj;
			obj = this.m_Ragdolls[i];
			obj.Rigidbody.WakeUp();
			obj.Enable();
		}
	}

	private void FixedUpdate()
	{
		if (!this.m_IsActive || this.m_Model == null || this.m_Ragdolls == null || this.m_Ragdolls.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		bool flag;
		flag = false;
		for (int i = 0; i < this.m_Ragdolls.Length; i = unchecked(i + 1))
		{
			Ragdoll obj;
			obj = this.m_Ragdolls[i];
			obj.Rigidbody.sleepThreshold = 1f;
			if (obj.Rigidbody.IsSleeping())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.Sleep();
		}
	}

	public void Sleep()
	{
		for (int i = 0; i < this.m_Ragdolls.Length; i = unchecked(i + 1))
		{
			this.m_Ragdolls[i].Disable();
		}
	}

	public void Activate(RaycastHit hit, bool isColor = false)
	{
		this.m_IsActive = true;
		unchecked
		{
			for (int i = 0; i < this.m_Ragdolls.Length; i++)
			{
				this.m_Ragdolls[i].Activate(isColor);
			}
			for (int j = 0; j < this.m_Ragdolls.Length; j++)
			{
				this.m_Ragdolls[j].AddForce(50f, hit.point, 20f, 1f);
			}
		}
	}

	public void ForceActivate()
	{
		this.m_IsActive = true;
		for (int i = 0; i < this.m_Ragdolls.Length; i = unchecked(i + 1))
		{
			this.m_Ragdolls[i].Activate();
		}
	}
}
