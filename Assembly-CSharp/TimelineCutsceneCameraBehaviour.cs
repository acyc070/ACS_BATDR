using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutsceneCameraBehaviour : PlayableBehaviour
{
	public TimelineCutsceneCameraType CutsceneType;

	public Transform Target;

	public Transform StartLocation;

	public Transform EndLocation;

	public bool UnlockWeapons = true;

	public bool EndCrouching;

	public bool IsReal;

	public bool IsSnap;

	private bool m_IsPlayed;

	private GameObject m_Owner;

	private CutsceneDirector m_CutsceneDirector;

	public void SetOwner(GameObject owner)
	{
		this.m_Owner = owner;
	}

	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		if (Application.isPlaying && !this.m_IsPlayed)
		{
			this.m_IsPlayed = true;
			if (this.CutsceneType == TimelineCutsceneCameraType.Enter)
			{
				this.Enter();
			}
			else if (this.CutsceneType == TimelineCutsceneCameraType.Exit)
			{
				this.Exit();
			}
		}
	}

	private void Enter()
	{
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.HideHealthBar();
			GameManager.Instance.HideSprintBar();
			GameManager.Instance.HideCrosshair();
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.HideTeleport();
			}
			GameManager.Instance.Player.SetCollision(active: false);
			GameManager.Instance.Player.SetState(State.Player.Cutscene);
			GameManager.Instance.Player.SetAllTrackers(active: false);
			GameManager.Instance.Player.HideFirstPersonArms();
			GameManager.Instance.Player.ForceEnemiesFlee();
			GameManager.Instance.GameCamera.Camera.DOFieldOfView(55f, 0.15f);
			if (this.StartLocation != null)
			{
				Sequence sequence;
				sequence = DOTween.Sequence();
				sequence.Insert(0f, GameManager.Instance.Player.transform.DOMove(this.StartLocation.position, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.Insert(0f, GameManager.Instance.Player.transform.DORotate(this.StartLocation.eulerAngles, 0.25f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
				sequence.OnComplete(this.EnterGameCamera);
			}
			else
			{
				this.EnterGameCamera();
			}
		}
		else if (this.StartLocation != null)
		{
			Sequence sequence2;
			sequence2 = DOTween.Sequence();
			sequence2.Insert(0f, GameManager.Instance.BeastBendy.transform.DOMove(this.StartLocation.position, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			sequence2.Insert(0f, GameManager.Instance.BeastBendy.transform.DORotate(this.StartLocation.eulerAngles, 0.5f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed));
			sequence2.OnComplete(this.EnterGameCamera);
		}
		else
		{
			this.EnterGameCamera();
		}
	}

	private void EnterGameCamera()
	{
		Transform transform;
		transform = GameManager.Instance.GameCamera.InitializeFreeRoamCam(this.Target);
		transform.DOKill();
		if (!this.IsSnap)
		{
			transform.DOLocalMove(Vector3.zero, 0.75f).SetEase(Ease.InOutSine);
			transform.DOLocalRotate(Vector3.zero, 0.75f).SetEase(Ease.InOutSine);
		}
		else
		{
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
		}
		if (GameManager.Instance.BeastBendy == null)
		{
			GameManager.Instance.Player.gameObject.SetActive(value: false);
			return;
		}
		GameManager.Instance.BeastBendy.Disable();
		GameManager.Instance.BeastBendy.gameObject.SetActive(value: false);
	}

	private void Exit()
	{
		if (GameManager.Instance.BeastBendy == null)
		{
			if (GameManager.Instance.GameCamera.FreeRoamCam == null)
			{
				return;
			}
			this.SendToEndLocation();
			GameManager.Instance.GameCamera.ExitFreeRoamCam();
			if (this.m_Owner != null)
			{
				this.m_CutsceneDirector = this.m_Owner.GetComponent<CutsceneDirector>();
				if (this.m_CutsceneDirector != null && this.m_CutsceneDirector.IsSkipped)
				{
					GameManager.Instance.Player.HeadContainer.localPosition = Vector3.zero;
					GameManager.Instance.Player.HeadContainer.localEulerAngles = Vector3.zero;
				}
			}
			GameManager.Instance.Player.ForceResetAnimation();
			GameManager.Instance.Player.ResetRotation();
			if (this.EndCrouching)
			{
				GameManager.Instance.Player.ForceCrouch();
			}
			else
			{
				GameManager.Instance.Player.ForceStand();
			}
			GameManager.Instance.Player.SetAllTrackers(active: true);
			if (GameManager.Instance.Player.Health < UpgradeCheck.GetHealth())
			{
				GameManager.Instance.ShowHealthBar(GameManager.Instance.Player.Health, UpgradeCheck.GetHealth());
			}
			if (this.UnlockWeapons)
			{
				GameManager.Instance.Player.ShowFirstPersonArms();
				GameManager.Instance.ShowCrosshair(this.IsReal);
			}
			GameManager.Instance.Player.SetHeadContainerSlerp(active: true);
			GameManager.Instance.Player.SetState(State.Player.Default);
			GameManager.Instance.Player.SetCollision(active: true);
			if (!GameManager.Instance.Player.HasTeleport() || GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus != AbilityStatus.Active)
			{
				return;
			}
			if (GameManager.Instance.Player.Abilities != null && this.m_CutsceneDirector != null && this.m_CutsceneDirector.PlayableDirector != null)
			{
				foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
				{
					if (ability != null && ability is PlayerAbilityStateFlow && ability.IsCooldown)
					{
						ability.ForceSetCooldown(ability._Cooldown + (float)this.m_CutsceneDirector.PlayableDirector.duration);
					}
				}
			}
			GameManager.Instance.DisplayTeleport();
			GameManager.Instance.Player.UnlockAbilities();
		}
		else
		{
			GameManager.Instance.BeastBendy.gameObject.SetActive(value: true);
			GameManager.Instance.BeastBendy.Enable();
		}
	}

	private void SendToEndLocation(bool ignoreOffset = false)
	{
		if (!(this.EndLocation == null))
		{
			GameManager.Instance.Player.SetHeadContainerSlerp(active: false);
			GameManager.Instance.Player.ResetRotation();
			Vector3 vector;
			vector = Vector3.up * GameManager.Instance.Player.CharacterController.skinWidth;
			Vector3 vector2;
			vector2 = (ignoreOffset ? Vector3.zero : (this.EndLocation.forward * 0.5368653f));
			Vector3 position;
			position = this.EndLocation.position - vector2 + vector;
			GameManager.Instance.Player.transform.position = position;
			GameManager.Instance.Player.transform.eulerAngles = this.EndLocation.eulerAngles;
		}
	}
}
