using Code2.Net.Dns.WireSerializers;
using NSubstitute;

namespace Code2.Net.Dns.Tests.Serializers;

public class QuestionSerializerTests()
{
	[Fact]
	public void Deserialize_When_ReadNameFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);
		var serializer = new QuestionSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadTypeFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(false);
		var serializer = new QuestionSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadClassFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(true);
		int i = 0;
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => i++ == 0 ? true : false);
		var serializer = new QuestionSerializer();

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ReadProperties_Expect_PropertiesSet()
	{
		string name = "test.name.com";
		ushort type = 1;
		ushort cls = 12;
		var reader = Substitute.For<IWireReader>();
		reader.TryReadName(out Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { x[0] = name; return true; });
		ushort i = 0;
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => { x[0] = i++ == 0 ? type : cls; return true; });
		var serializer = new QuestionSerializer();

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(name, result.Value.Name);
		Assert.Equal(type, result.Value.Type);
		Assert.Equal(cls, result.Value.Class);
	}
}
