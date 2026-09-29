using System;

[Serializable]
public class State
{
	public enum Character
	{
		Alert = 0,
		Attack = 1,
		Companion = 2,
		Controllable = 3,
		Cutscene = 4,
		Death = 5,
		Evade = 6,
		Flee = 7,
		Follow = 8,
		Hit = 9,
		Idle = 10,
		Patrol = 11,
		Seasonal = 12,
		SeasonalHit = 13,
		Stun = 14
	}

	public enum Player
	{
		Ability = 0,
		Cutscene = 1,
		CutscenePeek = 2,
		Default = 3,
		Peek = 4
	}

	public enum PlayerAbility
	{
		Banish = 0,
		FastTravel = 1,
		Flow = 2
	}
}
public abstract class State<TActor, TIdentifier> : JDisposable
{
	private bool m_IsActive;

	protected TActor Actor { get; private set; }

	public TIdentifier ID { get; protected set; }

	public bool IsActive
	{
		get
		{
			if (this.Actor != null)
			{
				return this.m_IsActive;
			}
			return false;
		}
	}

	public State(TActor actor)
	{
		this.Actor = actor;
		this.SetActive(active: true);
	}

	public abstract void OnStateEnter();

	public abstract void OnStateExit();

	public abstract void Update();

	public abstract void FixedUpdate();

	public abstract void LateUpdate();

	public void Enable()
	{
		this.SetActive(active: true);
	}

	public void Disable()
	{
		this.SetActive(active: false);
	}

	protected void SetActive(bool active)
	{
		this.m_IsActive = active;
	}

	protected void GetActions(params Action[] actions)
	{
		if (!base.IsDisposed && this.m_IsActive && !GameManager.Instance.IsPaused && this.Actor != null)
		{
			for (int i = 0; i < actions.Length; i = unchecked(i + 1))
			{
				actions[i]?.Invoke();
			}
		}
	}

	protected override void OnDisposed()
	{
		this.Actor = default;
		base.OnDisposed();
	}
}
