using System;
using UnityEngine;

namespace S13Audio;

public static class S13Debug
{
	[Flags]
	public enum LogType
	{
		None = 0,
		TriggerMessage = 1,
		Soundbank = 2,
		Action = 4,
		AnimationEvent = 8,
		AudioMixer = 0x10,
		Switch = 0x20,
		Accessor = 0x40,
		Sequencer = 0x80,
		Event = 0x100,
		ControlGroup = 0x200,
		AreaSound = 0x400,
		Midi = 0x800,
		All = -1
	}

	private readonly struct S13Warning(object message)
	{
		private readonly object _message = message;

		public override string ToString()
		{
			return this._message.ToString();
		}
	}

	private const string STAMP_PREFIX = "STAMP| ";

	public static void Log(object message, LogType logType = LogType.All)
	{
		S13Debug.Log(message, null, logType);
	}

	public static void Log(object message, global::UnityEngine.Object context, LogType logType = LogType.All)
	{
	}

	public static void LogWarning(object message, global::UnityEngine.Object context = null, LogType logType = LogType.All)
	{
	}

	public static void LogError(object message, global::UnityEngine.Object context = null)
	{
		Debug.LogError("STAMP| " + message, context);
	}

	public static void DisplayCanvasText(string textTitle, string text)
	{
	}

	public static void HideCanvasText(string textTitle)
	{
	}
}
