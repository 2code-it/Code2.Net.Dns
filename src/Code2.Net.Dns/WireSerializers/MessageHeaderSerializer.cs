using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;

namespace Code2.Net.Dns.WireSerializers;

public class MessageHeaderSerializer : IWireSerializer<MessageHeader>
{
	private static IWireSerializer<MessageHeader>? _instance;
	public static IWireSerializer<MessageHeader> Instance => _instance ?? (_instance = new MessageHeaderSerializer());
	public IResult<MessageHeader> Deserialize(IWireReader reader)
	{
		if (!reader.CanRead(12)) return Result.Fail<MessageHeader>("Failed to read header, out of data range.");
		var id = reader.ReadUInt16();
		var flags = reader.ReadUInt16();

		var header = new MessageHeader
		{
			Id = id,
			IsResponse = GetBit(flags, 15),
			OperationCode = GetBits(flags, 11, 4),
			IsAuthoritativeAnswer = GetBit(flags, 10),
			IsTruncated = GetBit(flags, 9),
			IsRecursionDesired = GetBit(flags, 8),
			IsRecursionAvailable = GetBit(flags, 7),
			ResponseCode = GetBits(flags, 0, 4),
			QuestionCount = reader.ReadUInt16(),
			AnswerRecordCount = reader.ReadUInt16(),
			NameServerRecordCount = reader.ReadUInt16(),
			AdditionalRecordCount = reader.ReadUInt16()
		};

		return Result.Ok(header);
	}

	public IResult Serialize(IWireWriter writer, MessageHeader value)
	{
		ushort flags = 0;
		SetBit(ref flags, value.IsResponse, 15);
		SetBits(ref flags, value.OperationCode, 11, 4);
		SetBit(ref flags, value.IsAuthoritativeAnswer, 10);
		SetBit(ref flags, value.IsTruncated, 9);
		SetBit(ref flags, value.IsRecursionDesired, 8);
		SetBit(ref flags, value.IsRecursionAvailable, 7);
		SetBits(ref flags, value.ResponseCode, 0, 4);

		writer.WriteUInt16(value.Id);
		writer.WriteUInt16(flags);
		writer.WriteUInt16(value.QuestionCount);
		writer.WriteUInt16(value.AnswerRecordCount);
		writer.WriteUInt16(value.NameServerRecordCount);
		writer.WriteUInt16(value.AdditionalRecordCount);

		return Result.Ok();
	}

	private static byte GetBits(ushort source, byte start, byte size = 1)
	{
		ushort mask = (ushort)((1 << size) - 1);
		return (byte)((source >> start) & mask);
	}

	private static bool GetBit(ushort source, byte position)
		=> GetBits(source, position, 1) == 1;

	private static void SetBits(ref ushort source, ushort value, byte start, byte size = 1)
	{
		ushort mask = (ushort)((1 << size) - 1);
		source &= (ushort)~(mask << start);
		source |= (ushort)((value & mask) << start);
	}

	private static void SetBit(ref ushort source, bool value, byte position)
		=> SetBits(ref source, (ushort)(value ? 1 : 0), position, 1);
}
