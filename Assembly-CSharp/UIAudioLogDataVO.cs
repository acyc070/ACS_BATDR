using I2.Loc;
using UnityEngine;

public class UIAudioLogDataVO : JDisposable
{
	public LocalizedString Name;

	public LocalizedString Log;

	public string NameString = string.Empty;

	public string LogString = string.Empty;

	public Vector3 LogWorldPosition;

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
