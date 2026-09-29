using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRUIAudioManager : MonoBehaviour
{
	private static BATDRUIAudioManager _Instance;

	[SerializeField]
	private S13LocalAction onHighlightAction;

	[SerializeField]
	private S13LocalAction onInteractPrompt;

	private bool highlightFlag;

	private bool promptFlag;

	private static BATDRUIAudioManager Instance
	{
		get
		{
			if (!BATDRUIAudioManager._Instance)
			{
				BATDRUIAudioManager._Instance = Object.FindObjectOfType<BATDRUIAudioManager>();
				if (!BATDRUIAudioManager._Instance)
				{
					Debug.LogError("BATDRUIAudioManager does not exist within scene. One must be added in this or previously loaded scene for UI audio to function");
				}
			}
			return BATDRUIAudioManager._Instance;
		}
	}

	public static void UIHighlightBegin()
	{
		if (!BATDRUIAudioManager.Instance.highlightFlag)
		{
			BATDRUIAudioManager.Instance.highlightFlag = true;
			if (BATDRUIAudioManager.Instance.onHighlightAction.IsExecutable)
			{
				BATDRUIAudioManager.Instance.onHighlightAction.Execute();
			}
		}
	}

	public static void UIHighlightEnd()
	{
		BATDRUIAudioManager.Instance.highlightFlag = false;
	}

	public static void UIInteractPromptBegin()
	{
		if (!BATDRUIAudioManager.Instance.promptFlag)
		{
			BATDRUIAudioManager.Instance.promptFlag = true;
			if (!BATDRUIAudioManager.Instance.highlightFlag && BATDRUIAudioManager.Instance.onInteractPrompt.IsExecutable)
			{
				BATDRUIAudioManager.Instance.onInteractPrompt.Execute();
			}
		}
	}

	public static void UIInteractPromptEnd()
	{
		BATDRUIAudioManager.Instance.promptFlag = false;
	}
}
