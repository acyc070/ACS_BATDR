using System;
using System.Collections;
using UnityEngine;

public class CollectableSpawner : JMonoBehaviour
{
	private const float TIMER_LIMIT = 300f;

	private const float SPAWN_PERCENT = 0.7f;

	[Header("Prefabs")]
	[SerializeField]
	private ActionEventController[] m_Prefabs;

	private float m_Timer;

	private ActionEventController m_Collectable;

	public bool IsSpawned { get; private set; }

	public override void Start()
	{
		base.StartCoroutine(this.InternalStart());
	}

	private IEnumerator InternalStart()
	{
		while (GameManager.Instance.Player == null || !GameManager.Instance.Player.gameObject.activeInHierarchy || GameManager.Instance.IsPaused)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Vector3.Distance(GameManager.Instance.Player.transform.position, base.transform.position) > 40f)
		{
			this.Spawn();
		}
		yield return null;
	}

	private void Update()
	{
		if (this.IsSpawned || GameManager.Instance.Player == null || base.IsDisposed || GameManager.Instance.IsPaused)
		{
			return;
		}
		if (this.m_Timer >= 300f)
		{
			if (Vector3.Distance(base.transform.position, GameManager.Instance.Player.transform.position) > 40f)
			{
				this.Spawn();
			}
		}
		else
		{
			this.m_Timer += Time.deltaTime;
		}
	}

	private void Spawn(bool isForced = false)
	{
		if (this.IsSpawned)
		{
			return;
		}
		if (isForced || global::UnityEngine.Random.value <= 0.7f)
		{
			this.IsSpawned = true;
			this.m_Collectable = GameManager.Instance.AssetManager.CreateAsset<ActionEventController>(this.m_Prefabs[global::UnityEngine.Random.Range(0, this.m_Prefabs.Length)]);
			this.m_Collectable.SetParent(base.transform);
			this.m_Collectable.transform.localPosition = Vector3.zero;
			this.m_Collectable.transform.localEulerAngles = Vector3.zero;
			this.m_Collectable.OnActivate -= this.HandleCollectableOnActivate;
			this.m_Collectable.OnActivate += this.HandleCollectableOnActivate;
			if (this.m_Collectable.Content is SlugContent slugContent)
			{
				slugContent.InitializeContent();
			}
			else if (this.m_Collectable.Content is GentCardContent gentCardContent)
			{
				gentCardContent.InitializeContent();
			}
			else if (this.m_Collectable.Content is FoodContent foodContent)
			{
				foodContent.InitializeContent();
			}
			else if (this.m_Collectable.Content is GentPartsContent gentPartsContent)
			{
				gentPartsContent.InitializeContent();
			}
			else if (this.m_Collectable.Content is GentToolkitContent gentToolkitContent)
			{
				gentToolkitContent.InitializeContent();
			}
			else if (this.m_Collectable.Content is GentBatteryContent gentBatteryContent)
			{
				gentBatteryContent.InitializeContent();
			}
		}
		this.m_Timer = 0f;
	}

	public void ForceSpawn()
	{
		if (this.m_Collectable != null)
		{
			this.m_Collectable.OnActivate -= this.HandleCollectableOnActivate;
			this.m_Collectable.Dispose();
			this.m_Collectable = null;
			this.IsSpawned = false;
			this.m_Timer = 0f;
		}
		this.Spawn(isForced: true);
	}

	private void HandleCollectableOnActivate(object sender, EventArgs e)
	{
		this.m_Collectable.OnActivate -= this.HandleCollectableOnActivate;
		this.m_Collectable = null;
		this.IsSpawned = false;
		this.m_Timer = 0f;
	}

	protected override void OnDisposed()
	{
		base.StopCoroutine(this.InternalStart());
		if (this.m_Collectable != null)
		{
			this.m_Collectable.OnActivate -= this.HandleCollectableOnActivate;
			this.m_Collectable = null;
		}
		base.OnDisposed();
	}
}
