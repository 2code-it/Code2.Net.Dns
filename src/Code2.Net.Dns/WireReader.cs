using Code2.Net.Dns.Internal;
using Code2.Net.Dns.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Code2.Net.Dns;

public sealed class WireReader : IWireReader
{
	private readonly IBinaryPrimitives _binary;
	private readonly byte[] _data;
	public WireReader(byte[] data) : this(data, new Internal.BinaryPrimitives()) { }

	internal WireReader(byte[] data, IBinaryPrimitives binary)
	{
		_data = data;
		_binary = binary;
	}

	public Dictionary<int, string> PointerCache { get; private set; } = [];

	public int Position { get; set; }

	public bool CanRead(int size)
		=> Position + size <= _data.Length;

	public int Length => _data.Length;
	public IWireReader Read(int size)
	{
		var reader = new WireReader(_data.AsSpan(Position, size).ToArray());
		Position += size;
		return reader;
	}

	public int Read()
		=> CanRead(1) ? _data[Position++] : -1;

	public void Read(byte[] buffer)
	{
		if (!CanRead(buffer.Length)) throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer length out of range");
		Array.Copy(_data, Position, buffer, 0, buffer.Length);
		Position += buffer.Length;
	}

	public bool TryRead(byte[] buffer)
	{
		if (!CanRead(buffer.Length)) return false;
		Read(buffer);
		return true;
	}

	public ushort ReadUInt16()
	{
		ThrowIfOutOfRange(2);
		var result = _binary.ReadUInt16BigEndian(_data.AsSpan(Position, 2));
		Position += 2;
		return result;
	}

	public uint ReadUInt32()
	{
		ThrowIfOutOfRange(4);
		var result = _binary.ReadUInt32BigEndian(_data.AsSpan(Position, 4));
		Position += 4;
		return result;
	}

	public UInt128 ReadUInt128()
	{
		ThrowIfOutOfRange(16);
		var result = _binary.ReadUInt128BigEndian(_data.AsSpan(Position, 16));
		Position += 16;
		return result;
	}

	public bool TryReadUInt16(out ushort value)
	{
		value = 0;
		if (!CanRead(2)) return false;
		value = ReadUInt16();
		return true;
	}

	public bool TryReadUInt32(out uint value)
	{
		value = 0;
		if (!CanRead(4)) return false;
		value = ReadUInt32();
		return true;
	}

	public bool TryReadUInt128(out UInt128 value)
	{
		value = 0;
		if (!CanRead(16)) return false;
		value = ReadUInt128();
		return true;
	}

	public bool TryReadName([NotNullWhen(true)] out string? name, [NotNullWhen(false)] out string? error)
	{
		name = null;
		error = null;
		var nameSegments = new List<NameSegment>();

		while (Position < Length)
		{
			var lengthOrPointer = Read();
			if (lengthOrPointer == 0) break;

			if ((lengthOrPointer & 0b1100_0000) == 0b1100_0000)
			{
				int pointer = ((lengthOrPointer & 0b0011_1111) << 8) | (byte)Read();
				if (!PointerCache.TryGetValue(pointer, out string? suffix))
				{
					error = $"Invalid label pointer in name: {pointer}";
					return false;
				}
				nameSegments.Add(new NameSegment(pointer, suffix, true));
				break;
			}
			else
			{
				if (lengthOrPointer > 63)
				{
					error = $"Invalid label exceeds max length of 63, length: {lengthOrPointer}";
					return false;
				}
				int position = Position - 1;
				var textBytes = new byte[lengthOrPointer];
				Read(textBytes);
				var text = Encoding.ASCII.GetString(textBytes);
				nameSegments.Add(new NameSegment(position, text, false));
			}
		}

		for (var i = 0; i < nameSegments.Count; i++)
		{
			if (nameSegments[i].IsPointer) break;
			PointerCache[nameSegments[i].Position] = string.Join('.', nameSegments.GetRange(i, nameSegments.Count - i).Select(x => x.Value));
		}

		name = string.Join('.', nameSegments.Select(x => x.Value));
		return true;
	}

	private void ThrowIfOutOfRange(int size)
	{
		if (!CanRead(size)) throw new ArgumentOutOfRangeException();
	}
}