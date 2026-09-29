using UnityEngine;

public class InteractableInputDisplay : InteractableProximity
{
	[Header("Visual Options")]
	[SerializeField]
	protected bool m_IsReal;

	[SerializeField]
	protected bool m_DisableInputVisuals;

	[Header("Input Options")]
	[SerializeField]
	private InteractionType m_InteractionType;

	public void SetInteractionType(InteractionType interactionType)
	{
		this.m_InteractionType = interactionType;
	}

	protected override void OnInternalEnter(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInteractInvalid();
		if (!this.m_DisableInputVisuals)
		{
			string text;
			text = TextUtility.GetKey(this.m_InteractionType.ToString());
			if (this.m_IsReal)
			{
				text = text.ToUpper();
			}
			GameManager.Instance.ShowInteraction(text, this.m_IsReal);
		}
	}

	protected override void OnInternalExit(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInteraction();
	}

	protected override void OnInternalInteract(Vector3 origin, RaycastHit hit, object sender = null)
	{
		GameManager.Instance.HideInteraction();
		GameManager.Instance.Player.Interaction.ResetInteraction();
		if (base.m_presetRumble != ControllerRumble.RUMBLE_PRESETS.NONE)
		{
			GameManager.Instance.TriggerRumble(ControllerRumble.rumblePresets[unchecked((int)base.m_presetRumble)]);
		}
		else
		{
			GameManager.Instance.TriggerRumble(base.m_CustomRumble);
		}
	}
}
