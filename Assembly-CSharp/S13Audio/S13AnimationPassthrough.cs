using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S13Audio;

public class S13AnimationPassthrough : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private List<S13AnimationActions> animationCommands = new List<S13AnimationActions>();

	[Tooltip("Should events that are not on the most prominent layer be allowed to execute")]
	[SerializeField]
	private bool FilterNonProminentEvents;

	private Dictionary<string, S13AnimationActions> animationDictionary = new Dictionary<string, S13AnimationActions>();

	private void Start()
	{
		if (!this.animator || this.animator.gameObject != base.gameObject)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.animationCommands.Count < 1)
		{
			this.animationCommands.AddRange(base.GetComponentsInChildren<S13AnimationActions>(includeInactive: true));
		}
		foreach (S13AnimationActions animationCommand in this.animationCommands)
		{
			if (animationCommand != null)
			{
				animationCommand.Load();
			}
		}
		this.GenerateAnimationDictionary();
	}

	public void OnAnimationEventRaised(AnimationEvent animEvent)
	{
		string stringParameter;
		stringParameter = animEvent.stringParameter;
		if (this.FilterNonProminentEvents && !this.VerifyEventIsProminent(animEvent, out var mostProminentClip))
		{
			S13Debug.Log("Most prominent AnimationClip is \"" + mostProminentClip?.name + "\". AnimationCommand \"" + stringParameter + "\" on " + base.gameObject.name + " Failed", base.gameObject, S13Debug.LogType.AnimationEvent);
		}
		else
		{
			string stringParameter2;
			stringParameter2 = animEvent.stringParameter;
			if (this.animationDictionary.TryGetValue(stringParameter2, out var value))
			{
				value.TriggerAnimationAction(stringParameter2);
			}
		}
	}

	public void RegisterAnimationActions(params S13AnimationActions[] animActions)
	{
		for (int i = 0; i < animActions.Length; i = unchecked(i + 1))
		{
			if (animActions[i] == null || this.animationCommands.Contains(animActions[i]))
			{
				return;
			}
			animActions[i].Load();
			this.animationCommands.Add(animActions[i]);
		}
		this.GenerateAnimationDictionary();
	}

	public void DeregisterAnimationActions(params S13AnimationActions[] animActions)
	{
		for (int i = 0; i < animActions.Length; i = unchecked(i + 1))
		{
			if (!this.animationCommands.Contains(animActions[i]))
			{
				return;
			}
			this.animationCommands.Remove(animActions[i]);
		}
		this.GenerateAnimationDictionary();
	}

	private void GenerateAnimationDictionary()
	{
		this.animationDictionary.Clear();
		foreach (S13AnimationActions animationCommand in this.animationCommands)
		{
			if (animationCommand == null)
			{
				continue;
			}
			string[] actionKeys;
			actionKeys = animationCommand.ActionKeys;
			foreach (string text in actionKeys)
			{
				if (this.animationDictionary.ContainsKey(text))
				{
					S13Debug.LogWarning($"AnimationDictionary already contains Key \"{text}\" as part of AnimationActions \"{this.animationDictionary[text]}\". Duplicate Key names are not alloed", animationCommand, S13Debug.LogType.AnimationEvent);
				}
				else
				{
					this.animationDictionary.Add(text, animationCommand);
				}
			}
		}
	}

	private bool VerifyEventIsProminent(AnimationEvent animEvent, out AnimationClip mostProminentClip)
	{
		float num;
		num = float.MinValue;
		mostProminentClip = null;
		unchecked
		{
			for (int i = 0; i < this.animator.layerCount; i++)
			{
				AnimatorClipInfo[] currentAnimatorClipInfo;
				currentAnimatorClipInfo = this.animator.GetCurrentAnimatorClipInfo(i);
				if (currentAnimatorClipInfo.Length <= 1 || !Enumerable.Contains(currentAnimatorClipInfo, animEvent.animatorClipInfo))
				{
					continue;
				}
				AnimatorClipInfo[] array;
				array = currentAnimatorClipInfo;
				for (int j = 0; j < array.Length; j++)
				{
					AnimatorClipInfo animatorClipInfo;
					animatorClipInfo = array[j];
					if (animatorClipInfo.weight > num)
					{
						num = animatorClipInfo.weight;
						mostProminentClip = animatorClipInfo.clip;
					}
				}
				break;
			}
			if (mostProminentClip != null)
			{
				return mostProminentClip == animEvent.animatorClipInfo.clip;
			}
			return true;
		}
	}
}
