using System.Collections;

public interface ICommandIEnumerator
{
	IEnumerator Initialize();

	void Enable();

	void Disable();

	void Inactive();

	void ForceComplete();
}
