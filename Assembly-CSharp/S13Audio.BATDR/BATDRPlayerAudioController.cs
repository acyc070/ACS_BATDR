using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRPlayerAudioController : MonoBehaviour
{
	public enum FloorMaterials
	{
		NONE = -1,
		Wood = 0,
		Tile = 1,
		Stone = 2,
		Pipe = 3,
		Metal = 4,
		Carpet = 6,
		Vent = 9,
		Body = 10,
		InkPuddle = 7,
		InkShallow = 5,
		InkDeep = 8
	}

	[Serializable]
	private class MatEventPairList : ReorderableArray<MatEventPair>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true)]
	private class MatEventPair : IS13HorizontalPropertyHolder
	{
		public FloorMaterials mat;

		public S13ScriptableEvent evt;
	}

	public enum GentPipeImpactType
	{
		Generic = 0,
		Plaster = 1,
		Metal = 2,
		Tile = 3,
		Wood = 4,
		InkSolid = 5,
		InkLiquid = 6,
		Cutout = 7
	}

	[Serializable]
	private class ImpactActionList : ReorderableArray<ImpactActionPair>
	{
	}

	[Serializable]
	private class ImpactActionPair
	{
		public GentPipeImpactType impactType;

		public S13LocalAction action;
	}

	public enum PlayerReaction
	{
		NONE = 0,
		Respawn = 1,
		Dead = 2,
		Hit = 3,
		Land = 4,
		Load = 5,
		FastTravel = 6
	}

	[Serializable]
	private class ReactionList : ReorderableArray<ReactionEntry>
	{
	}

	[Serializable]
	private class ReactionEntry
	{
		public PlayerReaction reaction;

		public S13LocalAction action;
	}

	[Header("Floor Materials")]
	[Reorderable]
	[SerializeField]
	private MatEventPairList matEvents;

	[SerializeField]
	[ReadOnly]
	private FloorMaterials currentFloorMaterial = FloorMaterials.NONE;

	[Header("Wetness")]
	[SerializeField]
	[S13LockableField]
	private string wetAccessorID = string.Empty;

	[SerializeField]
	private float secondsUntilDry = 1f;

	[ReadOnly]
	[SerializeField]
	private float currentFootWetness;

	[Header("Audrey Reactions")]
	[Reorderable(elementNameProperty = "reaction")]
	[SerializeField]
	private ReactionList reactionList;

	[Header("Gent Pipe Impacts")]
	[SerializeField]
	[Reorderable(elementNameProperty = "impactType")]
	private ImpactActionList impactActionList;

	[Header("Cart Move")]
	[SerializeField]
	private S13LocalAction onEnterCartMove;

	[SerializeField]
	private S13LocalAction onExitCartMove;

	[Header("Climb")]
	[SerializeField]
	private S13LocalAction onEnterClimb;

	[SerializeField]
	private float onEnterClimbDelay;

	[SerializeField]
	private S13LocalAction onExitClimb;

	[SerializeField]
	private float onExitClimbDelay;

	[Header("Hide")]
	[SerializeField]
	private S13LocalAction onEnterHide;

	[SerializeField]
	private S13LocalAction onExitHide;

	[Header("Crouch")]
	[SerializeField]
	private S13LocalAction onEnterCrouch;

	[SerializeField]
	private S13LocalAction onExitCrouch;

	[Header("Peek")]
	[SerializeField]
	private S13LocalAction onEnterPeek;

	[SerializeField]
	private S13LocalAction onExitPeek;

	private FloorMaterials _lastFloorMaterial;

	public void React(PlayerReaction reaction)
	{
		if (reaction != PlayerReaction.NONE)
		{
			ReactionEntry reactionEntry;
			reactionEntry = this.reactionList.FirstOrDefault((ReactionEntry x) => x.reaction == reaction);
			if (reactionEntry != null && reactionEntry.action.IsExecutable)
			{
				S13Debug.Log($"Player received reaction {reaction}", this);
				reactionEntry.action.Execute();
			}
		}
	}

	public void SetFloorMaterial(FloorMaterials newFloorMaterial)
	{
		if (newFloorMaterial == FloorMaterials.InkPuddle)
		{
			this.StepInInkPuddle();
			return;
		}
		if (newFloorMaterial == FloorMaterials.Body)
		{
			this.StepInInkPuddle();
		}
		if (this.currentFloorMaterial != newFloorMaterial)
		{
			this._lastFloorMaterial = this.currentFloorMaterial;
			this.currentFloorMaterial = newFloorMaterial;
			if (this.IsInkMaterial(this._lastFloorMaterial) && !this.IsInkMaterial(this.currentFloorMaterial))
			{
				this.StepInInkPuddle();
			}
			this.matEvents.FirstOrDefault((MatEventPair x) => x.mat == newFloorMaterial)?.evt.Raise();
		}
	}

	public void StepInInkPuddle()
	{
		this.currentFootWetness = 1f;
	}

	public void GentPipeImpact(GentPipeImpactType impactType)
	{
		ImpactActionPair impactActionPair;
		impactActionPair = this.impactActionList.FirstOrDefault((ImpactActionPair x) => x.impactType == impactType);
		if (impactActionPair != null && impactActionPair.action.IsExecutable)
		{
			impactActionPair.action.Execute();
		}
	}

	public void SetHiding(bool isHiding)
	{
		if (isHiding)
		{
			if (this.onEnterHide.IsExecutable)
			{
				this.onEnterHide.Execute();
			}
		}
		else if (this.onExitHide.IsExecutable)
		{
			this.onExitHide.Execute();
		}
	}

	public void SetCrouching(bool isCrouching)
	{
		if (isCrouching)
		{
			if (this.onEnterCrouch.IsExecutable)
			{
				this.onEnterCrouch.Execute();
			}
		}
		else if (this.onExitCrouch.IsExecutable)
		{
			this.onExitCrouch.Execute();
		}
	}

	public void SetPeeking(bool isPeeking)
	{
		if (isPeeking)
		{
			if (this.onEnterPeek.IsExecutable)
			{
				this.onEnterPeek.Execute();
			}
		}
		else if (this.onExitPeek.IsExecutable)
		{
			this.onExitPeek.Execute();
		}
	}

	public void SetClimbing(bool isClimbing)
	{
		if (isClimbing)
		{
			if (this.onEnterClimb.IsExecutable)
			{
				if (this.onEnterClimbDelay > 0f)
				{
					base.StartCoroutine(S13CoroutineUtil.WaitForDuration(this.onEnterClimbDelay, ignoreTimeScale: false, this.onEnterClimb.Execute));
				}
				else
				{
					this.onEnterClimb.Execute();
				}
			}
		}
		else if (this.onExitClimb.IsExecutable)
		{
			if (this.onExitClimbDelay > 0f)
			{
				base.StartCoroutine(S13CoroutineUtil.WaitForDuration(this.onExitClimbDelay, ignoreTimeScale: false, this.onExitClimb.Execute));
			}
			else
			{
				this.onExitClimb.Execute();
			}
		}
	}

	public void SetCartMove(bool isMoving)
	{
		if (isMoving)
		{
			if (this.onEnterCartMove.IsExecutable)
			{
				this.onEnterCartMove.Execute();
			}
		}
		else if (this.onExitCartMove.IsExecutable)
		{
			this.onExitCartMove.Execute();
		}
	}

	private void FixedUpdate()
	{
		if (this.currentFootWetness > 0f)
		{
			this.currentFootWetness = Mathf.Max(0f, this.currentFootWetness - Time.fixedDeltaTime * (1f / this.secondsUntilDry));
			if (!string.IsNullOrEmpty(this.wetAccessorID))
			{
				S13Manager.SetAccessor(this.wetAccessorID, this.currentFootWetness);
			}
		}
	}

	private bool IsInkMaterial(FloorMaterials material)
	{
		if (material != FloorMaterials.InkShallow && material != FloorMaterials.InkDeep)
		{
			return material == FloorMaterials.Body;
		}
		return true;
	}
}
