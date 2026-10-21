using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers;

public class ResourceRecordSerializer : IWireSerializer<ResourceRecord>
{
	private readonly IFormatSerializerManager _serializerManager;
	public ResourceRecordSerializer() : this(null) { }
	public ResourceRecordSerializer(IFormatSerializerManager? serializerManager)
	{
		_serializerManager = serializerManager ?? FormatSerializerManager.Instance;
	}

	private static IWireSerializer<ResourceRecord>? _instance;
	public static IWireSerializer<ResourceRecord> Instance => _instance ?? (_instance = new ResourceRecordSerializer());

	public IResult<ResourceRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<ResourceRecord>(error);
		if (!reader.TryReadUInt16(out var typeCode)) return Result.Fail<ResourceRecord>("Failed to read resource record type.");
		if (!reader.TryReadUInt16(out var cls)) return Result.Fail<ResourceRecord>("Failed to read resource record class.");
		if (!reader.TryReadUInt32(out var ttl)) return Result.Fail<ResourceRecord>("Failed to read resource record ttl.");
		if (!reader.TryReadUInt16(out var length)) return Result.Fail<ResourceRecord>("Failed to read resource record data length.");

		var record = new ResourceRecord
		{
			Name = name,
			Type = typeCode,
			Class = cls,
			TimeToLiveInSeconds = ttl,
			DataLength = length
		};

		object? recordData = null;
		if (_serializerManager.HasSerializer(typeCode))
		{
			var result = _serializerManager.Deserialize(typeCode, reader);
			if (!result.IsSuccess) return Result.Fail<ResourceRecord>(result);
			recordData = result.Value;
		}

		if (recordData is null)
		{
			var data = new byte[length];
			reader.Read(data);
			recordData = new UnknownRecord() { Value = data };
		}

		record.Data = recordData;

		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, ResourceRecord value)
	{
		var formatWriter = new WireWriter();
		var type = value.Data.GetType();
		if (type == typeof(UnknownRecord) || !_serializerManager.HasSerializer(type))
		{
			var format = (UnknownRecord)value.Data;
			formatWriter.Write(format.Value);
		}
		else
		{
			if (!_serializerManager.IsMatch(value.Type, type)) return Result.Fail($"Type code {value.Type} does not match type {type.Name}");
			var result = _serializerManager.Serialize(value.Type, formatWriter, value.Data);
			if (!result.IsSuccess) return result;
		}

		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		writer.WriteUInt16(value.Type);
		writer.WriteUInt16(value.Class);
		writer.WriteUInt32(value.TimeToLiveInSeconds);
		writer.WriteUInt16((ushort)formatWriter.Position);
		writer.Write(formatWriter.ToArray());
		return Result.Ok();
	}
}
