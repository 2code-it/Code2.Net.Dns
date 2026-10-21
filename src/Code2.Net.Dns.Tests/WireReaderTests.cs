using System;
using System.Linq;

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
		var reader = new WireReader(data);
		reader.Position = position;

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
		data = data.Concat(new byte[] { 0b1100_0000, 0b0000_0001 }).ToArray();
		var reader = new WireReader(data);

		var result = reader.TryReadName(out _, out var error);

		Assert.False(result);
		Assert.NotNull(error);
	}

	//[Fact]
	//public void ReadHeader_When_DataToReadIsLessThan12_Expect_ResultFail()
	//{
	//	var data = new byte[0];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadHeader();

	//	Assert.False(result.IsSuccess);
	//}

	//[Fact]
	//public void ReadHeader_When_NetworkBytesAreProvided_Expected_HeaderValuesSet()
	//{
	//	byte[] data = [0, 1, 0b10001110, 0b00000011, 0, 1, 0, 1, 0, 0, 0, 0];
	//	var reader = new WireReader(data);
	//	var result = reader.ReadHeader();

	//	if (!result.IsSuccess)
	//	{
	//		Assert.Fail(result.Message);
	//		return;
	//	}

	//	Assert.Equal(1, result.Value.Id);
	//	Assert.True(result.Value.IsResponse);
	//	Assert.Equal(1, result.Value.OperationCode);
	//	Assert.True(result.Value.IsAuthoritativeAnswer);
	//	Assert.True(result.Value.IsTruncated);
	//	Assert.Equal(3, result.Value.ResponseCode);
	//	Assert.Equal(1, result.Value.QuestionCount);
	//	Assert.Equal(1, result.Value.AnswerRecordCount);
	//}

	//[Fact]
	//public void ReadName_When_NameRead_Expect_PopulatedPointerCache()
	//{
	//	string name = "www.test-name.com";
	//	byte[] labelBytes = GetLabelBytes(name);
	//	byte[] data = [.. labelBytes, 0];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadName();
	//	var pointers = reader.GetCachedPointers();

	//	if (!result.IsSuccess)
	//	{
	//		Assert.Fail(result.Message);
	//		return;
	//	}

	//	Assert.Equal(name, result.Value);
	//	Assert.Equal(name, pointers[0].Value);
	//	Assert.Equal(name.Split('.', 2)[1], pointers[1].Value);
	//	Assert.Equal(name.Split('.', 3)[2], pointers[2].Value);
	//	Assert.Equal(0, pointers[0].Position);
	//}

	//[Fact]
	//public void ReadName_When_NameContainsInvalidPointer_Expect_ResultFail()
	//{
	//	byte[] data = [3, 119, 119, 119, 0b1100_0000, 1];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadName();

	//	Assert.False(result.IsSuccess);
	//	Assert.Contains("Invalid label pointer", result.Message);
	//}

	//[Fact]
	//public void ReadName_When_LabelExceedsMaxLength_Expect_ResultFail()
	//{
	//	byte[] data = [64, 119, 119, 119];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadName();

	//	Assert.False(result.IsSuccess);
	//	Assert.Contains("exceeds max length", result.Message);
	//}


	//[Fact]
	//public void ReadQuestion_When_ReadValidQuestion_Expect_ResulSuccess()
	//{
	//	var name = "www.test-name.com";
	//	ushort type = Constants.RecordTypes.A;
	//	ushort cls = Constants.RecordClasses.IN;
	//	byte[] data = [.. GetLabelBytes(name), 0, 0, (byte)type, 0, (byte)cls];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadQuestion();

	//	Assert.True(result.IsSuccess);
	//	Assert.Equal(name, result.Value.Name);
	//	Assert.Equal(type, result.Value.Type);
	//	Assert.Equal(cls, result.Value.Class);
	//}

	//[Fact]
	//public void ReadResourceRecord_When_ReadValidResourceRecord_Expect_ResultSuccess()
	//{
	//	var name = "www.test-name.com";
	//	ushort type = Constants.RecordTypes.A;
	//	ushort cls = Constants.RecordClasses.IN;
	//	uint ttl = 100;
	//	byte[] rrData = [192, 168, 1, 1];
	//	byte[] data = [.. GetLabelBytes(name), 0, 0, (byte)type, 0, (byte)cls, 0, 0, 0, (byte)ttl, 0, (byte)rrData.Length, .. rrData];
	//	var reader = new WireReader(data);

	//	var result = reader.ReadResourceRecord();

	//	Assert.True(result.IsSuccess);
	//	Assert.Equal(name, result.Value.Name);
	//	Assert.Equal(type, result.Value.Type);
	//	Assert.Equal(cls, result.Value.Class);
	//	Assert.Equal(ttl, result.Value.TimeToLiveInSeconds);
	//	Assert.Equal(rrData, result.Value.Data);
	//}

	private byte[] GetLabelBytes(string name)
		=> name.Split('.').SelectMany(x => new byte[] { (byte)x.Length }.Concat(x.Select(c => (byte)c))).ToArray();
}
