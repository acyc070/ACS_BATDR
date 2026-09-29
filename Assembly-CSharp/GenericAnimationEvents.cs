using UnityEngine;

public class GenericAnimationEvents : JMonoBehaviour
{
	[SerializeField]
	protected MonoBehaviour m_FunctionReciever;

	[SerializeField]
	protected bool m_FireEvents = true;

	[SerializeField]
	protected bool m_DisableEvents;

	public Animator Animator { get; private set; }

	public void SetActive(bool active)
	{
		this.m_FireEvents = active;
	}

	public void SetReciever(MonoBehaviour reciever)
	{
		this.m_FunctionReciever = reciever;
	}

	public override void Start()
	{
		this.Animator = base.GetComponent<Animator>();
		if (this.Animator != null)
		{
			this.Animator.fireEvents = !this.m_DisableEvents;
		}
	}

	public void TriggerEvent(AnimationEvent animationEvent)
	{
		if (this.m_FireEvents)
		{
			this.SendFunction(animationEvent.stringParameter, animationEvent.floatParameter, animationEvent.intParameter);
		}
	}

	private void SendFunction(string name, float floatValue, int intValue)
	{
		if (this.m_FunctionReciever == null)
		{
			Debug.LogError("m_FunctionReciever is null on " + base.name);
		}
		else if (!(name == ""))
		{
			if (floatValue != 0f && intValue != 0)
			{
				object[] value;
				value = new object[2] { floatValue, intValue };
				this.m_FunctionReciever?.SendMessage(name, value, SendMessageOptions.DontRequireReceiver);
			}
			else if (floatValue != 0f)
			{
				this.m_FunctionReciever?.SendMessage(name, floatValue, SendMessageOptions.DontRequireReceiver);
			}
			else if (intValue != 0)
			{
				this.m_FunctionReciever?.SendMessage(name, intValue, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				this.m_FunctionReciever?.SendMessage(name, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	protected override void OnDisposed()
	{
		this.Animator = null;
		base.OnDisposed();
	}

	public void RumbleEvent(AnimationEvent animationEvent)
	{
		if (this.m_FireEvents)
		{
			float strength;
			strength = float.Parse(animationEvent.stringParameter);
			GameManager.Instance.TriggerRumble(animationEvent.floatParameter, strength);
		}
	}
}
