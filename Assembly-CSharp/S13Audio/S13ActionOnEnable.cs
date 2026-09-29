using UnityEngine;

namespace S13Audio;

public class S13ActionOnEnable : MonoBehaviour
{
	[SerializeField]
	public S13LocalAction onEnableAction;

	[SerializeField]
	public S13LocalAction onDisableAction;

	[SerializeField]
	[Tooltip("Should the OnEnable and OnDisable calls wait until the S13Manager has finished loading?")]
	private bool waitForS13ManagerReady = true;

	private void OnEnable()
	{
		if (this.waitForS13ManagerReady && !S13Manager.IsManagerLoaded)
		{
			S13Manager.OnS13ManagerLoad -= this.ExecuteOnEnable;
			S13Manager.OnS13ManagerLoad += this.ExecuteOnEnable;
		}
		else
		{
			this.ExecuteOnEnable();
		}
	}

	private void OnDisable()
	{
		if (this.waitForS13ManagerReady && !S13Manager.IsManagerLoaded)
		{
			S13Manager.OnS13ManagerLoad -= this.ExecuteOnDisable;
			S13Manager.OnS13ManagerLoad += this.ExecuteOnDisable;
		}
		else
		{
			this.ExecuteOnDisable();
		}
	}

	private void ExecuteOnEnable()
	{
		if (this.onEnableAction.IsExecutable)
		{
			S13Debug.Log("ActionOnEnable " + base.name, this, S13Debug.LogType.Action);
			this.onEnableAction.Execute();
		}
		S13Manager.OnS13ManagerLoad -= this.ExecuteOnEnable;
	}

	private void ExecuteOnDisable()
	{
		if (this.onDisableAction.IsExecutable)
		{
			S13Debug.Log("ActionOnDisable " + base.name, this, S13Debug.LogType.Action);
			this.onDisableAction.Execute();
		}
		S13Manager.OnS13ManagerLoad -= this.ExecuteOnDisable;
	}
}
