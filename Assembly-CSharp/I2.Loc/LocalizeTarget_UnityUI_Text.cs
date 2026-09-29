using UnityEngine;
using UnityEngine.UI;

namespace I2.Loc;

public class LocalizeTarget_UnityUI_Text : LocalizeTarget<Text>
{
	private TextAnchor mAlignment_RTL = TextAnchor.UpperRight;

	private TextAnchor mAlignment_LTR;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_UnityUI_Text()
	{
		LocalizeTarget_UnityUI_Text.AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoRegister()
	{
		LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<Text, LocalizeTarget_UnityUI_Text>
		{
			Name = "Text",
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
		secondaryTerm = ((base.mTarget.font != null) ? base.mTarget.font.name : string.Empty);
	}

	public override void DoLocalize(Localize cmp, string mainTranslation, string secondaryTranslation)
	{
		Font secondaryTranslatedObj;
		secondaryTranslatedObj = cmp.GetSecondaryTranslatedObj<Font>(ref mainTranslation, ref secondaryTranslation);
		if (secondaryTranslatedObj != null && secondaryTranslatedObj != base.mTarget.font)
		{
			base.mTarget.font = secondaryTranslatedObj;
		}
		if (this.mInitializeAlignment)
		{
			this.mInitializeAlignment = false;
			this.mAlignmentWasRTL = LocalizationManager.IsRight2Left;
			this.InitAlignment(this.mAlignmentWasRTL, base.mTarget.alignment, out this.mAlignment_LTR, out this.mAlignment_RTL);
		}
		else
		{
			this.InitAlignment(this.mAlignmentWasRTL, base.mTarget.alignment, out var alignLTR, out var alignRTL);
			if ((this.mAlignmentWasRTL && this.mAlignment_RTL != alignRTL) || (!this.mAlignmentWasRTL && this.mAlignment_LTR != alignLTR))
			{
				this.mAlignment_LTR = alignLTR;
				this.mAlignment_RTL = alignRTL;
			}
			this.mAlignmentWasRTL = LocalizationManager.IsRight2Left;
		}
		if (mainTranslation != null && base.mTarget.text != mainTranslation)
		{
			if (cmp.CorrectAlignmentForRTL)
			{
				base.mTarget.alignment = (LocalizationManager.IsRight2Left ? this.mAlignment_RTL : this.mAlignment_LTR);
			}
			base.mTarget.text = mainTranslation;
			base.mTarget.SetVerticesDirty();
		}
	}

	private void InitAlignment(bool isRTL, TextAnchor alignment, out TextAnchor alignLTR, out TextAnchor alignRTL)
	{
		alignLTR = (alignRTL = alignment);
		if (isRTL)
		{
			switch (alignment)
			{
			case TextAnchor.UpperRight:
				alignLTR = TextAnchor.UpperLeft;
				break;
			case TextAnchor.MiddleRight:
				alignLTR = TextAnchor.MiddleLeft;
				break;
			case TextAnchor.LowerRight:
				alignLTR = TextAnchor.LowerLeft;
				break;
			case TextAnchor.UpperLeft:
				alignLTR = TextAnchor.UpperRight;
				break;
			case TextAnchor.MiddleLeft:
				alignLTR = TextAnchor.MiddleRight;
				break;
			case TextAnchor.LowerLeft:
				alignLTR = TextAnchor.LowerRight;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				break;
			}
		}
		else
		{
			switch (alignment)
			{
			case TextAnchor.UpperRight:
				alignRTL = TextAnchor.UpperLeft;
				break;
			case TextAnchor.MiddleRight:
				alignRTL = TextAnchor.MiddleLeft;
				break;
			case TextAnchor.LowerRight:
				alignRTL = TextAnchor.LowerLeft;
				break;
			case TextAnchor.UpperLeft:
				alignRTL = TextAnchor.UpperRight;
				break;
			case TextAnchor.MiddleLeft:
				alignRTL = TextAnchor.MiddleRight;
				break;
			case TextAnchor.LowerLeft:
				alignRTL = TextAnchor.LowerRight;
				break;
			case TextAnchor.UpperCenter:
			case TextAnchor.MiddleCenter:
			case TextAnchor.LowerCenter:
				break;
			}
		}
	}
}
