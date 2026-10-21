using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;

namespace Code2.Net.Dns.WireSerializers;

public class QuestionSerializer : IWireSerializer<Question>
{
	private static IWireSerializer<Question>? _instance;
	public static IWireSerializer<Question> Instance => _instance ?? (_instance = new QuestionSerializer());

	public IResult<Question> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<Question>(error);
		if (!reader.TryReadUInt16(out var type)) return Result.Fail<Question>("Failed to read question type.");
		if (!reader.TryReadUInt16(out var cls)) return Result.Fail<Question>("Failed to read question class.");

		var question = new Question
		{
			Name = name,
			Type = type,
			Class = cls
		};

		return Result.Ok(question);
	}

	public IResult Serialize(IWireWriter writer, Question value)
	{
		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		writer.WriteUInt16(value.Type);
		writer.WriteUInt16(value.Class);
		return Result.Ok();
	}
}
