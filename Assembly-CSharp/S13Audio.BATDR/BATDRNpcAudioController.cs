using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRNpcAudioController : MonoBehaviour
{
	public enum NPCReaction
	{
		NONE = 0,
		Alert = 3,
		Alive = 1,
		Attack = 4,
		Dead = 2,
		Evade = 5,
		Flee = 6,
		Hit = 7
	}

	[Serializable]
	private class ReactionList : ReorderableArray<ReactionEntry>
	{
	}

	[Serializable]
	private class ReactionEntry
	{
		public NPCReaction reaction;

		public S13LocalAction action;
	}

	[Serializable]
	private class DialogueList : ReorderableArray<DialogueEntry>
	{
	}

	[Serializable]
	[S13HorizontalPropertyHolder(forceExpand = true, label = "")]
	private class DialogueEntry : IS13HorizontalPropertyHolder
	{
		public string dialogueKey = string.Empty;

		public AudioClip dialogueClip;
	}

	[Header("Reactions")]
	[SerializeField]
	[Reorderable(elementNameProperty = "reaction")]
	[Tooltip("A list of NPC reactions and the corresponding S13LocalAction that should be executed")]
	private ReactionList npcReactionList;

	[Header("Dialogue")]
	[SerializeField]
	[Tooltip("AudioHandler that contains the S13ObjectDynamic used for dialogue")]
	private S13AudioHandler dialogueHandlerReference;

	[SerializeField]
	[Reorderable(elementNameProperty = "dialogueKey")]
	private DialogueList dialogueList = new DialogueList();

	public void React(NPCReaction reaction)
	{
		if (reaction != NPCReaction.NONE)
		{
			S13Debug.Log($"Received NPC reaction {reaction} on npc {base.name}", this);
			ReactionEntry reactionEntry;
			reactionEntry = this.npcReactionList.FirstOrDefault((ReactionEntry x) => x.reaction == reaction);
			if (reactionEntry != null && reactionEntry.action.IsExecutable)
			{
				reactionEntry.action.Execute();
			}
		}
	}

	public void DialogueClip(string key)
	{
		if (!this.dialogueHandlerReference)
		{
			S13Debug.LogWarning("Attempted to play Dialogue Clip on " + base.name + ", but dialogueHandlerRef is null", this);
			return;
		}
		S13HandlerDynamic s13HandlerDynamic;
		s13HandlerDynamic = this.dialogueHandlerReference.GetHandler() as S13HandlerDynamic;
		if (s13HandlerDynamic == null)
		{
			S13Debug.LogWarning("Attempted to play Dialogue Clip on " + base.name + ", but the S13AudioHandler does not reference a S13ObjectDynamic", this);
			return;
		}
		DialogueEntry dialogueEntry;
		dialogueEntry = this.dialogueList.FirstOrDefault((DialogueEntry x) => string.Equals(x.dialogueKey, key));
		if (dialogueEntry != null)
		{
			s13HandlerDynamic.SetNextClip(dialogueEntry.dialogueClip);
			s13HandlerDynamic.Play(base.transform);
		}
	}
}
