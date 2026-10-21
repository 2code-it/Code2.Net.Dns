using System;

namespace Code2.Net.Dns.Internal;

internal interface IBinaryPrimitives
{
	ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source);
	uint ReadUInt32BigEndian(ReadOnlySpan<byte> source);
	UInt128 ReadUInt128BigEndian(ReadOnlySpan<byte> source);

	void WriteUInt16BigEndian(Span<byte> destination, ushort value);
	void WriteUInt32BigEndian(Span<byte> destination, uint value);
	void WriteUInt128BigEndian(Span<byte> destination, UInt128 value);
}
