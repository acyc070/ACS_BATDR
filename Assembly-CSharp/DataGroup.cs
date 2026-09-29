using System;

[Serializable]
public class DataGroup<Key, Value> : Data<Key, Value> where Key : IConvertible where Value : IDataObject<Key>
{
	public DataGroup()
		: base(typeof(Value).Name)
	{
	}
}
