public class ItemContent<TController, TData> : ItemContent
{
	public TController Controller => (TController)base.m_Controller;

	public TData Data => (TData)base.m_Data;
}
public class ItemContent : JMonoBehaviour, IInitializer
{
	protected object m_Controller { get; set; }

	protected object m_Data { get; set; }

	public virtual void Initialize(object data)
	{
		this.m_Data = data;
	}

	protected override void OnDisposed()
	{
		this.m_Controller = null;
		this.m_Data = null;
		base.OnDisposed();
	}
}
