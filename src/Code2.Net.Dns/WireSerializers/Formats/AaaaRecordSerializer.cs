using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class AaaaRecordSerializer : IWireSerializer<AaaaRecord>, IFormatWireSerializer
{
	public ushort RecordTypeCode => Constants.RecordTypes.AAAA;
	public IResult<AaaaRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadUInt128(out var address)) return Result.Fail<AaaaRecord>("Failed to read address");
		var record = new AaaaRecord() { Address = address };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, AaaaRecord value)
	{
		writer.WriteUInt128(value.Address);
		return Result.Ok();
	}
}
