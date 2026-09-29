using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data<Key, Value> : Data where Key : IConvertible where Value : IDataObject<Key>
{
	[SerializeField]
	private string m_Name = string.Empty;

	[SerializeField]
	private List<Key> m_Keys = new List<Key>();

	[SerializeField]
	private List<Value> m_Values = new List<Value>();

	public override string Name => this.m_Name;

	public List<Key> Keys => this.m_Keys;

	public List<Value> Values => this.m_Values;

	public override int Count => this.m_Keys.Count;

	public Data(string name)
	{
		this.m_Name = name;
	}

	public override bool Add(object key, object value)
	{
		return this.InternalAdd((Key)key, (Value)value);
	}

	private bool InternalAdd(Key key, Value value)
	{
		bool flag;
		flag = key != null && value != null;
		if (flag && !this.m_Keys.Contains(key))
		{
			this.m_Keys.Add(key);
			this.m_Keys.Sort();
			this.m_Values.Insert(this.m_Keys.IndexOf(key), value);
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public override void Remove(object value)
	{
		this.InternalRemove((Value)value);
	}

	private void InternalRemove(Value value)
	{
		if (this.m_Values.Contains(value))
		{
			int index;
			index = this.m_Values.IndexOf(value);
			this.m_Keys.RemoveAt(index);
			this.m_Values.RemoveAt(index);
		}
	}

	public override bool ChangeValue(object key, object value)
	{
		return this.InternalChangeValue((Key)key, (Value)value);
	}

	private bool InternalChangeValue(Key key, Value value)
	{
		if (key == null || value == null)
		{
			return false;
		}
		if (this.m_Keys.Contains(key))
		{
			int index;
			index = this.m_Keys.IndexOf(key);
			this.m_Values[index] = value;
			return true;
		}
		return this.InternalAdd(key, value);
	}

	public override object GetValue(object key)
	{
		return this.InternalGetValue((Key)key);
	}

	private Value InternalGetValue(Key key)
	{
		Value result;
		result = default;
		if (this.m_Keys.Contains(key))
		{
			int index;
			index = this.m_Keys.IndexOf(key);
			return this.m_Values[index];
		}
		return result;
	}

	public override object GetValueOfIndex(int index)
	{
		return this.InternalGetValueOfIndex(index);
	}

	private Value InternalGetValueOfIndex(int index)
	{
		Value result;
		result = default;
		if (index < this.m_Values.Count)
		{
			return this.m_Values[index];
		}
		return result;
	}

	public override bool IndexOf(object value, out int index)
	{
		return this.InternalIndexOf((Value)value, out index);
	}

	private bool InternalIndexOf(Value value, out int index)
	{
		bool flag;
		flag = value != null;
		if (flag)
		{
			index = this.m_Values.IndexOf(value);
			return flag;
		}
		index = 0;
		return flag;
	}

	public override bool ContainsKey(object key)
	{
		return this.InternalContainsKey((Key)key);
	}

	private bool InternalContainsKey(Key key)
	{
		return this.m_Keys.Contains(key);
	}

	public override bool ContainsValue(object value)
	{
		return this.InternalContainsValue((Value)value);
	}

	private bool InternalContainsValue(Value value)
	{
		return this.m_Values.Contains(value);
	}

	public override void Clear()
	{
		this.m_Keys.Clear();
		this.m_Values.Clear();
	}
}
[Serializable]
public abstract class Data
{
	public abstract string Name { get; }

	public abstract int Count { get; }

	public abstract bool Add(object key, object value);

	public abstract void Remove(object value);

	public abstract bool ChangeValue(object key, object value);

	public abstract object GetValue(object key);

	public abstract object GetValueOfIndex(int index);

	public abstract bool IndexOf(object value, out int index);

	public abstract bool ContainsKey(object key);

	public abstract bool ContainsValue(object value);

	public abstract void Clear();
}
