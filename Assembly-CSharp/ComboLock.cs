using System;
using UnityEngine;

public class ComboLock : JMonoBehaviour
{
	[Header("ID")]
	[SerializeField]
	private int m_ID;

	[Header("Code")]
	[SerializeField]
	private int m_Code = 414;

	[Header("Combo Lock")]
	[SerializeField]
	private InteractableComboLock m_Content;

	public int ID => this.m_ID;

	public InteractableComboLock Content => this.m_Content;

	public bool IsComplete { get; private set; }

	public event EventHandler OnComplete;

	public void Initialize()
	{
		this.m_Content.Initialize(this.m_Code);
		this.m_Content.OnSuccess += this.HandleComboLockOnSuccess;
	}

	public void InitializeCode(int code)
	{
		this.m_Content.InitializeCode(code, this.m_Code);
		this.m_Content.OnSuccess += this.HandleComboLockOnSuccess;
	}

	private void HandleComboLockOnSuccess(object sender, EventArgs e)
	{
		this.m_Content.OnSuccess -= this.HandleComboLockOnSuccess;
		this.IsComplete = true;
		this.OnComplete.Send(this);
	}

	public void ForceComplete()
	{
		this.m_Content.ForceComplete(this.m_Code);
		this.IsComplete = true;
	}

	protected override void OnDisposed()
	{
		this.OnComplete = null;
		this.m_Content.OnSuccess -= this.HandleComboLockOnSuccess;
		base.OnDisposed();
	}
}
