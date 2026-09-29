namespace S13Audio;

public abstract class S13Action_Group : IS13Executable
{
	public abstract bool IsExecutable { get; }

	public abstract void Execute();
}
