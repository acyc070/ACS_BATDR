using System;
using UnityEngine;

public static class AnimationEventUtility
{
	public static void AddAnimationEvent(ref Animator animator, string clipName, string functionName, int frame, string stringParameter)
	{
		AnimationEventUtility.SetupNewEvent(AnimationEventUtility.GetAnimationClip(ref animator, clipName), functionName, (float)frame / 30f, stringParameter);
	}

	public static void AddAnimationEvent(ref Animator animator, string clipName, string functionName, float time, string stringParameter)
	{
		AnimationEventUtility.SetupNewEvent(AnimationEventUtility.GetAnimationClip(ref animator, clipName), functionName, time, stringParameter);
	}

	private static AnimationClip GetAnimationClip(ref Animator animator, string clipName)
	{
		AnimationClip result;
		result = null;
		for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i = unchecked(i + 1))
		{
			if (animator.runtimeAnimatorController.animationClips[i].name == clipName)
			{
				result = animator.runtimeAnimatorController.animationClips[i];
				break;
			}
		}
		return result;
	}

	private static void SetupNewEvent(AnimationClip clip, string functionName, float time, string stringParameter)
	{
		if (clip != null)
		{
			time = (float)Math.Round(time, 3);
			if (!AnimationEventUtility.ContainsEvent(clip, functionName, time, stringParameter))
			{
				clip.AddEvent(AnimationEventUtility.AddEvent(functionName, time, stringParameter));
			}
		}
	}

	private static bool ContainsEvent(AnimationClip clip, string functionName, float time, string stringParameter)
	{
		bool result;
		result = false;
		AnimationEvent[] events;
		events = clip.events;
		foreach (AnimationEvent animationEvent in events)
		{
			if (animationEvent.functionName == functionName && animationEvent.time == time && animationEvent.stringParameter == stringParameter)
			{
				result = true;
			}
		}
		return result;
	}

	private static AnimationEvent AddEvent(string functionName, float time, string stringParameter)
	{
		return new AnimationEvent
		{
			functionName = functionName,
			stringParameter = stringParameter,
			time = time
		};
	}
}
