using System.Threading;
using UnityEngine;

public class TextureScale
{
	public class ThreadData
	{
		public int start;

		public int end;

		public ThreadData(int s, int e)
		{
			this.start = s;
			this.end = e;
		}
	}

	private static Color[] texColors;

	private static Color[] newColors;

	private static int w;

	private static float ratioX;

	private static float ratioY;

	private static int w2;

	private static int finishCount;

	private static Mutex mutex;

	public static void Point(Texture2D tex, int newWidth, int newHeight)
	{
		if (!(tex == null))
		{
			TextureScale.ThreadedScale(tex, newWidth, newHeight, useBilinear: false);
		}
	}

	public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
	{
		if (!(tex == null))
		{
			TextureScale.ThreadedScale(tex, newWidth, newHeight, useBilinear: true);
		}
	}

	private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
	{
		TextureScale.texColors = tex.GetPixels();
		unchecked
		{
			TextureScale.newColors = new Color[newWidth * newHeight];
			if (useBilinear)
			{
				TextureScale.ratioX = 1f / ((float)newWidth / (float)(tex.width - 1));
				TextureScale.ratioY = 1f / ((float)newHeight / (float)(tex.height - 1));
			}
			else
			{
				TextureScale.ratioX = (float)tex.width / (float)newWidth;
				TextureScale.ratioY = (float)tex.height / (float)newHeight;
			}
			TextureScale.w = tex.width;
			TextureScale.w2 = newWidth;
			int num;
			num = Mathf.Min(SystemInfo.processorCount, newHeight);
			int num2;
			num2 = newHeight / num;
			TextureScale.finishCount = 0;
			if (TextureScale.mutex == null)
			{
				TextureScale.mutex = new Mutex(initiallyOwned: false);
			}
			if (num > 1)
			{
				int num3;
				num3 = 0;
				ThreadData parameter;
				for (num3 = 0; num3 < num - 1; num3++)
				{
					parameter = new ThreadData(num2 * num3, num2 * (num3 + 1));
					new Thread(useBilinear ? new ParameterizedThreadStart(TextureScale.BilinearScale) : new ParameterizedThreadStart(TextureScale.PointScale)).Start(parameter);
				}
				parameter = new ThreadData(num2 * num3, newHeight);
				if (useBilinear)
				{
					TextureScale.BilinearScale(parameter);
				}
				else
				{
					TextureScale.PointScale(parameter);
				}
				while (TextureScale.finishCount < num)
				{
					Thread.Sleep(1);
				}
			}
			else
			{
				ThreadData obj;
				obj = new ThreadData(0, newHeight);
				if (useBilinear)
				{
					TextureScale.BilinearScale(obj);
				}
				else
				{
					TextureScale.PointScale(obj);
				}
			}
			tex.Reinitialize(newWidth, newHeight);
			tex.SetPixels(TextureScale.newColors);
			tex.Apply();
			TextureScale.texColors = null;
			TextureScale.newColors = null;
		}
	}

	public static void BilinearScale(object obj)
	{
		ThreadData threadData;
		threadData = (ThreadData)obj;
		unchecked
		{
			for (int i = threadData.start; i < threadData.end; i++)
			{
				int num;
				num = (int)Mathf.Floor((float)i * TextureScale.ratioY);
				int num2;
				num2 = num * TextureScale.w;
				int num3;
				num3 = (num + 1) * TextureScale.w;
				int num4;
				num4 = i * TextureScale.w2;
				for (int j = 0; j < TextureScale.w2; j++)
				{
					int num5;
					num5 = (int)Mathf.Floor((float)j * TextureScale.ratioX);
					float value;
					value = (float)j * TextureScale.ratioX - (float)num5;
					TextureScale.newColors[num4 + j] = TextureScale.ColorLerpUnclamped(TextureScale.ColorLerpUnclamped(TextureScale.texColors[num2 + num5], TextureScale.texColors[num2 + num5 + 1], value), TextureScale.ColorLerpUnclamped(TextureScale.texColors[num3 + num5], TextureScale.texColors[num3 + num5 + 1], value), (float)i * TextureScale.ratioY - (float)num);
				}
			}
			TextureScale.mutex.WaitOne();
			TextureScale.finishCount++;
			TextureScale.mutex.ReleaseMutex();
		}
	}

	public static void PointScale(object obj)
	{
		ThreadData threadData;
		threadData = (ThreadData)obj;
		unchecked
		{
			for (int i = threadData.start; i < threadData.end; i++)
			{
				int num;
				num = (int)(TextureScale.ratioY * (float)i) * TextureScale.w;
				int num2;
				num2 = i * TextureScale.w2;
				for (int j = 0; j < TextureScale.w2; j++)
				{
					TextureScale.newColors[num2 + j] = TextureScale.texColors[(int)((float)num + TextureScale.ratioX * (float)j)];
				}
			}
			TextureScale.mutex.WaitOne();
			TextureScale.finishCount++;
			TextureScale.mutex.ReleaseMutex();
		}
	}

	private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
	{
		return new Color(c1.r + (c2.r - c1.r) * value, c1.g + (c2.g - c1.g) * value, c1.b + (c2.b - c1.b) * value, c1.a + (c2.a - c1.a) * value);
	}
}
