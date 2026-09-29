using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineShaderValueBehaviour : PlayableBehaviour
{
	public GameObject Target;

	public int MaterialIndex;

	public string PropertyName;

	public float Value;

	public float Duration = 1f;

	private bool m_IsPlayed;

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			this.Target.GetComponent<MeshRenderer>().sharedMaterials[this.MaterialIndex].DOFloat(this.Value, this.PropertyName, this.Duration).SetEase(Ease.Linear);
		}
	}
}
