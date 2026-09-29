using System;
using UnityEngine;

namespace S13Audio;

[AddComponentMenu("")]
public class S13PrimitiveVoiceHelper : MonoBehaviour
{
	private Transform targetParent;

	private Vector3 targetPosition;

	private Vector3 targetOffset;

	public S13Object.FollowMode followMode;

	public void SetVoiceParent(Transform parent, Vector3 offset)
	{
		this.targetOffset = offset;
		switch (this.followMode)
		{
		case S13Object.FollowMode.Update:
			this.targetParent = parent;
			base.enabled = true;
			break;
		case S13Object.FollowMode.Static:
			this.targetPosition = parent.position;
			base.transform.position = this.targetPosition + this.targetOffset;
			base.enabled = false;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void ClearVoiceParent()
	{
		this.targetParent = null;
	}

	private void Update()
	{
		if (!this.targetParent)
		{
			base.enabled = false;
			return;
		}
		switch (this.followMode)
		{
		case S13Object.FollowMode.Update:
			base.transform.position = this.targetParent.position + this.targetOffset;
			break;
		case S13Object.FollowMode.Static:
			base.transform.position = this.targetPosition + this.targetOffset;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
