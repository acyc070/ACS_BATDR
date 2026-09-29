using System;

[Serializable]
public class DataDirectory<Key, Value> : Data<Key, Value> where Key : IConvertible where Value : IDataObject<Key>
{
	public DataDirectory()
		: base(typeof(Key).Name + typeof(Value).Name)
	{
	}
}
