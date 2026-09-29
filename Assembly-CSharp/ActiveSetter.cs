using UnityEngine;

[DefaultExecutionOrder(200)]
public class ActiveSetter : JMonoBehaviour
{
	[Header("Setters")]
	[SerializeField]
	private bool m_InitializeOnStart = true;

	[SerializeField]
	private bool m_ActiveOnStart;

	[Header("Setters")]
	[SerializeField]
	private GameObject[] m_Activated;

	[SerializeField]
	private GameObject[] m_Deactivated;

	[SerializeField]
	private GameObject[] m_ForceComplete;

	private bool m_IsForceComplete;

	public bool IsActive { get; private set; }

	public override void Awake()
	{
		if (!this.m_InitializeOnStart)
		{
			this.InitializeOnStart();
		}
	}

	public override void Start()
	{
		if (this.m_InitializeOnStart)
		{
			this.InitializeOnStart();
		}
	}

	private void InitializeOnStart()
	{
		if (!this.m_IsForceComplete)
		{
			this.SetActive(this.m_ActiveOnStart);
		}
	}

	public void SetActiveTrue()
	{
		this.SetActive(active: true);
	}

	public void SetActiveFalse()
	{
		this.SetActive(active: false);
	}

	public void SetActive(bool active)
	{
		this.IsActive = active;
		unchecked
		{
			if (this.m_Activated != null)
			{
				for (int i = 0; i < this.m_Activated.Length; i++)
				{
					GameObject gameObject;
					gameObject = this.m_Activated[i];
					if (gameObject != null)
					{
						gameObject.SetActive(this.IsActive);
					}
				}
			}
			if (this.m_Deactivated == null)
			{
				return;
			}
			for (int j = 0; j < this.m_Deactivated.Length; j++)
			{
				GameObject gameObject2;
				gameObject2 = this.m_Deactivated[j];
				if (gameObject2 != null)
				{
					gameObject2.SetActive(!this.IsActive);
				}
			}
		}
	}

	public void ForceComplete()
	{
		if (this.m_ForceComplete != null)
		{
			for (int i = 0; i < this.m_ForceComplete.Length; i = unchecked(i + 1))
			{
				GameObject gameObject;
				gameObject = this.m_ForceComplete[i];
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		this.SetActiveTrue();
		this.m_IsForceComplete = true;
	}
}
