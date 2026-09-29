using System.Collections.Generic;

namespace S13Audio;

public interface IS13HandlerCompound
{
	List<S13Handler> SubHandlers { get; }

	void RegisterSubHandler(S13Handler subHandler);
}
