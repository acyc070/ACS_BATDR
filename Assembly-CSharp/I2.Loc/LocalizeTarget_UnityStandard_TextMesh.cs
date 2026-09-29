using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_UnityStandard_TextMesh : LocalizeTarget<TextMesh>
{
	private TextAlignment mAlignment_RTL = TextAlignment.Right;

	private TextAlignment mAlignment_LTR;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_UnityStandard_TextMesh()
	{
		LocalizeTarget_UnityStandard_TextMesh.AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoRegister()
	{
		LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMesh, LocalizeTarget_UnityStandard_TextMesh>
		{
			Name = "TextMesh",
			Priority = 100
		});
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.Font;
	}

	public override bool CanUseSecondaryTerm()
	{
		return true;
	}

	public override bool AllowMainTermToBeRTL()
	{
		return true;
	}

	public override bool AllowSecondTermToBeRTL()
	{
		return false;
	}

	public override void GetFinalTerms(Localize cmp, string Main, string Secondary, out string primaryTerm, out string secondaryTerm)
	{
		primaryTerm = (base.mTarget ? base.mTarget.text : null);
		secondaryTerm = ((string.IsNullOrEmpty(Secondary) && base.mTarget.font != null) ? base.mTarget.font.name : null);
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		Font secondaryTranslatedObj;
		secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
		if (secondaryTranslatedObj != null && base.mTarget.font != secondaryTranslatedObj)
		{
			base.mTarget.font = secondaryTranslatedObj;
		}
		if (this.mInitializeAlignment)
		{
			this.mInitializeAlignment = false;
			this.mAlignment_LTR = (this.mAlignment_RTL = base.mTarget.alignment);
			if (LocalizationManager.IsRight2Left && this.mAlignment_RTL == TextAlignment.Right)
			{
				this.mAlignment_LTR = TextAlignment.Left;
			}
			if (!LocalizationManager.IsRight2Left && this.mAlignment_LTR == TextAlignment.Left)
			{
				this.mAlignment_RTL = TextAlignment.Right;
			}
		}
		if (mainTranslation != null && base.mTarget.text != mainTranslation)
		{
			if (cmp.CorrectAlignmentForRTL && base.mTarget.alignment != TextAlignment.Center)
			{
				base.mTarget.alignment = (LocalizationManager.IsRight2Left ? this.mAlignment_RTL : this.mAlignment_LTR);
			}
			base.mTarget.font.RequestCharactersInTexture(mainTranslation);
			base.mTarget.text = mainTranslation;
		}
	}
}
