using S13Audio.BATDR;
using UnityEngine;

public class FloorMaterial : JComponent
{
	[SerializeField]
	private BATDRPlayerAudioController.FloorMaterials m_FloorType;

	public BATDRPlayerAudioController.FloorMaterials FloorType => this.m_FloorType;
}
