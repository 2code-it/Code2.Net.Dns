using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class PtrRecordSerializer : IWireSerializer<PtrRecord>, IFormatWireSerializer
{
	public ushort RecordTypeCode => Constants.RecordTypes.PTR;

	public IResult<PtrRecord> Deserialize(IWireReader reader)
	{
		if (!reader.TryReadName(out var name, out var error)) return Result.Fail<PtrRecord>(error);
		var record = new PtrRecord() { Name = name };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, PtrRecord value)
	{
		if (!writer.TryWriteName(value.Name, out var error)) return Result.Fail(error);
		return Result.Ok();
	}
}