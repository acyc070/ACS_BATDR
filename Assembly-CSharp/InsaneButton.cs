using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InsaneButton : JMonoBehaviour
{
	[SerializeField]
	private Transform m_FootLocation;

	[SerializeField]
	private GameObject m_Active;

	[SerializeField]
	private GameObject m_Close;

	[SerializeField]
	private GameObject m_Inactive;

	private List<InsaneFoot> m_Feet = new List<InsaneFoot>();

	private bool m_IsActivated;

	public override void Awake()
	{
		this.m_Active.SetActive(value: true);
		this.m_Inactive.SetActive(value: false);
		this.m_Close.SetActive(value: false);
	}

	private void Update()
	{
		if (this.m_IsActivated || this.m_Feet.Count <= 0 || !PlayerInput.InteractOnPressed())
		{
			return;
		}
		InsaneFoot closestFoot;
		closestFoot = this.GetClosestFoot(this.m_Feet, this.m_FootLocation);
		if (closestFoot != null)
		{
			this.m_IsActivated = true;
			closestFoot.Interact(this.m_FootLocation.position);
			closestFoot.transform.parent.transform.DOMove(this.m_FootLocation.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
			{
				this.m_Active.SetActive(value: false);
				this.m_Close.SetActive(value: false);
				this.m_Inactive.SetActive(value: true);
			});
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.m_IsActivated)
		{
			return;
		}
		InsaneFoot component;
		component = other.GetComponent<InsaneFoot>();
		if (!(component == null))
		{
			if (!this.m_Feet.Contains(component))
			{
				this.m_Feet.Add(component);
			}
			this.m_Active.SetActive(value: false);
			this.m_Close.SetActive(value: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.m_IsActivated)
		{
			return;
		}
		InsaneFoot component;
		component = other.GetComponent<InsaneFoot>();
		if (!(component == null))
		{
			if (this.m_Feet.Contains(component))
			{
				this.m_Feet.Remove(component);
			}
			if (this.m_Feet.Count <= 0)
			{
				this.m_Active.SetActive(value: true);
				this.m_Close.SetActive(value: false);
			}
		}
	}

	private InsaneFoot GetClosestFoot(List<InsaneFoot> _targets, Transform fromThis)
	{
		InsaneFoot result;
		result = null;
		float num;
		num = float.PositiveInfinity;
		Vector3 position;
		position = fromThis.position;
		foreach (InsaneFoot _target in _targets)
		{
			float sqrMagnitude;
			sqrMagnitude = (_target.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = _target;
			}
		}
		return result;
	}
}
