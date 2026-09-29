using UnityEngine;

public class PlayerModelLayers : JMonoBehaviour
{
	[Header("Cut Scene")]
	[SerializeField]
	[LayerSelector]
	private string m_CutsceneLayer;

	[SerializeField]
	private GameObject[] m_CutsceneLayers;

	[Header("First Person")]
	[SerializeField]
	[LayerSelector]
	private string m_MirrorLayer;

	[SerializeField]
	private GameObject[] m_MirrorLayers;

	[Header("3rd Person")]
	[SerializeField]
	[LayerSelector]
	private string m_DefaultLayer;

	[SerializeField]
	private GameObject[] m_AllLayers;

	[Header("Head")]
	[SerializeField]
	[LayerSelector]
	private string m_HeadLayer;

	[SerializeField]
	private GameObject[] m_HeadLayers;

	[Header("Ability Arm")]
	[SerializeField]
	[LayerSelector]
	private string m_AbilityLayer;

	[SerializeField]
	private GameObject[] m_AbilityLayers;

	[Header("Weapon Arm")]
	[SerializeField]
	[LayerSelector]
	private string m_WeaponLayer;

	[SerializeField]
	private GameObject[] m_WeaponLayers;

	public void DisableHead()
	{
		this.SetLayer(ref this.m_HeadLayers, this.m_HeadLayer);
	}

	public void DisableAbility()
	{
		this.SetLayer(ref this.m_AbilityLayers, this.m_DefaultLayer);
	}

	public void DisableWeapon()
	{
		this.SetLayer(ref this.m_WeaponLayers, this.m_DefaultLayer);
	}

	public void EnableAll()
	{
		this.SetLayer(ref this.m_AllLayers, this.m_DefaultLayer);
	}

	public void EnableCutscene()
	{
		this.SetLayer(ref this.m_CutsceneLayers, this.m_CutsceneLayer);
	}

	public void EnableFirstPerson()
	{
		this.SetLayer(ref this.m_MirrorLayers, this.m_MirrorLayer);
	}

	public void EnableAbility()
	{
		this.SetLayer(ref this.m_AbilityLayers, this.m_AbilityLayer);
	}

	public void EnableWeapon()
	{
		this.SetLayer(ref this.m_WeaponLayers, this.m_WeaponLayer);
	}

	private void SetLayer(ref GameObject[] gameObjects, string layer)
	{
		for (int i = 0; i < gameObjects.Length; i = unchecked(i + 1))
		{
			gameObjects[i].layer = LayerMask.NameToLayer(layer);
		}
	}
}
