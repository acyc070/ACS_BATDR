using System;
using UnityEngine;

namespace S13Audio.BATDR;

[Serializable]
public class S13AreaHandlerReference : IS13AreaSound
{
	[SerializeField]
	private S13PortalFocusAreaSound portalFocusAreaSound;

	[SerializeField]
	private S13SimpleAreaSound simpleAreaSound;

	public Vector3 targetPosition
	{
		get
		{
			if ((bool)this.portalFocusAreaSound)
			{
				return this.portalFocusAreaSound.targetPosition;
			}
			if ((bool)this.simpleAreaSound)
			{
				return this.simpleAreaSound.targetPosition;
			}
			return Vector3.zero;
		}
		set
		{
			if ((bool)this.portalFocusAreaSound)
			{
				this.portalFocusAreaSound.targetPosition = value;
			}
			else if ((bool)this.simpleAreaSound)
			{
				this.simpleAreaSound.targetPosition = value;
			}
		}
	}

	public bool targetWithinTrigger
	{
		get
		{
			if ((bool)this.portalFocusAreaSound)
			{
				return this.portalFocusAreaSound.targetWithinTrigger;
			}
			if ((bool)this.simpleAreaSound)
			{
				return this.simpleAreaSound.targetWithinTrigger;
			}
			return false;
		}
	}

	public string name
	{
		get
		{
			if ((bool)this.portalFocusAreaSound)
			{
				return this.portalFocusAreaSound.name;
			}
			if ((bool)this.simpleAreaSound)
			{
				return this.simpleAreaSound.name;
			}
			return "Empty S13AreaHandlerReference";
		}
	}

	public event Action onAreaEnter
	{
		add
		{
			if ((bool)this.portalFocusAreaSound)
			{
				this.portalFocusAreaSound.onAreaEnter += value;
			}
			else if ((bool)this.simpleAreaSound)
			{
				this.simpleAreaSound.onAreaEnter += value;
			}
		}
		remove
		{
			if ((bool)this.portalFocusAreaSound)
			{
				this.portalFocusAreaSound.onAreaEnter -= value;
			}
			else if ((bool)this.simpleAreaSound)
			{
				this.simpleAreaSound.onAreaEnter -= value;
			}
		}
	}

	public event Action onAreaExit
	{
		add
		{
			if ((bool)this.portalFocusAreaSound)
			{
				this.portalFocusAreaSound.onAreaExit += value;
			}
			else if ((bool)this.simpleAreaSound)
			{
				this.simpleAreaSound.onAreaExit += value;
			}
		}
		remove
		{
			if ((bool)this.portalFocusAreaSound)
			{
				this.portalFocusAreaSound.onAreaExit -= value;
			}
			else if ((bool)this.simpleAreaSound)
			{
				this.simpleAreaSound.onAreaExit -= value;
			}
		}
	}
}
