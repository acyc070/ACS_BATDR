using System;

public abstract class DataMonoBehaviour<Key, Value> : JMonoBehaviour, IData where Key : IConvertible where Value : IDataObject<Key>, new()
{
	protected Value m_Data;

	public object ID => this.m_ID;

	protected abstract Key m_ID { get; }

	public object Data => this.m_Data;

	public Type KeyType => typeof(Key);

	public Type DataType => typeof(Value);

	public bool IsInitialized { get; private set; }

	public bool IsLoaded { get; private set; }

	public void Initialize()
	{
		if (!this.IsInitialized)
		{
			this.IsInitialized = true;
			this.InternalInitialize();
		}
	}

	protected abstract void InternalInitialize();

	public void Load(object data)
	{
		if (!this.IsLoaded)
		{
			if (data == null)
			{
				data = DataObject<Key, Value>.Create(this.m_ID);
			}
			this.m_Data = (Value)data;
			this.IsLoaded = true;
		}
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
