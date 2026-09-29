using System;

public interface IData
{
	Type KeyType { get; }

	Type DataType { get; }

	object ID { get; }

	object Data { get; }

	void Load(object data);

	void Initialize();
}
