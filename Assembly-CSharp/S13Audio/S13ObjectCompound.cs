using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

public abstract class S13ObjectCompound<TObject, THandler> : S13Object<TObject, THandler>, IS13ObjectCompound, IS13Controllable where TObject : S13ObjectCompound<TObject, THandler> where THandler : S13HandlerCompound<TObject, THandler>
{
	protected enum S13CompoundInterruptMode
	{
		Continue = 0,
		Stop = 1
	}

	[FormerlySerializedAs("ForceGlobalSubHandlers")]
	[SerializeField]
	[Tooltip("If set to False, local S13AudioHandler's will create subHandlers in local space. Playing this S13Object globally will always use the global subHandler for each child. Default: True")]
	private bool _forceGlobalSubHandlers = true;

	public bool ForceGlobalSubHandlers => this._forceGlobalSubHandlers;
}
