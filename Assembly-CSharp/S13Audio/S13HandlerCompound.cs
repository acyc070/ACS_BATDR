using System.Collections.Generic;
using System.Linq;

namespace S13Audio;

public abstract class S13HandlerCompound<TObject, THandler> : S13Handler<TObject, THandler>, IS13HandlerCompound where TObject : S13ObjectCompound<TObject, THandler> where THandler : S13HandlerCompound<TObject, THandler>
{
	protected List<S13Handler> subHandlers = new List<S13Handler>();

	public override int NumActiveVoices => this.subHandlers.Distinct().Sum((S13Handler x) => x.NumActiveVoices);

	public override bool isPlaying
	{
		get
		{
			if (this.subHandlers != null)
			{
				return this.subHandlers.Any((S13Handler x) => x.isPlaying);
			}
			return false;
		}
	}

	public List<S13Handler> SubHandlers => this.subHandlers;

	public virtual void RegisterSubHandler(S13Handler subHandler)
	{
		this.subHandlers.Add(subHandler);
	}
}
