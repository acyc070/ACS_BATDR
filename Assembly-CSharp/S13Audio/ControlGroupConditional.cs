using System;
using System.Linq;
using S13Audio.Malee;
using UnityEngine;

namespace S13Audio;

[Serializable]
public class ControlGroupConditional : IS13ControlGroupConditional
{
	public enum ControlGroupCondition
	{
		None = 0,
		ReplayGate = 1,
		MaxVoices = 2,
		SoloObj = 3,
		RaycastToListener = 4,
		CameraFacing = 5
	}

	[SerializeField]
	public ControlGroupCondition controlGroupCondition;

	[SerializeField]
	public S13ScriptableControlGroup.S13ConditionalForceMode forceMode;

	[Header("ReplayGate")]
	public float replayGateTime;

	[Header("MaxVoices")]
	public int maxVoices = 1;

	public bool countChildVoices = true;

	[Header("SoloObj")]
	[Tooltip("If an element of soloList is playing, reject incoming S13Objects")]
	[Reorderable]
	public S13ObjectList soloList;

	[Tooltip("Should multiple instances of the same S13Object be allowed to play? If false, S13Object's will solo self")]
	public bool allowMultipleInstances = true;

	[Header("LayerMask")]
	[Tooltip("Layer's to perform the raycast on")]
	public LayerMask raycastCheckMask;

	[Header("Ignore Raycast Distance")]
	[Tooltip("If the distance between the listener and this transform is less than this value, allow the Play call")]
	public float ignoreRaycastDistance = -1f;

	[Header("Debug")]
	[Tooltip("Draw debug rays to the screen to assist with debugging. Green is a successful check, Red is a failed check, yellow is a successful check due to distance")]
	public bool raycastDebug;

	private RaycastHit[] raycastHits = new RaycastHit[1];

	[Header("Angle")]
	[Tooltip("The minimum angle that the S13AudioHandler needs to be within the camera's forward direction")]
	public float minimumFacingAngle = 90f;

	[Header("Ignore Camera Facing Distance")]
	[Tooltip("If the distance between the listener and the S13AudioHandler is less than this value, allow the Play call")]
	public float ignoreFacingDistance = -1f;

	[Header("Debug")]
	[Tooltip("Draw debug rays to the screen to assist with debugging. Green is a successful check, Red is a failed check, yellow is a successful check due to distance")]
	public bool cameraFacingDebug;

	public bool CheckCondition(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		bool result;
		result = false;
		switch (this.controlGroupCondition)
		{
		case ControlGroupCondition.None:
			resultMessage = "Approved : No Condition";
			break;
		case ControlGroupCondition.ReplayGate:
			result = this.CheckReplayGate(audioHandler, controlGroupHandler);
			break;
		case ControlGroupCondition.MaxVoices:
			result = this.CheckMaxVoices(audioHandler, controlGroupHandler);
			break;
		case ControlGroupCondition.SoloObj:
			result = this.CheckSoloObj(audioHandler, controlGroupHandler);
			break;
		case ControlGroupCondition.RaycastToListener:
			result = this.CheckRaycastToListener(audioHandler, controlGroupHandler);
			break;
		case ControlGroupCondition.CameraFacing:
			result = this.CheckCameraFacing(audioHandler, controlGroupHandler);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		S13Handler handler;
		handler = audioHandler.GetHandler();
		string text;
		text = handler.name;
		if (handler is S13HandlerSwitch s13HandlerSwitch)
		{
			text = text + " (" + s13HandlerSwitch.CurrentScriptableSwitch.name + ")";
		}
		resultMessage = $"Check on AudioHandler: {audioHandler.name} | Handler: {text} | Control Group: {controlGroupHandler.reference.name} | Type: {this.controlGroupCondition}";
		return result;
	}

	public bool ForcePlay(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		bool flag;
		flag = false;
		if (this.forceMode != S13ScriptableControlGroup.S13ConditionalForceMode.IgnoreNew)
		{
			switch (this.controlGroupCondition)
			{
			case ControlGroupCondition.None:
				resultMessage = "Approved : No Condition";
				flag = true;
				break;
			case ControlGroupCondition.ReplayGate:
				flag = this.ForceReplayGate(controlGroupHandler, out resultMessage);
				break;
			case ControlGroupCondition.MaxVoices:
				flag = this.ForceMaxVoices(controlGroupHandler, out resultMessage);
				break;
			case ControlGroupCondition.SoloObj:
				flag = this.ForceSoloObject(controlGroupHandler, out resultMessage);
				break;
			case ControlGroupCondition.RaycastToListener:
				flag = this.ForceRaycastToListener(controlGroupHandler, out resultMessage);
				break;
			case ControlGroupCondition.CameraFacing:
				flag = this.ForceCameraFacing(controlGroupHandler, out resultMessage);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		else
		{
			resultMessage = "ForceMode is Ignore New";
			flag = false;
		}
		S13Handler handler;
		handler = audioHandler.GetHandler();
		resultMessage = $"Force AudioHandler: {audioHandler.name} | Handler: {handler.name} | Control Group: {controlGroupHandler.reference.name} | Type: {this.controlGroupCondition}: {resultMessage}";
		return flag;
	}

	private bool CheckReplayGate(S13AudioHandler audiohandler, S13ControlGroupHandler controlGroupHandler)
	{
		return Time.time - controlGroupHandler.lastPlayApprovedTime > this.replayGateTime;
	}

	private bool ForceReplayGate(S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		resultMessage = "ReplayGate can not be Forced";
		return false;
	}

	private bool CheckMaxVoices(S13AudioHandler audiohandler, S13ControlGroupHandler controlGroupHandler)
	{
		return ((!this.countChildVoices) ? controlGroupHandler.Count((S13Handler x) => x.isPlaying) : controlGroupHandler.Sum((S13Handler x) => x.NumActiveVoices)) < this.maxVoices;
	}

	private bool ForceMaxVoices(S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		S13Handler s13Handler;
		s13Handler = null;
		if (this.maxVoices <= 0)
		{
			resultMessage = "MaxVoices <= 0.";
			return false;
		}
		switch (this.forceMode)
		{
		case S13ScriptableControlGroup.S13ConditionalForceMode.KillRandom:
			s13Handler = controlGroupHandler.RandomElement();
			break;
		case S13ScriptableControlGroup.S13ConditionalForceMode.KillOld:
			s13Handler = controlGroupHandler.GetOldestHandler();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case S13ScriptableControlGroup.S13ConditionalForceMode.IgnoreNew:
			break;
		}
		if (s13Handler == null)
		{
			resultMessage = "MaxVoices Could not find S13Handler to stop";
			return false;
		}
		resultMessage = $" MaxVoices stopping {s13Handler}";
		s13Handler.Stop();
		return true;
	}

	private bool CheckSoloObj(S13AudioHandler audiohandler, S13ControlGroupHandler controlGroupHandler)
	{
		bool result;
		if (this.allowMultipleInstances)
		{
			result = true;
			foreach (S13Object solo in this.soloList)
			{
				if (controlGroupHandler.IsObjectPlaying(solo) && (!this.allowMultipleInstances || !(solo == audiohandler.GetObject())))
				{
					result = false;
					break;
				}
			}
		}
		else
		{
			result = this.soloList.All((S13Object soloObj) => !controlGroupHandler.IsObjectPlaying(soloObj));
		}
		return result;
	}

	private bool ForceSoloObject(S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		resultMessage = "SoloObject can not be Forced";
		return false;
	}

	private bool CheckRaycastToListener(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler)
	{
		Vector3 position;
		position = audioHandler.transform.position;
		AudioListener audioListener;
		audioListener = S13Manager.GetAudioListener();
		if (audioListener == null)
		{
			return false;
		}
		Vector3 position2;
		position2 = audioListener.transform.position;
		float num;
		num = Vector3.Distance(position, position2);
		Color color;
		color = Color.green;
		bool flag;
		flag = false;
		if (num > this.ignoreRaycastDistance)
		{
			if (Physics.RaycastNonAlloc(position, position2 - position, this.raycastHits, num, this.raycastCheckMask) > 0)
			{
				flag = true;
				color = Color.red;
			}
		}
		else
		{
			color = Color.yellow;
		}
		if (this.raycastDebug)
		{
			Debug.DrawLine(position, flag ? this.raycastHits[0].point : position2, color, Time.deltaTime * 3f);
		}
		return !flag;
	}

	private bool ForceRaycastToListener(S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		resultMessage = "RaycastToListener can not be Forced";
		return false;
	}

	private bool CheckCameraFacing(S13AudioHandler audioHandler, S13ControlGroupHandler controlGroupHandler)
	{
		Vector3 position;
		position = audioHandler.transform.position;
		if (GameManager.Instance.GameCamera == null)
		{
			return true;
		}
		Transform transform;
		transform = GameManager.Instance.GameCamera.transform;
		Color color;
		color = Color.green;
		bool flag;
		flag = true;
		if (Vector3.Distance(transform.position, position) <= this.ignoreFacingDistance)
		{
			color = Color.yellow;
		}
		else
		{
			flag = Vector3.Angle(to: (position - transform.position).normalized, from: transform.forward) <= this.minimumFacingAngle / 2f;
		}
		if (this.cameraFacingDebug)
		{
			if (!flag)
			{
				color = Color.red;
			}
			Debug.DrawLine(position, transform.position, color, Time.deltaTime * 3f);
		}
		return flag;
	}

	private bool ForceCameraFacing(S13ControlGroupHandler controlGroupHandler, out string resultMessage)
	{
		resultMessage = "CameraFacing can not be Forced";
		return false;
	}
}
