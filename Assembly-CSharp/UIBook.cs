using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBook : UIController
{
	[Header("Images")]
	[SerializeField]
	private Image m_BG;

	[Header("Labels")]
	[SerializeField]
	private TextMeshProUGUI m_BookLabel;

	[Header("Pages")]
	[SerializeField]
	private RectTransform m_Pages;

	[Header("Pages Left")]
	[SerializeField]
	private RectTransform m_PageLeft;

	[SerializeField]
	private TextMeshProUGUI m_PagesLeftActionLabel;

	[SerializeField]
	private RectTransform m_PagesLeftKeyboard;

	[SerializeField]
	private RectTransform m_PagesLeftController;

	[SerializeField]
	private TextMeshProUGUI m_PagesLeftControllerLabel;

	[Header("Pages Right")]
	[SerializeField]
	private RectTransform m_PageRight;

	[SerializeField]
	private TextMeshProUGUI m_PagesRightActionLabel;

	[SerializeField]
	private RectTransform m_PagesRightKeyboard;

	[SerializeField]
	private RectTransform m_PagesRightController;

	[SerializeField]
	private TextMeshProUGUI m_PagesRightControllerLabel;

	private UIBookDataVO m_DataVO;

	private float m_InputDelay = 0.25f;

	private float m_InputTimer;

	private bool m_HasPages;

	private int m_PageIndex;

	public event EventHandler OnPageChange;

	protected override void OnInitialized(object _data)
	{
		this.m_DataVO = (UIBookDataVO)_data;
		this.m_DataVO.Book.Content.gameObject.SetActive(value: false);
		this.m_HasPages = this.m_DataVO.BookType == BookType.MugAndTheMaiden;
		this.m_Pages.gameObject.SetActive(this.m_HasPages);
		if (this.m_HasPages)
		{
			this.SetupPages();
		}
		else
		{
			this.m_BookLabel.text = this.m_DataVO.BookLabel;
		}
	}

	private void Update()
	{
		if (base.IsDisposed)
		{
			return;
		}
		if (this.m_InputTimer >= this.m_InputDelay)
		{
			if (PlayerInput.InteractOnReleased())
			{
				this.m_DataVO.Book.Content.gameObject.SetActive(value: true);
				this.m_DataVO.Book.Content.ForceDeactivate();
				GameManager.Instance.ShowCrosshair();
				GameManager.Instance.HideBook();
			}
			else
			{
				if (!this.m_HasPages)
				{
					return;
				}
				if (GameManager.Instance.HasController)
				{
					if (PlayerInput.ControllerLeftTrigger())
					{
						this.GetPreviousPage();
					}
					else if (PlayerInput.ControllerRightTrigger())
					{
						this.GetNextPage();
					}
				}
				else if (Input.GetKeyDown(KeyCode.A))
				{
					this.GetPreviousPage();
				}
				else if (Input.GetKeyDown(KeyCode.D))
				{
					this.GetNextPage();
				}
			}
		}
		else
		{
			this.m_InputTimer += Time.unscaledDeltaTime;
		}
	}

	private void SetupPages()
	{
		if (!GameManager.Instance.HasController)
		{
			this.m_PagesLeftController.gameObject.SetActive(value: false);
			this.m_PagesRightController.gameObject.SetActive(value: false);
		}
		else
		{
			this.m_PagesLeftKeyboard.gameObject.SetActive(value: false);
			this.m_PagesRightKeyboard.gameObject.SetActive(value: false);
			string text;
			text = "LT";
			string text2;
			text2 = "RT";
			this.m_PagesLeftControllerLabel.text = text;
			this.m_PagesRightControllerLabel.text = text2;
		}
		this.m_PagesLeftActionLabel.text = TextUtility.GetKey("UI_ADDITIONAL_PAGE_PREVIOUS");
		this.m_PagesRightActionLabel.text = TextUtility.GetKey("UI_ADDITIONAL_PAGE_NEXT");
		this.GetPage();
	}

	private void GetNextPage()
	{
		unchecked
		{
			this.m_PageIndex++;
			if (this.m_PageIndex >= this.m_DataVO.BookPages.Length)
			{
				this.m_PageIndex = this.m_DataVO.BookPages.Length - 1;
			}
			else
			{
				this.OnPageChange.Send(this);
			}
			this.GetPage();
		}
	}

	private void GetPreviousPage()
	{
		unchecked
		{
			this.m_PageIndex--;
			if (this.m_PageIndex <= 0)
			{
				this.m_PageIndex = 0;
			}
			else
			{
				this.OnPageChange.Send(this);
			}
			this.GetPage();
		}
	}

	private void GetPage()
	{
		this.m_BookLabel.text = this.m_DataVO.BookPages[this.m_PageIndex];
		this.m_PageLeft.gameObject.SetActive(this.m_PageIndex != 0);
		this.m_PageRight.gameObject.SetActive(this.m_PageIndex < unchecked(this.m_DataVO.BookPages.Length - 1));
	}

	protected override void OnDisposed()
	{
		this.OnPageChange = null;
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
