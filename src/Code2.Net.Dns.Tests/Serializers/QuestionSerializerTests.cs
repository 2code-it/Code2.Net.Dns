using Code2.Net.Dns.Models;
using Code2.Net.Dns.WireSerializers;
using NSubstitute;
using Xunit;

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
		reader.TryReadUInt16(out Arg.Any<ushort>()).Returns(x => i++ == 0);
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

	[Fact]
	public void Serialize_When_TryWriteNameFails_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		var serializer = new QuestionSerializer();
		writer.TryWriteName(Arg.Any<string?>(), out Arg.Any<string?>()).Returns(false);

		var result = serializer.Serialize(writer, new Question());

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_ValuesSet_Expect_ValuesWritten()
	{
		var writer = Substitute.For<IWireWriter>();
		var serializer = new QuestionSerializer();
		var question = new Question() { Name = "test-it.com", Type = Constants.RecordTypes.A, Class = Constants.RecordClasses.ANY };
		string? name = null;
		ushort[] u16Values = new ushort[2];
		int i = 0;
		writer.TryWriteName(Arg.Any<string?>(), out Arg.Any<string?>()).Returns(x => { name = (string?)x[0]; return true; });
		writer.When(x => x.WriteUInt16(Arg.Any<ushort>())).Do(x => u16Values[i++] = x.Arg<ushort>());

		var result = serializer.Serialize(writer, question);

		Assert.Equal(question.Name, name);
		Assert.Equal(question.Type, u16Values[0]);
		Assert.Equal(question.Class, u16Values[1]);
	}
}
