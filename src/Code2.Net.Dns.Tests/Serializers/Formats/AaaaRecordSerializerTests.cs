using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;
using System;
using Xunit;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class AaaaRecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		UInt128 receivedAddress = 0;
		UInt128 address = 11223344;
		var writer = Substitute.For<IWireWriter>();
		writer.When(x => x.WriteUInt128(Arg.Any<UInt128>())).Do(x => receivedAddress = (UInt128)x[0]);
		var record = new AaaaRecord() { Address = address };
		var serializer = new AaaaRecordSerializer();

		var result = serializer.Serialize(writer, record);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(address, receivedAddress);
	}

	[Fact]
	public void Deserialize_When_AttributesPresent_Expect_RecordAttributesSet()
	{
		UInt128 address = 1223334444;
		var reader = Substitute.For<IWireReader>();
		reader.TryReadUInt128(out Arg.Any<UInt128>()).Returns(x => { x[0] = address; return true; });
		var serializer = new AaaaRecordSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(address, result.Value.Address);
	}

	[Fact]
	public void Deserialize_When_ReadFailed_Expect_Failure()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadUInt128(out Arg.Any<UInt128>()).Returns(false);
		var serializer = new AaaaRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}
}
