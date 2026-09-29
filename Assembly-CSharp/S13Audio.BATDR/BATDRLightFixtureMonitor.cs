using UnityEngine;
using UnityEngine.Serialization;

namespace S13Audio.BATDR;

public class BATDRLightFixtureMonitor : MonoBehaviour
{
	private enum TriggerMode
	{
		Climbing = 0,
		Descending = 1
	}

	[SerializeField]
	[Tooltip("The light fixture to target. We check this lights Emission Value for calculations")]
	private LightFixture light;

	[FormerlySerializedAs("onMaxBrightness")]
	[SerializeField]
	private S13LocalAction onTriggerAction;

	[SerializeField]
	[Tooltip("Defines if the action should execute when the brightness is greater than (climbing) or less than (descending)")]
	private TriggerMode triggerMode;

	[Range(0f, 1f)]
	[SerializeField]
	[Tooltip("The light bounces between 0 and 1, and when its value passes this point, it will cause the sound to output. See TriggerMode. Note that values > 0.8 and < 0.2 may not work as intended, as the light could 'skip' these values")]
	private float triggerPoint = 0.7f;

	[SerializeField]
	[Tooltip("Setting this bool will have the monitor consider itself tripped at startup, essentially skipping the first trigger")]
	private bool skipFirstCycle;

	private bool _hasPlayedThisCycle;

	private void Start()
	{
		if (this.light == null)
		{
			base.enabled = false;
		}
		if (this.skipFirstCycle)
		{
			this._hasPlayedThisCycle = true;
		}
	}

	private void Update()
	{
		bool flag;
		flag = false;
		if (this.light != null)
		{
			flag = ((this.triggerMode == TriggerMode.Climbing) ? (this.light.EmissionValue > this.triggerPoint) : (this.light.EmissionValue < this.triggerPoint));
		}
		if (!this._hasPlayedThisCycle & flag)
		{
			if (this.onTriggerAction.IsExecutable)
			{
				this.onTriggerAction.Execute();
			}
			this._hasPlayedThisCycle = true;
		}
		else if (this._hasPlayedThisCycle && !flag)
		{
			this._hasPlayedThisCycle = false;
		}
	}
}
