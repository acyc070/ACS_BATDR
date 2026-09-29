using TMPro;
using UnityEngine;

public class MenuItemSave : JMonoBehaviour
{
	[Header("Button")]
	[SerializeField]
	private UIButton m_Button;

	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_SaveLbl;

	[SerializeField]
	private TextMeshProUGUI m_DescriptionLbl;

	[Header("Arrows")]
	[SerializeField]
	private GameObject m_Arrows;

	public UIButton Button => this.m_Button;
}
