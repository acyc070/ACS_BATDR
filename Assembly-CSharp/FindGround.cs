using UnityEngine;

public class FindGround : JMonoBehaviour
{
	[SerializeField]
	private Transform m_Content;

	private void Update()
	{
		if (!(this.m_Content == null) && !base.IsDisposed && !GameManager.Instance.IsPaused)
		{
			Vector3 vector;
			vector = base.transform.position + Vector3.up * 5f;
			if (Physics.Linecast(vector, vector + Vector3.down * 10f, out var hitInfo, ~((1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("InvisibleCollider"))), QueryTriggerInteraction.Ignore))
			{
				Vector3 point;
				point = hitInfo.point;
				point += Vector3.up * 0.01f;
				this.m_Content.position = point;
			}
		}
	}
}
