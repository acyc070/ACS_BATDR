using TMPro;
using UnityEngine;

public class MenuItemCategory : JMonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI m_Category;

	public void Init(Transform parent, string category)
	{
		base.transform.SetParent(parent);
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		this.m_Category.text = category;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
