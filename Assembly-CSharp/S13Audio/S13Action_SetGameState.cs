using System;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class S13Action_SetGameState : IS13Executable
{
	[Serializable]
	public enum GamePauseState
	{
		NoChange = 0,
		Pause = 1,
		Resume = 2
	}

	[Tooltip("The pause state of the game to set.")]
	[SerializeField]
	private GamePauseState gamePauseState;

	public bool IsExecutable => this.gamePauseState != GamePauseState.NoChange;

	public void Execute()
	{
		switch (this.gamePauseState)
		{
		case GamePauseState.Pause:
			S13Manager.PauseGame();
			break;
		case GamePauseState.Resume:
			S13Manager.ResumeGame();
			break;
		case GamePauseState.NoChange:
			break;
		}
	}

	public override string ToString()
	{
		return $" GamePauseState {this.gamePauseState}";
	}
}
