using System;
using S13Audio.BATDR;
using UnityEngine;

public class MeshOutline : JComponent
{
	[Serializable]
	public class MeshOutlineGroup
	{
		public MeshRenderer MeshRenderer;

		public SkinnedMeshRenderer SkinnedMeshRenderer;

		public Material DefaultMaterial;

		public Material OutlineMaterial;

		public Material InteractMaterial;

		public int MaterialIndex = -1;
	}

	[SerializeField]
	private bool m_IsActive = true;

	[SerializeField]
	private Interactable m_Interactable;

	[Header("Mesh Group")]
	[SerializeField]
	private MeshOutlineGroup[] m_MeshOutlineGroup;

	private bool m_IsInteractableDisabled;

	public MeshOutlineGroup[] MeshOutlineGroups => this.m_MeshOutlineGroup;

	public override void Start()
	{
		if (this.m_IsActive)
		{
			this.Enable();
		}
	}

	private void Update()
	{
		if (!base.IsDisposed && !GameManager.Instance.IsPaused && !this.m_Interactable.IsActive && !this.m_IsInteractableDisabled)
		{
			this.m_IsInteractableDisabled = true;
			this.Exit();
		}
	}

	private void HandleInteractableOnEnter(object sender, EventArgs e)
	{
		if (this.m_Interactable.IsActive)
		{
			this.Enter();
		}
	}

	public void Enter()
	{
		for (int i = 0; i < this.m_MeshOutlineGroup.Length; i = unchecked(i + 1))
		{
			MeshOutlineGroup meshOutlineGroup;
			meshOutlineGroup = this.m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.OutlineMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.OutlineMaterial, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
		}
		this.m_IsInteractableDisabled = false;
		BATDRUIAudioManager.UIHighlightBegin();
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.Interact();
	}

	public void Interact()
	{
		for (int i = 0; i < this.m_MeshOutlineGroup.Length; i = unchecked(i + 1))
		{
			MeshOutlineGroup meshOutlineGroup;
			meshOutlineGroup = this.m_MeshOutlineGroup[i];
			Material material;
			material = ((meshOutlineGroup.InteractMaterial != null) ? meshOutlineGroup.InteractMaterial : meshOutlineGroup.DefaultMaterial);
			if (meshOutlineGroup.MeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.MeshRenderer, material, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, material, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
		}
	}

	private void HandleInteractableOnExit(object sender, EventArgs e)
	{
		if (this.m_Interactable.IsActive)
		{
			this.Exit();
		}
	}

	public void Exit()
	{
		for (int i = 0; i < this.m_MeshOutlineGroup.Length; i = unchecked(i + 1))
		{
			MeshOutlineGroup meshOutlineGroup;
			meshOutlineGroup = this.m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.DefaultMaterial, isEnter: false, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.DefaultMaterial, isEnter: false, meshOutlineGroup.MaterialIndex);
			}
		}
		BATDRUIAudioManager.UIHighlightEnd();
	}

	private void SetMaterial(Renderer renderer, Material material, bool isEnter, int materialIndex)
	{
		HighlightDistance highlightDistance;
		highlightDistance = null;
		if (renderer != null)
		{
			Material[] sharedMaterials;
			sharedMaterials = renderer.sharedMaterials;
			if (materialIndex == -1)
			{
				for (int i = 0; i < sharedMaterials.Length; i = unchecked(i + 1))
				{
					sharedMaterials[i] = material;
				}
			}
			else
			{
				sharedMaterials[materialIndex] = material;
			}
			renderer.sharedMaterials = sharedMaterials;
			highlightDistance = renderer.gameObject.GetComponent<HighlightDistance>();
		}
		if (highlightDistance != null)
		{
			if (isEnter)
			{
				highlightDistance.Enter();
			}
			else
			{
				highlightDistance.Exit();
			}
		}
	}

	private void SetMaterial(Renderer renderer, Material material, int materialIndex)
	{
		if (!(renderer != null))
		{
			return;
		}
		Material[] sharedMaterials;
		sharedMaterials = renderer.sharedMaterials;
		if (materialIndex == -1)
		{
			for (int i = 0; i < sharedMaterials.Length; i = unchecked(i + 1))
			{
				sharedMaterials[i] = material;
			}
		}
		else
		{
			sharedMaterials[materialIndex] = material;
		}
		renderer.sharedMaterials = sharedMaterials;
	}

	public void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	public void Enable()
	{
		this.AddListeners();
		for (int i = 0; i < this.m_MeshOutlineGroup.Length; i = unchecked(i + 1))
		{
			MeshOutlineGroup meshOutlineGroup;
			meshOutlineGroup = this.m_MeshOutlineGroup[i];
			if (meshOutlineGroup.MeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.MeshRenderer, meshOutlineGroup.DefaultMaterial, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, meshOutlineGroup.DefaultMaterial, meshOutlineGroup.MaterialIndex);
			}
		}
		this.SetActive(active: true);
	}

	public void Disable()
	{
		this.SetActive(active: false);
		this.RemoveListeners();
		for (int i = 0; i < this.m_MeshOutlineGroup.Length; i = unchecked(i + 1))
		{
			MeshOutlineGroup meshOutlineGroup;
			meshOutlineGroup = this.m_MeshOutlineGroup[i];
			Material material;
			material = ((meshOutlineGroup.InteractMaterial != null) ? meshOutlineGroup.InteractMaterial : meshOutlineGroup.DefaultMaterial);
			if (meshOutlineGroup.MeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.MeshRenderer, material, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
			else if (meshOutlineGroup.SkinnedMeshRenderer != null)
			{
				this.SetMaterial(meshOutlineGroup.SkinnedMeshRenderer, material, isEnter: true, meshOutlineGroup.MaterialIndex);
			}
		}
	}

	private void AddListeners()
	{
		this.RemoveListeners();
		if ((bool)this.m_Interactable)
		{
			this.m_Interactable.OnEnter += this.HandleInteractableOnEnter;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
			this.m_Interactable.OnExit += this.HandleInteractableOnExit;
		}
	}

	private void RemoveListeners()
	{
		if ((bool)this.m_Interactable)
		{
			this.m_Interactable.OnEnter -= this.HandleInteractableOnEnter;
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnExit -= this.HandleInteractableOnExit;
		}
	}

	protected override void OnDisposed()
	{
		this.RemoveListeners();
		base.OnDisposed();
	}
}
