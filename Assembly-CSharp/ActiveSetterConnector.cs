using System;
using UnityEngine;

public class ActiveSetterConnector : JMonoBehaviour
{
	[SerializeField]
	private ActionEvent m_ActionEvent;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	public override void Start()
	{
		this.m_ActionEvent.OnInteract += this.HandleActionEventOnInteract;
	}

	private void HandleActionEventOnInteract(object sender, EventArgs e)
	{
		this.m_ActiveSetter.SetActive(!this.m_ActiveSetter.IsActive);
	}
}
