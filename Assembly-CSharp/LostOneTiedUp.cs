using System;
using S13Audio;
using UnityEngine;

public class LostOneTiedUp : JMonoBehaviour
{
	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Character Content")]
	[SerializeField]
	private CharacterContent m_CharacterContent;

	[Header("Animations")]
	[SerializeField]
	private AnimationClip m_TalkAnimationClip;

	[SerializeField]
	private AnimationClip m_ExecutionAnimationClip;

	[Header("Audio and Subttiles")]
	[SerializeField]
	private string m_SubtitleKey;

	[SerializeField]
	private S13AudioHandler m_DialogueAudioHandler;

	private S13Handler m_DialogueHandler;

	private S13ObjectSimple m_DialogueAudio;

	private bool m_IsInteracted;

	public event EventHandler OnInteract;

	public event EventHandler OnComplete;

	public override void Start()
	{
		this.m_DialogueAudio = this.m_DialogueAudioHandler.GetObject() as S13ObjectSimple;
		this.m_DialogueHandler = this.m_DialogueAudioHandler.GetHandler();
		this.RemoveListeners();
		this.AddListeners();
	}

	private void Update()
	{
		if (this.m_IsInteracted && !base.IsDisposed && !GameManager.Instance.IsPaused && !this.m_DialogueHandler.isPlaying)
		{
			this.Enable(onComplete: true);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.SetAnimation("Interact", this.m_TalkAnimationClip);
		this.m_DialogueAudioHandler.Play();
		GameManager.Instance.ShowSubtitles(TextUtility.GetKey(this.m_SubtitleKey), this.m_DialogueAudio.clip.length, isTrimmed: true);
		this.Disable(onInteract: true);
	}

	public void Execute()
	{
		this.SetAnimation("Death", this.m_ExecutionAnimationClip);
		this.Disable();
	}

	public void Enable(bool onComplete = false)
	{
		this.m_IsInteracted = false;
		this.AddListeners();
		this.m_Interactable.SetActive(active: true);
		this.m_Interactable.ResetAction();
		if (onComplete)
		{
			this.OnComplete.Send(this);
		}
	}

	public void Disable(bool onInteract = false)
	{
		this.RemoveListeners();
		this.m_Interactable.SetActive(active: false);
		this.m_IsInteracted = onInteract;
		if (onInteract)
		{
			this.OnInteract.Send(this);
		}
	}

	private void SetAnimation(string trigger, AnimationClip animationClip)
	{
		animationClip.name = trigger;
		this.m_CharacterContent.UpdateClipOverrides(animationClip);
		this.m_CharacterContent.SetAnimationTrigger(trigger);
	}

	private void AddListeners()
	{
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
	}

	private void RemoveListeners()
	{
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.OnInteract = null;
		this.OnComplete = null;
		this.m_DialogueHandler = null;
		this.m_DialogueAudio = null;
		base.OnDisposed();
	}
}
