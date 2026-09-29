using UnityEngine;

public class SeasonalHatLoader : JMonoBehaviour
{
	[SerializeField]
	private SeasonalType m_SeasonalType;

	[SerializeField]
	private GameObject m_Hat;

	[SerializeField]
	private bool m_IsPlayer;

	public override void Start()
	{
		if ((!this.m_IsPlayer || SeasonalCheck.HasHat) && SeasonalCheck.IsSeasonal(out var seasonalType) && GameManager.Instance.PlayerSettings.Seasonal && seasonalType == this.m_SeasonalType)
		{
			GameObject gameObject;
			gameObject = Object.Instantiate(this.m_Hat);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
		}
	}
}
