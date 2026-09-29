using System.Collections.Generic;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13CompoundTriggerHelper : MonoBehaviour
{
	public List<IS13CompoundTrigger> owners = new List<IS13CompoundTrigger>();

	private void OnTriggerStay(Collider other)
	{
		for (int i = 0; i < this.owners.Count; i = unchecked(i + 1))
		{
			this.owners[i].RecordTriggerStay(other);
		}
	}

	public void AddOwner(IS13CompoundTrigger owner)
	{
		this.owners.Add(owner);
	}
}
