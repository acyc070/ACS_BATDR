using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_UnityStandard_AudioSource : LocalizeTarget<AudioSource>
{
	static LocalizeTarget_UnityStandard_AudioSource()
	{
		LocalizeTarget_UnityStandard_AudioSource.AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoRegister()
	{
		LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<AudioSource, LocalizeTarget_UnityStandard_AudioSource>
		{
			Name = "AudioSource",
			Priority = 100
		});
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.AudioClip;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override bool CanUseSecondaryTerm()
	{
		return false;
	}

	public override bool AllowMainTermToBeRTL()
	{
		return false;
	}

	public override bool AllowSecondTermToBeRTL()
	{
		return false;
	}

	public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = (base.mTarget.clip ? base.mTarget.clip.name : string.Empty);
		secondaryTerm = null;
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		bool num;
		num = (base.mTarget.isPlaying || base.mTarget.loop) && Application.isPlaying;
		AudioClip clip;
		clip = base.mTarget.clip;
		AudioClip audioClip;
		audioClip = cmp.FindTranslatedObject<AudioClip>(mainTranslation);
		if (clip != audioClip)
		{
			base.mTarget.clip = audioClip;
		}
		if (num && (bool)base.mTarget.clip)
		{
			base.mTarget.Play();
		}
	}
}
