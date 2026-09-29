using UnityEngine;

public class CharacterRotation : JDisposable
{
	[SerializeField]
	private float m_Sensitivity = 3f;

	private Quaternion m_CharacterTargetRotation;

	private bool m_IsRotationInitialized;

	private float m_RotationInput;

	public Character Character { get; private set; }

	public Quaternion ForcedRotation { get; private set; }

	public CharacterRotation(Character character)
	{
		this.Character = character;
		this.ResetRotation(this.Character.transform);
	}

	public void ForceRotation(Quaternion rotation)
	{
		this.m_CharacterTargetRotation = rotation;
	}

	public void SetRotation(Quaternion rotation)
	{
		this.ForcedRotation = rotation;
	}

	public void UpdateInput()
	{
		this.m_RotationInput = this.Character.RotateXInput * this.m_Sensitivity;
	}

	public void Update()
	{
		if (!this.IsNullRotation(this.m_RotationInput))
		{
			this.m_CharacterTargetRotation *= Quaternion.Euler(0f, this.m_RotationInput, 0f);
			this.Character.transform.rotation = this.m_CharacterTargetRotation;
		}
		else
		{
			this.Character.transform.rotation = this.m_CharacterTargetRotation;
		}
	}

	public void ResetRotation()
	{
		this.ResetRotation(this.Character.transform);
	}

	public void ResetRotation(Transform character)
	{
		this.m_CharacterTargetRotation.eulerAngles = new Vector3(0f, character.localRotation.eulerAngles.y, 0f);
	}

	private bool IsNullRotation(float horizontal)
	{
		if (horizontal == 0f)
		{
			return true;
		}
		if (!this.m_IsRotationInitialized)
		{
			this.m_IsRotationInitialized = true;
			return true;
		}
		return false;
	}

	public void SetSensitivity(float sensitivity)
	{
		this.m_Sensitivity = sensitivity * 5f;
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
