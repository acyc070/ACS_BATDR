using UnityEngine;

public class Moon : JMonoBehaviour
{
	[SerializeField]
	private float m_Distance = 20f;

	[SerializeField]
	private Renderer m_VisibleCheck;

	[SerializeField]
	private ActiveSetter m_ActiveSetter;

	private Vector3 m_OriginPosition;

	private bool m_IsActive;

	private bool m_IsActivated;

	public override void Start()
	{
		this.m_OriginPosition = base.transform.position;
	}

	private void Update()
	{
		Camera camera;
		camera = GameManager.Instance.GameCamera.Camera;
		Vector3 vector;
		vector = this.m_OriginPosition - camera.transform.position;
		Vector3 position;
		position = camera.transform.position + vector.normalized * (camera.farClipPlane - this.m_Distance);
		base.transform.position = position;
		if (!this.m_IsActive)
		{
			return;
		}
		if (this.m_VisibleCheck.isVisible)
		{
			if (!this.m_IsActivated)
			{
				this.m_IsActivated = true;
			}
		}
		else if (this.m_IsActivated)
		{
			this.SetActive(active: false);
			this.m_IsActivated = false;
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
		if (this.m_ActiveSetter != null)
		{
			this.m_ActiveSetter.SetActive(active);
		}
	}
}
