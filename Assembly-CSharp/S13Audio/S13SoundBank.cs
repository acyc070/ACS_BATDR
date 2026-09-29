using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[CreateAssetMenu(menuName = "Studio13/Scriptable/Sound Bank", order = 2)]
public class S13SoundBank : ScriptableObject, IEnumerable<S13Object>, IEnumerable
{
	[Tooltip("The set of sound objects that this sound bank should load.")]
	[SerializeField]
	[Reorderable]
	private S13ObjectList containedObjects = new S13ObjectList(0);

	public void Load()
	{
		S13Manager.LoadSoundBank(this);
	}

	public void Unload()
	{
		S13Manager.UnloadSoundBank(this);
	}

	public IEnumerator<S13Object> GetEnumerator()
	{
		foreach (S13Object item in this.containedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (S13Object item in this.containedObjects.Where((S13Object x) => x))
		{
			yield return item;
		}
	}
}
