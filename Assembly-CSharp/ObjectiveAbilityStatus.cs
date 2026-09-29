using UnityEngine;

public class ObjectiveAbilityStatus : Objective
{
	[SerializeField]
	private AbilityStatus m_AbilityStatus;

	[SerializeField]
	private bool m_ForceCooldown = true;

	[SerializeField]
	private bool m_IsSilent;

	protected override void InternalInitialize()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(this.m_AbilityStatus);
		if (this.m_AbilityStatus != AbilityStatus.None)
		{
			if (!this.m_IsSilent)
			{
				CameraEffects.GainPower(0.75f);
				CameraEffects.ShakeRotation(2f, 1f);
			}
			if (this.m_AbilityStatus == AbilityStatus.Active)
			{
				GameManager.Instance.Player.UnlockAbilities(playAudio: true);
				if (GameManager.Instance.Player.Abilities != null && GameManager.Instance.Player.HasTeleport())
				{
					GameManager.Instance.EnableTeleport();
					GameManager.Instance.DisplayTeleport();
					foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
					{
						if (ability != null && ability is PlayerAbilityStateFlow)
						{
							if (this.m_ForceCooldown)
							{
								ability.ForceCooldown();
							}
							else
							{
								ability.ForceSetCooldown(ability._CooldownCount);
							}
							break;
						}
					}
				}
			}
			else if (this.m_AbilityStatus == AbilityStatus.Inactive)
			{
				GameManager.Instance.Player.LockAbilities(!this.m_IsSilent);
				if (GameManager.Instance.Player.HasTeleport())
				{
					GameManager.Instance.HideTeleport();
				}
			}
		}
		base.SendOnComplete();
	}
}
