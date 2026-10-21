using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class ARecordSerializer : IWireSerializer<ARecord>, IFormatWireSerializer
{
	public ushort RecordTypeCode => Constants.RecordTypes.A;
	public IResult<ARecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadUInt32(out var address)) return Result.Fail<ARecord>("Failed to read address");
		var record = new ARecord() { Address = address };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, ARecord value)
	{
		writer.WriteUInt32(value.Address);
		return Result.Ok();
	}
}
