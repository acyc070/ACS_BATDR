using UnityEngine;

namespace S13Audio;

public class S13ActionManual : MonoBehaviour
{
	[SerializeField]
	private S13LocalAction action = new S13LocalAction();

	public void Execute()
	{
		this.action.Execute();
	}
}
