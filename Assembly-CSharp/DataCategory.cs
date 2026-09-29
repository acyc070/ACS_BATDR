using System.Collections.Generic;

public class DataCategory
{
	public List<Data> Data { get; private set; }

	public DataCategory()
	{
		this.Data = new List<Data>();
	}

	protected void AddData(Data data)
	{
		if (!this.Data.Contains(data))
		{
			this.Data.Add(data);
		}
	}

	public bool GetData(string name, out Data data)
	{
		bool result;
		result = false;
		data = null;
		for (int i = 0; i < this.Data.Count; i = unchecked(i + 1))
		{
			data = this.Data[i];
			if (data.Name == name)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
