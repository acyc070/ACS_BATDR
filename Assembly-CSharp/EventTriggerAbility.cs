using System;
using UnityEngine;

public class EventTriggerAbility : EventTrigger
{
	[Space(10f)]
	[Header("Requirements")]
	[SerializeField]
	private bool m_IsRequirementsComplete;

	[SerializeField]
	private Requirements m_Requirements;

	[Header("Ability Status")]
	[SerializeField]
	private AbilityStatus m_AbilityStatus;

	[Header("Notification")]
	[SerializeField]
	private string m_Label = string.Empty;

	[SerializeField]
	private bool m_IsKey;

	protected override void OnInternalEnter(Collider col)
	{
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == this.m_AbilityStatus)
		{
			return;
		}
		if (this.m_IsRequirementsComplete)
		{
			if (this.CheckStatus())
			{
				this.SetStatus();
			}
		}
		else if (!this.CheckStatus())
		{
			this.SetStatus();
		}
	}

	private void SetStatus()
	{
		GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.SetStatus(this.m_AbilityStatus);
		if (this.m_AbilityStatus == AbilityStatus.None)
		{
			return;
		}
		CameraEffects.GainPower(0.75f);
		CameraEffects.ShakeRotation(2f, 1f);
		if (this.m_AbilityStatus == AbilityStatus.Active)
		{
			GameManager.Instance.Player.UnlockAbilities(playAudio: true);
			if (!GameManager.Instance.Player.HasTeleport())
			{
				return;
			}
			if (GameManager.Instance.Player.Abilities != null)
			{
				foreach (PlayerAbilityState ability in GameManager.Instance.Player.Abilities)
				{
					if (ability != null && ability is PlayerAbilityStateFlow)
					{
						ability.ForceCooldown();
						break;
					}
				}
			}
			GameManager.Instance.DisplayTeleport();
		}
		else
		{
			if (this.m_AbilityStatus != AbilityStatus.Inactive)
			{
				return;
			}
			GameManager.Instance.Player.LockAbilities(playAudio: true);
			if (GameManager.Instance.Player.HasTeleport())
			{
				GameManager.Instance.HideTeleport();
			}
			if (this.m_Label != string.Empty)
			{
				if (this.m_IsKey)
				{
					GameManager.Instance.ShowNotificationText(TextUtility.GetKey(this.m_Label).GetColorNotification().GetWhite()
						.ToUpper());
				}
				else
				{
					GameManager.Instance.ShowNotificationText(this.m_Label);
				}
			}
		}
	}

	private bool CheckStatus()
	{
		bool result;
		result = true;
		if (this.m_Requirements != null)
		{
			result = this.m_Requirements.IsComplete();
		}
		return result;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}
}
