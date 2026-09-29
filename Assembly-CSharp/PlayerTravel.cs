using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using S13Audio.BATDR;
using UnityEngine;

public class PlayerTravel : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Travel Identifier")]
	[SerializeField]
	private TravelID m_TravelID;

	[Header("Travel Section")]
	[SerializeField]
	private SectionID m_SectionID;

	[SerializeReference]
	private TravelID m_TravelToID;

	[SerializeField]
	private List<SectionID> m_ConnectedSections = new List<SectionID>();

	[Header("Interactable")]
	[SerializeField]
	private Interactable m_Interactable;

	[Header("Respawner")]
	[SerializeField]
	private PlayerRespawn m_PlayerRespawn;

	[Header("Emission")]
	[SerializeField]
	private MeshRenderer m_MeshRenderer;

	public TravelID TravelID => this.m_TravelID;

	public SectionID SectionID => this.m_SectionID;

	public List<SectionID> ConnectedSections => this.m_ConnectedSections;

	public PlayerRespawn PlayerRespawn => this.m_PlayerRespawn;

	public bool HasRequirements { get; private set; }

	public bool IsActive { get; private set; }

	public override void Start()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.SetActive(active: false);
		}
	}

	private void Update()
	{
		if ((!this.HasRequirements || !this.IsActive) && !GameManager.Instance.IsPaused && !base.IsDisposed)
		{
			if (this.HasRequirements && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.FastTravel) && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active)
			{
				this.IsActive = true;
				this.m_Interactable.SetActive(this.IsActive);
				this.m_MeshRenderer.material.SetFloat("_Power", 1f);
			}
			else if (this.IsActive)
			{
				this.IsActive = false;
				this.m_Interactable.SetActive(active: false);
				this.m_MeshRenderer.material.SetFloat("_Power", 0f);
			}
		}
	}

	private void Activate()
	{
		this.IsActive = GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.ContainsKey(State.PlayerAbility.FastTravel) && GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active;
		(this.m_Interactable as InteractableInputDisplay).SetInteractionType(InteractionType.INTERACTION_TRAVEL);
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
		this.m_Interactable.SetActive(this.IsActive);
		if (this.IsActive)
		{
			this.m_MeshRenderer.material.SetFloat("_Power", 1f);
		}
		else
		{
			this.m_MeshRenderer.material.SetFloat("_Power", 0f);
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		GameManager.Instance.HideHealthBar();
		GameManager.Instance.HideCrosshair();
		if (GameManager.Instance.GameData.CurrentSave.PlayerData.AbilityDirectory.AbilityStatus == AbilityStatus.Active && GameManager.Instance.Player.HasTeleport())
		{
			GameManager.Instance.HideTeleport();
		}
		GameManager.Instance.Player.SetCollision(active: false);
		GameManager.Instance.Player.SetCombatStatus(CombatStatus.Hide);
		GameManager.Instance.Player.ClearEnemies();
		this.m_Interactable.ResetAction();
		GameManager.Instance.Player.OnAnimationComplete -= this.HandlePlayerOnAnimationComplete;
		GameManager.Instance.Player.OnAnimationComplete += this.HandlePlayerOnAnimationComplete;
	}

	private void HandlePlayerOnAnimationComplete(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnAnimationComplete -= this.HandlePlayerOnAnimationComplete;
		GameManager.Instance.ShowScreenBlocker(0f, 0f, () =>
		{
			GameManager.Instance.Player.transform.position = new Vector3(0f, 2400f, 0f);
			base.StartCoroutine(this.LoadSections());
		});
	}

	private IEnumerator LoadSections()
	{
		yield return new WaitForEndOfFrame();
		GameManager.Instance.ShowAsyncLoader();
		if (!GameManager.Instance.SectionManager.Contains(this.m_SectionID))
		{
			Task sectionTask;
			sectionTask = GameManager.Instance.SectionManager.LoadSectionAsync(this.m_SectionID);
			while (!sectionTask.IsCompleted)
			{
				yield return null;
			}
		}
		PlayerTravel playerTravel;
		playerTravel = this.GetPlayerTravel(GameManager.Instance.SectionManager.GetSection(this.m_SectionID));
		foreach (SectionID connectedSection in playerTravel.ConnectedSections)
		{
			Task sectionTask;
			sectionTask = GameManager.Instance.SectionManager.LoadSectionAsync(connectedSection, initialize: false);
			while (!sectionTask.IsCompleted)
			{
				yield return null;
			}
		}
		GameManager.Instance.ClearAsyncLoader();
		playerTravel.PlayerRespawn.Respawn();
		GameManager.Instance.Player.S13SetPlayerReaction(BATDRPlayerAudioController.PlayerReaction.FastTravel);
		yield return new WaitForEndOfFrame();
		Section[] allSections;
		allSections = GameManager.Instance.SectionManager.GetAllSections();
		unchecked
		{
			for (int num = allSections.Length - 1; num >= 0; num--)
			{
				Section section;
				section = allSections[num];
				if (section.SectionID != this.m_SectionID && !playerTravel.ConnectedSections.Contains(section.SectionID))
				{
					GameManager.Instance.SectionManager.Remove(section.SectionID);
				}
			}
			Section componentInParent;
			componentInParent = base.GetComponentInParent<Section>(includeInactive: true);
			if (this.m_SectionID != componentInParent.SectionID && !playerTravel.ConnectedSections.Contains(componentInParent.SectionID))
			{
				GameManager.Instance.SectionManager.Remove(componentInParent.SectionID);
			}
		}
	}

	private PlayerTravel GetPlayerTravel(Section section)
	{
		PlayerTravel playerTravel;
		playerTravel = null;
		PlayerTravel[] componentsInChildren;
		componentsInChildren = section.GetComponentsInChildren<PlayerTravel>();
		for (int i = 0; i < componentsInChildren.Length; i = unchecked(i + 1))
		{
			playerTravel = componentsInChildren[i];
			if (playerTravel.TravelID == this.m_TravelToID)
			{
				break;
			}
			playerTravel = null;
		}
		return playerTravel;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		JDebug.Log("PlayerTravel :: HandleOnObjectiveComplete", this, JDebug.JDebugType.Objectives);
		this.CheckStatus();
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.HasRequirements = true;
			this.Activate();
		}
		return flag;
	}

	protected override void OnDisposed()
	{
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnAnimationComplete -= this.HandlePlayerOnAnimationComplete;
		}
		this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
		base.OnDisposed();
	}
}
