public class UIBookDataVO
{
	public Book Book;

	public BookType BookType;

	public string BookLabel;

	public string[] BookPages;

	public UIBookDataVO(Book book, BookType bookType, string bookLabel)
	{
		this.Book = book;
		this.BookType = bookType;
		this.BookLabel = bookLabel;
	}

	public UIBookDataVO(Book book, BookType bookType, params string[] bookPages)
	{
		this.Book = book;
		this.BookType = bookType;
		this.BookPages = bookPages;
	}
}
