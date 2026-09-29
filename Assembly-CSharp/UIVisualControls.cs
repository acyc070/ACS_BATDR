using UnityEngine;
using UnityEngine.EventSystems;

public class UIVisualControls : JMonoBehaviour
{
	[SerializeField]
	private Camera m_UICamera;

	[SerializeField]
	private EventSystem m_EventSystem;

	[SerializeField]
	private JInputModule m_InputModule;

	public Camera UICamera => this.m_UICamera;

	public EventSystem EventSystem => this.m_EventSystem;

	public JInputModule InputModule => this.m_InputModule;
}
