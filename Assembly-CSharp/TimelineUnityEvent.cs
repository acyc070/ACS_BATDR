using UnityEngine.Events;

public class TimelineUnityEvent : JMonoBehaviour
{
	public UnityEvent UnityEvent;

	public void Action()
	{
		if (this.UnityEvent != null)
		{
			this.UnityEvent.Invoke();
		}
	}
}
