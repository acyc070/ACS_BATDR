using System;
using TMPro;
using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_TextMeshPro_UGUI : LocalizeTarget<TextMeshProUGUI>
{
	public TextAlignmentOptions mAlignment_RTL = TextAlignmentOptions.Right;

	public TextAlignmentOptions mAlignment_LTR = TextAlignmentOptions.Left;

	public bool mAlignmentWasRTL;

	public bool mInitializeAlignment = true;

	static LocalizeTarget_TextMeshPro_UGUI()
	{
		LocalizeTarget_TextMeshPro_UGUI.AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoRegister()
	{
		LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMeshProUGUI, LocalizeTarget_TextMeshPro_UGUI>
		{
			Name = "TextMeshPro UGUI",
			Priority = 100
		});
	}

	public override eTermType GetPrimaryTermType(Localize cmp)
	{
		return eTermType.Text;
	}

	public override eTermType GetSecondaryTermType(Localize cmp)
	{
		return eTermType.TextMeshPFont;
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
		secondaryTerm = ((base.mTarget.font != null) ? base.mTarget.font.name : string.Empty);
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		TMP_FontAsset secondaryTranslatedObj;
		secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<TMP_FontAsset>(ref mainTranslation, ref secondaryTranslation);
		if (secondaryTranslatedObj != null)
		{
			LocalizeTarget_TextMeshPro_Label.SetFont(base.mTarget, secondaryTranslatedObj);
		}
		else
		{
			Material secondaryTranslatedObj2;
			secondaryTranslatedObj2 = cmp.GetSecondaryTranslatedObj<Material>(ref mainTranslation, ref secondaryTranslation);
			if (secondaryTranslatedObj2 != null && base.mTarget.fontMaterial != secondaryTranslatedObj2)
			{
				if (!secondaryTranslatedObj2.name.StartsWith(base.mTarget.font.name, StringComparison.Ordinal))
				{
					secondaryTranslatedObj = LocalizeTarget_TextMeshPro_Label.GetTMPFontFromMaterial(cmp, secondaryTranslation.EndsWith(secondaryTranslatedObj2.name, StringComparison.Ordinal) ? secondaryTranslation : secondaryTranslatedObj2.name);
					if (secondaryTranslatedObj != null)
					{
						LocalizeTarget_TextMeshPro_Label.SetFont(base.mTarget, secondaryTranslatedObj);
					}
				}
				LocalizeTarget_TextMeshPro_Label.SetMaterial(base.mTarget, secondaryTranslatedObj2);
			}
		}
		if (this.mInitializeAlignment)
		{
			this.mInitializeAlignment = false;
			this.mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			LocalizeTarget_TextMeshPro_Label.InitAlignment_TMPro(this.mAlignmentWasRTL, base.mTarget.alignment, out this.mAlignment_LTR, out this.mAlignment_RTL);
		}
		else
		{
			LocalizeTarget_TextMeshPro_Label.InitAlignment_TMPro(this.mAlignmentWasRTL, base.mTarget.alignment, out var alignLTR, out var alignRTL);
			if ((this.mAlignmentWasRTL && this.mAlignment_RTL != alignRTL) || (!this.mAlignmentWasRTL && this.mAlignment_LTR != alignLTR))
			{
				this.mAlignment_LTR = alignLTR;
				this.mAlignment_RTL = alignRTL;
			}
			this.mAlignmentWasRTL = LocalizationManager.IsRight2Left;
		}
		if (mainTranslation == null || !(base.mTarget.text != mainTranslation))
		{
			return;
		}
		if (mainTranslation != null && cmp.CorrectAlignmentForRTL)
		{
			base.mTarget.alignment = (LocalizationManager.IsRight2Left ? this.mAlignment_RTL : this.mAlignment_LTR);
			base.mTarget.isRightToLeftText = LocalizationManager.IsRight2Left;
			if (LocalizationManager.IsRight2Left)
			{
				mainTranslation = I2Utils.ReverseText(mainTranslation);
			}
		}
		base.mTarget.text = mainTranslation;
	}
}
