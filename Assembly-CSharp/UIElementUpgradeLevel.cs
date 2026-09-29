using UnityEngine;
using UnityEngine.UI;

public class UIElementUpgradeLevel : JMonoBehaviour, IUIElement
{
	[SerializeField]
	private Image m_Icon;

	[SerializeField]
	private Image m_IconSelected;

	public string PrefabKey => "UI/Elements/UIElementUpgradeLevel";

	public UIElementUpgradeLevelDataVO Data { get; private set; }

	public bool IsSelected { get; private set; }

	public void Initialize(object data)
	{
		this.Data = (UIElementUpgradeLevelDataVO)data;
		this.m_Icon.enabled = this.Data.IsActive;
		this.m_IconSelected.gameObject.SetActive(value: false);
	}

	public void Initialized()
	{
	}

	public void Select()
	{
		this.IsSelected = true;
		this.m_IconSelected.gameObject.SetActive(value: true);
	}

	public void Deselect()
	{
		this.IsSelected = false;
		this.m_IconSelected.gameObject.SetActive(value: false);
	}

	protected override void OnDisposed()
	{
		this.Data = null;
		base.OnDisposed();
	}
}
