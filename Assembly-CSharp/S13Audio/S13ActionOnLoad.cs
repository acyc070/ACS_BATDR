using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

public class S13ActionOnLoad : MonoBehaviour
{
	[FormerlySerializedAs("onSceneLoadAction")]
	[Tooltip("Action to run when S13Manager is loaded, or immediately if S13Manager is already loaded")]
	[SerializeField]
	private S13LocalAction onLoadAction;

	private void Awake()
	{
		if (S13Manager.IsManagerLoaded)
		{
			this.OnSceneLoad();
		}
		else
		{
			S13Manager.OnS13ManagerLoad += this.OnSceneLoad;
		}
	}

	private void OnSceneLoad()
	{
		if (this.onLoadAction.IsExecutable)
		{
			this.onLoadAction.Execute();
		}
		S13Manager.OnS13ManagerLoad -= this.OnSceneLoad;
	}
}
