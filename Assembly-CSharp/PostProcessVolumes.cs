using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessVolumes : JMonoBehaviour
{
	[SerializeField]
	private PostProcessVolume m_BasePostProcessVolume;

	[SerializeField]
	private PostProcessVolume m_ColorPostProcessVolume;

	public PostProcessVolume BasePostProcessVolume => this.m_BasePostProcessVolume;

	public PostProcessVolume ColorPostProcessVolume => this.m_ColorPostProcessVolume;

	public PostProcessProfile BasePostProcess => this.m_BasePostProcessVolume.profile;

	public PostProcessProfile ColorPostProcess => this.m_ColorPostProcessVolume.profile;
}
