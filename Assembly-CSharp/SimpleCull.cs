using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleCull : MonoBehaviour
{
	[FormerlySerializedAs("overrideMaxDist")]
	public bool overrideCullingDistance;

	[FormerlySerializedAs("lightCullingDistance")]
	[Tooltip("When 'override culling distance' is false, this is set to the far clipping plane of the player camera.")]
	public float cullingDistance = 100f;

	[Header("Debugging")]
	[SerializeField]
	private Transform _cullTransform;

	[SerializeField]
	private Camera playerCamera;

	private HashSet<ParticleSystem> particleSystems = new HashSet<ParticleSystem>();

	private HashSet<Light> _lightSet = new HashSet<Light>();

	private Vector3 lightVector;

	private Vector3 itemPosition;

	public Transform cullTransform => this._cullTransform;

	private void Awake()
	{
		this.SetupLights();
		this.SetupParticleSystems();
	}

	private IEnumerator Start()
	{
		int counter;
		counter = 100;
		this.playerCamera = null;
		do
		{
			this.playerCamera = GameManager.Instance.GameCamera.Camera;
			yield return null;
		}
		while (this.playerCamera == null && unchecked(counter--) > 0);
		if (counter <= 0)
		{
			Debug.LogError(this.Log("Could not find GameManager.Instance.GameCamera...."), this);
			yield break;
		}
		this._cullTransform = this.playerCamera.transform;
		float farClipPlane;
		farClipPlane = this.playerCamera.farClipPlane;
		if (!this.overrideCullingDistance)
		{
			this.cullingDistance = farClipPlane;
		}
	}

	private void Update()
	{
		if ((bool)this._cullTransform)
		{
			this.CullLights();
			this.CullParticleSystems();
		}
	}

	private void OnDrawGizmosSelected()
	{
		foreach (Light item in this._lightSet)
		{
			if ((bool)item)
			{
				Gizmos.color = (item.enabled ? Color.green : Color.red);
				Gizmos.DrawWireSphere(item.transform.position, 3f);
			}
		}
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			if ((bool)particleSystem)
			{
				Gizmos.color = (particleSystem.isPaused ? Color.red : Color.green);
				Gizmos.DrawWireCube(particleSystem.transform.position, Vector3.one * 3f);
			}
		}
	}

	public IEnumerable<Light> GetLightList()
	{
		return this._lightSet.ToList();
	}

	public void AddLights(IEnumerable<Light> list)
	{
		this._lightSet.AddRange(list);
	}

	public void RemoveLights(IEnumerable<Light> list)
	{
		this._lightSet.RemoveRange(list);
	}

	private void CullLights()
	{
		Vector3 forward;
		forward = this._cullTransform.forward;
		foreach (Light item in this._lightSet)
		{
			if (!item)
			{
				continue;
			}
			this.itemPosition = item.transform.position;
			this.lightVector = this.itemPosition - this._cullTransform.position;
			float num;
			num = this.cullingDistance + item.range;
			if (this.lightVector.sqrMagnitude > num * num)
			{
				if (item.enabled)
				{
					item.enabled = false;
				}
			}
			else if (Vector3.Dot(this.lightVector, forward) < 0f && Vector3.Project(this.lightVector, forward).sqrMagnitude > item.range * item.range)
			{
				if (item.enabled)
				{
					item.enabled = false;
				}
			}
			else if (!item.enabled)
			{
				item.enabled = true;
			}
		}
	}

	private void SetupLights()
	{
		this._lightSet.Clear();
		this._lightSet.AddRange(Object.FindObjectsOfType<Light>());
		List<Light> list;
		list = new List<Light>();
		foreach (Light item in this._lightSet)
		{
			if (!item.isActiveAndEnabled || !item.gameObject.activeInHierarchy)
			{
				list.Add(item);
			}
			else if ((bool)item.gameObject.GetComponentInParent<NoLightCullingMarker>())
			{
				list.Add(item);
			}
		}
		this._lightSet.RemoveRange(list);
	}

	private void CullParticleSystems()
	{
		float num;
		num = this.cullingDistance * this.cullingDistance;
		foreach (ParticleSystem particleSystem in this.particleSystems)
		{
			if (!particleSystem)
			{
				continue;
			}
			if ((this._cullTransform.position - particleSystem.transform.position).sqrMagnitude > num)
			{
				if (!particleSystem.isPaused)
				{
					particleSystem.Pause();
				}
			}
			else if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	private void SetupParticleSystems()
	{
		this.particleSystems.Clear();
		ParticleSystem[] array;
		array = Object.FindObjectsOfType<ParticleSystem>();
		foreach (ParticleSystem particleSystem in array)
		{
			if (particleSystem.main.loop && particleSystem.gameObject.activeInHierarchy && particleSystem.isPlaying && !particleSystem.GetComponent<ParticleSystemPause>())
			{
				this.particleSystems.Add(particleSystem);
			}
		}
	}

	private string Log(string msg)
	{
		return "[SIMPLE CULL] " + msg;
	}
}
