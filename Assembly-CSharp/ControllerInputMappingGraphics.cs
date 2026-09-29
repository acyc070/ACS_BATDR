using System;
using InControl;
using UnityEngine;

[CreateAssetMenu]
public class ControllerInputMappingGraphics : ScriptableObject
{
	[Serializable]
	public class InputMapping
	{
		public string Name;

		public InputControlType ControlType;

		public Sprite Playstation;

		public Sprite XBox;
	}

	[Header("Input Mapping")]
	[SerializeField]
	private InputMapping[] m_InputMappings;

	public Sprite GetSprite(InputControlType inputControlType)
	{
		InputMapping[] inputMappings;
		inputMappings = this.m_InputMappings;
		foreach (InputMapping inputMapping in inputMappings)
		{
			if (inputMapping.ControlType == inputControlType)
			{
				string text;
				text = InputManager.ActiveDevice.DeviceStyle.ToString().ToLower();
				if (text.Contains("xbox"))
				{
					return inputMapping.XBox;
				}
				if (text.Contains("playstation"))
				{
					return inputMapping.Playstation;
				}
				return inputMapping.XBox;
			}
		}
		return null;
	}

	public string GetString(string input)
	{
		string text;
		text = input;
		string text2;
		text2 = InputManager.ActiveDevice.DeviceStyle.ToString().ToLower();
		if (text2.Contains("xbox"))
		{
			if (input.ToLower() == "lmb")
			{
				text = "<size=200%><sprite index=2>";
			}
			else if (input.ToLower() == "tab")
			{
				text = "<size=200%><sprite index=0>";
			}
		}
		else if (text2.Contains("playstation"))
		{
			if (input.ToLower() == "lmb")
			{
				text = "<size=200%><sprite index=3>";
			}
			else if (input.ToLower() == "tab")
			{
				text = "<size=200%><sprite index=1>";
			}
		}
		else if (input.ToLower() == "lmb")
		{
			text = "<size=200%><sprite index=2>";
		}
		else if (input.ToLower() == "tab")
		{
			text = "<size=200%><sprite index=0>";
		}
		return text + "</size>";
	}
}
