using System.Collections.Generic;
using UnityEngine;

public class LightGroups : MonoBehaviour
{
	public List<LightGroup> lightGroups;

	private void OnValidate()
	{
		this.lightGroups.Clear();
		this.lightGroups.AddRange(base.GetComponentsInChildren<LightGroup>());
	}

	public void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (LightGroup lightGroup in this.lightGroups)
		{
			if ((bool)lightGroup)
			{
				lightGroup.OnDrawGizmosSelected();
			}
		}
	}

	public void TurnLightGroupOn(string groupName)
	{
		this.ToggleLightGroup(groupName);
	}

	public void TrunLightGroupOff(string groupName)
	{
		this.ToggleLightGroup(groupName, turnOn: false);
	}

	public void TurnLightGroupOn(GameObject groupParentObject)
	{
		this.ToggleLightGroup(groupParentObject);
	}

	public void TurnLightGroupOff(GameObject groupParentObject)
	{
		this.ToggleLightGroup(groupParentObject, turnOn: false);
	}

	private void ToggleLightGroup(string groupName, bool turnOn = true)
	{
		foreach (LightGroup lightGroup in this.lightGroups)
		{
			if (lightGroup.name.ToLower() == groupName.ToLower())
			{
				lightGroup.ToggleLightGroup(turnOn);
				break;
			}
		}
	}

	private void ToggleLightGroup(GameObject groupParent, bool turnOn = true)
	{
		foreach (LightGroup lightGroup in this.lightGroups)
		{
			if (lightGroup.gameObject == groupParent)
			{
				lightGroup.ToggleLightGroup(turnOn);
				break;
			}
		}
	}
}
