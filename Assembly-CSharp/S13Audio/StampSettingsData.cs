using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class StampSettingsData
{
	[Header("Message Settings")]
	public S13Debug.LogType _enabledLogs;

	[Header("Log Options")]
	public bool _frameCount;

	public bool _logTypePrefix;

	[Header("Text Settings")]
	public int _textSize = 15;

	[Header("Area Sound Debug")]
	public bool _areaSoundColors;

	public bool _areaSoundUI;

	public Color _unselectedAreaSoundColor = new Color(0.25f, 0.25f, 0.25f, 0.75f);

	public Color _simpleAreaSoundColor = new Color(0f, 1f, 1f);

	public Color _portalAreaSoundColor = new Color(0.05f, 0.6f, 0.73f);
}
