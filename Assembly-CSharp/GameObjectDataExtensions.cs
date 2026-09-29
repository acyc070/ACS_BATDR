using UnityEngine;

public static class GameObjectDataExtensions
{
	public static void SetData(this GameObject gameObject, GameObjectData gameObjectData)
	{
		gameObject.name = gameObjectData.Name;
		gameObject.tag = gameObjectData.Tag;
		gameObject.layer = gameObjectData.Layer;
		gameObject.SetActive(gameObjectData.ActiveSelf);
		gameObject.transform.SetData(gameObjectData.Transform);
	}
}
