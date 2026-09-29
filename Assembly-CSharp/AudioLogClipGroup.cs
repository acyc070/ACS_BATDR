using UnityEngine;

[CreateAssetMenu(fileName = "AudioLogClipGroup", menuName = "Game Data/Audio Log/Audio Log Clip Group", order = 1)]
public class AudioLogClipGroup : ScriptableObject
{
	[SerializeField]
	private AudioLogData[] m_AudioLogs;

	public AudioLogData[] AudioLogs => this.m_AudioLogs;
}
