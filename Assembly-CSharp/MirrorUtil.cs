using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

internal static class MirrorUtil
{
	public static bool isPresent()
	{
		List<XRDisplaySubsystem> list;
		list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetInstances(list);
		foreach (XRDisplaySubsystem item in list)
		{
			if (item.running)
			{
				return true;
			}
		}
		return false;
	}
}
