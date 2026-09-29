using System;

public abstract class DataObject<Key, Value> where Key : IConvertible where Value : IDataObject<Key>, new()
{
	protected Key m_ID;

	public abstract Key ID { get; }

	public DataObject()
	{
	}

	public static Value Create(Key id)
	{
		Value val;
		val = new Value();
		(val as DataObject<Key, Value>).m_ID = id;
		(val as DataObject<Key, Value>).Deserialize();
		return val;
	}

	protected abstract void Deserialize();
}
