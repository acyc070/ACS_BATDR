public class Item<TContent, TData> : Item
{
	public TContent Content => (TContent)base.m_Content;

	public TData Data => (TData)base.m_Data;
}
public class Item : JMonoBehaviour, IInitializer
{
	protected object m_Content { get; set; }

	protected object m_Data { get; set; }

	public virtual void Initialize(object data)
	{
		this.m_Data = data;
	}

	protected override void OnDisposed()
	{
		this.m_Content = null;
		this.m_Data = null;
		base.OnDisposed();
	}
}
