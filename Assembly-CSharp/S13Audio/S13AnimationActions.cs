using System;
using System.Collections.Generic;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

public class S13AnimationActions : MonoBehaviour
{
	[Serializable]
	private class S13ActionEventList : ReorderableArray<AnimationActionEvent>
	{
		public S13ActionEventList(int length)
			: base(length)
		{
		}
	}

	[Serializable]
	public class AnimationActionEvent
	{
		public string name = "";

		public S13LocalAction action = new S13LocalAction();

		public AnimationClip animationClip;

		public int[] frames = new int[0];

		public IEnumerable<AnimationEvent> ToAnimationEvents()
		{
			if ((bool)this.animationClip)
			{
				int[] array;
				array = this.frames;
				foreach (int num in array)
				{
					AnimationEvent animationEvent;
					animationEvent = new AnimationEvent();
					animationEvent.time = (float)num / this.animationClip.frameRate;
					animationEvent.stringParameter = this.name;
					animationEvent.functionName = "OnAnimationEventRaised";
					yield return animationEvent;
				}
			}
		}
	}

	private const string FunctionName = "OnAnimationEventRaised";

	[Reorderable]
	[SerializeField]
	private S13ActionEventList commandEventsList = new S13ActionEventList(0);

	private Dictionary<string, AnimationActionEvent> actionDictionary = new Dictionary<string, AnimationActionEvent>();

	public string[] ActionKeys => this.actionDictionary.Keys.ToArray();

	public void Load()
	{
		this.actionDictionary.Clear();
		foreach (AnimationActionEvent commandEvents in this.commandEventsList)
		{
			if (!commandEvents.animationClip || string.IsNullOrEmpty(commandEvents.name) || commandEvents.frames.Length == 0)
			{
				S13Debug.LogWarning("AnimationActions " + base.gameObject.name + " contains invalid actions. " + ((!string.IsNullOrEmpty(commandEvents.name)) ? commandEvents.name : ""), this, S13Debug.LogType.AnimationEvent);
				continue;
			}
			foreach (AnimationEvent e in commandEvents.ToAnimationEvents())
			{
				if (!commandEvents.animationClip.events.Any((AnimationEvent x) => S13AnimationActions.CompareEvents(x, e)))
				{
					commandEvents.animationClip.AddEvent(e);
				}
			}
			if (this.actionDictionary.ContainsKey(commandEvents.name))
			{
				S13Debug.LogError("Duplicate action name in " + base.gameObject.name + ", this is not allowed", this);
			}
			else
			{
				this.actionDictionary.Add(commandEvents.name, commandEvents);
			}
		}
	}

	public void TriggerAnimationAction(string eventName)
	{
		if (this.actionDictionary.TryGetValue(eventName, out var value))
		{
			value.action.Execute();
		}
	}

	private static bool CompareEvents(AnimationEvent a, AnimationEvent b)
	{
		if (a.stringParameter == b.stringParameter && a.functionName == b.functionName)
		{
			return Mathf.Approximately(a.time, b.time);
		}
		return false;
	}
}
