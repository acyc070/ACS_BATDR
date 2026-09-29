using System;
using DG.Tweening;
using UnityEngine;

public class EnemyFleeController : JMonoBehaviour
{
	[SerializeField]
	private bool m_IsActive;

	[SerializeField]
	private Transform m_NodeParent;

	[SerializeField]
	private Transform m_InkBulbParent;

	private CharacterNode[] m_Nodes;

	private InkBulb[] m_InkBulbs;

	public override void Awake()
	{
		this.m_Nodes = this.m_NodeParent.GetComponentsInChildren<CharacterNode>(includeInactive: true);
		this.m_InkBulbs = this.m_InkBulbParent.GetComponentsInChildren<InkBulb>(includeInactive: true);
		if (this.m_InkBulbs != null)
		{
			for (int i = 0; i < this.m_InkBulbs.Length; i = unchecked(i + 1))
			{
				InkBulb obj;
				obj = this.m_InkBulbs[i];
				obj.OnFlee -= this.HandleInkBulbOnFlee;
				obj.OnFlee += this.HandleInkBulbOnFlee;
			}
		}
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public void Flee(bool deactivate = false)
	{
		if (this.m_Nodes == null || this.m_InkBulbs == null)
		{
			return;
		}
		CharacterNode characterNode;
		characterNode = this.m_Nodes[global::UnityEngine.Random.Range(0, this.m_Nodes.Length)];
		CharacterNode lastNode;
		lastNode = characterNode;
		unchecked
		{
			for (int i = 0; i < this.m_InkBulbs.Length; i++)
			{
				InkBulb inkBulb;
				inkBulb = this.m_InkBulbs[i];
				for (int j = 0; j < inkBulb.InkWidows.Count; j++)
				{
					this.Flee(inkBulb, (Enemy)inkBulb.InkWidows[j], characterNode, ref lastNode);
				}
				if (inkBulb.gameObject.activeInHierarchy & deactivate)
				{
					inkBulb.Deactivate();
				}
			}
		}
	}

	public void Flee(InkBulb inkBulb)
	{
		CharacterNode characterNode;
		characterNode = this.m_Nodes[global::UnityEngine.Random.Range(0, this.m_Nodes.Length)];
		CharacterNode lastNode;
		lastNode = characterNode;
		for (int i = 0; i < inkBulb.InkWidows.Count; i = unchecked(i + 1))
		{
			this.Flee(inkBulb, (Enemy)inkBulb.InkWidows[i], characterNode, ref lastNode);
		}
	}

	private void Flee(InkBulb inkBulb, Enemy inkWidow, CharacterNode node, ref CharacterNode lastNode)
	{
		if (!inkWidow.gameObject.activeInHierarchy)
		{
			return;
		}
		if (inkWidow.CurrentState != State.Character.Death)
		{
			if (inkWidow.IsInitialized)
			{
				inkWidow.CancelPath();
				inkWidow.SetTarget(null);
				if (this.m_Nodes.Length > 1)
				{
					while (node == lastNode)
					{
						node = this.m_Nodes[global::UnityEngine.Random.Range(0, this.m_Nodes.Length)];
					}
				}
				lastNode = node;
				inkWidow.SetDisableVision(active: true);
				inkWidow.SetNode(node);
				inkWidow.OnNodeReached -= this.HandleEnemyOnNodeReached;
				inkWidow.OnNodeReached += this.HandleEnemyOnNodeReached;
				inkWidow.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
				inkWidow.OnCharacterDeath += this.HandleEnemyOnCharacterDeath;
				inkWidow.SetState(State.Character.Flee);
			}
			else
			{
				this.KillInkWidow(inkBulb, inkWidow);
			}
		}
		else
		{
			this.KillInkWidow(inkBulb, inkWidow);
		}
	}

	private void HandleEnemyOnNodeReached(object sender, EventArgs e)
	{
		Character character;
		character = (Character)sender;
		character.OnNodeReached -= this.HandleEnemyOnNodeReached;
		character.SetState(State.Character.Cutscene);
		character.Agent.Agent.isStopped = true;
		character.Agent.Agent.enabled = false;
		character.Controller.enabled = false;
		character.Content.Animator.SetMovementState(1f, smooth: false);
		character.Content.Animator.SetMovementSpeed(2f, smooth: false);
		character.transform.DOKill();
		character.transform.DOLocalMove(character.CurrentNode.transform.position + character.CurrentNode.transform.forward * 4f, 1f).SetEase(Ease.Linear).OnComplete(() =>
		{
			this.KillInkWidow(character);
		});
	}

	private void HandleEnemyOnCharacterDeath(object sender, EventArgs e)
	{
		Enemy obj;
		obj = sender as Enemy;
		obj.OnCharacterDeath -= this.HandleEnemyOnCharacterDeath;
		obj.OnNodeReached -= this.HandleEnemyOnNodeReached;
	}

	private void HandleInkBulbOnFlee(object sender, EventArgs e)
	{
		if (this.m_IsActive)
		{
			this.Flee(sender as InkBulb);
		}
	}

	private void KillInkWidow(InkBulb inkBulb, Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			inkBulb?.KillInkWidow(inkWidow);
		}
	}

	private void KillInkWidow(Character inkWidow)
	{
		if (!(inkWidow == null))
		{
			for (int i = 0; i < this.m_InkBulbs.Length; i = unchecked(i + 1))
			{
				this.m_InkBulbs[i].KillInkWidow(inkWidow);
			}
		}
	}

	private void RemoveListeners()
	{
		if (this.m_InkBulbs != null)
		{
			for (int i = 0; i < this.m_InkBulbs.Length; i = unchecked(i + 1))
			{
				this.m_InkBulbs[i].OnFlee -= this.HandleInkBulbOnFlee;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		this.m_Nodes = null;
		this.m_InkBulbs = null;
		base.OnDisposed();
	}
}
