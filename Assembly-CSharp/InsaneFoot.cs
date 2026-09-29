using System;
using UnityEngine;

public class InsaneFoot : JMonoBehaviour
{
	public Vector3 ToPosition { get; private set; }

	public event EventHandler OnInteract;

	public void Interact(Vector3 toPosition)
	{
		this.ToPosition = toPosition;
		this.OnInteract.Send(this);
	}
}
