using UnityEngine;
using UnityEngine.EventSystems;

public class JInputModule : StandaloneInputModule
{
	private Vector2 m_cursorPos;

	private readonly MouseState m_MouseState = new MouseState();

	protected override MouseState GetMousePointerEventData(int id = 0)
	{
		bool pointerData;
		pointerData = base.GetPointerData(-1, out var data, create: true);
		data.Reset();
		if (pointerData)
		{
			data.position = Input.mousePosition;
		}
		Vector2 vector;
		vector = Input.mousePosition;
		data.delta = vector - data.position;
		data.position = vector;
		data.scrollDelta = Input.mouseScrollDelta;
		data.button = PointerEventData.InputButton.Left;
		base.eventSystem.RaycastAll(data, base.m_RaycastResultCache);
		RaycastResult pointerCurrentRaycast;
		pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(base.m_RaycastResultCache);
		data.pointerCurrentRaycast = pointerCurrentRaycast;
		base.m_RaycastResultCache.Clear();
		base.GetPointerData(-2, out var data2, create: true);
		base.CopyFromTo(data, data2);
		data2.button = PointerEventData.InputButton.Right;
		base.GetPointerData(-3, out var data3, create: true);
		base.CopyFromTo(data, data3);
		data3.button = PointerEventData.InputButton.Middle;
		this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, base.StateForMouseButton(0), data);
		this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, base.StateForMouseButton(1), data2);
		this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, base.StateForMouseButton(2), data3);
		return this.m_MouseState;
	}

	private void GetSelectState(PointerEventData data, PointerEventData.InputButton inputButton, bool wasPressed, bool wasReleased)
	{
		PointerEventData.FramePressState stateForMouseButton;
		stateForMouseButton = PointerEventData.FramePressState.NotChanged;
		if (wasPressed)
		{
			stateForMouseButton = PointerEventData.FramePressState.Pressed;
		}
		else if (wasReleased)
		{
			stateForMouseButton = PointerEventData.FramePressState.Released;
		}
		this.m_MouseState.SetButtonState(inputButton, stateForMouseButton, data);
	}
}
