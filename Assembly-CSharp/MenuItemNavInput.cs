using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemNavInput : JMonoBehaviour
{
	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI m_InputLbl;

	[SerializeField]
	private Localize m_InputDescriptionLbl;

	[Header("Images")]
	[SerializeField]
	private Image m_CircleImg;

	private UINavInputDataVO m_DataVO;

	private RectTransform m_RectTransform;

	public RectTransform rectTransform
	{
		get
		{
			if (this.m_RectTransform == null)
			{
				this.m_RectTransform = base.GetComponent<RectTransform>();
			}
			return this.m_RectTransform;
		}
	}

	public void Init(UINavInputDataVO vo)
	{
		this.m_DataVO = vo;
		this.m_InputLbl.text = this.m_DataVO.Input;
		this.m_InputDescriptionLbl.SetTerm(this.m_DataVO.Description);
	}

	protected override void OnDisposed()
	{
		base.OnDisposed();
	}
}
