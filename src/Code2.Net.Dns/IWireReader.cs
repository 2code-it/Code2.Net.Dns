using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Code2.Net.Dns;

public interface IWireReader
{
	int Position { get; set; }
	int Length { get; }
	bool CanRead(int size);
	Dictionary<int, string> PointerCache { get; }
	IWireReader Read(int size);
	int Read();
	void Read(byte[] buffer);
	ushort ReadUInt16();
	uint ReadUInt32();
	UInt128 ReadUInt128();
	bool TryRead(byte[] buffer);
	bool TryReadUInt16(out ushort value);
	bool TryReadUInt32(out uint value);
	bool TryReadUInt128(out UInt128 value);
	bool TryReadName([NotNullWhen(true)] out string? name, [NotNullWhen(false)] out string? error);
}