using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LightGroup : MonoBehaviour
{
	[Header("General")]
	public bool startOff = true;

	[Header("Bounds")]
	[Tooltip("When true, will use the bounds to turn on this volume if the player is contained by any of the child bounds. When false, you will have to manually turn on and off this Light Group, via the LightGroups public API.")]
	public bool useBounds = true;

	[Header("Debugging")]
	[SerializeField]
	private SimpleCull sc;

	[SerializeField]
	private bool isOn = true;

	private HashSet<BoundObject> boundsList = new HashSet<BoundObject>();

	private HashSet<Light> lights = new HashSet<Light>();

	private HashSet<VideoPlayer> videoPlayerList = new HashSet<VideoPlayer>();

	private bool _started;

	private void Awake()
	{
		this.boundsList.Clear();
		this.boundsList.AddRange(base.GetComponentsInChildren<BoundObject>());
		this.isOn = true;
	}

	private IEnumerator Start()
	{
		int counter;
		counter = 100;
		unchecked
		{
			do
			{
				yield return null;
				this.sc = Object.FindObjectOfType<SimpleCull>();
			}
			while (!this.sc && counter-- > 0);
			if (counter <= 0)
			{
				Debug.LogError(this.Log("Unable to find instance of SimpleCull..."), this);
				yield break;
			}
			counter = 100;
			do
			{
				yield return null;
			}
			while (!this.sc.cullTransform && counter-- > 0);
			if (counter <= 0)
			{
				Debug.LogError(this.Log("Unable to find sc.cullTransform"), this);
				yield break;
			}
			this.SetupLights(this.sc.GetLightList());
			this.SetupVideoPlayers();
			if (this.startOff)
			{
				this.ToggleLightGroup(turnOn: false);
			}
			this._started = true;
		}
	}

	private void Update()
	{
		if (this._started && this.useBounds)
		{
			this.ToggleLightGroup(this.Contains(this.sc.cullTransform.position));
		}
	}

	public void OnDrawGizmosSelected()
	{
		foreach (Light light in this.lights)
		{
			if ((bool)light && light.enabled)
			{
				Gizmos.color = (light.enabled ? Color.green : Color.red);
				Gizmos.DrawWireSphere(light.transform.position, 3f);
			}
		}
		if (!Application.isPlaying || !this.useBounds)
		{
			return;
		}
		Gizmos.color = (this.isOn ? Color.green : Color.red);
		foreach (BoundObject bounds in this.boundsList)
		{
			if ((bool)bounds)
			{
				Gizmos.DrawWireCube(bounds.bounds.center, bounds.bounds.size);
			}
		}
	}

	public bool Contains(Vector3 point)
	{
		foreach (BoundObject bounds in this.boundsList)
		{
			if (bounds.bounds.Contains(point))
			{
				return true;
			}
		}
		return false;
	}

	public void ToggleLightGroup(bool turnOn)
	{
		if (turnOn && !this.isOn)
		{
			if ((bool)this.sc)
			{
				this.sc.AddLights(this.lights);
			}
			this.videoPlayerList.ForEach((VideoPlayer x) =>
			{
				if ((bool)x && (bool)x.gameObject && !x.gameObject.activeSelf)
				{
					x.gameObject.SetActive(value: true);
				}
			});
		}
		else if (!turnOn && this.isOn)
		{
			if ((bool)this.sc)
			{
				this.sc.RemoveLights(this.lights);
			}
			this.lights.ForEach((Light x) =>
			{
				if ((bool)x && x.enabled)
				{
					x.enabled = false;
				}
			});
			this.videoPlayerList.ForEach((VideoPlayer x) =>
			{
				if ((bool)x && (bool)x.gameObject && x.gameObject.activeSelf)
				{
					x.gameObject.SetActive(value: false);
				}
			});
		}
		this.isOn = turnOn;
	}

	private void SetupLights(IEnumerable<Light> newLights)
	{
		this.lights.Clear();
		foreach (Light newLight in newLights)
		{
			if (!newLight || (bool)newLight.GetComponent<NoLightCullingMarker>())
			{
				continue;
			}
			foreach (BoundObject bounds in this.boundsList)
			{
				if (bounds.includeLightsInBounds && bounds.bounds.Contains(newLight.transform.position))
				{
					this.lights.Add(newLight);
				}
			}
		}
	}

	private void SetupVideoPlayers()
	{
		this.videoPlayerList.Clear();
		VideoPlayer[] array;
		array = Object.FindObjectsOfType<VideoPlayer>();
		foreach (VideoPlayer videoPlayer in array)
		{
			if (!videoPlayer || !videoPlayer.gameObject.activeSelf || !videoPlayer.playOnAwake)
			{
				continue;
			}
			foreach (BoundObject bounds in this.boundsList)
			{
				if (bounds.includeLightsInBounds && bounds.bounds.Contains(videoPlayer.transform.position))
				{
					this.videoPlayerList.Add(videoPlayer);
				}
			}
		}
	}

	private string Log(string msg)
	{
		return "[LIGHT GROUP (" + base.gameObject.name + ")] " + msg;
	}
}
