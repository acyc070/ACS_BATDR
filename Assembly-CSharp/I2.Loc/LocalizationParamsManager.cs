using System;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc;

public class LocalizationParamsManager : MonoBehaviour, ILocalizationParamsManager
{
	[Serializable]
	public struct ParamValue
	{
		public string Name;

		public string Value;
	}

	[SerializeField]
	public List<ParamValue> _Params = new List<ParamValue>();

	public bool _IsGlobalManager;

	public string GetParameterValue(string ParamName)
	{
		if (this._Params != null)
		{
			int i;
			i = 0;
			for (int count = this._Params.Count; i < count; i = unchecked(i + 1))
			{
				if (this._Params[i].Name == ParamName)
				{
					return this._Params[i].Value;
				}
			}
		}
		return null;
	}

	public void SetParameterValue(string ParamName, string ParamValue, bool localize = true)
	{
		bool flag;
		flag = false;
		int i;
		i = 0;
		for (int count = this._Params.Count; i < count; i = unchecked(i + 1))
		{
			if (this._Params[i].Name == ParamName)
			{
				ParamValue value;
				value = this._Params[i];
				value.Value = ParamValue;
				this._Params[i] = value;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this._Params.Add(new ParamValue
			{
				Name = ParamName,
				Value = ParamValue
			});
		}
		if (localize)
		{
			this.OnLocalize();
		}
	}

	public void OnLocalize()
	{
		Localize component;
		component = base.GetComponent<Localize>();
		if (component != null)
		{
			component.OnLocalize(Force: true);
		}
	}

	public virtual void OnEnable()
	{
		if (this._IsGlobalManager)
		{
			this.DoAutoRegister();
		}
	}

	public void DoAutoRegister()
	{
		if (!LocalizationManager.ParamManagers.Contains(this))
		{
			LocalizationManager.ParamManagers.Add(this);
			LocalizationManager.LocalizeAll(Force: true);
		}
	}

	public void OnDisable()
	{
		LocalizationManager.ParamManagers.Remove(this);
	}
}
