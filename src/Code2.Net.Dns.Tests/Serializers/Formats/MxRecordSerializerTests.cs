using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class MxRecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		string? receivedExchange = null;
		ushort receivedPreference = 0;
		string exchange = "test-name.com";
		ushort preference = 10;
		var writer = Substitute.For<IWireWriter>();
		writer.TryWriteName(Arg.Any<string>(), out Arg.Any<string?>()).Returns(x => { receivedExchange = (string)x[0]; return true; });
		writer.When(x => x.WriteUInt16(Arg.Any<ushort>())).Do(x => receivedPreference = (ushort)x[0]);
		var record = new MxRecord() { Preference = preference, Exchange = exchange };
		var serializer = new MxRecordSerializer();

		var result = serializer.Serialize(writer, record);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(exchange, receivedExchange);
		Assert.Equal(preference, receivedPreference);
	}

	[Fact]
	public void Deserialize_When_AttributesPresent_Expect_RecordAttributesSet()
	{
		string exchange = "test-name.com";
		ushort preference = 10;
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { x[0] = exchange; return true; });
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => { x[0] = preference; return true; });
		var serializer = new MxRecordSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(preference, result.Value.Preference);
		Assert.Equal(exchange, result.Value.Exchange);
	}

	[Fact]
	public void Deserialize_When_ReadFailed_Expect_Failure()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);
		var serializer = new MxRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}
}
