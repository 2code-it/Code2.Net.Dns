namespace Code2.Net.Dns.Models.Formats;

public class SoaRecord
{
	public string MName { get; set; } = string.Empty;
	public string RName { get; set; } = string.Empty;
	public uint Serial { get; set; }
	public uint Refresh { get; set; }
	public uint Retry { get; set; }
	public uint Expire { get; set; }
	public uint Minimum { get; set; }
}