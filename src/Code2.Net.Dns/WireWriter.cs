using Code2.Net.Dns.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Code2.Net.Dns;

public class WireWriter : IWireWriter
{
	private readonly IBinaryPrimitives _binary;
	private readonly int _bufferChunkSize;

	public WireWriter(int bufferChunkSize = 64) : this(bufferChunkSize, new Internal.BinaryPrimitives()) { }
	internal WireWriter(int bufferChunkSize, IBinaryPrimitives binary)
	{
		_binary = binary;
		_bufferChunkSize = bufferChunkSize;
		_buffer = new byte[bufferChunkSize];
		Position = 0;
	}

	private byte[] _buffer;

	public int Position { get; private set; }
	public Dictionary<string, int> SuffixCache { get; private set; } = [];
	public int Length => _buffer.Length;


	private void EnsureCapacity(int additional)
	{
		int required = Position + additional;
		if (required <= _buffer.Length) return;

		int newSize = _buffer.Length + _bufferChunkSize;
		while (newSize < required)
			newSize += _bufferChunkSize;

		Array.Resize(ref _buffer, newSize);
	}

	public void Write(byte value)
	{
		EnsureCapacity(1);
		_buffer[Position++] = value;
	}

	public void Write(byte[] source)
	{
		EnsureCapacity(source.Length);
		source.CopyTo(_buffer.AsSpan(Position));
		Position += source.Length;
	}

	public void WriteUInt16(ushort value)
	{
		EnsureCapacity(2);
		_binary.WriteUInt16BigEndian(_buffer.AsSpan(Position), value);
		Position += 2;
	}

	public void WriteUInt32(uint value)
	{
		EnsureCapacity(4);
		_binary.WriteUInt32BigEndian(_buffer.AsSpan(Position), value);
		Position += 4;
	}

	public void WriteUInt128(UInt128 value)
	{
		EnsureCapacity(16);
		_binary.WriteUInt128BigEndian(_buffer.AsSpan(Position), value);
		Position += 16;
	}

	public bool TryWriteName(string? name, [NotNullWhen(false)] out string? error)
	{
		error = null;
		if (string.IsNullOrWhiteSpace(name))
		{
			error = "Name cannot be null or empty";
			return false;
		}

		if (name.Length > 255)
		{
			error = "Name cannot be longer than 255 characters";
			return false;
		}

		var suffix = name;

		while (true)
		{
			if (SuffixCache.TryGetValue(suffix, out int pointerPos))
			{
				Write((byte)(0b1100_0000 | (pointerPos >> 8)));
				Write((byte)(pointerPos & 0b1111_1111));
				break;
			}

			SuffixCache[suffix] = Position;

			var parts = suffix.Split('.', 2);
			if (parts[0].Length > 63)
			{
				error = "Label cannot be larger than 63 characters";
				return false;
			}
			Write([(byte)parts[0].Length, .. Encoding.ASCII.GetBytes(parts[0])]);

			if (parts.Length == 1)
			{
				Write(0);
				break;
			}

			suffix = parts[1];
		}

		return true;
	}

	public byte[] ToArray()
	{
		return _buffer.AsSpan(0, Position).ToArray();
	}

}
