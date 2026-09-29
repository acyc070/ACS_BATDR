namespace S13Audio;

public abstract class S13ObjectPrimitiveMultiClip<TObject, THandler> : S13ObjectPrimitive<TObject, THandler>, IS13ObjectPrimitiveMultiClip, IS13ObjectPrimitive, IS13Controllable where TObject : S13ObjectPrimitiveMultiClip<TObject, THandler> where THandler : S13HandlerPrimitiveMultiClip<TObject, THandler>
{
	public override float startTime => 0f;
}
