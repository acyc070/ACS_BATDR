using UnityEngine;

public class Switch : ActionEventController<SwitchContent, SwitchData>
{
	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.InteractableTransform.position, "Switch Icon", allowScaling: true);
	}
}
