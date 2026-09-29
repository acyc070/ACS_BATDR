using DG.Tweening;
using UnityEngine;

public class UIModelHandSigil : JMonoBehaviour
{
	private const string EMISSION_POWER = "_EmitPower";

	private const string EMISSION_TEXTURE = "_Emission";

	[Header("Renderer")]
	[SerializeField]
	private SkinnedMeshRenderer m_InkHand;

	[Header("Sigils")]
	[SerializeField]
	private Texture m_BanishSigil;

	[SerializeField]
	private Texture m_FlowSigil;

	[SerializeField]
	private Texture m_FastTravelSigil;

	private bool m_HasBanish;

	private bool m_HasFlow;

	private bool m_HasFastTravel;

	private Sequence m_Sequence;

	public override void OnEnable()
	{
		AbilityDataDirectory abilityDataDirectory;
		abilityDataDirectory = GameManager.Instance.GameData.CurrentSave?.PlayerData.AbilityDirectory;
		if (abilityDataDirectory != null)
		{
			this.m_HasBanish = abilityDataDirectory.ContainsKey(State.PlayerAbility.Banish);
			this.m_HasFlow = abilityDataDirectory.ContainsKey(State.PlayerAbility.Flow);
			this.m_HasFastTravel = abilityDataDirectory.ContainsKey(State.PlayerAbility.FastTravel);
		}
		this.BeginSequence();
	}

	public override void OnDisable()
	{
		this.KillSequence();
		this.ClearSigil();
	}

	private void BeginSequence()
	{
		if (!this.m_HasBanish && !this.m_HasFlow && !this.m_HasFastTravel)
		{
			return;
		}
		this.ResetSequence();
		float num;
		num = 0f;
		if (this.m_HasBanish && !this.m_HasFlow && !this.m_HasFastTravel)
		{
			this.SetSigil(this.m_BanishSigil);
			this.m_Sequence.Insert(num, this.m_InkHand.material.DOFloat(1.5f, "_EmitPower", 0.5f).SetEase(Ease.InSine));
			this.m_Sequence.Insert(num + 0.5f, this.m_InkHand.material.DOFloat(1f, "_EmitPower", 0.5f).SetEase(Ease.OutSine));
			return;
		}
		if (this.m_HasBanish)
		{
			this.DOEmission(num, this.m_BanishSigil);
			num += 5f;
		}
		if (this.m_HasFlow)
		{
			this.DOEmission(num, this.m_FlowSigil);
			num += 5f;
		}
		if (this.m_HasFastTravel)
		{
			this.DOEmission(num, this.m_FastTravelSigil);
			num += 5f;
		}
		this.m_Sequence.InsertCallback(num, this.BeginSequence);
	}

	private void DOEmission(float atPosition, Texture texture)
	{
		this.m_Sequence.InsertCallback(atPosition, () =>
		{
			this.SetSigil(texture);
		});
		this.m_Sequence.Insert(atPosition, this.m_InkHand.material.DOFloat(1.5f, "_EmitPower", 0.5f).SetEase(Ease.InSine));
		atPosition += 0.5f;
		this.m_Sequence.Insert(atPosition, this.m_InkHand.material.DOFloat(1f, "_EmitPower", 0.5f).SetEase(Ease.OutSine));
		atPosition += 4f;
		this.m_Sequence.Insert(atPosition, this.m_InkHand.material.DOFloat(0f, "_EmitPower", 0.5f).SetEase(Ease.InSine));
	}

	private void SetSigil(Texture texture)
	{
		this.m_InkHand.material.SetTexture("_Emission", texture);
	}

	private void ClearSigil()
	{
		this.SetSigil(null);
		this.m_InkHand.material.SetFloat("_EmitPower", 0f);
	}

	private void ResetSequence()
	{
		this.KillSequence();
		this.m_Sequence = DOTween.Sequence().SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
	}

	private void KillSequence()
	{
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
	}

	protected override void OnDisposed()
	{
		this.KillSequence();
		base.OnDisposed();
	}
}
