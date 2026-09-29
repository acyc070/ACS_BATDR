using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S13Audio.Malee;

[Serializable]
public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	[SerializeField]
	private T[] array = new T[0];

	public T this[int index]
	{
		get
		{
			return this.array[index];
		}
		set
		{
			this.array[index] = value;
		}
	}

	public int Length => this.array.Length;

	public bool IsReadOnly => this.array.IsReadOnly;

	public int Count => this.array.Length;

	public ReorderableArray()
		: this(0)
	{
	}

	public ReorderableArray(int length)
	{
		this.array = new T[length];
	}

	public object Clone()
	{
		return this.array.Clone();
	}

	public bool Contains(T value)
	{
		return Array.IndexOf(this.array, value) >= 0;
	}

	public int IndexOf(T value)
	{
		return Array.IndexOf(this.array, value);
	}

	public void Insert(int index, T item)
	{
		List<T> list;
		list = new List<T>(this.array);
		list.Insert(index, item);
		Array.Resize(ref this.array, unchecked(this.array.Length + 1));
		list.CopyTo(this.array);
	}

	public void RemoveAt(int index)
	{
		List<T> list;
		list = new List<T>(this.array);
		list.RemoveAt(index);
		Array.Resize(ref this.array, unchecked(this.array.Length - 1));
		list.CopyTo(this.array);
	}

	public void Add(T item)
	{
		unchecked
		{
			Array.Resize(ref this.array, this.array.Length + 1);
			this.array[this.array.Length - 1] = item;
		}
	}

	public void Clear()
	{
		this.array = new T[0];
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)this.array).CopyTo(array, arrayIndex);
	}

	public bool Remove(T item)
	{
		List<T> list;
		list = new List<T>(this.array);
		bool flag;
		flag = list.Remove(item);
		if (flag)
		{
			Array.Resize(ref this.array, unchecked(this.array.Length - 1));
			list.CopyTo(this.array);
		}
		return flag;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)this.array).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<T>)this.array).GetEnumerator();
	}

	public static implicit operator Array(ReorderableArray<T> reorderableArray)
	{
		return reorderableArray.array;
	}

	public static implicit operator T[](ReorderableArray<T> reorderableArray)
	{
		return reorderableArray.array;
	}
}
