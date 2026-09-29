using System;
using DG.Tweening;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRDOTweenHook : MonoBehaviour
{
	private enum TweenEventType
	{
		OnTweenCreated = 0,
		OnStart = 1,
		OnPlay = 2,
		OnUpdate = 3,
		OnStepComplete = 4,
		OnComplete = 5,
		OnRewind = 6
	}

	[Serializable]
	private class TweenEventActionList : ReorderableArray<TweenEventAction>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder]
	private class TweenEventAction
	{
		public TweenEventType type;

		public S13LocalAction action;

		private MonoBehaviour _enableCheck;

		public void Init(MonoBehaviour enableCheck)
		{
			this._enableCheck = enableCheck;
		}

		public void Execute()
		{
			if ((bool)this._enableCheck && this._enableCheck.enabled)
			{
				this.action.Execute();
			}
		}
	}

	[SerializeField]
	[Tooltip("Target DOTweenAnimation that we will attach to")]
	private DOTweenAnimation target;

	[Reorderable]
	[SerializeField]
	private TweenEventActionList tweenEventActions;

	private void Start()
	{
		if (!this.target)
		{
			return;
		}
		foreach (TweenEventAction tweenEventAction in this.tweenEventActions)
		{
			if (tweenEventAction.action.IsExecutable)
			{
				tweenEventAction.Init(this);
				switch (tweenEventAction.type)
				{
				case TweenEventType.OnTweenCreated:
					this.target.onTweenCreated?.AddListener(tweenEventAction.Execute);
					this.target.hasOnTweenCreated = true;
					break;
				case TweenEventType.OnStart:
					this.target.onStart?.AddListener(tweenEventAction.Execute);
					this.target.hasOnStart = true;
					break;
				case TweenEventType.OnPlay:
					this.target.onPlay?.AddListener(tweenEventAction.Execute);
					this.target.hasOnPlay = true;
					break;
				case TweenEventType.OnUpdate:
					this.target.onUpdate?.AddListener(tweenEventAction.Execute);
					this.target.hasOnUpdate = true;
					break;
				case TweenEventType.OnStepComplete:
					this.target.onStepComplete?.AddListener(tweenEventAction.Execute);
					this.target.hasOnStepComplete = true;
					break;
				case TweenEventType.OnComplete:
					this.target.onRewind?.AddListener(tweenEventAction.Execute);
					this.target.hasOnRewind = true;
					break;
				case TweenEventType.OnRewind:
					this.target.onRewind?.AddListener(tweenEventAction.Execute);
					this.target.hasOnComplete = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}
