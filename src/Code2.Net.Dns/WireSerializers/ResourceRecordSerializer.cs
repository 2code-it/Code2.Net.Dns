using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;

namespace Code2.Net.Dns.WireSerializers;

public class ResourceRecordSerializer : IWireSerializer<ResourceRecord>
{

	private static IWireSerializer<ResourceRecord>? _instance;
	public static IWireSerializer<ResourceRecord> Instance => _instance ?? (_instance = new ResourceRecordSerializer());

	public IResult<ResourceRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<ResourceRecord>(error);
		if (!reader.TryReadUInt16(out var typeCode)) return Result.Fail<ResourceRecord>("Failed to read resource record type.");
		if (!reader.TryReadUInt16(out var cls)) return Result.Fail<ResourceRecord>("Failed to read resource record class.");
		if (!reader.TryReadUInt32(out var ttl)) return Result.Fail<ResourceRecord>("Failed to read resource record ttl.");
		if (!reader.TryReadUInt16(out var length)) return Result.Fail<ResourceRecord>("Failed to read resource record data length.");
		if (!reader.CanRead(length)) return Result.Fail<ResourceRecord>("Failed to read resource record data length exceeds actual data.");
		var data = new byte[length];
		reader.Read(data);

		var record = new ResourceRecord
		{
			Name = name,
			Type = typeCode,
			Class = cls,
			TimeToLiveInSeconds = ttl,
			DataLength = length,
			Data = data
		};

		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, ResourceRecord value)
	{
		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		writer.WriteUInt16(value.Type);
		writer.WriteUInt16(value.Class);
		writer.WriteUInt32(value.TimeToLiveInSeconds);
		writer.WriteUInt16((ushort)value.Data.Length);
		writer.Write(value.Data);
		return Result.Ok();
	}
}
