using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;
using Code2.Net.Dns.WireSerializers;
using NSubstitute;
using System.Linq;
using Xunit;


namespace Code2.Net.Dns.Tests.Serializers;

public class MessageSerializerTests
{
	public MessageSerializerTests()
	{
		_headerSerializer = Substitute.For<IWireSerializer<MessageHeader>>();
		_questionSerializer = Substitute.For<IWireSerializer<Question>>();
		_recordSerializer = Substitute.For<IWireSerializer<ResourceRecord>>();
	}

	private readonly IWireSerializer<MessageHeader> _headerSerializer;
	private readonly IWireSerializer<Question> _questionSerializer;
	private readonly IWireSerializer<ResourceRecord> _recordSerializer;


	[Fact]
	public void Deserialize_When_HeaderDeserializeFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		_headerSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Fail<MessageHeader>());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_QuestionDeserializeFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		_headerSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Ok(new MessageHeader() { QuestionCount = 1 }));
		_questionSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Fail<Question>());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Deserialize_When_ResourceRecordDeserializeFails_Expect_ResultFail()
	{
		var reader = Substitute.For<IWireReader>();
		_headerSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Ok(new MessageHeader() { AnswerRecordCount = 1 }));
		_recordSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Fail<ResourceRecord>());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Deserialize(reader);

		Assert.False(result.IsSuccess);
	}

	[Theory]
	[InlineData(1, 0, 0, 0)]
	[InlineData(1, 4, 1, 3)]
	[InlineData(1, 1, 1, 10)]
	public void Deserialize_When_HeaderRecordCountSet_Expect_RecordDeserializeCount(ushort questionCount, ushort answerCount, ushort serverRecordCount, ushort additionalRecordCount)
	{
		var reader = Substitute.For<IWireReader>();
		var header = new MessageHeader
		{
			QuestionCount = questionCount,
			AnswerRecordCount = answerCount,
			NameServerRecordCount = serverRecordCount,
			AdditionalRecordCount = additionalRecordCount
		};
		_headerSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Ok(header));
		_questionSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Ok(new Question()));
		_recordSerializer.Deserialize(Arg.Any<IWireReader>()).Returns(Result.Ok(new ResourceRecord()));
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Deserialize(reader);

		if (!result.IsSuccess)
		{
			Assert.Fail(result.Message);
			return;
		}

		Assert.Equal(questionCount, result.Value.Questions.Count);
		Assert.Equal(answerCount, result.Value.Answers.Count);
		Assert.Equal(serverRecordCount, result.Value.Authority.Count);
		Assert.Equal(additionalRecordCount, result.Value.Additional.Count);
	}

	[Fact]
	public void Serialize_When_HeaderSerializeFails_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		_headerSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<MessageHeader>()).Returns(Result.Fail());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Serialize(writer, new Message());

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_QuestionSerializeFails_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		_headerSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<MessageHeader>()).Returns(Result.Ok());
		_questionSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<Question>()).Returns(Result.Fail());
		var message = new Message();
		message.Header.QuestionCount = 1;
		message.Questions.Add(new());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Serialize(writer, message);

		Assert.False(result.IsSuccess);
	}

	[Fact]
	public void Serialize_When_ResourceRecordSerializeFails_Expect_ResultFail()
	{
		var writer = Substitute.For<IWireWriter>();
		_headerSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<MessageHeader>()).Returns(Result.Ok());
		_recordSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<ResourceRecord>()).Returns(Result.Fail());
		var message = new Message();
		message.Header.AnswerRecordCount = 1;
		message.Answers.Add(new());
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Serialize(writer, message);

		Assert.False(result.IsSuccess);
	}

	[Theory]
	[InlineData(1, 0, 0, 0)]
	[InlineData(1, 4, 1, 3)]
	[InlineData(1, 1, 1, 10)]
	public void Serialize_When_HeaderRecordCountSet_Expect_RecordSerializeCount(ushort questionCount, ushort answerCount, ushort serverRecordCount, ushort additionalRecordCount)
	{
		var writer = Substitute.For<IWireWriter>();
		_headerSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<MessageHeader>()).Returns(Result.Ok());
		_questionSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<Question>()).Returns(Result.Ok());
		_recordSerializer.Serialize(Arg.Any<IWireWriter>(), Arg.Any<ResourceRecord>()).Returns(Result.Ok());
		var message = new Message();
		message.Header.QuestionCount = questionCount;
		message.Header.AnswerRecordCount = answerCount;
		message.Header.NameServerRecordCount = serverRecordCount;
		message.Header.AdditionalRecordCount = additionalRecordCount;
		message.Questions.AddRange(Enumerable.Range(0, questionCount).Select(x => new Question()));
		message.Answers.AddRange(Enumerable.Range(0, answerCount).Select(x => new ResourceRecord()));
		message.Authority.AddRange(Enumerable.Range(0, serverRecordCount).Select(x => new ResourceRecord()));
		message.Additional.AddRange(Enumerable.Range(0, additionalRecordCount).Select(x => new ResourceRecord()));
		var serializer = new MessageSerializer(_headerSerializer, _questionSerializer, _recordSerializer);

		var result = serializer.Serialize(writer, message);

		Assert.True(result.IsSuccess);
	}
}
