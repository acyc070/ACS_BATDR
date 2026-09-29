using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio;

public class S13ReverbCalculator : MonoBehaviour
{
	private class Raycaster
	{
		private readonly RaycastHit[] _hits;

		private readonly Vector3 _direction;

		private int _numHits;

		public Vector3 direction => this._direction;

		public Raycaster(Vector3 direction, RaycastHit[] hits)
		{
			this._direction = direction;
			this._hits = hits;
		}

		public float FireRay(Vector3 origin, float rayDistance, LayerMask ignoreLayerMask)
		{
			this._numHits = Physics.RaycastNonAlloc(origin, this.direction, this._hits, rayDistance, ignoreLayerMask);
			return this._numHits;
		}

		public RaycastHit GetClosestHit()
		{
			return (from x in this._hits.Take(this._numHits)
				orderby x.distance
				select x).FirstOrDefault();
		}
	}

	[Header("Settings")]
	[SerializeField]
	[Tooltip("Layers to ignore collisions with")]
	private LayerMask ignoreLayerMask;

	[SerializeField]
	[Tooltip("How far each ray should travel")]
	private float rayDistance = 5f;

	[SerializeField]
	[Tooltip("How many Raycast's to fire on horizontal axis")]
	private int numHorizontalRays = 4;

	[SerializeField]
	[Tooltip("Should a ray be shot upwards?")]
	private bool enableUpRay;

	[SerializeField]
	[Tooltip("Should a ray be shot downwards?")]
	private bool enableDownRay;

	[SerializeField]
	[Tooltip("The minimum movement that must be made before rays are fired again")]
	private float minimumMovement = 0.2f;

	[FormerlySerializedAs("enableSkewedFitness")]
	[Header("Exposure Settings")]
	[SerializeField]
	[Tooltip("Should we skew exposure along a curve?")]
	private bool enableSkew;

	[FormerlySerializedAs("fitnessSkewCurve")]
	[SerializeField]
	[S13CurveRange(0f, 0f, 1f, 1f)]
	private AnimationCurve skewCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	[S13ReadOnlyField]
	[Tooltip("Value between 0 and 1 that represents how open or enclosed the area is. 0 is fully enclosed, 1 is fully open")]
	private float exposure;

	[FormerlySerializedAs("skewedFitness")]
	[SerializeField]
	[S13ReadOnlyField]
	[Tooltip("Exposure after being skewed by skewCurve")]
	private float skewedExposure;

	[Header("Accessor")]
	[Tooltip("Unique Identifier to access the exposure value from S13Manager")]
	[S13LockableField]
	[SerializeField]
	private string accessorID = S13ReverbCalculator.DEFAULT_ACCESSOR_ID;

	private static string DEFAULT_ACCESSOR_ID;

	[Header("Debug")]
	[SerializeField]
	private bool debugMode;

	private Raycaster[] raycasters;

	private Vector3 lastPlayerPosition;

	private void Start()
	{
		this.SetupRaycasters();
	}

	private void Update()
	{
		if (Vector3.Distance(base.transform.position, this.lastPlayerPosition) > this.minimumMovement)
		{
			this.lastPlayerPosition = base.transform.position;
			this.FireAllRays();
		}
		this.CalculateExposure();
		if (this.accessorID != S13ReverbCalculator.DEFAULT_ACCESSOR_ID)
		{
			S13Manager.SetAccessor(this.accessorID, this.skewedExposure);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Color color;
		color = Gizmos.color;
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, this.exposure * this.rayDistance);
		Gizmos.color = color;
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.SetupRaycasters();
			if (!this.debugMode)
			{
				S13Debug.HideCanvasText(base.name + "_REVERB_RAW_EXPOSURE");
				S13Debug.HideCanvasText(base.name + "_REVERB_SKEWED_EXPOSURE");
			}
		}
	}

	private void SetupRaycasters()
	{
		List<Raycaster> list;
		list = new List<Raycaster>();
		Quaternion rotation;
		rotation = base.transform.rotation;
		base.transform.forward = Vector3.forward;
		float num;
		num = 0f;
		for (int i = 0; i < this.numHorizontalRays; i = unchecked(i + 1))
		{
			list.Add(new Raycaster(new Vector3(Mathf.Sin(num), 0f, Mathf.Cos(num)), new RaycastHit[3]));
			num += MathF.PI * 2f / (float)this.numHorizontalRays;
		}
		base.transform.rotation = rotation;
		if (this.enableUpRay)
		{
			list.Add(new Raycaster(Vector3.up, new RaycastHit[3]));
		}
		if (this.enableDownRay)
		{
			list.Add(new Raycaster(Vector3.down, new RaycastHit[3]));
		}
		this.raycasters = list.ToArray();
	}

	private void CalculateExposure()
	{
		float num;
		num = 0f;
		for (int i = 0; i < this.raycasters.Length; i = unchecked(i + 1))
		{
			RaycastHit closestHit;
			closestHit = this.raycasters[i].GetClosestHit();
			num += (closestHit.transform ? closestHit.distance : this.rayDistance);
		}
		this.exposure = num / ((float)this.raycasters.Length * this.rayDistance);
		this.skewedExposure = (this.enableSkew ? this.skewCurve.Evaluate(this.exposure) : this.exposure);
	}

	private void FireAllRays()
	{
		Raycaster[] array;
		array = this.raycasters;
		for (int i = 0; i < array.Length; i = unchecked(i + 1))
		{
			this.FireRay(array[i]);
		}
	}

	private void FireRay(Raycaster raycaster)
	{
		raycaster.FireRay(base.transform.position, this.rayDistance, ~(int)this.ignoreLayerMask);
		raycaster.GetClosestHit();
	}

	static S13ReverbCalculator()
	{
		S13ReverbCalculator.DEFAULT_ACCESSOR_ID = "";
	}
}
