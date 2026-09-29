using System;
using TMPro;
using UnityEngine;

namespace I2.Loc;

public class LocalizeTarget_TextMeshPro_Label : LocalizeTarget<TextMeshPro>
{
	private TextAlignmentOptions mAlignment_RTL = TextAlignmentOptions.Right;

	private TextAlignmentOptions mAlignment_LTR = TextAlignmentOptions.Left;

	private bool mAlignmentWasRTL;

	private bool mInitializeAlignment = true;

	static LocalizeTarget_TextMeshPro_Label()
	{
		LocalizeTarget_TextMeshPro_Label.AutoRegister();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void AutoRegister()
	{
		LocalizationManager.RegisterTarget(new LocalizeTargetDesc_Type<TextMeshPro, LocalizeTarget_TextMeshPro_Label>
		{
			Name = "TextMeshPro Label",
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

	internal static TMP_FontAsset GetTMPFontFromMaterial(Localize cmp, string matName)
	{
		string text;
		text = " .\\/-[]()";
		unchecked
		{
			int num;
			num = matName.Length - 1;
			while (num > 0)
			{
				while (num > 0 && text.IndexOf(matName[num]) >= 0)
				{
					num--;
				}
				if (num <= 0)
				{
					break;
				}
				TMP_FontAsset tMP_FontAsset;
				tMP_FontAsset = cmp.GetObject<TMP_FontAsset>(matName.Substring(0, num + 1));
				if (tMP_FontAsset != null)
				{
					return tMP_FontAsset;
				}
				while (num > 0 && text.IndexOf(matName[num]) < 0)
				{
					num--;
				}
			}
			return null;
		}
	}

	internal static void InitAlignment_TMPro(bool isRTL, TextAlignmentOptions alignment, out TextAlignmentOptions alignLTR, out TextAlignmentOptions alignRTL)
	{
		alignLTR = (alignRTL = alignment);
		if (isRTL)
		{
			switch (alignment)
			{
			case TextAlignmentOptions.TopRight:
				alignLTR = TextAlignmentOptions.TopLeft;
				break;
			case TextAlignmentOptions.Right:
				alignLTR = TextAlignmentOptions.Left;
				break;
			case TextAlignmentOptions.BottomRight:
				alignLTR = TextAlignmentOptions.BottomLeft;
				break;
			case TextAlignmentOptions.BaselineRight:
				alignLTR = TextAlignmentOptions.BaselineLeft;
				break;
			case TextAlignmentOptions.MidlineRight:
				alignLTR = TextAlignmentOptions.MidlineLeft;
				break;
			case TextAlignmentOptions.CaplineRight:
				alignLTR = TextAlignmentOptions.CaplineLeft;
				break;
			case TextAlignmentOptions.TopLeft:
				alignLTR = TextAlignmentOptions.TopRight;
				break;
			case TextAlignmentOptions.Left:
				alignLTR = TextAlignmentOptions.Right;
				break;
			case TextAlignmentOptions.BottomLeft:
				alignLTR = TextAlignmentOptions.BottomRight;
				break;
			case TextAlignmentOptions.BaselineLeft:
				alignLTR = TextAlignmentOptions.BaselineRight;
				break;
			case TextAlignmentOptions.MidlineLeft:
				alignLTR = TextAlignmentOptions.MidlineRight;
				break;
			case TextAlignmentOptions.CaplineLeft:
				alignLTR = TextAlignmentOptions.CaplineRight;
				break;
			}
		}
		else
		{
			switch (alignment)
			{
			case TextAlignmentOptions.TopRight:
				alignRTL = TextAlignmentOptions.TopLeft;
				break;
			case TextAlignmentOptions.Right:
				alignRTL = TextAlignmentOptions.Left;
				break;
			case TextAlignmentOptions.BottomRight:
				alignRTL = TextAlignmentOptions.BottomLeft;
				break;
			case TextAlignmentOptions.BaselineRight:
				alignRTL = TextAlignmentOptions.BaselineLeft;
				break;
			case TextAlignmentOptions.MidlineRight:
				alignRTL = TextAlignmentOptions.MidlineLeft;
				break;
			case TextAlignmentOptions.CaplineRight:
				alignRTL = TextAlignmentOptions.CaplineLeft;
				break;
			case TextAlignmentOptions.TopLeft:
				alignRTL = TextAlignmentOptions.TopRight;
				break;
			case TextAlignmentOptions.Left:
				alignRTL = TextAlignmentOptions.Right;
				break;
			case TextAlignmentOptions.BottomLeft:
				alignRTL = TextAlignmentOptions.BottomRight;
				break;
			case TextAlignmentOptions.BaselineLeft:
				alignRTL = TextAlignmentOptions.BaselineRight;
				break;
			case TextAlignmentOptions.MidlineLeft:
				alignRTL = TextAlignmentOptions.MidlineRight;
				break;
			case TextAlignmentOptions.CaplineLeft:
				alignRTL = TextAlignmentOptions.CaplineRight;
				break;
			}
		}
	}

	internal static void SetFont(TMP_Text label, TMP_FontAsset newFont)
	{
		if (label.font != newFont)
		{
			label.font = newFont;
		}
		if (label.linkedTextComponent != null)
		{
			LocalizeTarget_TextMeshPro_Label.SetFont(label.linkedTextComponent, newFont);
		}
	}

	internal static void SetMaterial(TMP_Text label, Material newMat)
	{
		if (label.fontSharedMaterial != newMat)
		{
			label.fontSharedMaterial = newMat;
		}
		if (label.linkedTextComponent != null)
		{
			LocalizeTarget_TextMeshPro_Label.SetMaterial(label.linkedTextComponent, newMat);
		}
	}
}
