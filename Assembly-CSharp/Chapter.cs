public static class Chapter
{
	public static int GetChapterID(ChapterID id)
	{
		return unchecked((int)id);
	}

	public static string GetChapterString(ChapterID id)
	{
		string result;
		result = "";
		switch (id)
		{
		case ChapterID.Chapter_One:
			result = "CHAPTER_ONE";
			break;
		case ChapterID.Chapter_Two:
			result = "CHAPTER_TWO";
			break;
		case ChapterID.Chapter_Three:
			result = "CHAPTER_THREE";
			break;
		case ChapterID.Chapter_Four:
			result = "CHAPTER_FOUR";
			break;
		case ChapterID.Chapter_Five:
			result = "CHAPTER_FIVE";
			break;
		}
		return result;
	}

	public static string GetChapterName(ChapterID id)
	{
		string result;
		result = "";
		switch (id)
		{
		case ChapterID.Chapter_One:
			result = "CHAPTER_ONE_TITLE";
			break;
		case ChapterID.Chapter_Two:
			result = "CHAPTER_TWO_TITLE";
			break;
		case ChapterID.Chapter_Three:
			result = "CHAPTER_THREE_TITLE";
			break;
		case ChapterID.Chapter_Four:
			result = "CHAPTER_FOUR_TITLE";
			break;
		case ChapterID.Chapter_Five:
			result = "CHAPTER_FIVE_TITLE";
			break;
		}
		return result;
	}
}
