using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObjectData
{
	[SerializeField]
	private float m_ID;

	[SerializeField]
	private string m_Name;

	[SerializeField]
	private string m_Tag;

	[SerializeField]
	private int m_Layer;

	[SerializeField]
	private bool m_ActiveSelf;

	[SerializeField]
	private TransformData m_Transform;

	public float ID => this.m_ID;

	public string Name => this.m_Name;

	public string Tag => this.m_Tag;

	public int Layer => this.m_Layer;

	public bool ActiveSelf => this.m_ActiveSelf;

	public TransformData Transform => this.m_Transform;

	public static GameObjectData GetData(GameObject gameObject)
	{
		return GameObjectData.InternalGetData(gameObject);
	}

	public static List<GameObjectData> GetChildrenData(GameObject gameObject)
	{
		Transform[] componentsInChildren;
		componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
		List<GameObjectData> list;
		list = new List<GameObjectData>();
		int num;
		num = 0;
		Transform[] array;
		array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (num > 0)
			{
				if (transform.GetComponent<SeasonalHat>() != null || transform.GetComponent<SeasonalHatContainer>() != null || transform.GetComponent<SeasonalHatLoader>() != null)
				{
					continue;
				}
				list.Add(GameObjectData.InternalGetData(transform.gameObject));
			}
			num = unchecked(num + 1);
		}
		return list;
	}

	private static GameObjectData InternalGetData(GameObject gameObject)
	{
		return new GameObjectData
		{
			m_Name = gameObject.name,
			m_Tag = gameObject.tag,
			m_Layer = gameObject.layer,
			m_ActiveSelf = gameObject.activeSelf,
			m_Transform = TransformData.Get(gameObject.transform)
		};
	}
}
