using System;
using UnityEngine;

public class BookContent : ActionEventContent<BookContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable BookContent_Fishing;

		public Interactable BookContent_Cooking;

		public Interactable BookContent_Spaces;

		public Interactable BookContent_Mug;
	}

	private Book m_Book;

	private UIBookDataVO m_DataVO;

	protected override void OnInitialize()
	{
		this.m_Book = (Book)base.Connectable;
		this.InitializeBookData();
		if (this.m_Book.Data != null && base.m_Properties != null && base.m_Properties.Length != 0)
		{
			Properties properties;
			properties = base.m_Properties[0];
			properties.ActionEvent = this.GenerateBook(properties);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
	}

	private void InitializeBookData()
	{
		string text;
		text = this.m_Book.Data.ID.ToString();
		if (this.m_Book.Data.BookType == BookType.MugAndTheMaiden)
		{
			this.m_DataVO = new UIBookDataVO(bookPages: new string[9]
			{
				TextUtility.GetKey(text + "_P01"),
				TextUtility.GetKey(text + "_P02"),
				TextUtility.GetKey(text + "_P03"),
				TextUtility.GetKey(text + "_P04"),
				TextUtility.GetKey(text + "_P05"),
				TextUtility.GetKey(text + "_P06"),
				TextUtility.GetKey(text + "_P07"),
				TextUtility.GetKey(text + "_P08"),
				TextUtility.GetKey(text + "_P09")
			}, book: this.m_Book, bookType: this.m_Book.Data.BookType);
		}
		else
		{
			this.m_DataVO = new UIBookDataVO(this.m_Book, this.m_Book.Data.BookType, TextUtility.GetKey(text));
		}
	}

	private Interactable GenerateBook(Properties properties)
	{
		return this.m_DataVO.BookType switch
		{
			BookType.FishGuide => this.CreateBook(properties.BookContent_Fishing), 
			BookType.CookingForEveryday => this.CreateBook(properties.BookContent_Cooking), 
			BookType.TheoryofSpaces => this.CreateBook(properties.BookContent_Spaces), 
			BookType.MugAndTheMaiden => this.CreateBook(properties.BookContent_Mug), 
			_ => null, 
		};
	}

	private Interactable CreateBook(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnActivate()
	{
		this.InitializeBookData();
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_EXIT.ToString()));
		GameManager.Instance.ShowBook(this.m_DataVO);
	}

	protected override void OnDeactivate()
	{
		base.DeactivateComplete();
	}

	protected override void OnDisposed()
	{
		this.m_Book = null;
		this.m_DataVO = null;
		base.OnDisposed();
	}
}
