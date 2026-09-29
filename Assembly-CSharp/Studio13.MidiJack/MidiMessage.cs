namespace Studio13.MidiJack;

public struct MidiMessage
{
	public uint source;

	public byte status;

	public byte data1;

	public byte data2;

	public MidiMessage(ulong data)
	{
		unchecked
		{
			this.source = (uint)(data & 0xFFFFFFFFu);
			this.status = (byte)((data >> 32) & 0xFF);
			this.data1 = (byte)((data >> 40) & 0xFF);
			this.data2 = (byte)((data >> 48) & 0xFF);
		}
	}

	public override string ToString()
	{
		return $"s({this.status:X2}) d({this.data1:X2},{this.data2:X2}) from {this.source:X8}";
	}
}
