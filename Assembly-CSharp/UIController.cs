using System;
using UnityEngine;

public abstract class UIController : JMonoBehaviour
{
	[SerializeField]
	protected RectTransform m_Visuals;

	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if (!(this.m_RectTransform == null))
			{
				return this.m_RectTransform;
			}
			return this.m_RectTransform = base.GetComponent<RectTransform>();
		}
	}

	public event EventHandler OnPlayInComplete;

	public event EventHandler OnPlayOutComplete;

	public void Initialize(object _data)
	{
		Canvas component;
		component = base.GetComponent<Canvas>();
		Camera camera;
		camera = (component.worldCamera = GameManager.Instance.UIManager.Camera);
		component.pixelPerfect = false;
		component.planeDistance = Math.Abs(component.transform.parent.position.z - camera.transform.position.z);
		component.sortingOrder = unchecked((int)(0f - component.planeDistance));
		this.OnInitialized(_data);
	}

	protected virtual void OnInitialized(object _data)
	{
	}

	public virtual void PlayIn()
	{
		this.PlayInComplete();
	}

	public virtual void PlayInComplete()
	{
		this.OnPlayInComplete.Send(this);
	}

	public virtual void PlayOut()
	{
		this.PlayOutComplete();
	}

	public virtual void PlayOutComplete()
	{
		this.OnPlayOutComplete.Send(this);
		base.Dispose();
	}

	protected override void OnDisposed()
	{
		this.OnPlayInComplete = null;
		this.OnPlayOutComplete = null;
		this.m_RectTransform = null;
		base.OnDisposed();
	}
}
