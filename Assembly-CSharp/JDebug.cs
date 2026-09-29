using System;
using UnityEngine;

public static class JDebug
{
	[Flags]
	public enum JDebugType
	{
		None = 0,
		Initialize = 1,
		PlayerPrefs = 2,
		SaveData = 4,
		SectionController = 8,
		Cutscene = 0x10,
		Door = 0x20,
		Loot = 0x40,
		Breakable = 0x80,
		Zones = 0x100,
		UI = 0x200,
		Player = 0x400,
		Objectives = 0x800,
		Collectables = 0x1000,
		AI = 0x2000,
		All = -1
	}

	private readonly struct JDebugWarning(object message)
	{
		private readonly object _message = message;

		public override string ToString()
		{
			return this._message.ToString();
		}
	}

	private const string DEBUG_PREFIX = "[JDS] - ";

	public static void Log(object message, JDebugType debugType = JDebugType.All)
	{
		JDebug.Log(message, null, debugType);
	}

	public static void Log(object message, global::UnityEngine.Object context = null, JDebugType debugType = JDebugType.All)
	{
	}

	public static void LogWarning(object message, JDebugType debugType = JDebugType.All)
	{
		JDebug.LogWarning(message, null, debugType);
	}

	public static void LogWarning(object message, global::UnityEngine.Object context = null, JDebugType debugType = JDebugType.All)
	{
	}

	public static void LogError(object message, global::UnityEngine.Object context = null)
	{
		Debug.LogError("[JDS] - " + message, context);
	}
}
