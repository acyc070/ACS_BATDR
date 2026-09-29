using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutsceneControllerVibrationBehaviour : PlayableBehaviour
{
	public bool IsEnd;

	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

	private bool m_IsPlayed;

	private GameObject m_Owner;

	private CutsceneDirector m_CutsceneDirector;

	public float m_Duration;

	public float m_Strength;

	public void SetOwner(GameObject owner)
	{
		this.m_Owner = owner;
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (!this.IsEnd)
			{
				this.Begin();
			}
			else
			{
				this.End();
			}
		}
	}

	private void Begin()
	{
		GameManager.Instance.TriggerRumble(this.m_Duration, this.m_Strength);
	}

	private void End()
	{
	}
}
