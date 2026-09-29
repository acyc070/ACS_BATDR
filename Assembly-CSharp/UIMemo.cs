using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMemo : UIController
{
	[Header("Images")]
	[SerializeField]
	private Image m_MemoBG_Default;

	[SerializeField]
	private Image m_MemoBG_JDS;

	[SerializeField]
	private Image m_MemoBG_Gent;

	[SerializeField]
	private Image m_MemoBars;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_MemoLabel;

	[SerializeField]
	private TextMeshProUGUI m_MemoTypeLabel;

	[SerializeField]
	private TMP_FontAsset m_LostOneFont;

	private UIMemoDataVO m_DataVO;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIMemoDataVO)_data;
		if (this.m_DataVO == null)
		{
			Debug.LogError("UIMemo :: OnInitialized => m_DataVO = " + this.m_DataVO, this);
		}
		this.m_MemoBG_Default.enabled = this.m_DataVO.MemoType == MemoType.LostOne;
		this.m_MemoBG_JDS.enabled = this.m_DataVO.MemoType == MemoType.JDS;
		this.m_MemoBG_Gent.enabled = this.m_DataVO.MemoType == MemoType.GENT;
		bool flag;
		flag = this.m_DataVO.MemoType == MemoType.LostOne;
		this.m_MemoBars.gameObject.SetActive(!flag);
		this.m_MemoTypeLabel.gameObject.SetActive(!flag);
		this.m_MemoLabel.text = this.m_DataVO.MemoLabel;
		if (this.m_DataVO.MemoType == MemoType.LostOne)
		{
			this.m_MemoLabel.font = this.m_LostOneFont;
			this.m_MemoLabel.characterSpacing = 5f;
			this.m_MemoLabel.wordSpacing = 5f;
			this.m_MemoLabel.lineSpacing = 5f;
			this.m_MemoLabel.UpdateFontAsset();
		}
		else if (this.m_DataVO.MemoType == MemoType.JDS)
		{
			this.m_MemoTypeLabel.text = "JOEY DREW STUDIOS - PERSONAL MEMO";
		}
		else if (this.m_DataVO.MemoType == MemoType.GENT)
		{
			this.m_MemoTypeLabel.text = "PROPERTY OF THE GENT CORPORATION";
		}
		this.m_DataVO.Memo.Content.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (!base.IsDisposed)
		{
			if (this.m_InputTimer >= this.m_InputDelay && PlayerInput.InteractOnReleased())
			{
				this.m_DataVO.Memo.Content.ForceDeactivate();
				this.m_DataVO.Memo.Dispose();
				GameManager.Instance.HideMemo();
			}
			else
			{
				this.m_InputTimer += Time.unscaledDeltaTime;
			}
		}
	}

	protected override void OnDisposed()
	{
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
