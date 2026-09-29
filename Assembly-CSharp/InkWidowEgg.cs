using System;
using DG.Tweening;
using UnityEngine;

public class InkWidowEgg : JMonoBehaviour
{
	[SerializeField]
	private GameObject m_Egg;

	[SerializeField]
	private GameObject m_Ink;

	[SerializeField]
	private ParticleSystem m_BrokenEgg;

	[SerializeField]
	private Enemy[] m_InkWidowPrefabs;

	private Enemy m_InkWidow;

	private Sequence m_Sequence;

	public event EventHandler OnInitialize;

	public event EventHandler OnDeath;

	public override void Start()
	{
		this.m_Ink.SetActive(value: false);
	}

	public void Initialize()
	{
		this.m_Ink.SetActive(value: true);
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		GameManager.Instance.Player.OnDeath += this.HandlePlayerOnDeath;
		this.m_Sequence = DOTween.Sequence();
		this.m_Sequence.InsertCallback(1f, () =>
		{
			this.m_Egg.SetActive(value: false);
			this.m_BrokenEgg.Play();
			this.m_InkWidow = GameManager.Instance.AssetManager.CreateAsset<Enemy>(this.m_InkWidowPrefabs[global::UnityEngine.Random.Range(0, this.m_InkWidowPrefabs.Length)]);
			this.m_InkWidow.transform.SetParent(base.transform);
			this.m_InkWidow.transform.position = base.transform.position;
			this.m_InkWidow.transform.eulerAngles = new Vector3(0f, global::UnityEngine.Random.Range(0f, 360f), 0f);
			this.m_InkWidow.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
			this.m_InkWidow.OnInitializeOnComplete += this.HandleInkWidowOnInitializeOnComplete;
			this.m_InkWidow.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
			this.m_InkWidow.OnCharacterDeath += this.HandleInkWidowOnCharacterDeath;
			this.OnInitialize.Send(this);
		});
	}

	private void HandlePlayerOnDeath(object sender, EventArgs e)
	{
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		if (this.m_InkWidow != null)
		{
			this.m_InkWidow.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
			this.m_InkWidow.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
		}
		this.SendDeath();
	}

	private void HandleInkWidowOnCharacterDeath(object sender, EventArgs e)
	{
		this.m_InkWidow.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
		GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		if (this.m_InkWidow != null)
		{
			this.m_InkWidow.transform.SetParent(null);
		}
		this.SendDeath();
	}

	private void HandleInkWidowOnInitializeOnComplete(object sender, EventArgs e)
	{
		this.m_InkWidow.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
		this.m_InkWidow.SetTarget(GameManager.Instance.Player.transform);
		this.m_InkWidow.SetState(State.Character.Follow);
	}

	private void SendDeath()
	{
		this.OnDeath.Send(this);
		base.Dispose();
	}

	protected override void OnDisposed()
	{
		if (GameManager.Instance.Player != null)
		{
			GameManager.Instance.Player.OnDeath -= this.HandlePlayerOnDeath;
		}
		this.OnInitialize = null;
		this.OnDeath = null;
		if (this.m_InkWidow != null)
		{
			this.m_InkWidow.OnCharacterDeath -= this.HandleInkWidowOnCharacterDeath;
			this.m_InkWidow.OnInitializeOnComplete -= this.HandleInkWidowOnInitializeOnComplete;
		}
		if (this.m_Sequence != null)
		{
			this.m_Sequence.Kill();
			this.m_Sequence = null;
		}
		base.OnDisposed();
	}
}
