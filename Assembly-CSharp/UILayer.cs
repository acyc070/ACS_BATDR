using UnityEngine;

public class UILayer : JMonoBehaviour
{
	private UIController m_ActiveController;

	private RectTransform m_RectTransform;

	public bool IsActive { get; private set; }

	public string Name { get; private set; }

	public float Order { get; private set; }

	public static UILayer Create(string layerName, float layerOrder, Transform layerParent)
	{
		UILayer uILayer;
		uILayer = new GameObject
		{
			name = "[UI Layer] - " + layerName
		}.AddComponent<UILayer>();
		uILayer.Name = layerName;
		uILayer.Order = layerOrder;
		uILayer.Init(layerParent);
		return uILayer;
	}

	public void Init(Transform parent)
	{
		this.m_RectTransform = base.gameObject.AddComponent<RectTransform>();
		this.m_RectTransform.SetParent(parent);
		this.m_RectTransform.localPosition = new Vector3(0f, 0f, this.Order);
		this.m_RectTransform.localEulerAngles = Vector3.zero;
		this.m_RectTransform.localScale = Vector3.one;
		this.m_RectTransform.pivot = new Vector2(0.5f, 0.5f);
		this.m_RectTransform.anchorMax = new Vector2(1f, 1f);
		this.m_RectTransform.anchorMin = new Vector2(0f, 0f);
		this.m_RectTransform.offsetMax = new Vector2(0f, 0f);
		this.m_RectTransform.offsetMin = new Vector2(0f, 0f);
	}

	public void SetController(UIController controller)
	{
		this.m_ActiveController?.Dispose();
		this.m_ActiveController = controller;
	}

	protected override void OnDisposed()
	{
		this.m_RectTransform = null;
		base.OnDisposed();
	}
}
