using UnityEngine;

namespace S13Audio.BATDR;

public class BATDRSetSwitchOnAnimatorValue : MonoBehaviour
{
	public Animator animator;

	public string animatorParamName = string.Empty;

	[Header("Values")]
	[ReadOnly]
	public float floatValue;

	[Header("SwitchSettings")]
	[SerializeField]
	private S13SetSwitchOnParameter setSwitchReference;

	private int paramIndex = -1;

	private void Start()
	{
		if (!this.animator)
		{
			base.enabled = false;
			return;
		}
		AnimatorControllerParameter[] parameters;
		parameters = this.animator.parameters;
		for (int i = 0; i < parameters.Length; i = unchecked(i + 1))
		{
			if (parameters[i].name == this.animatorParamName)
			{
				this.paramIndex = i;
				break;
			}
		}
	}

	private void Update()
	{
		if (this.paramIndex >= 0)
		{
			this.floatValue = this.animator.GetFloat(this.animatorParamName);
			if (this.setSwitchReference != null)
			{
				this.setSwitchReference.SetValue(this.floatValue);
			}
		}
	}
}
