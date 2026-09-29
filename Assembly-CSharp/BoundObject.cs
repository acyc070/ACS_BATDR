using UnityEngine;

public class BoundObject : MonoBehaviour
{
	[SerializeField]
	private bool _includeLightsInBounds = true;

	[HideInInspector]
	public Bounds bounds;

	public bool includeLightsInBounds => this._includeLightsInBounds;

	private void OnValidate()
	{
		this.bounds.center = base.transform.position;
		this.bounds.size = base.transform.localScale;
	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying)
		{
			Gizmos.color = (this.includeLightsInBounds ? Color.yellow : Color.cyan);
			Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
		}
	}
}
