using System;
using System.Linq;
using Xunit;

namespace Code2.Net.Dns.Tests;

public class WireReaderTests
{
	[Theory]
	[InlineData(10, 3, 5, true)]
	[InlineData(10, 0, 11, false)]
	[InlineData(10, 0, 5, true)]
	[InlineData(10, 10, 1, false)]
	public void CanRead_When_DataLengthAndPositionAndSizeAreProvided_Expect_ExpectedResult(int dataLength, int position, int canReadSize, bool expectedResult)
	{
		var data = new byte[dataLength];
		var reader = new WireReader(data) { Position = position };

		var result = reader.CanRead(canReadSize);

		Assert.Equal(expectedResult, result);
	}

	[Fact]
	public void Read_When_BufferSizeEqualsDataSize_Expect_BytesReadEqualData()
	{
		var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		var buffer = new byte[10];
		var reader = new WireReader(data);

		reader.Read(buffer);

		Assert.Equal(data, buffer);
	}


	[Fact]
	public void Read_When_PositionIsOffset_Expect_BytesReadFromPosition()
	{
		var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		var buffer = new byte[5];
		var position = 5;
		var reader = new WireReader(data) { Position = position };

		reader.Read(buffer);

		Assert.Equal(data[position], buffer[0]);
	}

	[Fact]
	public void Read_When_ExceedsLength_Expect_Exception()
	{
		var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		var buffer = new byte[10];
		var position = 5;
		var reader = new WireReader(data) { Position = position };

		Assert.Throws<ArgumentOutOfRangeException>(() => reader.Read(buffer));
	}

	[Theory]
	[InlineData(10, 9, 3, false)]
	[InlineData(10, 0, 11, false)]
	[InlineData(10, 0, 5, true)]
	public void TryRead_When_ExceedsLength_Expect_False(int dataLength, int position, int readLength, bool expectedResult)
	{
		var data = new byte[dataLength];
		var buffer = new byte[readLength];
		var reader = new WireReader(data) { Position = position };

		var result = reader.TryRead(buffer);

		Assert.Equal(expectedResult, result);
	}

	[Fact]
	public void ReadUInt16_When_ExceedsLength_Expect_Exception()
	{
		var data = new byte[] { 0, 1 };
		var position = 2;
		var reader = new WireReader(data) { Position = position };

		Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadUInt16());
	}

	[Fact]
	public void ReadUInt16_When_BytesSet_Expect_UInt16()
	{
		ushort value = 12;
		var data = new byte[] { (byte)(value >> 8), (byte)value };
		var reader = new WireReader(data);

		ushort a = reader.ReadUInt16();

		Assert.Equal(value, a);
	}

	[Fact]
	public void ReadUInt32_When_ExceedsLength_Expect_Exception()
	{
		var data = new byte[] { 0, 1, 1, 1 };
		var position = 2;
		var reader = new WireReader(data) { Position = position };

		Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadUInt32());
	}

	[Fact]
	public void ReadUInt32_When_BytesSet_Expect_UInt32()
	{
		uint value = 34456;
		var data = Enumerable.Range(1, 4).Select(x => (byte)(value >> 32 - x * 8)).ToArray();
		var reader = new WireReader(data);

		uint a = reader.ReadUInt32();

		Assert.Equal(value, a);
	}

	[Fact]
	public void ReadUInt128_When_ExceedsLength_Expect_Exception()
	{
		var data = new byte[] { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 };
		var position = 2;
		var reader = new WireReader(data) { Position = position };

		Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadUInt128());
	}

	[Fact]
	public void ReadUInt128_When_BytesSet_Expect_UInt128()
	{
		UInt128 value = 1223334444;
		var data = Enumerable.Range(1, 16).Select(x => (byte)(value >> 128 - x * 8)).ToArray();
		var reader = new WireReader(data);

		UInt128 a = reader.ReadUInt128();

		Assert.Equal(value, a);
	}

	[Fact]
	public void TryReadName_When_NameSet_Expect_Name()
	{
		var name = "www.test-it.com";
		var data = GetLabelBytes(name);
		var reader = new WireReader(data);

		_ = reader.TryReadName(out var value, out _);

		Assert.Equal(name, value);
	}

	[Fact]
	public void TryReadName_When_LabelToBig_Expect_Error()
	{
		var name = "www.test-it.com".PadLeft(100, 'w');
		var data = GetLabelBytes(name);
		var reader = new WireReader(data);

		var result = reader.TryReadName(out _, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	[Fact]
	public void TryReadName_When_PointerInvalid_Expect_Error()
	{
		var name = "www.test-it.com";
		var data = GetLabelBytes(name);
		data = [.. data, .. new byte[] { 0b1100_0000, 0b0000_0001 }];
		var reader = new WireReader(data);

		var result = reader.TryReadName(out _, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	private static byte[] GetLabelBytes(string name)
		=> [.. name.Split('.').SelectMany(x => new byte[] { (byte)x.Length }.Concat(x.Select(c => (byte)c)))];
}
