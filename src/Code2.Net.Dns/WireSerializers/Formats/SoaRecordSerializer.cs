using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class SoaRecordSerializer : IWireSerializer<SoaRecord>
{
	public IResult<SoaRecord> Deserialize(IWireReader reader)
	{

		if (!reader.TryReadName(out var mname, out var error1)) return Result.Fail<SoaRecord>(error1);
		if (!reader.TryReadName(out var rname, out var error2)) return Result.Fail<SoaRecord>(error2);
		if (!reader.TryReadUInt32(out var serial)) return Result.Fail<SoaRecord>("Failed to read serial");
		if (!reader.TryReadUInt32(out var refresh)) return Result.Fail<SoaRecord>("Failed to read refresh");
		if (!reader.TryReadUInt32(out var retry)) return Result.Fail<SoaRecord>("Failed to read retry");
		if (!reader.TryReadUInt32(out var expire)) return Result.Fail<SoaRecord>("Failed to read expire");
		if (!reader.TryReadUInt32(out var minimum)) return Result.Fail<SoaRecord>("Failed to read minimum");

		var record = new SoaRecord()
		{
			MName = mname,
			RName = rname,
			Serial = serial,
			Refresh = refresh,
			Retry = retry,
			Expire = expire,
			Minimum = minimum
		};
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, SoaRecord value)
	{
		if (!writer.TryWriteName(value.MName, out var error1)) return Result.Fail(error1);
		if (!writer.TryWriteName(value.RName, out var error2)) return Result.Fail(error2);
		writer.WriteUInt32(value.Serial);
		writer.WriteUInt32(value.Refresh);
		writer.WriteUInt32(value.Retry);
		writer.WriteUInt32(value.Expire);
		writer.WriteUInt32(value.Minimum);
		return Result.Ok();
	}
}