using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;
using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers;
using NSubstitute;
using System;
using Xunit;

namespace Code2.Net.Dns.Tests.Serializers;

public class ResourceRecordSerializerTests
{
	[Fact]
	public void Deserialize_When_ReadNameFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);
		var serializer = new ResourceRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadTypeFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(false);
		var serializer = new ResourceRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadClassFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		int i = 0;
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => i++<1);
		var serializer = new ResourceRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadLengthFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		int i = 0;
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => i++ < 2);
		var serializer = new ResourceRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadTtlFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(true);
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(false);
		var serializer = new ResourceRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_FormatDeserializeFailed_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(true);
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(true);
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(true);
		formatManager.Deserialize(Arg.Any<ushort>(), Arg.Any<IWireReader>()).Returns(Result.Fail<object>());
		var serializer = new ResourceRecordSerializer(formatManager);

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_NotHasFormatSerializer_Expect_RecordUnknown()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(true);
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(true);
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(false);
		var serializer = new ResourceRecordSerializer(formatManager);

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}
		var typeExpected = typeof(UnknownRecord);
		var typeActual = result.Value.Data.GetType();

		Assert.Equal(typeExpected, typeActual);
	}

	[Fact]
	public void Deserialize_When_HasFormatSerializer_Expect_RecordTypeForTypeCode()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(true);
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(true);
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(true);
		formatManager.Deserialize(Arg.Any<ushort>(), Arg.Any<IWireReader>()).Returns(Result.Ok<object>(new ARecord()));
		var serializer = new ResourceRecordSerializer(formatManager);

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}
		var typeExpected = typeof(ARecord);
		var typeActual = result.Value.Data.GetType();

		Assert.Equal(typeExpected, typeActual);
	}

	[Fact]
	public void Serialize_When_PropertiesGiven_Expect_PropertiesSet()
	{
		var writer = Substitute.For<IWireWriter>();
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(false);
		string name = string.Empty;
		ushort[] u16Values = new ushort[3];
		uint ttl = 0;
		ushort dataLength = 0;
		int i = 0;
		writer.TryWriteName(Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { name = (string)x[0]; return true; });
		writer.When(x => x.WriteUInt16(Arg.Any<ushort>())).Do(x => u16Values[i++] = x.Arg<ushort>());
		writer.When(x => x.WriteUInt32(Arg.Any<uint>())).Do(x => ttl = x.Arg<uint>());
		writer.When(x => x.Write(Arg.Any<byte[]>())).Do(x => dataLength = (ushort)x.Arg<byte[]>().Length);
		var serializer = new ResourceRecordSerializer(formatManager);
		var record = new ResourceRecord() { Name = "test-it.com", Type = Constants.RecordTypes.A, Class = Constants.RecordClasses.ANY, TimeToLiveInSeconds = 60 };

		serializer.Serialize(writer, record);

		Assert.Equal(record.Name, name);
		Assert.Equal(record.Type, u16Values[0]);
		Assert.Equal(record.Class, u16Values[1]);
		Assert.Equal(record.DataLength, dataLength);
		Assert.Equal(record.TimeToLiveInSeconds, ttl);
	}

	[Fact]
	public void Serialize_When_FormatSerializeFails_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(true);
		formatManager.Serialize(Arg.Any<ushort>(), Arg.Any<IWireWriter>(), Arg.Any<object>()).Returns(Result.Fail());
		var serializer = new ResourceRecordSerializer(formatManager);
		var record = new ResourceRecord();

		var result = serializer.Serialize(writer, record);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_FormatNotUnknownTypeAndNoFormatSerializer_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(false);
		var serializer = new ResourceRecordSerializer(formatManager);
		var record = new ResourceRecord() { Type = 1, Data = new ARecord() };

		var result = serializer.Serialize(writer, record);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_FormatNotUnknownTypeAndFormatSerializerTypeMismatch_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		var formatManager = Substitute.For<IFormatSerializerManager>();
		formatManager.HasSerializer(Arg.Any<ushort>()).Returns(true);
		formatManager.IsMatch(Arg.Any<ushort>(), Arg.Any<Type>()).Returns(false);
		var serializer = new ResourceRecordSerializer(formatManager);
		var record = new ResourceRecord() { Type = 1, Data = new ARecord() };

		var result = serializer.Serialize(writer, record);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_FormatIsUnknownType_Expect_UnknownRecordValueWritten()
	{
		var writer = Substitute.For<IWireWriter>();
		writer.TryWriteName(Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		var serializer = new ResourceRecordSerializer();
		var format = new UnknownRecord { Value = new byte[10] };
		var record = new ResourceRecord() { Data = format };
		var dataLength = 0;
		writer.When(x => x.Write(Arg.Any<byte[]>())).Do(x => dataLength = x.Arg<byte[]>().Length);

		var result = serializer.Serialize(writer, record);

		Assert.Equal(format.Value.Length, dataLength);
	}
}
