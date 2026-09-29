using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc;

[AddComponentMenu("I2/Localization/SetLanguage Dropdown")]
public class SetLanguageDropdown : MonoBehaviour
{
	private void OnEnable()
	{
		Dropdown component;
		component = base.GetComponent<Dropdown>();
		if (!(component == null))
		{
			string currentLanguage;
			currentLanguage = LocalizationManager.CurrentLanguage;
			if (LocalizationManager.Sources.Count == 0)
			{
				LocalizationManager.UpdateSources();
			}
			List<string> allLanguages;
			allLanguages = LocalizationManager.GetAllLanguages();
			component.ClearOptions();
			component.AddOptions(allLanguages);
			component.value = allLanguages.IndexOf(currentLanguage);
			component.onValueChanged.RemoveListener(this.OnValueChanged);
			component.onValueChanged.AddListener(this.OnValueChanged);
		}
	}

	private void OnValueChanged(int index)
	{
		Dropdown component;
		component = base.GetComponent<Dropdown>();
		if (index < 0)
		{
			index = 0;
			component.value = index;
		}
		LocalizationManager.CurrentLanguage = component.options[index].text;
	}
}
