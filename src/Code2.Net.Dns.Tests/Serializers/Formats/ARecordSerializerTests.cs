using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;
using Xunit;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class ARecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		uint receivedAddress = 0;
		uint address = 11223344;
		var writer = Substitute.For<IWireWriter>();
		writer.When(x => x.WriteUInt32(Arg.Any<uint>())).Do(x => receivedAddress = (uint)x[0]);
		var record = new ARecord() { Address = address };
		var serializer = new ARecordSerializer();

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
		uint address = 1223334444;
		var reader = Substitute.For<IWireReader>();
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(x => { x[0] = address; return true; });
		var serializer = new ARecordSerializer();

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
		reader.TryReadUInt32(out Arg.Any<uint>()).Returns(false);
		var serializer = new ARecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}
}
