using Code2.Net.Dns.Models.Formats;
using Code2.Net.Dns.WireSerializers.Formats;
using NSubstitute;

namespace Code2.Net.Dns.Tests.Serializers.Formats;

public class PtrRecordSerializerTests
{
	[Fact]
	public void Serialize_When_Serialized_Expect_WriterWriteAttributes()
	{
		string? receivedName = null;
		var name = "test-name.com";
		var writer = Substitute.For<IWireWriter>();
		writer.TryWriteName(Arg.Any<string>(), out Arg.Any<string?>()).Returns(x => { receivedName = (string)x[0]; return true; });
		var record = new PtrRecord() { Name = name };
		var serializer = new PtrRecordSerializer();

		var result = serializer.Serialize(writer, record);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(name, receivedName);
	}

	[Fact]
	public void Deserialize_When_AttributesPresent_Expect_RecordAttributesSet()
	{
		string name = "test-name.com";
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { x[0] = name; return true; });
		var serializer = new PtrRecordSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(name, result.Value.Name);
	}

	[Fact]
	public void Deserialize_When_ReadFailed_Expect_Failure()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);
		var serializer = new PtrRecordSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}
}
