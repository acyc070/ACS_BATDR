using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S13Audio;

internal static class S13DebugCanvas
{
	private static Canvas _canvas;

	private static Dictionary<string, Text> textOutputs;

	public static void DisplayVar(string varName, object value)
	{
		if (!S13DebugCanvas._canvas)
		{
			S13DebugCanvas.CreateCanvas();
		}
		if (!S13DebugCanvas.textOutputs.ContainsKey(varName))
		{
			S13DebugCanvas.CreateTextOutput(varName);
		}
		S13DebugCanvas.textOutputs[varName].text = $"{varName}: {value}";
	}

	private static void CreateTextOutput(string varName)
	{
		Text text;
		text = new GameObject(varName).AddComponent<Text>();
		text.transform.SetParent(S13DebugCanvas._canvas.transform);
		text.color = Color.white;
		text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.fontSize = S13Settings.Debug_TextSize;
		S13DebugCanvas.textOutputs.Add(varName, text);
	}

	private static void CreateCanvas()
	{
		GameObject gameObject;
		gameObject = new GameObject("S13Debug Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(VerticalLayoutGroup));
		VerticalLayoutGroup component;
		component = gameObject.GetComponent<VerticalLayoutGroup>();
		component.childForceExpandHeight = false;
		component.childAlignment = TextAnchor.MiddleLeft;
		S13DebugCanvas._canvas = gameObject.GetComponent<Canvas>();
		S13DebugCanvas._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
	}

	public static void RemoveVar(string varName)
	{
		if (S13DebugCanvas.textOutputs.ContainsKey(varName))
		{
			Text text;
			text = S13DebugCanvas.textOutputs[varName];
			S13DebugCanvas.textOutputs.Remove(varName);
			if (text != null)
			{
				Object.Destroy(text.gameObject);
			}
		}
	}

	static S13DebugCanvas()
	{
		S13DebugCanvas.textOutputs = new Dictionary<string, Text>();
	}
}
