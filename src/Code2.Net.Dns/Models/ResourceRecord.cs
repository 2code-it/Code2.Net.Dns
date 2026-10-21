using Code2.Net.Dns.Models.Formats;

namespace Code2.Net.Dns.Models;

public class ResourceRecord
{
	public string Name { get; set; } = string.Empty;
	public ushort Type { get; set; }
	public ushort Class { get; set; }
	public uint TimeToLiveInSeconds { get; set; }
	public ushort DataLength { get; set; }
	public object Data { get; set; } = new UnknownRecord();
}
