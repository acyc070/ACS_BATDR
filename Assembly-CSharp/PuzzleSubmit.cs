using System;
using UnityEngine;

public class PuzzleSubmit : JMonoBehaviour
{
	[Header("Code")]
	[SerializeField]
	private int m_Code = 3645;

	[Header("Lever")]
	[SerializeField]
	private Lever m_Lever;

	[Header("Wheels")]
	[SerializeField]
	private PuzzleWheel m_Wheel1;

	[SerializeField]
	private PuzzleWheel m_Wheel2;

	[SerializeField]
	private PuzzleWheel m_Wheel3;

	[SerializeField]
	private PuzzleWheel m_Wheel4;

	[Header("Effects")]
	[SerializeField]
	private LightFixture m_SubmitEmission;

	[SerializeField]
	private GameObject m_Electricity;

	[SerializeField]
	private LightFlicker[] m_LightFlickers;

	[Header("Nav Marker")]
	[SerializeField]
	private Transform m_Navigation;

	[Header("Cutscene")]
	[SerializeField]
	private Cutscene m_SuccessCutscene;

	[SerializeField]
	private Cutscene m_FailCutscene;

	public event EventHandler OnPowerUp;

	public event EventHandler OnPowerDown;

	public override void Start()
	{
		this.RemoveListeners();
		this.AddListeners();
		this.SetActiveFail(active: false);
		this.m_Electricity.SetActive(value: false);
		string text;
		text = this.m_Code.ToString();
		this.m_Wheel1.Initialize(text[0].ToString().ParseInt());
		this.m_Wheel2.Initialize(text[1].ToString().ParseInt());
		this.m_Wheel3.Initialize(text[2].ToString().ParseInt());
		this.m_Wheel4.Initialize(text[3].ToString().ParseInt());
	}

	public void DisableFailCutscene()
	{
		this.m_FailCutscene.SetActive(active: false);
	}

	private void SetActiveSuccess(bool active)
	{
		this.m_SuccessCutscene.SetActive(active);
		this.m_SuccessCutscene.gameObject.SetActive(active);
	}

	private void SetActiveFail(bool active)
	{
		this.m_FailCutscene.SetActive(active);
		this.m_FailCutscene.gameObject.SetActive(active);
	}

	private void SetActiveEffects(bool active)
	{
		unchecked
		{
			if (active)
			{
				for (int i = 0; i < this.m_LightFlickers.Length; i++)
				{
					this.m_LightFlickers[i].TurnOn();
				}
				this.m_Electricity.SetActive(value: true);
				this.m_SubmitEmission.SetEmission(1f);
				GameManager.Instance.ShowNavigation(this.m_Navigation);
				this.OnPowerUp.Send(this);
			}
			else
			{
				for (int j = 0; j < this.m_LightFlickers.Length; j++)
				{
					this.m_LightFlickers[j].TurnOff();
				}
				this.m_Electricity.SetActive(value: false);
				this.m_SubmitEmission.SetEmission(0f);
				GameManager.Instance.ClearNavigation();
				this.OnPowerDown.Send(this);
			}
		}
	}

	private void SetActiveWheels(bool active)
	{
		this.m_Wheel1.SetActive(active);
		this.m_Wheel2.SetActive(active);
		this.m_Wheel3.SetActive(active);
		this.m_Wheel4.SetActive(active);
	}

	private void HandleSwitchOnActivated(object sender, EventArgs e)
	{
		this.m_Lever.Content.Disable();
		this.SetActiveEffects(active: true);
		this.SetActiveWheels(active: false);
		if (string.Concat(string.Concat(string.Concat("" + this.m_Wheel1.ID, this.m_Wheel2.ID.ToString()), this.m_Wheel3.ID.ToString()), this.m_Wheel4.ID.ToString()).ParseInt() == this.m_Code)
		{
			this.SetActiveFail(active: false);
			this.SetActiveSuccess(active: true);
		}
		else
		{
			this.SetActiveSuccess(active: false);
			this.SetActiveFail(active: true);
		}
	}

	private void HandleSuccessCutsceneOnComplete(object sender, EventArgs e)
	{
		this.m_SuccessCutscene.OnComplete -= this.HandleSuccessCutsceneOnComplete;
		this.m_Lever.Content.Disable();
		this.SetActiveEffects(active: false);
	}

	private void HandleFailCutsceneOnComplete(object sender, EventArgs e)
	{
		this.ResetSwitch();
	}

	public void ResetSwitch()
	{
		this.m_Lever.Content.ForceDeactivateComplete();
		this.m_Lever.Content.Enable();
		this.SetActiveEffects(active: false);
		this.SetActiveWheels(active: true);
		this.SetActiveSuccess(active: false);
		this.m_FailCutscene.SetActive(active: false);
	}

	private void AddListeners()
	{
		this.m_Lever.OnActivated += this.HandleSwitchOnActivated;
		this.m_SuccessCutscene.OnComplete += this.HandleSuccessCutsceneOnComplete;
		this.m_FailCutscene.OnComplete += this.HandleFailCutsceneOnComplete;
	}

	private void RemoveListeners()
	{
		this.m_Lever.OnActivated -= this.HandleSwitchOnActivated;
		this.m_SuccessCutscene.OnComplete -= this.HandleSuccessCutsceneOnComplete;
		this.m_FailCutscene.OnComplete -= this.HandleFailCutsceneOnComplete;
	}

	protected override void OnDisposed()
	{
		this.OnPowerUp = null;
		this.OnPowerDown = null;
		this.RemoveListeners();
		base.OnDisposed();
	}
}
