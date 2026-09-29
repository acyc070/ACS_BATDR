using UnityEngine;

public class PhotoModeController : JMonoBehaviour
{
	[SerializeField]
	private Transform m_CameraPivot;

	[SerializeField]
	private Transform m_CameraContainer;

	[SerializeField]
	private Transform m_CameraLookAt;

	[Space]
	[SerializeField]
	private AudioSource m_AudioSourceMusic;

	[SerializeField]
	private GameObject m_Overlay;

	private float m_LookX;

	private float m_LookY;

	private Transform m_Freeroam;

	private bool m_IsActive;

	private bool m_HasCamera;

	private float m_Distance = 8f;

	public override void Start()
	{
		this.m_Overlay.SetActive(value: false);
	}

	public void Enable()
	{
		this.m_HasCamera = true;
	}

	public void Disable()
	{
		this.m_HasCamera = false;
	}

	private void Update()
	{
		if (!this.m_HasCamera)
		{
			return;
		}
		if (this.m_IsActive)
		{
			if (PlayerInput.Pause())
			{
				this.Deactivate();
				return;
			}
			this.m_LookX = PlayerInput.LookX();
			this.m_LookY = PlayerInput.LookY();
			this.m_Distance -= Input.mouseScrollDelta.y;
		}
		else if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.P))
		{
			this.Activate();
		}
	}

	private void LateUpdate()
	{
		this.UpdateCamera();
	}

	private void UpdateCamera()
	{
		if (this.m_HasCamera && this.m_HasCamera && !(this.m_Freeroam == null))
		{
			this.m_CameraPivot.eulerAngles += new Vector3(0f, this.m_LookX, 0f);
			Vector3 localPosition;
			localPosition = this.m_CameraContainer.localPosition;
			if (this.m_Distance <= 3f)
			{
				this.m_Distance = 3f;
			}
			else if (this.m_Distance >= 30f)
			{
				this.m_Distance = 30f;
			}
			localPosition.z = this.m_Distance;
			this.m_CameraContainer.localPosition = localPosition;
			Vector3 vector;
			vector = base.transform.position + Vector3.up * 5f;
			Vector3 direction;
			direction = this.m_CameraContainer.position - vector;
			Vector3 b;
			b = this.m_CameraContainer.position;
			if (Physics.SphereCast(layerMask: ~((1 << LayerMask.NameToLayer("InvisibleCollider")) | (1 << LayerMask.NameToLayer("AI")) | (1 << LayerMask.NameToLayer("Player"))), origin: vector, radius: 1f, direction: direction, hitInfo: out var hitInfo, maxDistance: this.m_Distance, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				Vector3 vector2;
				vector2 = hitInfo.point - vector;
				b = hitInfo.point - vector2.normalized * 0.5f;
			}
			Vector3 b2;
			b2 = this.m_CameraLookAt.localPosition + new Vector3(0f, this.m_LookY, 0f);
			if (b2.y <= 2f)
			{
				b2.y = 2f;
			}
			else if (b2.y >= 8f)
			{
				b2.y = 8f;
			}
			this.m_CameraLookAt.localPosition = Vector3.Lerp(this.m_CameraLookAt.localPosition, b2, Time.unscaledDeltaTime * 5f);
			b.y = this.m_CameraContainer.position.y;
			this.m_Freeroam.position = Vector3.Lerp(this.m_Freeroam.position, b, Time.unscaledDeltaTime * 10f);
			Quaternion b3;
			b3 = Quaternion.LookRotation(this.m_CameraLookAt.position - this.m_Freeroam.position);
			this.m_Freeroam.rotation = Quaternion.Slerp(this.m_Freeroam.rotation, b3, Time.unscaledDeltaTime * 15f);
		}
	}

	private void Activate()
	{
		this.m_IsActive = true;
		this.m_Overlay.SetActive(value: true);
		GameManager.Instance.Player.SetAllTrackers(active: false);
		GameManager.Instance.Player.ModelLayers.EnableAll();
		GameManager.Instance.Pause();
		if (this.m_CameraPivot == null)
		{
			this.m_CameraPivot = new GameObject().transform;
			this.m_CameraPivot.position = GameManager.Instance.Player.transform.position + Vector3.up * 5f;
		}
		this.m_Distance = 8f;
		this.m_Freeroam = GameManager.Instance.GameCamera.InitializeFreeRoamCam(this.m_CameraPivot);
		this.m_Freeroam.localPosition = this.m_CameraContainer.localPosition;
		this.m_Freeroam.localEulerAngles = this.m_CameraContainer.localEulerAngles;
		this.m_AudioSourceMusic.Play();
		UIManager.SetCursor(active: false);
	}

	private void Deactivate()
	{
		this.m_IsActive = false;
		this.m_Overlay.SetActive(value: false);
		GameManager.Instance.Unpause();
		GameManager.Instance.GameCamera.ExitFreeRoamCam();
		this.m_Freeroam = null;
		GameManager.Instance.Player.ResetRotation();
		GameManager.Instance.Player.SetHeadTracker(active: true);
		GameManager.Instance.Player.ModelLayers.EnableFirstPerson();
		GameManager.Instance.Player.ShowFirstPersonArms();
		this.m_AudioSourceMusic.Stop();
		UIManager.SetCursor(active: false);
	}
}
