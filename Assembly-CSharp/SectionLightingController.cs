using UnityEngine;

public class SectionLightingController : JMonoBehaviour
{
	private Light[] m_Lights;

	public override void Start()
	{
		this.m_Lights = base.GetComponentsInChildren<Light>(includeInactive: true);
	}

	private void Update()
	{
		if (GameManager.Instance.GameCamera == null || this.m_Lights == null || this.m_Lights.Length == 0 || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		float farClipPlane;
		farClipPlane = GameManager.Instance.GameCamera.Camera.farClipPlane;
		Transform transform;
		transform = GameManager.Instance.GameCamera.transform;
		Light[] lights;
		lights = this.m_Lights;
		foreach (Light light in lights)
		{
			float minDistance;
			minDistance = Vector3.Distance(transform.position, light.transform.position);
			float num;
			num = light.range + 10f;
			float maxDistance;
			maxDistance = farClipPlane + num;
			if (Vector3.Angle(to: (light.transform.position - transform.position).normalized, from: transform.forward) < 80f)
			{
				this.CheckDistance(minDistance, maxDistance, light);
			}
			else
			{
				this.CheckDistance(minDistance, num, light);
			}
		}
	}

	private void CheckDistance(float minDistance, float maxDistance, Light light)
	{
		if (minDistance > maxDistance)
		{
			this.DisableLight(light);
		}
		else
		{
			this.EnableLight(light);
		}
	}

	private void EnableLight(Light light)
	{
		if (!light.enabled)
		{
			light.enabled = true;
		}
	}

	private void DisableLight(Light light)
	{
		if (light.enabled)
		{
			light.enabled = false;
		}
	}

	protected override void OnDisposed()
	{
		this.m_Lights = null;
		base.OnDisposed();
	}
}
