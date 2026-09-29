using UnityEngine;

public class AnimationEvents : JMonoBehaviour
{
	public const string TRIGGER_FUNCTION_NAME = "TriggerEventCommand";

	[SerializeField]
	private MonoBehaviour m_CommandReciever;

	[SerializeField]
	private CharacterAnimationEventData[] m_AnimationEventData;

	private Animator m_Animator;

	public override void Start()
	{
		if (this.m_Animator == null)
		{
			this.m_Animator = base.GetComponent<Animator>();
		}
		if (this.m_AnimationEventData == null)
		{
			return;
		}
		CharacterAnimationEventData[] animationEventData;
		animationEventData = this.m_AnimationEventData;
		foreach (CharacterAnimationEventData characterAnimationEventData in animationEventData)
		{
			foreach (AnimationEventCommand eventCommand in characterAnimationEventData.AnimationEventCommands.EventCommands)
			{
				AnimationEventUtility.AddAnimationEvent(ref this.m_Animator, characterAnimationEventData.Clip.name, "TriggerEventCommand", eventCommand.Frame, eventCommand.CommandName);
			}
		}
	}

	public void TriggerEventCommand(AnimationEvent animationEvent)
	{
		this.SendCommand(animationEvent.stringParameter);
	}

	private void SendCommand(string name)
	{
		if (name != string.Empty)
		{
			this.m_CommandReciever?.SendMessage(name, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetReciever(MonoBehaviour reciever)
	{
		this.m_CommandReciever = reciever;
	}

	protected override void OnDisposed()
	{
		this.m_Animator = null;
		base.OnDisposed();
	}
}
