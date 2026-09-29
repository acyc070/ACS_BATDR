using DG.Tweening;
using JDS.PostProcessing;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[DefaultExecutionOrder(-95)]
public class GameCamera : JMonoBehaviour
{
	private static readonly int kFirstPersonProjMatrixId;

	[Header("Post Process")]
	[SerializeField]
	private PostProcessVolumes m_PostProceussVolumes;

	[Header("First Person")]
	[SerializeField]
	private Camera m_FirstPersonCamera;

	[SerializeField]
	private GameObject m_FirstPersonArms;

	[SerializeField]
	private Transform m_ArmsContainer;

	[Header("Particles")]
	[SerializeField]
	private ParticleSystem m_TeleportParticles;

	[SerializeField]
	private ParticleSystem m_BanishParticles;

	[Header("Underwater")]
	[SerializeField]
	private GameObject m_UnderWater;

	private PostProcessLayer m_PostProcessLayer;

	private MotionBlur m_MotionBlur;

	private AmbientOcclusion m_AmbientOcclusion;

	private DepthOfField m_DepthOfField;

	private Bloom m_Bloom;

	private ColorGrading m_ColorGrading;

	private DamageEffect m_DamagePostProcessEffect;

	private Takedown m_TakedownPostProcessEffect;

	private GainPowerEffect m_GainPowerPostProcessEffect;

	private InkDemonEffect m_InkDemonPostProcessEffect;

	private VisionEffect m_VisionPostProcessEffect;

	private Sequence m_DamageSequence;

	private Sequence m_TakedownSequence;

	private Sequence m_GainPowerSequence;

	private Sequence m_InkDemonSequence;

	private Renderer[] renderers;

	private Tweener m_CameraShakeVibration;

	public Camera FirstPersonCamera => this.m_FirstPersonCamera;

	public Transform ArmsContainer => this.m_ArmsContainer;

	public ParticleSystem TeleportParticles => this.m_TeleportParticles;

	public ParticleSystem BanishParticles => this.m_BanishParticles;

	public bool IsUnderWater
	{
		get
		{
			if (this.m_UnderWater != null)
			{
				return this.m_UnderWater.activeSelf;
			}
			return false;
		}
	}

	public Camera Camera { get; private set; }

	public Transform CameraContainer { get; private set; }

	public Transform HeadContainer { get; private set; }

	public Transform FreeRoamCam { get; private set; }

	public Animator FirstPersonArmsAnimator { get; private set; }

	public CameraDepthOfFieldDistanceSetter DOFDistanceSetter { get; private set; }

	public PostProcessVolume ColorPostProcess => this.m_PostProceussVolumes.ColorPostProcessVolume;

	public PostProcessVolume BasePostProcess => this.m_PostProceussVolumes.BasePostProcessVolume;

	public bool AA
	{
		get
		{
			return this.m_PostProcessLayer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing;
		}
		set
		{
			this.m_PostProcessLayer.antialiasingMode = (value ? PostProcessLayer.Antialiasing.TemporalAntialiasing : PostProcessLayer.Antialiasing.None);
		}
	}

	public bool MotionBlur
	{
		get
		{
			return this.m_MotionBlur.active;
		}
		set
		{
			this.m_MotionBlur.active = value;
		}
	}

	public bool DoF
	{
		get
		{
			return this.m_DepthOfField.active;
		}
		set
		{
			this.m_DepthOfField.active = value;
			if (this.DOFDistanceSetter == null)
			{
				this.DOFDistanceSetter = base.GetComponent<CameraDepthOfFieldDistanceSetter>();
			}
			this.DOFDistanceSetter.Initialize(value, this.m_DepthOfField);
		}
	}

	public bool Bloom
	{
		get
		{
			return this.m_Bloom.active;
		}
		set
		{
			this.m_Bloom.active = value;
		}
	}

	public bool AmbientOcclusion
	{
		get
		{
			return this.m_AmbientOcclusion.active;
		}
		set
		{
			this.m_AmbientOcclusion.active = value;
		}
	}

	public float Brightness
	{
		get
		{
			return this.m_ColorGrading.brightness.value;
		}
		set
		{
			unchecked
			{
				int num;
				num = (int)(value * 10f);
				if (num <= 0)
				{
					num = 0;
				}
				else if (num >= 10)
				{
					num = 10;
				}
				float num2;
				num2 = 0f;
				num2 = ((num > 5) ? ((float)(num - 5) * 2.5f) : (0f - (float)(5 - num) * 2.5f));
				int num3;
				num3 = (GameManager.Instance.IsRealWorld ? 5 : 32);
				this.m_ColorGrading.brightness.value = (float)num3 + num2;
			}
		}
	}

	public override void Awake()
	{
		if (this.m_UnderWater != null)
		{
			this.m_UnderWater.SetActive(value: false);
		}
		this.Camera = base.GetComponent<Camera>();
		if (this.Camera != null)
		{
			this.Camera.enabled = false;
		}
		if (this.m_PostProcessLayer == null)
		{
			this.m_PostProcessLayer = base.GetComponent<PostProcessLayer>();
		}
		if (this.m_PostProcessLayer != null)
		{
			this.AA = GameManager.Instance.PlayerSettings.AA;
		}
		this.m_TakedownPostProcessEffect = this.BasePostProcess.profile.GetSetting<Takedown>();
		if (this.m_TakedownPostProcessEffect != null)
		{
			this.m_TakedownPostProcessEffect._Power.value = 0f;
		}
		this.m_GainPowerPostProcessEffect = this.BasePostProcess.profile.GetSetting<GainPowerEffect>();
		if (this.m_GainPowerPostProcessEffect != null)
		{
			this.m_GainPowerPostProcessEffect._Power.value = 0f;
		}
		this.m_VisionPostProcessEffect = this.BasePostProcess.profile.GetSetting<VisionEffect>();
		if (this.m_VisionPostProcessEffect != null)
		{
			this.m_VisionPostProcessEffect.active = false;
		}
		this.m_DamagePostProcessEffect = this.ColorPostProcess.profile.GetSetting<DamageEffect>();
		if (this.m_DamagePostProcessEffect != null)
		{
			this.m_DamagePostProcessEffect._Power.value = 0f;
		}
		this.m_InkDemonPostProcessEffect = this.ColorPostProcess.profile.GetSetting<InkDemonEffect>();
		if (this.m_InkDemonPostProcessEffect != null)
		{
			this.m_InkDemonPostProcessEffect._Power.value = 0f;
		}
		this.m_MotionBlur = this.BasePostProcess.profile.GetSetting<MotionBlur>();
		if (this.m_MotionBlur != null)
		{
			this.MotionBlur = GameManager.Instance.PlayerSettings.MotionBlur;
		}
		this.m_AmbientOcclusion = this.BasePostProcess.profile.GetSetting<AmbientOcclusion>();
		if (this.m_AmbientOcclusion != null)
		{
			this.AmbientOcclusion = GameManager.Instance.PlayerSettings.AmbientOcclusion;
		}
		this.m_DepthOfField = this.BasePostProcess.profile.GetSetting<DepthOfField>();
		if (this.m_DepthOfField != null)
		{
			this.DoF = GameManager.Instance.PlayerSettings.DoF;
		}
		this.m_Bloom = this.ColorPostProcess.profile.GetSetting<Bloom>();
		if (this.m_Bloom != null)
		{
			this.m_Bloom.active = GameManager.Instance.PlayerSettings.Bloom;
		}
		this.m_ColorGrading = this.ColorPostProcess.profile.GetSetting<ColorGrading>();
		if (this.m_ColorGrading != null)
		{
			this.Brightness = GameManager.Instance.PlayerSettings.Brightness;
		}
		if (this.m_FirstPersonArms != null)
		{
			this.FirstPersonArmsAnimator = this.m_FirstPersonArms.GetComponent<Animator>();
			this.renderers = this.m_FirstPersonArms.GetComponentsInChildren<Renderer>();
		}
	}

	public override void Start()
	{
		this.Camera.enabled = true;
		this.FreeRoamCam = new GameObject("FreeRoamCam").transform;
		Object.DontDestroyOnLoad(this.FreeRoamCam);
	}

	private void LateUpdate()
	{
		if (this.m_FirstPersonCamera != null)
		{
			Shader.SetGlobalMatrix(GameCamera.kFirstPersonProjMatrixId, this.m_FirstPersonCamera.projectionMatrix);
		}
	}

	public void Initialize(Transform headContainer, Transform cameraContainer)
	{
		this.HeadContainer = headContainer;
		this.CameraContainer = cameraContainer;
		GameManager.Instance.GameCamera = this;
	}

	public void SetFirstPersonArmsActive(bool active)
	{
		if (!(this.m_FirstPersonArms != null))
		{
			return;
		}
		this.m_FirstPersonArms.SetActive(active);
		unchecked
		{
			if (active)
			{
				Renderer[] array;
				array = this.renderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].material.EnableKeyword("FIRST_PERSON");
				}
			}
			else
			{
				Renderer[] array;
				array = this.renderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].material.DisableKeyword("FIRST_PERSON");
				}
			}
		}
	}

	public void SetUnderWater(bool active)
	{
		if (this.m_UnderWater != null)
		{
			this.m_UnderWater.SetActive(active);
		}
	}

	public Transform InitializeFreeRoamCam()
	{
		return this.InitializeFreeRoamCam(null);
	}

	public Transform InitializeFreeRoamCam(Transform parent)
	{
		if (this.FreeRoamCam == null)
		{
			this.FreeRoamCam = new GameObject("FreeRoamCam").transform;
		}
		if (this.HeadContainer != null)
		{
			this.FreeRoamCam.position = this.HeadContainer.position;
			this.FreeRoamCam.rotation = this.HeadContainer.rotation;
			this.HeadContainer.SetParent(this.FreeRoamCam);
		}
		this.FreeRoamCam.SetParent(parent);
		return this.FreeRoamCam;
	}

	public void ExitFreeRoamCam()
	{
		if (!(this.FreeRoamCam == null))
		{
			if (this.FreeRoamCam != null)
			{
				this.FreeRoamCam.SetParent(null);
			}
			if (!GameManager.Instance.Player.gameObject.activeSelf)
			{
				GameManager.Instance.Player.gameObject.SetActive(value: true);
			}
			Vector3 zero;
			zero = Vector3.zero;
			zero.x = this.FreeRoamCam.eulerAngles.x;
			Vector3 zero2;
			zero2 = Vector3.zero;
			zero2.y = this.FreeRoamCam.localEulerAngles.y;
			this.HeadContainer.SetParent(GameManager.Instance.Player.CameraPivot);
			GameManager.Instance.Player.ForceRotation(Quaternion.Euler(zero2), Quaternion.Euler(zero));
			this.FreeRoamCam.SetParent(null);
		}
	}

	public void ShakeCamera(float duration, float strength = 10f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, bool vibrate = true)
	{
		this.Camera.transform.DOKill();
		this.Camera.transform.localEulerAngles = Vector3.zero;
		this.Camera.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut).OnComplete(() =>
		{
			this.Camera.transform.localEulerAngles = Vector3.zero;
		});
		if (vibrate)
		{
			GameManager.Instance.TriggerRumble(duration, strength);
		}
	}

	public void Damage(float endDuration = 3f)
	{
		this.m_DamageSequence?.Kill();
		this.m_DamageSequence = DOTween.Sequence();
		float num;
		num = this.m_DamagePostProcessEffect._Power.value + 0.3f;
		if (num > 0.9f)
		{
			num = 0.9f;
		}
		float num2;
		num2 = 0.25f;
		this.m_DamageSequence.Insert(0f, DOTween.To(() => this.m_DamagePostProcessEffect._Power.value, (float x) =>
		{
			this.m_DamagePostProcessEffect._Power.value = x;
		}, num, num2).SetEase(Ease.OutSine));
		this.m_DamageSequence.Insert(num2 + 0.25f, DOTween.To(() => this.m_DamagePostProcessEffect._Power.value, (float x) =>
		{
			this.m_DamagePostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.OutSine));
	}

	public void Takedown(float endDuration = 3f)
	{
		this.m_TakedownSequence?.Kill();
		this.m_TakedownSequence = DOTween.Sequence();
		float num;
		num = this.m_TakedownPostProcessEffect._Power.value + 0.3f;
		if (num > 0.9f)
		{
			num = 0.9f;
		}
		float num2;
		num2 = 0.25f;
		this.m_TakedownSequence.Insert(0f, DOTween.To(() => this.m_TakedownPostProcessEffect._Power.value, (float x) =>
		{
			this.m_TakedownPostProcessEffect._Power.value = x;
		}, num, num2).SetEase(Ease.OutSine));
		this.m_TakedownSequence.Insert(num2 + 0.25f, DOTween.To(() => this.m_TakedownPostProcessEffect._Power.value, (float x) =>
		{
			this.m_TakedownPostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.OutSine));
	}

	public void GainPower(float endDuration = 3f)
	{
		this.m_GainPowerSequence?.Kill();
		this.m_GainPowerSequence = DOTween.Sequence();
		float num;
		num = 1f;
		this.m_GainPowerSequence.Insert(0f, DOTween.To(() => this.m_GainPowerPostProcessEffect._Power.value, (float x) =>
		{
			this.m_GainPowerPostProcessEffect._Power.value = x;
		}, 1f, num).SetEase(Ease.OutSine));
		this.m_GainPowerSequence.Insert(num + 0.25f, DOTween.To(() => this.m_GainPowerPostProcessEffect._Power.value, (float x) =>
		{
			this.m_GainPowerPostProcessEffect._Power.value = x;
		}, 0f, endDuration).SetEase(Ease.InSine));
	}

	public void InkDemon(float endDuration = 3f)
	{
		this.m_InkDemonSequence?.Kill();
		this.m_InkDemonSequence = DOTween.Sequence();
		this.m_InkDemonSequence.Insert(0f, DOTween.To(() => this.m_InkDemonPostProcessEffect._Power.value, (float x) =>
		{
			this.m_InkDemonPostProcessEffect._Power.value = x;
		}, 1f, 0.25f).SetEase(Ease.OutSine));
		this.m_InkDemonSequence.Insert(endDuration, DOTween.To(() => this.m_InkDemonPostProcessEffect._Power.value, (float x) =>
		{
			this.m_InkDemonPostProcessEffect._Power.value = x;
		}, 0f, 2f).SetEase(Ease.OutSine));
	}

	public void InkDemonOn()
	{
		this.m_InkDemonSequence?.Kill();
		this.m_InkDemonSequence = DOTween.Sequence();
		this.m_InkDemonSequence.Insert(0f, DOTween.To(() => this.m_InkDemonPostProcessEffect._Power.value, (float x) =>
		{
			this.m_InkDemonPostProcessEffect._Power.value = x;
		}, 1f, 0.25f).SetEase(Ease.OutSine));
	}

	public void InkDemonOff()
	{
		this.m_InkDemonSequence?.Kill();
		this.m_InkDemonSequence = DOTween.Sequence();
		this.m_InkDemonSequence.Insert(0f, DOTween.To(() => this.m_InkDemonPostProcessEffect._Power.value, (float x) =>
		{
			this.m_InkDemonPostProcessEffect._Power.value = x;
		}, 0f, 2f).SetEase(Ease.OutSine));
	}

	public void VisionOn()
	{
		this.m_VisionPostProcessEffect.active = true;
	}

	public void VisionOff()
	{
		this.m_VisionPostProcessEffect.active = false;
	}

	public void VisionTransition(bool active)
	{
		this.m_VisionPostProcessEffect._Transition.value = active;
	}

	public Vector3 WorldToScreenPoint(Vector3 position)
	{
		return this.Camera.WorldToScreenPoint(position);
	}

	public bool CheckPositionAngle(Vector3 position, float angle = 0f)
	{
		return this.WorldToScreenPoint(position).z < angle;
	}

	protected override void OnDisposed()
	{
		this.m_DamageSequence?.Kill();
		this.m_DamageSequence = null;
		this.m_TakedownSequence?.Kill();
		this.m_TakedownSequence = null;
		this.m_GainPowerSequence?.Kill();
		this.m_GainPowerSequence = null;
		this.m_InkDemonSequence?.Kill();
		this.m_InkDemonSequence = null;
		this.m_PostProcessLayer = null;
		this.m_MotionBlur = null;
		this.m_AmbientOcclusion = null;
		this.m_DepthOfField = null;
		this.m_Bloom = null;
		this.m_DamagePostProcessEffect = null;
		this.m_TakedownPostProcessEffect = null;
		this.m_GainPowerPostProcessEffect = null;
		this.m_VisionPostProcessEffect = null;
		this.m_InkDemonPostProcessEffect = null;
		this.Camera = null;
		this.CameraContainer = null;
		this.HeadContainer = null;
		this.FirstPersonArmsAnimator = null;
		this.DOFDistanceSetter = null;
		this.renderers = null;
		if (this.FreeRoamCam != null)
		{
			Object.Destroy(this.FreeRoamCam.gameObject);
			this.FreeRoamCam = null;
		}
		base.OnDisposed();
	}

	static GameCamera()
	{
		GameCamera.kFirstPersonProjMatrixId = Shader.PropertyToID("_FirstPersonProjMatrix");
	}
}
