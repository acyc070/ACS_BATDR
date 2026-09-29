using UnityEngine;

public class Impact : JComponent
{
	[SerializeField]
	private Transform m_Content;

	[SerializeField]
	private GameObject m_Models;

	public Transform Content => this.m_Content;

	public GameObject Models => this.m_Models;

	public void Initialize(RaycastHit hit, bool isPooled = false)
	{
		bool isVisible;
		isVisible = true;
		if (hit.collider != null)
		{
			if (hit.collider is MeshCollider meshCollider)
			{
				if (!meshCollider.convex)
				{
					base.transform.position = this.GetPosition(hit, out isVisible);
				}
				else
				{
					this.SetBasicImpact(hit, out isVisible);
				}
			}
			else
			{
				this.SetBasicImpact(hit, out isVisible);
			}
		}
		else
		{
			this.SetBasicImpact(hit, out isVisible);
		}
		if (this.m_Models != null)
		{
			this.m_Models.SetActive(isVisible);
		}
		base.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
		this.m_Content.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
		this.m_Content.localScale = Vector3.one * Random.Range(0.95f, 1f);
		base.transform.SetParent(hit.transform);
		if (!isPooled)
		{
			Object.Destroy(base.gameObject, 4f);
		}
	}

	private void SetBasicImpact(RaycastHit hit, out bool isVisible)
	{
		base.transform.position = hit.point;
		isVisible = false;
	}

	private Vector3 GetPosition(RaycastHit hit, out bool isVisible)
	{
		isVisible = true;
		MeshCollider meshCollider;
		meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null || meshCollider.sharedMesh == null || meshCollider.convex)
		{
			isVisible = false;
			return hit.point;
		}
		Vector3 position;
		position = hit.collider.transform.position;
		Quaternion quaternion;
		quaternion = new Quaternion
		{
			eulerAngles = hit.collider.transform.eulerAngles
		};
		Mesh sharedMesh;
		sharedMesh = meshCollider.sharedMesh;
		int[] triangles;
		triangles = sharedMesh.triangles;
		unchecked
		{
			Vector3 vector;
			vector = position + quaternion * sharedMesh.vertices[triangles[hit.triangleIndex * 3]];
			Vector3 vector2;
			vector2 = position + quaternion * sharedMesh.vertices[triangles[hit.triangleIndex * 3 + 1]];
			Vector3 vector3;
			vector3 = position + quaternion * sharedMesh.vertices[triangles[hit.triangleIndex * 3 + 2]];
			Vector3 vector4;
			vector4 = this.NearestPointOnLine(vector, vector - vector2, hit.point);
			Vector3 vector5;
			vector5 = this.NearestPointOnLine(vector2, vector2 - vector3, hit.point);
			Vector3 vector6;
			vector6 = this.NearestPointOnLine(vector3, vector3 - vector, hit.point);
			float num;
			num = Vector3.Distance(hit.point, vector4);
			float num2;
			num2 = Vector3.Distance(hit.point, vector5);
			float num3;
			num3 = Vector3.Distance(hit.point, vector6);
			float num4;
			num4 = num;
			if (num2 < num4)
			{
				num4 = num2;
			}
			if (num3 < num4)
			{
				num4 = num3;
			}
			Vector3 result;
			result = hit.point;
			float num5;
			num5 = 0.2f;
			if (num4 == num)
			{
				if (num4 < num5)
				{
					result = vector4 + (hit.point - vector4).normalized * num5;
				}
			}
			else if (num4 == num2)
			{
				if (num4 < num5)
				{
					result = vector5 + (hit.point - vector5).normalized * num5;
				}
			}
			else if (num4 == num3 && num4 < num5)
			{
				result = vector6 + (hit.point - vector6).normalized * num5;
			}
			if (num4 <= 0.01f)
			{
				isVisible = false;
			}
			return result;
		}
	}

	private Vector3 NearestPointOnLine(Vector3 startPoint, Vector3 direction, Vector3 point)
	{
		direction.Normalize();
		float num;
		num = Vector3.Dot(point - startPoint, direction);
		return startPoint + direction * num;
	}
}
