using System;
using DG.Tweening;
using UnityEngine;

public class SockPuppet : JMonoBehaviour
{
	[Header("Requirements")]
	[SerializeField]
	private Requirements m_Requirements;

	[Header("Cutscene Activator")]
	[SerializeField]
	private CutsceneActivator m_CutsceneActivator;

	[Header("Balls")]
	[SerializeField]
	private Interactable m_Interactable;

	[SerializeField]
	private Collectable[] m_Collectables;

	[SerializeField]
	private Transform m_PlopLocation;

	[Header("Prefabs")]
	[SerializeField]
	private Transform m_BallPrefab;

	private int m_BallCount;

	private int m_PlopCount;

	public event EventHandler OnPlop;

	public override void Start()
	{
		this.m_Interactable.SetActive(active: false);
		for (int i = 0; i < this.m_Collectables.Length; i = unchecked(i + 1))
		{
			this.m_Collectables[i].Content.Disable();
		}
		GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
		if (!this.CheckStatus())
		{
			GameManager.Instance.OnObjectiveComplete += this.HandleOnObjectiveComplete;
		}
	}

	private void HandleInteractableOnInteract(object sender, EventArgs e)
	{
		this.m_Interactable.ResetAction();
		if (this.m_BallCount <= 0 || this.m_PlopCount >= this.m_Collectables.Length || this.m_PlopCount >= this.m_BallCount)
		{
			return;
		}
		Debug.Log("plop less than ball");
		unchecked
		{
			this.m_PlopCount++;
			if (this.m_PlopCount >= this.m_Collectables.Length)
			{
				this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
				this.m_Interactable.Dispose();
			}
			Transform ball;
			ball = GameManager.Instance.AssetManager.CreateAsset<Transform>(this.m_BallPrefab);
			ball.position = GameManager.Instance.GameCamera.transform.position + Vector3.down;
			ball.eulerAngles = new Vector3(global::UnityEngine.Random.Range(0, 360), global::UnityEngine.Random.Range(0, 360), global::UnityEngine.Random.Range(0, 360));
			Vector3 endValue;
			endValue = ball.eulerAngles + new Vector3(global::UnityEngine.Random.Range(180, 720), global::UnityEngine.Random.Range(180, 720), global::UnityEngine.Random.Range(180, 720));
			Sequence sequence;
			sequence = DOTween.Sequence();
			sequence.Insert(0f, ball.DOMoveX(this.m_PlopLocation.position.x, 0.35f).SetEase(Ease.InSine));
			sequence.Insert(0f, ball.DOMoveZ(this.m_PlopLocation.position.z, 0.35f).SetEase(Ease.InSine));
			sequence.Insert(0f, ball.DOMoveY(this.m_PlopLocation.position.y, 0.55f).SetEase(Ease.InSine));
			sequence.Insert(0f, ball.DORotate(endValue, 0.55f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
			sequence.OnComplete(() =>
			{
				this.OnPlop.Send(this);
				global::UnityEngine.Object.Destroy(ball.gameObject);
				if (this.m_PlopCount >= this.m_Collectables.Length)
				{
					this.m_CutsceneActivator.ForcePlay();
				}
			});
		}
	}

	private void HandleCollectableOnActivate(object sender, EventArgs e)
	{
		(sender as Collectable).OnActivate -= this.HandleCollectableOnActivate;
		unchecked
		{
			this.m_BallCount++;
		}
	}

	private bool CheckStatus()
	{
		bool flag;
		flag = true;
		if (this.m_Requirements != null)
		{
			flag = this.m_Requirements.IsComplete();
		}
		if (flag)
		{
			GameManager.Instance.OnObjectiveComplete -= this.HandleOnObjectiveComplete;
			this.m_Interactable.OnInteract -= this.HandleInteractableOnInteract;
			this.m_Interactable.OnInteract += this.HandleInteractableOnInteract;
			this.m_Interactable.SetActive(active: true);
			this.m_Interactable.ResetAction();
			for (int i = 0; i < this.m_Collectables.Length; i = unchecked(i + 1))
			{
				Collectable obj;
				obj = this.m_Collectables[i];
				obj.OnActivate -= this.HandleCollectableOnActivate;
				obj.OnActivate += this.HandleCollectableOnActivate;
				obj.Content.Enable();
			}
		}
		return flag;
	}

	private void HandleOnObjectiveComplete(object sender, EventArgs e)
	{
		this.CheckStatus();
	}

	protected override void OnDisposed()
	{
		this.OnPlop = null;
		base.OnDisposed();
	}
}
