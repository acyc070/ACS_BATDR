using UnityEngine;
using UnityEngine.Playables;

public class TimelineCutsceneBarsBehaviour : PlayableBehaviour
{
	[Header("IsEnd")]
	public bool IsEnd;

	public bool ShowCrosshair = true;

	[Header("Player")]
	public bool SlowPlayer;

	public bool LockPlayer;

	public bool SkipPlayIn;

	public bool DisableSkip;

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
			if (!this.IsEnd)
			{
				this.Begin();
			}
			else
			{
				this.End();
			}
		}
	}

	private void Begin()
	{
		if (this.m_Owner != null)
		{
			this.m_CutsceneDirector = this.m_Owner.GetComponent<CutsceneDirector>();
		}
		GameManager.Instance.ShowCutsceneBars(this.m_CutsceneDirector, this.SkipPlayIn, this.DisableSkip);
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.HideSprintBar();
		GameManager.Instance.HideCrosshair();
		if (!(GameManager.Instance.Player != null))
		{
			return;
		}
		GameManager.Instance.Player.LockWeapon();
		if (GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
			GameManager.Instance.Player.LockAbilities();
		}
		if (this.SlowPlayer || this.LockPlayer)
		{
			GameManager.Instance.Player.Interaction.ResetInteraction();
			GameManager.Instance.Player.SetInteraction(active: false);
			GameManager.Instance.Player.PlayerMovement.ForceLockRun();
			GameManager.Instance.Player.PlayerMovement.LockJump();
			if (this.SlowPlayer)
			{
				GameManager.Instance.Player.PlayerMovement.SetMoveSpeed(0.5f, _isSlowed: true);
			}
			GameManager.Instance.Player.HideFirstPersonArms();
		}
	}

	private void End()
	{
		GameManager.Instance.HideCutsceneBars();
		if (this.ShowCrosshair)
		{
			GameManager.Instance.ShowCrosshair(GameManager.Instance.IsRealWorld);
		}
		if (GameManager.Instance.Player != null)
		{
			if (this.SlowPlayer || this.LockPlayer)
			{
				GameManager.Instance.Player.PlayerMovement.ForceUnlockRun();
				GameManager.Instance.Player.PlayerMovement.ResetMoveSpeed();
				GameManager.Instance.Player.PlayerMovement.UnlockJump();
				GameManager.Instance.Player.ShowFirstPersonArms();
				GameManager.Instance.Player.Interaction.ResetInteraction();
				GameManager.Instance.Player.SetInteraction(active: true);
			}
			if (GameManager.Instance.Player.Health < UpgradeCheck.GetHealth())
			{
				GameManager.Instance.ShowHealthBar(GameManager.Instance.Player.Health, UpgradeCheck.GetHealth());
			}
			GameManager.Instance.Player.UnlockWeapon();
			if (GameManager.Instance.Player.HasTeleport() && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				GameManager.Instance.DisplayTeleport();
				GameManager.Instance.Player.UnlockAbilities();
			}
		}
	}
}
