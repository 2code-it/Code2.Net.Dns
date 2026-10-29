using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class CnameRecordSerializer : IWireSerializer<CnameRecord>
{
	public IResult<CnameRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<CnameRecord>(error);
		var record = new CnameRecord() { Name = name };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, CnameRecord value)
	{
		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		return Result.Ok();
	}
}
