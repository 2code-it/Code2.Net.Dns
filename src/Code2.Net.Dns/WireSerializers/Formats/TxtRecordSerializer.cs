using Code2.Net.Dns.Common;
using Code2.Net.Dns.Models.Formats;
using System.Text;

namespace Code2.Net.Dns.WireSerializers.Formats;

public class TxtRecordSerializer : IWireSerializer<TxtRecord>, IFormatWireSerializer
{
	public ushort RecordTypeCode => Constants.RecordTypes.TXT;

	public IResult<TxtRecord> Deserialize(IWireReader reader)
	{
		var data = new byte[reader.Length];
		reader.Read(data);
		var record = new TxtRecord() { Text = Encoding.ASCII.GetString(data) };
		return Result.Ok(record);
	}

	public IResult Serialize(IWireWriter writer, TxtRecord value)
	{
		var bytes = Encoding.ASCII.GetBytes(value.Text);
		writer.Write(bytes);
		return Result.Ok();
	}
}