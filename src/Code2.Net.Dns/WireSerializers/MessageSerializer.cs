using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;
using System.Collections.Generic;

namespace Code2.Net.Dns.WireSerializers;

public class MessageSerializer : IWireSerializer<Message>
{
	private readonly IWireSerializer<MessageHeader> _headerSerializer;
	private readonly IWireSerializer<Question> _questionSerializer;
	private readonly IWireSerializer<ResourceRecord> _recordSerializer;

	public MessageSerializer() : this(MessageHeaderSerializer.Instance, QuestionSerializer.Instance, ResourceRecordSerializer.Instance) { }
	public MessageSerializer(IWireSerializer<MessageHeader> headerSerializer, IWireSerializer<Question> questionSerializer, IWireSerializer<ResourceRecord> recordSerializer)
	{
		_headerSerializer = headerSerializer;
		_questionSerializer = questionSerializer;
		_recordSerializer = recordSerializer;
	}

	private static IWireSerializer<Message>? _instance;
	public static IWireSerializer<Message> Instance => _instance ?? (_instance = new MessageSerializer());

	public IResult<Message> Deserialize(IWireReader reader)
	{
		var message = new Message();

		var headerResult = _headerSerializer.Deserialize(reader);
		if (!headerResult.IsSuccess) return Result.Fail<Message>(headerResult);
		message.Header = headerResult.Value;

		var result = AddToList(message.Questions, message.Header.QuestionCount, _questionSerializer, reader);
		if (!result.IsSuccess) return Result.Fail<Message>(result);

		result = AddToList(message.Answers, message.Header.AnswerRecordCount, _recordSerializer, reader);
		if (!result.IsSuccess) return Result.Fail<Message>(result);
		result = AddToList(message.Authority, message.Header.NameServerRecordCount, _recordSerializer, reader);
		if (!result.IsSuccess) return Result.Fail<Message>(result);
		result = AddToList(message.Additional, message.Header.AdditionalRecordCount, _recordSerializer, reader);
		if (!result.IsSuccess) return Result.Fail<Message>(result);

		return Result.Ok(message);
	}

	public IResult Serialize(IWireWriter writer, Message value)
	{

		var result = _headerSerializer.Serialize(writer, value.Header);
		if (!result.IsSuccess) return Result.Fail(result);

		result = WriteList(value.Questions, value.Header.QuestionCount, _questionSerializer, writer);
		if (!result.IsSuccess) return result;

		result = WriteList(value.Answers, value.Header.AnswerRecordCount, _recordSerializer, writer);
		if (!result.IsSuccess) return result;
		result = WriteList(value.Authority, value.Header.NameServerRecordCount, _recordSerializer, writer);
		if (!result.IsSuccess) return result;
		return WriteList(value.Additional, value.Header.AdditionalRecordCount, _recordSerializer, writer);
	}

	private static IResult AddToList<T>(List<T> list, int count, IWireSerializer<T> serializer, IWireReader reader)
	{
		for (var i = 0; i < count; i++)
		{
			var result = serializer.Deserialize(reader);
			if (!result.IsSuccess) return result;
			list.Add(result.Value);
		}
		return Result.Ok();
	}

	private static IResult WriteList<T>(List<T> list, int count, IWireSerializer<T> serializer, IWireWriter writer)
	{
		for (var i = 0; i < count; i++)
		{
			var result = serializer.Serialize(writer, list[i]);
			if (!result.IsSuccess) return result;
		}
		return Result.Ok();
	}
}
