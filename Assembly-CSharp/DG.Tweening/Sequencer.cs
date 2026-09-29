using System.Collections;

namespace DG.Tweening;

public class Sequencer : JDisposable
{
	private Sequence m_Sequence;

	private UpdateType m_UpdateType;

	private bool m_IsIndependent;

	public bool IsPlaying
	{
		get
		{
			if (this.m_Sequence != null)
			{
				if (this.m_Sequence.IsActive())
				{
					return this.m_Sequence.IsPlaying();
				}
				return false;
			}
			return false;
		}
	}

	public Sequencer(bool isIndependent = false, UpdateType updateType = UpdateType.Normal)
	{
		this.m_IsIndependent = isIndependent;
		this.m_UpdateType = updateType;
	}

	public Sequencer CreateSequence(params Tween[] tweens)
	{
		this.RestartSequence();
		IEnumerator enumerator;
		enumerator = tweens.GetEnumerator();
		while (enumerator.MoveNext())
		{
			this.m_Sequence?.Insert(0f, enumerator.Current as Tween);
		}
		return this;
	}

	public Sequencer New()
	{
		this.Kill();
		return this;
	}

	public Sequencer Insert(float atPosition, Tween tween)
	{
		if (this.m_Sequence == null)
		{
			this.RestartSequence();
		}
		this.m_Sequence?.Insert(atPosition, tween);
		return this;
	}

	public Sequencer Insert(float atPosition, TweenCallback callback)
	{
		if (this.m_Sequence == null)
		{
			this.RestartSequence();
		}
		this.m_Sequence?.InsertCallback(atPosition, callback);
		return this;
	}

	public Sequencer OnComplete(TweenCallback action)
	{
		this.m_Sequence?.OnComplete(action);
		return this;
	}

	public void RestartSequence()
	{
		this.Kill();
		this.m_Sequence = DOTween.Sequence().SetUpdate(this.m_UpdateType, this.m_IsIndependent);
	}

	public void Kill()
	{
		this.m_Sequence?.Kill();
		this.m_Sequence = null;
	}

	protected override void OnDisposed()
	{
		this.Kill();
		base.OnDisposed();
	}
}
