using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13CurveReference
{
	[SerializeField]
	private S13CurveTemplate curveTemplate;

	private static AnimationCurve _defaultCurve;

	public AnimationCurve Curve
	{
		get
		{
			if (!(this.curveTemplate != null))
			{
				return S13CurveReference._defaultCurve;
			}
			return this.curveTemplate.curve;
		}
	}

	static S13CurveReference()
	{
		S13CurveReference._defaultCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
	}
}
