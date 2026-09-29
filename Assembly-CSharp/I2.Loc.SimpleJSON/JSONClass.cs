using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace I2.Loc.SimpleJSON;

public class JSONClass : JSONNode, IEnumerable
{
	private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>(StringComparer.Ordinal);

	public override JSONNode this[string aKey]
	{
		get
		{
			if (this.m_Dict.ContainsKey(aKey))
			{
				return this.m_Dict[aKey];
			}
			return new JSONLazyCreator(this, aKey);
		}
		set
		{
			if (this.m_Dict.ContainsKey(aKey))
			{
				this.m_Dict[aKey] = value;
			}
			else
			{
				this.m_Dict.Add(aKey, value);
			}
		}
	}

	public override JSONNode this[int aIndex]
	{
		get
		{
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				return null;
			}
			return this.m_Dict.ElementAt(aIndex).Value;
		}
		set
		{
			if (aIndex >= 0 && aIndex < this.m_Dict.Count)
			{
				string key;
				key = this.m_Dict.ElementAt(aIndex).Key;
				this.m_Dict[key] = value;
			}
		}
	}

	public override int Count => this.m_Dict.Count;

	public override IEnumerable<JSONNode> Childs
	{
		get
		{
			foreach (KeyValuePair<string, JSONNode> item in this.m_Dict)
			{
				yield return item.Value;
			}
		}
	}

	public override void Add(string aKey, JSONNode aItem)
	{
		if (!string.IsNullOrEmpty(aKey))
		{
			if (this.m_Dict.ContainsKey(aKey))
			{
				this.m_Dict[aKey] = aItem;
			}
			else
			{
				this.m_Dict.Add(aKey, aItem);
			}
		}
		else
		{
			this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
		}
	}

	public override JSONNode Remove(string aKey)
	{
		if (!this.m_Dict.ContainsKey(aKey))
		{
			return null;
		}
		JSONNode result;
		result = this.m_Dict[aKey];
		this.m_Dict.Remove(aKey);
		return result;
	}

	public override JSONNode Remove(int aIndex)
	{
		if (aIndex < 0 || aIndex >= this.m_Dict.Count)
		{
			return null;
		}
		KeyValuePair<string, JSONNode> keyValuePair;
		keyValuePair = this.m_Dict.ElementAt(aIndex);
		this.m_Dict.Remove(keyValuePair.Key);
		return keyValuePair.Value;
	}

	public override JSONNode Remove(JSONNode aNode)
	{
		try
		{
			KeyValuePair<string, JSONNode> keyValuePair;
			keyValuePair = this.m_Dict.Where((KeyValuePair<string, JSONNode> k) => k.Value == aNode).First();
			this.m_Dict.Remove(keyValuePair.Key);
			return aNode;
		}
		catch
		{
			return null;
		}
	}

	public IEnumerator GetEnumerator()
	{
		foreach (KeyValuePair<string, JSONNode> item in this.m_Dict)
		{
			yield return item;
		}
	}

	public override string ToString()
	{
		string text;
		text = "{";
		foreach (KeyValuePair<string, JSONNode> item in this.m_Dict)
		{
			if (text.Length > 2)
			{
				text += ", ";
			}
			text = text + "\"" + JSONNode.Escape(item.Key) + "\":" + item.Value.ToString();
		}
		return text + "}";
	}

	public override string ToString(string aPrefix)
	{
		string text;
		text = "{ ";
		foreach (KeyValuePair<string, JSONNode> item in this.m_Dict)
		{
			if (text.Length > 3)
			{
				text += ", ";
			}
			text = text + "\n" + aPrefix + "   ";
			text = text + "\"" + JSONNode.Escape(item.Key) + "\" : " + item.Value.ToString(aPrefix + "   ");
		}
		return text + "\n" + aPrefix + "}";
	}

	public override void Serialize(BinaryWriter aWriter)
	{
		aWriter.Write((byte)2);
		aWriter.Write(this.m_Dict.Count);
		foreach (string key in this.m_Dict.Keys)
		{
			aWriter.Write(key);
			this.m_Dict[key].Serialize(aWriter);
		}
	}
}
