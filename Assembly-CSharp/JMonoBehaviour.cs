using System;
using UnityEngine;

public abstract class JMonoBehaviour : MonoBehaviour, IDisposable
{
	private Transform m_Transform;

	private GameObject m_GameObject;

	public bool IsDisposed { get; private set; }

	public bool IsDestroyed { get; private set; }

	public new Transform transform
	{
		get
		{
			if (!Application.isPlaying)
			{
				return base.transform;
			}
			return this.m_Transform ?? (this.m_Transform = base.transform);
		}
	}

	public new GameObject gameObject
	{
		get
		{
			if (!Application.isPlaying)
			{
				return base.gameObject;
			}
			return this.m_GameObject ?? (this.m_GameObject = base.gameObject);
		}
	}

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
	}

	protected virtual void OnDisposed()
	{
	}

	public void SetParent(Transform parent)
	{
		this.transform.SetParent(parent);
	}

	public void SetParentAndCenter(Transform parent)
	{
		this.SetParent(parent);
		this.transform.localPosition = Vector3.zero;
		this.transform.localEulerAngles = Vector3.zero;
	}

	public void SetParentAndCenterAndScale(Transform parent)
	{
		this.SetParentAndCenter(parent);
		this.transform.localScale = Vector3.one;
	}

	public void SetLayerRecursive(int layer)
	{
		this.SetLayerRecursive(this.gameObject, layer);
	}

	public void SetLayerRecursive(GameObject _gameObject, int _layer)
	{
		_gameObject.layer = _layer;
		foreach (Transform item in _gameObject.transform)
		{
			this.SetLayerRecursive(item.gameObject, _layer);
		}
	}

	public void Dispose()
	{
		if (!this.IsDisposed)
		{
			this.OnDisposed();
			this.IsDisposed = true;
			GC.SuppressFinalize(this);
			if (!this.IsDestroyed)
			{
				this.IsDestroyed = true;
				global::UnityEngine.Object.Destroy(this.gameObject);
			}
		}
	}

	public void OnDestroy()
	{
		if (!this.IsDestroyed)
		{
			this.IsDestroyed = true;
			this.Dispose();
		}
	}
}
