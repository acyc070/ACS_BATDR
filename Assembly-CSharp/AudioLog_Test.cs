using UnityEngine;

public class AudioLog_Test : DataMonoBehaviour<AudioLogID, AudioLogDataObject>
{
	[Header("AudioLog Identifier")]
	[SerializeField]
	protected AudioLogID m_AudioLogID;

	[Header("AudioLog Data")]
	[SerializeField]
	private AudioLogData m_AudioLogData;

	protected override AudioLogID m_ID => this.m_AudioLogID;

	public AudioLogID AudioLogID => this.m_AudioLogID;

	public override void Awake()
	{
	}

	protected override void InternalInitialize()
	{
	}
}
