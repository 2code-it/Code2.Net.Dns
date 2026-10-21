using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class MxRecordSerializer : IWireSerializer<MxRecord>, IFormatWireSerializer
{
	public ushort RecordTypeCode => Constants.RecordTypes.MX;

	public IResult<MxRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadUInt16(out var preference)) return Result.Fail<MxRecord>("Failed to read preference");
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<MxRecord>(error);
		var record = new MxRecord() { Preference = preference, Exchange = name };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, MxRecord value)
	{
		writer.WriteUInt16(value.Preference);
		if (!writer.TryWriteName(value.Exchange, out var error)) return Result.Fail(error);
		return Result.Ok();
	}
}
