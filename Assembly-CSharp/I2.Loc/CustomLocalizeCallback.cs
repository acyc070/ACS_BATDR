using UnityEngine;
using UnityEngine.Events;

namespace I2.Loc;

[AddComponentMenu("I2/Localization/I2 Localize Callback")]
public class CustomLocalizeCallback : MonoBehaviour
{
	public UnityEvent _OnLocalize = new UnityEvent();

	public void OnEnable()
	{
		LocalizationManager.OnLocalizeEvent -= this.OnLocalize;
		LocalizationManager.OnLocalizeEvent += this.OnLocalize;
	}

	public void OnDisable()
	{
		LocalizationManager.OnLocalizeEvent -= this.OnLocalize;
	}

	public void OnLocalize()
	{
		this._OnLocalize.Invoke();
	}
}
