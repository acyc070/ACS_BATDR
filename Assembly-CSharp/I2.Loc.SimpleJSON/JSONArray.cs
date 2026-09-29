using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace I2.Loc.SimpleJSON;

public class JSONArray : JSONNode, IEnumerable
{
	private List<JSONNode> m_List = new List<JSONNode>();

	public override JSONNode this[int aIndex]
	{
		get
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return new JSONLazyCreator(this);
			}
			return this.m_List[aIndex];
		}
		set
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				this.m_List.Add(value);
			}
			else
			{
				this.m_List[aIndex] = value;
			}
		}
	}

	public override JSONNode this[string aKey]
	{
		get
		{
			return new JSONLazyCreator(this);
		}
		set
		{
			this.m_List.Add(value);
		}
	}

	public override int Count => this.m_List.Count;

	public override IEnumerable<JSONNode> Childs
	{
		get
		{
			foreach (JSONNode item in this.m_List)
			{
				yield return item;
			}
		}
	}

	public override void Add(string aKey, JSONNode aItem)
	{
		this.m_List.Add(aItem);
	}

	public override JSONNode Remove(int aIndex)
	{
		if (aIndex < 0 || aIndex >= this.m_List.Count)
		{
			return null;
		}
		JSONNode result;
		result = this.m_List[aIndex];
		this.m_List.RemoveAt(aIndex);
		return result;
	}

	public override JSONNode Remove(JSONNode aNode)
	{
		this.m_List.Remove(aNode);
		return aNode;
	}

	public IEnumerator GetEnumerator()
	{
		foreach (JSONNode item in this.m_List)
		{
			yield return item;
		}
	}

	public override string ToString()
	{
		string text;
		text = "[ ";
		foreach (JSONNode item in this.m_List)
		{
			if (text.Length > 2)
			{
				text += ", ";
			}
			text += item.ToString();
		}
		return text + " ]";
	}

	public override string ToString(string aPrefix)
	{
		string text;
		text = "[ ";
		foreach (JSONNode item in this.m_List)
		{
			if (text.Length > 3)
			{
				text += ", ";
			}
			text = text + "\n" + aPrefix + "   ";
			text += item.ToString(aPrefix + "   ");
		}
		return text + "\n" + aPrefix + "]";
	}

	public override void Serialize(BinaryWriter aWriter)
	{
		aWriter.Write((byte)1);
		aWriter.Write(this.m_List.Count);
		for (int i = 0; i < this.m_List.Count; i = unchecked(i + 1))
		{
			this.m_List[i].Serialize(aWriter);
		}
	}
}
