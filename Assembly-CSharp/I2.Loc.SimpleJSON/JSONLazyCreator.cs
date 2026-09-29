namespace I2.Loc.SimpleJSON;

internal class JSONLazyCreator : JSONNode
{
	private JSONNode m_Node;

	private string m_Key;

	public override JSONNode this[int aIndex]
	{
		get
		{
			return new JSONLazyCreator(this);
		}
		set
		{
			JSONArray jSONArray;
			jSONArray = new JSONArray();
			jSONArray.Add(value);
			this.Set(jSONArray);
		}
	}

	public override JSONNode this[string aKey]
	{
		get
		{
			return new JSONLazyCreator(this, aKey);
		}
		set
		{
			JSONClass jSONClass;
			jSONClass = new JSONClass();
			jSONClass.Add(aKey, value);
			this.Set(jSONClass);
		}
	}

	public override int AsInt
	{
		get
		{
			this.Set(new JSONData(0));
			return 0;
		}
		set
		{
			this.Set(new JSONData(value));
		}
	}

	public override float AsFloat
	{
		get
		{
			this.Set(new JSONData(0f));
			return 0f;
		}
		set
		{
			this.Set(new JSONData(value));
		}
	}

	public override double AsDouble
	{
		get
		{
			this.Set(new JSONData(0.0));
			return 0.0;
		}
		set
		{
			this.Set(new JSONData(value));
		}
	}

	public override bool AsBool
	{
		get
		{
			this.Set(new JSONData(aData: false));
			return false;
		}
		set
		{
			this.Set(new JSONData(value));
		}
	}

	public override JSONArray AsArray
	{
		get
		{
			JSONArray jSONArray;
			jSONArray = new JSONArray();
			this.Set(jSONArray);
			return jSONArray;
		}
	}

	public override JSONClass AsObject
	{
		get
		{
			JSONClass jSONClass;
			jSONClass = new JSONClass();
			this.Set(jSONClass);
			return jSONClass;
		}
	}

	public JSONLazyCreator(JSONNode aNode)
	{
		this.m_Node = aNode;
		this.m_Key = null;
	}

	public JSONLazyCreator(JSONNode aNode, string aKey)
	{
		this.m_Node = aNode;
		this.m_Key = aKey;
	}

	private void Set(JSONNode aVal)
	{
		if (this.m_Key == null)
		{
			this.m_Node.Add(aVal);
		}
		else
		{
			this.m_Node.Add(this.m_Key, aVal);
		}
		this.m_Node = null;
	}

	public override void Add(JSONNode aItem)
	{
		JSONArray jSONArray;
		jSONArray = new JSONArray();
		jSONArray.Add(aItem);
		this.Set(jSONArray);
	}

	public override void Add(string aKey, JSONNode aItem)
	{
		JSONClass jSONClass;
		jSONClass = new JSONClass();
		jSONClass.Add(aKey, aItem);
		this.Set(jSONClass);
	}

	public static bool operator ==(JSONLazyCreator a, object b)
	{
		if (b == null)
		{
			return true;
		}
		return (object)a == b;
	}

	public static bool operator !=(JSONLazyCreator a, object b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return true;
		}
		return (object)this == obj;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return "";
	}

	public override string ToString(string aPrefix)
	{
		return "";
	}
}
