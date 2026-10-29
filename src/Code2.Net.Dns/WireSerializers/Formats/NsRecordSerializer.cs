using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class NsRecordSerializer : IWireSerializer<NsRecord>
{
	public IResult<NsRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<NsRecord>(error);
		var record = new NsRecord() { Name = name };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, NsRecord value)
	{
		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		return Result.Ok();
	}
}
