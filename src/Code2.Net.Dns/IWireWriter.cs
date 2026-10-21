using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Code2.Net.Dns;

public interface IWireWriter
{
	int Length { get; }
	int Position { get; }
	Dictionary<string, int> SuffixCache { get; }
	byte[] ToArray();
	bool TryWriteName(string? name, [NotNullWhen(false)] out string? error);
	void Write(byte value);
	void Write(byte[] source);
	void WriteUInt128(UInt128 value);
	void WriteUInt16(ushort value);
	void WriteUInt32(uint value);
}