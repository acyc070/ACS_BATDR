using UnityEngine;

namespace S13Audio;

public class S13CloseablePortalLink : S13ClosablePortal
{
	[SerializeField]
	private S13ClosablePortal target;

	private void Start()
	{
		if (!this.target)
		{
			S13Debug.LogWarning("S13CloseablePortalLink on " + base.name + " is not set up, removing", base.gameObject);
			Object.Destroy(this);
		}
	}

	public override bool IsOpen()
	{
		if (!(this.target == null))
		{
			return this.target.IsOpen();
		}
		return true;
	}
}
