using System;
using DG.Tweening;
using UnityEngine;

public class Companion : Character
{
	protected CompanionDirector m_Director;

	private HeadTracker m_HeadTracker;

	private TextureSwapper m_TextureSwapper;

	private bool m_IsExiting;

	public float Timer;

	public CompanionDirector Director => this.m_Director;

	private CompanionNode m_CurrentCompanionNode => (CompanionNode)base.CurrentNode;

	private CompanionNode m_PreviousCompanionNode => (CompanionNode)base.PreviousNode;

	public override bool JumpInput => false;

	public override bool CrouchInput => false;

	public override bool RunInput => false;

	public override float MoveXInput => 0f;

	public override float MoveYInput => base.m_MoveSpeed;

	public override float RotateXInput => 0f;

	public override bool AttackInput => false;

	protected override bool UseAwake => false;

	public event EventHandler OnSpecialAnimationComplete;

	public event EventHandler OnGentPipeHit;

	public void SetDirector(CompanionDirector director)
	{
		this.m_Director = director;
	}

	protected override void InternalInitializeOnComplete()
	{
		base.GetCharacterContent();
		base.SetupCharacterContent();
		base.Agent.Agent.enabled = true;
		base.SetState(State.Character.Companion);
		this.GetHeadTracker();
		base.GetAudioController();
		base.SendOnInitializeOnComplete();
	}

	private void GetHeadTracker()
	{
		Transform transform;
		transform = base.transform.FindDeepChild("BN_Head");
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head001");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("BN_Head_01");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("Head_Ctrl");
		}
		if (transform == null)
		{
			transform = base.transform.FindDeepChild("CTRL_Mouth");
		}
		if (transform != null)
		{
			this.m_HeadTracker = transform.gameObject.AddComponent<HeadTracker>();
			Transform transform2;
			transform2 = new GameObject().transform;
			transform2.SetParent(base.transform);
			transform2.localPosition = ((base.Content.EyeSight != null) ? base.Content.EyeSight.localPosition : Vector3.zero);
			transform2.localEulerAngles = Vector3.zero;
			this.m_HeadTracker.SetForwardDirection(transform2);
			this.m_HeadTracker.SetOffset(new Vector3(0f, 270f, 260f));
			this.m_HeadTracker.SetAngle(55f);
			this.m_HeadTracker.SetSpeed(6f);
			this.m_HeadTracker.SetDistance(12f);
			this.m_TextureSwapper = base.gameObject.GetComponentInChildren<TextureSwapper>();
		}
	}

	protected override void InternalUpdate()
	{
		if (base.CurrentState == State.Character.Cutscene && !this.m_IsExiting && Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 20f)
		{
			CompanionNode closest;
			closest = this.m_Director.GetClosest(GameManager.Instance.Player.transform.position);
			if (this.m_CurrentCompanionNode != closest)
			{
				this.ForceExitNode();
			}
		}
	}

	public void ForceExitNode(bool isInitialize = false)
	{
		base.Content.Animator.ResetTrigger("SpecialAnimationExit");
		base.Content.Animator.ResetTrigger("SpecialAnimationEnter");
		if (!isInitialize)
		{
			this.m_IsExiting = true;
			if (this.m_CurrentCompanionNode.NodeType == CompanionNodeType.Location)
			{
				base.Agent.Agent.enabled = true;
				base.SetState(State.Character.Companion);
				this.m_IsExiting = false;
			}
			else
			{
				base.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
				base.OnAnimationComplete += this.HandleExitOnAnimationComplete;
				base.Content.Animator.SetTrigger("SpecialAnimationExit");
			}
		}
		else
		{
			base.Content.Animator.SetTrigger("Exit");
			base.Agent.Agent.enabled = true;
			base.SetState(State.Character.Companion);
			this.m_IsExiting = false;
		}
	}

	private void HandleExitOnAnimationComplete(object sender, EventArgs e)
	{
		base.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		this.OnSpecialAnimationComplete.Send(this);
		base.Agent.Agent.enabled = true;
		base.SetState(State.Character.Companion);
		this.m_IsExiting = false;
	}

	protected override void InternalSetNode(CharacterNode node)
	{
		CompanionNode companionNode;
		companionNode = (CompanionNode)base.CurrentNode;
		if (companionNode.NodeType == CompanionNodeType.Sit)
		{
			if (companionNode.SitLocation != null)
			{
				base.OnAnimationEvent -= this.HandleOnAnimationEvent;
				base.OnAnimationEvent += this.HandleOnAnimationEvent;
				base.OnAnimationEvent2 -= this.HandleOnAnimationEvent2;
				base.OnAnimationEvent2 += this.HandleOnAnimationEvent2;
			}
			this.m_Director.UpdateAnimationClips("Sit");
		}
		else if (companionNode.NodeType == CompanionNodeType.Point)
		{
			this.m_Director.UpdateAnimationClips("Point");
		}
	}

	private void HandleOnAnimationEvent(object sender, EventArgs e)
	{
		base.OnAnimationEvent -= this.HandleOnAnimationEvent;
		CompanionNode companionNode;
		companionNode = (CompanionNode)base.CurrentNode;
		if (companionNode.SitLocation != null)
		{
			base.transform.DOKill();
			base.transform.DOMove(companionNode.SitLocation.position, 0.25f).SetEase(Ease.Linear);
		}
	}

	private void HandleOnAnimationEvent2(object sender, EventArgs e)
	{
		base.OnAnimationEvent2 -= this.HandleOnAnimationEvent2;
		CompanionNode companionNode;
		companionNode = (CompanionNode)base.CurrentNode;
		base.transform.DOKill();
		base.transform.DOMove(companionNode.transform.position, 0.15f).SetEase(Ease.Linear);
	}

	public void UpdateAnimationClipOverrides(AnimationClip[] animationClips)
	{
		base.Content.UpdateClipOverrides(animationClips);
	}

	public void SetAnimationTrigger(string trigger)
	{
		base.Content.Animator.SetTrigger(trigger);
	}

	public void ResetAnimationTrigger(string trigger)
	{
		base.Content.Animator.ResetTrigger(trigger);
	}

	protected override void InternalHandleNodeOnReached(CharacterNode node)
	{
		this.NodeReached(node);
	}

	public void NodeReached(CharacterNode node, bool isInstant = false)
	{
		CompanionNode companionNode;
		companionNode = (CompanionNode)node;
		JDebug.Log("HandleNodeOnReached :: " + node.name + "  ::  " + companionNode.NodeType.ToString() + " :: isInstant: " + isInstant, JDebug.JDebugType.AI);
		base.SetState(State.Character.Cutscene);
		base.Agent.Agent.enabled = false;
		base.Content.Animator.ResetTrigger("SpecialAnimationExit");
		string text;
		text = string.Empty;
		if (companionNode.NodeType == CompanionNodeType.Sit)
		{
			text = (isInstant ? "SpecialAnimationEnterInstant" : "SpecialAnimationEnter");
		}
		else if (companionNode.NodeType == CompanionNodeType.Point)
		{
			text = (isInstant ? "SpecialAnimationEnterInstant" : "SpecialAnimationEnter");
		}
		else if (companionNode.NodeType == CompanionNodeType.Loot)
		{
			text = "Loot";
		}
		else if (companionNode.NodeType == CompanionNodeType.Interact)
		{
			text = (isInstant ? "InteractInstant" : "Interact");
		}
		else if (companionNode.NodeType == CompanionNodeType.Location)
		{
			base.ForceStop(smooth: false);
		}
		if (text != string.Empty)
		{
			base.Content.Animator.ResetTrigger(text);
			base.Content.Animator.SetTrigger(text);
		}
	}

	public override void OnHit(RaycastHit hit)
	{
		this.m_HeadTracker.SetActive(active: false);
		float x;
		x = ((global::UnityEngine.Random.value > 0.5f) ? 1440 : (-1440));
		this.Shock();
		this.m_HeadTracker.transform.DOKill();
		this.m_HeadTracker.transform.DOLocalRotate(new Vector3(x, 0f, 0f), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutElastic).SetUpdate(UpdateType.Late)
			.OnComplete(() =>
			{
				this.m_HeadTracker.SetActive(active: true);
				this.Blink();
			});
		this.OnGentPipeHit.Send(this);
	}

	public void BlinkChance()
	{
		if (this.m_TextureSwapper != null && global::UnityEngine.Random.value < -0.5f)
		{
			this.m_TextureSwapper.Blink();
		}
	}

	public void Blink()
	{
		if (this.m_TextureSwapper != null)
		{
			this.m_TextureSwapper.Blink();
		}
	}

	public void Shock()
	{
		if (this.m_TextureSwapper != null)
		{
			this.m_TextureSwapper.Shock();
		}
	}

	public void Sad()
	{
		if (this.m_TextureSwapper != null)
		{
			this.m_TextureSwapper.Sad();
		}
	}

	protected override void OnDisposed()
	{
		this.OnSpecialAnimationComplete = null;
		this.OnGentPipeHit = null;
		this.m_Director = null;
		this.m_HeadTracker = null;
		this.m_TextureSwapper = null;
		base.OnAnimationComplete -= this.HandleExitOnAnimationComplete;
		base.OnAnimationEvent -= this.HandleOnAnimationEvent;
		base.OnAnimationEvent2 -= this.HandleOnAnimationEvent2;
		base.OnDisposed();
	}
}
