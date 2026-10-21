using System;

namespace Code2.Net.Dns.Internal;

internal class BinaryPrimitives : IBinaryPrimitives
{
	public ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source)
		=> System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(source);
	public uint ReadUInt32BigEndian(ReadOnlySpan<byte> source)
		=> System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(source);
	public UInt128 ReadUInt128BigEndian(ReadOnlySpan<byte> source)
		=> System.Buffers.Binary.BinaryPrimitives.ReadUInt128BigEndian(source);

	public void WriteUInt16BigEndian(Span<byte> destination, ushort value)
		=> System.Buffers.Binary.BinaryPrimitives.WriteUInt16BigEndian(destination, value);
	public void WriteUInt32BigEndian(Span<byte> destination, uint value)
		=> System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(destination, value);
	public void WriteUInt128BigEndian(Span<byte> destination, UInt128 value)
		=> System.Buffers.Binary.BinaryPrimitives.WriteUInt128BigEndian(destination, value);


}
