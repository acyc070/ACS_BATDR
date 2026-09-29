using UnityEngine;

public class Collectable : ActionEventController<CollectableContent, CollectableData>
{
	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.InteractableTransform.position, "Collectable Icon", allowScaling: true);
	}
}
