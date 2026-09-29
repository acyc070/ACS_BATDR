using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "I2LocalizationGroup", menuName = "I2 Localization/Id Localization Group", order = 1)]
public class I2LocalizationGroup : ScriptableObject
{
	[SerializeField]
	private LanguageSourceAsset[] m_LanguageSourceAssets;

	public LanguageSourceAsset[] LanguageSourceAssets => this.m_LanguageSourceAssets;
}
