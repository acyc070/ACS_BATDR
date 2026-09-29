using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class BaseUGUIDropdown : MonoBehaviour
{
	[SerializeField]
	public TextMeshProUGUI CaptionText;

	[SerializeField]
	public TextMeshProUGUI ItemTextMeshPro;

	[SerializeField]
	public Dropdown Dropdown;
}
