using System;

public interface IDataObject<Key> : IDataObject where Key : IConvertible
{
	Key ID { get; }
}
public interface IDataObject
{
}
