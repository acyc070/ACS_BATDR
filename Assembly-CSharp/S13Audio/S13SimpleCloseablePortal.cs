using UnityEngine;

namespace S13Audio;

public class S13SimpleCloseablePortal : S13ClosablePortal
{
	[SerializeField]
	private bool isOpen = true;

	[Tooltip("Prevent this portal from being opened/closed.")]
	[SerializeField]
	private bool lockState;

	public void Toggle()
	{
		if (!this.lockState)
		{
			this.isOpen = !this.isOpen;
		}
	}

	public void Open()
	{
		if (!this.lockState)
		{
			this.isOpen = true;
		}
	}

	public void Close()
	{
		if (!this.lockState)
		{
			this.isOpen = false;
		}
	}

	public override bool IsOpen()
	{
		return this.isOpen;
	}
}
