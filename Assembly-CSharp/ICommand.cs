public interface ICommand
{
	void Initialize();

	void Enable();

	void Disable();

	void Inactive();

	void ForceComplete();
}
