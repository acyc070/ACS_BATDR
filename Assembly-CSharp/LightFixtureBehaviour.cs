using UnityEngine;
using UnityEngine.Playables;

public class LightFixtureBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public bool On;

	private LightFixture m_LightFixture;

	private bool m_IsPlayed;

	private float m_OriginalMaterial;

	public override void OnGraphStart(Playable playable)
	{
		if (!(this.Target == null))
		{
			this.m_LightFixture = this.Target.GetComponent<LightFixture>();
			_ = this.m_LightFixture == null;
		}
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (!(this.m_LightFixture == null) && Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (this.On)
			{
				this.m_LightFixture.SetEmission(1f);
			}
			else
			{
				this.m_LightFixture.SetEmission(0f);
			}
		}
	}

	public override void PrepareFrame(Playable playable, FrameData info)
	{
	}

	public override void OnGraphStop(Playable playable)
	{
		_ = this.m_LightFixture == null;
	}
}
