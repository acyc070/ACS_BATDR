using UnityEngine;

public class Lever : ActionEventController<LeverContent, LeverData>
{
	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.InteractableTransform.position, "Lever Icon", allowScaling: true);
	}
}
