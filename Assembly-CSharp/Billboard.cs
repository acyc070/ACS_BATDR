using UnityEngine;

public class Billboard : JComponent
{
	private void Update()
	{
		if (!base.IsDisposed && !(GameManager.Instance.GameCamera == null))
		{
			base.transform.LookAt(GameManager.Instance.GameCamera.transform, Vector3.up);
		}
	}
}
