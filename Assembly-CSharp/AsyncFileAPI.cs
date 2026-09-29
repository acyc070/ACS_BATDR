using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public static class AsyncFileAPI
{
	private class State
	{
		private FileStream fStream;

		private byte[] writeArray;

		private byte[] readArray;

		private ManualResetEvent manualEvent;

		public FileStream FStream => this.fStream;

		public byte[] WriteArray => this.writeArray;

		public byte[] ReadArray => this.readArray;

		public ManualResetEvent ManualEvent => this.manualEvent;

		public State(FileStream fStream, byte[] writeArray, ManualResetEvent manualEvent)
		{
			this.fStream = fStream;
			this.writeArray = writeArray;
			this.manualEvent = manualEvent;
			this.readArray = new byte[writeArray.Length];
		}
	}

	public static Thread _thread;

	private static bool _started;

	private static string _fileName;

	private static GameData _data;

	private static ManualResetEvent manualEvent;

	public static event Action OnFileWrittenEvent;

	public static void SaveData(GameData gameData, string fileName)
	{
		if (AsyncFileAPI._started)
		{
			Debug.LogError("SaveData call already in progress on file '" + AsyncFileAPI._fileName + "'...");
			return;
		}
		AsyncFileAPI._fileName = fileName;
		AsyncFileAPI._data = gameData;
		AsyncFileAPI._thread = new Thread(AsyncFileAPI.ThreadedSave);
		AsyncFileAPI._thread.Start();
	}

	private static void ThreadedSave()
	{
		AsyncFileAPI._started = true;
		BinaryFormatter binaryFormatter;
		binaryFormatter = new BinaryFormatter();
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, AsyncFileAPI._data);
			array = memoryStream.ToArray();
		}
		AsyncFileAPI.manualEvent = new ManualResetEvent(initialState: false);
		FileStream fileStream;
		fileStream = new FileStream(AsyncFileAPI._fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, useAsync: true);
		fileStream.BeginWrite(array, 0, array.Length, AsyncFileAPI.EndWriteCallback, new State(fileStream, array, AsyncFileAPI.manualEvent));
		AsyncFileAPI.manualEvent.WaitOne();
		if (AsyncFileAPI.OnFileWrittenEvent != null)
		{
			AsyncFileAPI.OnFileWrittenEvent();
		}
		AsyncFileAPI._started = false;
	}

	private static void EndWriteCallback(IAsyncResult asyncResult)
	{
		State state;
		state = (State)asyncResult.AsyncState;
		FileStream fStream;
		fStream = state.FStream;
		fStream.EndWrite(asyncResult);
		fStream.Position = 0L;
		asyncResult = fStream.BeginRead(state.ReadArray, 0, state.ReadArray.Length, AsyncFileAPI.EndReadCallback, state);
	}

	private static void EndReadCallback(IAsyncResult asyncResult)
	{
		State state;
		state = (State)asyncResult.AsyncState;
		int num;
		num = state.FStream.EndRead(asyncResult);
		int num2;
		num2 = 0;
		while (num2 < num)
		{
			if (state.ReadArray[num2] != state.WriteArray[unchecked(num2++)])
			{
				Debug.LogError("Error writing data.");
				state.FStream.Close();
				return;
			}
		}
		state.FStream.Close();
		state.ManualEvent.Set();
	}
}
