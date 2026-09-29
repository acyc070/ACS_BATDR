using UnityEngine;

namespace S13Audio;

public class S13OccludablePortal : S13ClosablePortal
{
	[SerializeField]
	private OcclusionPortal _occlusionPortal;

	private void Start()
	{
		if (this._occlusionPortal == null)
		{
			S13Debug.LogWarning("S13OccludablePortal does not have occlusionPortal", this);
		}
	}

	public override bool IsOpen()
	{
		return this._occlusionPortal.open;
	}
}
