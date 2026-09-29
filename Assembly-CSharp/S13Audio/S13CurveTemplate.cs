using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/CurveTemplate", order = 5)]
public class S13CurveTemplate : ScriptableObject
{
	[S13CurveRange(0f, 0f, 1f, 1f)]
	[SerializeField]
	public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
