using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/Voice", order = 4)]
public class S13Voice : ScriptableObject
{
	[Tooltip("The AudioSource asset that this object represents.")]
	public AudioSource audioSource;
}
