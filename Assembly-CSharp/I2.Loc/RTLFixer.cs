using System;

namespace I2.Loc;

public class RTLFixer
{
	public static string Fix(string str)
	{
		return RTLFixer.Fix(str, showTashkeel: false, useHinduNumbers: true);
	}

	public static string Fix(string str, bool rtl)
	{
		if (rtl)
		{
			return RTLFixer.Fix(str);
		}
		string[] array;
		array = str.Split(' ');
		string text;
		text = "";
		string text2;
		text2 = "";
		string[] array2;
		array2 = array;
		foreach (string text3 in array2)
		{
			if (char.IsLower(text3.ToLower()[unchecked(text3.Length / 2)]))
			{
				text = text + RTLFixer.Fix(text2) + text3 + " ";
				text2 = "";
			}
			else
			{
				text2 = text2 + text3 + " ";
			}
		}
		if (text2 != "")
		{
			text += RTLFixer.Fix(text2);
		}
		return text;
	}

	public static string Fix(string str, bool showTashkeel, bool useHinduNumbers)
	{
		string text;
		text = HindiFixer.Fix(str);
		if (text != str)
		{
			return text;
		}
		RTLFixerTool.showTashkeel = showTashkeel;
		RTLFixerTool.useHinduNumbers = useHinduNumbers;
		if (str.Contains("\n"))
		{
			str = str.Replace("\n", Environment.NewLine);
		}
		if (str.Contains(Environment.NewLine))
		{
			string[] array;
			array = str.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
			if (array.Length == 0)
			{
				return RTLFixerTool.FixLine(str);
			}
			if (array.Length == 1)
			{
				return RTLFixerTool.FixLine(str);
			}
			string text2;
			text2 = RTLFixerTool.FixLine(array[0]);
			int i;
			i = 1;
			if (array.Length > 1)
			{
				for (; i < array.Length; i = unchecked(i + 1))
				{
					text2 = text2 + Environment.NewLine + RTLFixerTool.FixLine(array[i]);
				}
			}
			return text2;
		}
		return RTLFixerTool.FixLine(str);
	}
}
