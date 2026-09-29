using System;
using S13Audio;

[Serializable]
public class LocalAudioCommand
{
	public int ID;

	public string EventHandler;

	public S13LocalAction action;

	public void Execute()
	{
		this.action.Execute();
	}
}
