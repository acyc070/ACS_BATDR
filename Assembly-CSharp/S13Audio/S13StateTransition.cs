using System;

namespace S13Audio;

[Serializable]
public class S13StateTransition
{
	public S13ScriptableState from;

	public S13ScriptableState to;

	public S13GlobalActionGroup actions;
}
